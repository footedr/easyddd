using System.Net.Http.Headers;
using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyDdd.Kernel.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static KafkaProducerBuilder AddKafkaProducer(this IServiceCollection serviceCollection, Action<KafkaProducerOptions>? action = null)
        {
            serviceCollection.Configure(action);

            serviceCollection.AddSingleton(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<KafkaProducerOptions>>();
                var logger = serviceProvider.GetRequiredService<ILogger<IProducer<string, string>>>();
                var builder = new ProducerBuilder<string, string>(options.Value.ProducerConfig);

                builder.SetErrorHandler((producer, error) =>
                {
                    var logLevel = error.Code switch
                    {
                        // This error happens whenever the producer is idle for too long (starts after anywhere from 5-45 minutes of idle time)
                        // The next time an event is produced, things always seem to reconnect just fine, and the error goes away until the
                        // producer is idle again for too long.  To avoid polluting the logs, going to treat this as Info rather than a Warning
                        // https://dflcloud.visualstudio.com/Nucleus/_workitems/edit/11807
                        ErrorCode.Local_AllBrokersDown => LogLevel.Information, 
                        _ => LogLevel.Warning
                    };

                    logger.Log(
                        logLevel,
                        "{ErrorCode}: {ErrorReason}. IsError={IsError}, IsLocalError={IsLocalError}, IsBrokerError={IsBrokerError}, IsFatal={IsFatal}",
                        error.Code,
                        error.Reason,
                        error.IsError,
                        error.IsLocalError,
                        error.IsBrokerError,
                        error.IsFatal);
                });

                return builder.Build();
            });

            return new KafkaProducerBuilder(serviceCollection);
        }

        public static KafkaConsumerBuilder AddKafkaConsumer(this IServiceCollection serviceCollection, Action<KafkaConsumerOptions>? action = null)
        {
            serviceCollection.Configure(action);

            serviceCollection.AddHostedService<KafkaConsumer<string, string>>();

            serviceCollection.AddSingleton<KafkaConsumerHealthCheck>();

            return new KafkaConsumerBuilder(serviceCollection);
        }

        public static IHealthChecksBuilder AddConsumerGroupLagCheck(this IHealthChecksBuilder builder, ConfluentClusterConfig clusterConfig, ConsumerGroupLagOptions[] groups, HealthStatus? status = null, IEnumerable<string>? tags = null)
        {
            var httpClientName = nameof(ConsumerGroupLagHealthCheck);
            var highWaterMarks = new Dictionary<string, HighWaterMark>();
            var now = DateTimeOffset.UtcNow;

            foreach (var group in groups)
            {
                highWaterMarks[group.GroupId.ToLowerInvariant()] = new HighWaterMark(0, now);
            }

            builder.Services.AddHttpClient(httpClientName, httpClient =>
            {
                httpClient.BaseAddress = new Uri(clusterConfig.ApiEndpoint);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clusterConfig.ApiKey}:{clusterConfig.ApiSecret}")));
            });

            builder.Add(new HealthCheckRegistration(
                "consumer-group-lag",
                provider =>
                {
                    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

                    var logger = provider.GetRequiredService<ILogger<ConsumerGroupLagHealthCheck>>();

                    return new ConsumerGroupLagHealthCheck(
                        highWaterMarks,
                        () => httpClientFactory.CreateClient(httpClientName),
                        clusterConfig.ClusterId,
                        groups,
                        logger);
                },
                status,
                tags));

            return builder;
        }
    }

    public record ConfluentClusterConfig(string ApiEndpoint, string ClusterId, string ApiKey, string ApiSecret);
}
