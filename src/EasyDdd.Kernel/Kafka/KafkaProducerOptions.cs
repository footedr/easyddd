using Confluent.Kafka;

namespace EasyDdd.Kernel.Kafka
{
    public record KafkaProducerOptions
    {
        public ProducerConfig ProducerConfig { get; } = new ProducerConfig
        {
            EnableIdempotence = true
        };
    }
}
