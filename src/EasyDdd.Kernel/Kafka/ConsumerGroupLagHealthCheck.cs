using System.Net.Http.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.Kafka
{
    /// <summary>
    /// Lag Time Threshold used by the Kafka Consumer health check.  Lag measures how many messages have been published
    /// to a topic but not yet processed by a consumer.  Lag Time is the amount of time that has passed since the consumer
    /// offset has been incremented.  The health check will report Degraded if Lag > 0 and Lag Time is > this threshold.
    /// Default LagTimeThreshold is 1 minute.  Note the health check is not executed automatically.  Lag Time is only
    /// evaluated when the health check is executed.
    /// </summary>
    public record ConsumerGroupLagOptions(string GroupId, TimeSpan LagTimeThreshold)
    {
        public static implicit operator ConsumerGroupLagOptions(string GroupId) => new ConsumerGroupLagOptions(GroupId, TimeSpan.FromMinutes(1));
    }

    public class ConsumerGroupLagHealthCheck : IHealthCheck
    {
        private readonly Dictionary<string, HighWaterMark> _highWaterMarks;
        private readonly Func<HttpClient> _httpClientFactory;
        private readonly string _clusterId;
        private readonly IReadOnlyList<ConsumerGroupLagOptions> _lagOptions;
        private readonly ILogger<ConsumerGroupLagHealthCheck> _logger;

        public ConsumerGroupLagHealthCheck(Dictionary<string, HighWaterMark> highWaterMarks, Func<HttpClient> httpClientFactory, string clusterId, IReadOnlyList<ConsumerGroupLagOptions> lagOptions, ILogger<ConsumerGroupLagHealthCheck> logger)
        {
            _highWaterMarks = highWaterMarks;
            _httpClientFactory = httpClientFactory;
            _clusterId = clusterId;
            _lagOptions = lagOptions;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = _httpClientFactory();
                var offsetInfo = await httpClient.GetFromJsonAsync<ConfluentOffsetInfo[]>($"/2.0/consumer/offsets/{_clusterId}", cancellationToken);

                if (offsetInfo is null)
                {
                    throw new Exception("Unable to get offset info");
                }

                var laggingConsumers = new List<string>();
                var now = DateTimeOffset.UtcNow;

                foreach (var offset in offsetInfo)
                {
                    var consumerGroupId = offset.consumerGroupId.ToLowerInvariant();
                    if (!_highWaterMarks.ContainsKey(consumerGroupId))
                    {
                        continue;
                    }

                    var previousHighWaterMark = _highWaterMarks[consumerGroupId];

                    if (offset.totalLag == 0 || offset.sumCurrentOffset > previousHighWaterMark.SumCurrentOffset)
                    {
                        // If there is no lag, or the current offset has increased, assume the consumer is processing messages
                        _highWaterMarks[consumerGroupId] = new HighWaterMark(offset.sumCurrentOffset, now);
                        continue;
                    }

                    _logger.LogInformation("Lag for consumer group {ConsumerGroup} at {Lag} messages", consumerGroupId, offset.totalLag);

                    var options = _lagOptions.First(option => option.GroupId.ToLowerInvariant() == consumerGroupId);
                    var lagTime = now - previousHighWaterMark.Timestamp;
                    if (offset.totalLag > 0 && lagTime > options.LagTimeThreshold)
                    {
                        laggingConsumers.Add($"Lag for consumer group {consumerGroupId} at {offset.totalLag} messages.  No observed progress for {lagTime}.");
                        continue;
                    }
                }

                if (laggingConsumers.Count == 1)
                {
                    return HealthCheckResult.Degraded(laggingConsumers[0]);
                }
                else if (laggingConsumers.Count > 1)
                {
                    foreach (var lagger in laggingConsumers)
                    {
                        _logger.LogWarning(lagger);
                    }

                    return HealthCheckResult.Degraded($"Multiple consumers groups experiencing lag processing messages: {string.Join(", ", laggingConsumers)}");
                }
                else
                {
                    return HealthCheckResult.Healthy();
                }

            }
            catch (Exception ex)
            {
                var message = "Error checking lag for consumer groups";
                _logger.LogError(message, ex);
                return HealthCheckResult.Degraded(message);
            }
        }
    }

    public record ConfluentOffsetInfo(string consumerGroupId, int totalLag, int sumCurrentOffset, int sumEndOffset, int numConsumers, int numTopics);

    public record HighWaterMark(int SumCurrentOffset, DateTimeOffset Timestamp);
}
