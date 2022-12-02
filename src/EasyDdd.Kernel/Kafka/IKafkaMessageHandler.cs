using Confluent.Kafka;

namespace EasyDdd.Kernel.Kafka
{
    public interface IKafkaMessageHandler<TKey, TValue>
    {
        Task HandleMessage(Message<TKey, TValue> message, CancellationToken cancellationToken);
    }
}
