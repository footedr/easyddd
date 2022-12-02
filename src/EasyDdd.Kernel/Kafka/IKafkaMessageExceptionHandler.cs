using Confluent.Kafka;

namespace EasyDdd.Kernel.Kafka
{
    public interface IKafkaMessageExceptionHandler<TKey, TValue>
    {
        /// <summary>
        /// Attempts to handle an exception that was thrown by the registered <see cref="IKafkaMessageExceptionHandler{TKey, TValue}"/>.
        /// If this method returns without error, the message will be considered to have been processed successfully and the consumer
        /// offset will be updated.  If an exception is thrown, the consumer will stop.
        /// </summary>
        Task HandleException(Exception exception, ConsumeResult<TKey, TValue> consumeResult, CancellationToken stoppingToken);
    }
}
