using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyDdd.Kernel.Kafka
{
    public class KafkaConsumer<TKey, TValue> : BackgroundService
    {
        private readonly IOptions<KafkaConsumerOptions> _options;
        private readonly ILogger<KafkaConsumer<TKey, TValue>> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly KafkaConsumerHealthCheck _healthCheck;
        private int _consumeFailureCount = 0;

        public KafkaConsumer(IOptions<KafkaConsumerOptions> options, ILogger<KafkaConsumer<TKey, TValue>> logger, IServiceScopeFactory serviceScopeFactory, KafkaConsumerHealthCheck healthCheck)
        {
            _options = options;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _healthCheck = healthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.Value.ConsumerConfig.AllowAutoCreateTopics ?? false)
            {
                await CreateTopics();
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Run(() => Consume(stoppingToken), stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _consumeFailureCount++;

                    if (_consumeFailureCount >= _options.Value.ReconnectConsecutiveFailureLimit)
                    {
                        var message = $"Unable to start consumer after {_consumeFailureCount} attempts, stopping";

                        _healthCheck.Result = HealthCheckResult.Unhealthy(message);

                        throw new Exception(message, ex);
                    }
                    else
                    {
                        var message = $"Error consuming messages, failure number {_consumeFailureCount}. Waiting before restarting consumer.";

                        _healthCheck.Result = HealthCheckResult.Degraded(message);

                        _logger.LogWarning(message);

                        await _options.Value.ReconnectBackoffStrategy(_consumeFailureCount);

                        _logger.LogWarning("Restarting consumer");
                    }
                }
            }
        }

        private async Task Consume(CancellationToken stoppingToken)
        {
            var builder = new ConsumerBuilder<TKey, TValue>(_options.Value.ConsumerConfig)
                .SetErrorHandler((consumer, error) =>
                {
                    _logger.LogWarning(
                        "{ErrorCode}: {ErrorReason}. IsError={IsError}, IsLocalError={IsLocalError}, IsBrokerError={IsBrokerError}, IsFatal={IsFatal}",
                        error.Code,
                        error.Reason,
                        error.IsError,
                        error.IsLocalError,
                        error.IsBrokerError,
                        error.IsFatal);
                });

            using (var consumer = builder.Build())
            {
                consumer.Subscribe(_options.Value.TopicNames);

                foreach (var topicName in _options.Value.TopicNames)
                {
                    _logger.LogInformation("Subscribed to Kafka topic {TopicName}", topicName);
                }

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        _healthCheck.Result = HealthCheckResult.Healthy("Subscribed to topics and ready to consume messages");

                        var consumeResult = consumer.Consume(stoppingToken);

                        _logger.LogInformation("Received message on topic {TopicName}, partition {TopicPartition}, offset {TopicPartitionOffset}",
                            consumeResult.Topic,
                            consumeResult.TopicPartition.Partition.Value,
                            consumeResult.TopicPartitionOffset.Offset.Value);

                        await HandleMessage(consumeResult, stoppingToken);
                        consumer.StoreOffset(consumeResult);
                        consumer.Commit();
                        _consumeFailureCount = 0;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming messages from Kafka, closing consumer");
                    consumer.Close();
                    throw;
                }
            }
        }

        private async Task HandleMessage(ConsumeResult<TKey, TValue> consumeResult, CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var messageHandler = scope.ServiceProvider.GetRequiredService<IKafkaMessageHandler<TKey, TValue>>();
                try
                {
                    var context = new Polly.Context();
                    context.Add("Logger", _logger);
                    await _options.Value.RetryPolicy.ExecuteAsync(
                        (_, cancellationToken) => messageHandler.HandleMessage(consumeResult.Message, cancellationToken),
                        context,
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    var exceptionHandler = scope.ServiceProvider.GetService<IKafkaMessageExceptionHandler<TKey, TValue>>();
                    if (exceptionHandler is null)
                    {
                        _logger.LogInformation("Error handling message, no exception handler registered for Kafka messages, exception will be re-thrown");
                        throw;
                    }
                    await exceptionHandler.HandleException(ex, consumeResult, stoppingToken);
                }
            }
        }

        private async Task CreateTopics()
        {
            _logger.LogInformation("Creating topics if necessary");

            // When running in docker, there is a delay between when the Kafka container is started,
            // and when the container is ready to accept connections.  Here we try a few times with a delay between
            // each attempt to give Kafka enough time to warm up.
            var errors = new List<Exception>();
            var complete = false;

            for (var attempt = 1; attempt <= 5; attempt++)
            {
                try
                {
                    var builder = new AdminClientBuilder(new AdminClientConfig
                    {
                        BootstrapServers = _options.Value.ConsumerConfig.BootstrapServers
                    });

                    using (var adminClient = builder.Build())
                    {
                        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(1));

                        foreach (var topicName in _options.Value.TopicNames)
                        {
                            var topic = metadata.Topics.FirstOrDefault(t => t.Topic == topicName);
                            if (topic is not null)
                            {
                                _logger.LogInformation("Topic {TopicName} already exists", topicName);
                                continue;
                            }

                            await adminClient.CreateTopicsAsync(new[]
                            {
                                new TopicSpecification
                                {
                                    Name = topicName,
                                    NumPartitions = 1,
                                    ReplicationFactor = 1
                                }
                            });

                            _logger.LogInformation("Topic {TopicName} created", topicName);
                        }

                        return;
                    }
                }
                catch (KafkaException ex)
                {
                    errors.Add(ex);
                    Thread.Sleep(5000);
                }
            }

            if (!complete)
            {
                throw new AggregateException("Unable to setup kafka topics", errors);
            }
        }
    }
}
