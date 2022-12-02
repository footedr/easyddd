using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Polly;

namespace EasyDdd.Kernel.Kafka
{
    public record KafkaConsumerOptions
    {
        public ConsumerConfig ConsumerConfig { get; } = new ConsumerConfig()
        {
            AutoOffsetReset = AutoOffsetReset.Earliest,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = false
        };

        public List<string> TopicNames { get; } = new List<string>();

        /// <summary>
        /// Number of times the consumer should attempt to reconnect before stopping.  Note that
        /// whenever a message is successfully processed, the failure count is reset to 0.
        /// </summary>
        public int ReconnectConsecutiveFailureLimit { get; set; } = 5;

        /// <summary>
        /// Backoff strategy to use when reconnecting after a failure.  Func will receive the failure number (1
        /// for first failure, 2 for second failure, etc.).  Note that whenever a message is successfully processed,
        /// the failure count is reset to 0.
        /// </summary>
        public Func<int, Task> ReconnectBackoffStrategy { get; set; } = DefaultReconnectBackoffStrategy;

        /// <summary>
        /// Delays reconnect by 1 second for first failure, 2 seconds for second failure, 3 seconds for third failure etc.
        /// </summary>
        public static Func<int, Task> DefaultReconnectBackoffStrategy => failureNumber => Task.Delay(TimeSpan.FromSeconds(failureNumber));

        /// <summary>
        /// Policy to use when the configured <see cref="IKafkaMessageHandler{TKey, TValue}"/> encounters an error.  If not specified <see cref="DefaultRetryPolicy" /> is used.
        /// </summary>
        public AsyncPolicy RetryPolicy { get; set; } = DefaultRetryPolicy;

        /// <summary>
        /// Will retry 2 additional times after a failure, waiting 1 second between tries.
        /// </summary>
        public static AsyncPolicy DefaultRetryPolicy => Policy.Handle<Exception>()
            .WaitAndRetryAsync(Enumerable.Repeat(TimeSpan.FromSeconds(1), 2), (exception, delay, context) =>
            {
                context.TryGetValue("Logger", out var loggerObject);
                if (loggerObject is ILogger logger)
                {
                    logger.LogError(exception, $"Error consuming message, retrying after {delay.TotalSeconds} seconds");
                }
            });
    }
}
