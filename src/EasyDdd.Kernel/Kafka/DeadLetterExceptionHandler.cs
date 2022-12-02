using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyDdd.Kernel.Kafka
{
    public class DeadLetterExceptionHandler<TKey, TValue> : IKafkaMessageExceptionHandler<TKey, TValue>
    {
        private readonly IOptions<KafkaConsumerOptions> _options;
        private readonly ILogger<DeadLetterExceptionHandler<TKey, TValue>> _logger;
        private readonly IProducer<TKey, TValue> _producer;

        public DeadLetterExceptionHandler(IOptions<KafkaConsumerOptions> options, ILogger<DeadLetterExceptionHandler<TKey, TValue>> logger)
        {
            _options = options;
            _logger = logger;
            _producer = new ProducerBuilder<TKey, TValue>(new ProducerConfig
            {
                BootstrapServers = _options.Value.ConsumerConfig.BootstrapServers,
                SecurityProtocol = _options.Value.ConsumerConfig.SecurityProtocol,
                SaslMechanism = _options.Value.ConsumerConfig.SaslMechanism,
                SaslUsername = _options.Value.ConsumerConfig.SaslUsername,
                SaslPassword = _options.Value.ConsumerConfig.SaslPassword
            }).Build();
        }

        public async Task HandleException(Exception exception, ConsumeResult<TKey, TValue> consumeResult, CancellationToken stoppingToken)
        {
            try
            {
                var deadLetterTopic = $"{_options.Value.ConsumerConfig.GroupId}-deadletter";

                consumeResult.Message.Headers.Add("SourceTopic", Encoding.ASCII.GetBytes(consumeResult.Topic));
                consumeResult.Message.Headers.Add("Exception", Encoding.ASCII.GetBytes(exception.ToString()));

                await _producer.ProduceAsync(deadLetterTopic, consumeResult.Message, stoppingToken);

                _logger.LogError(exception, "Dead letter sent to topic {TopicName}", deadLetterTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dead lettering message from topic {TopicName}", consumeResult.Topic);
                throw;
            }
        }
    }
}
