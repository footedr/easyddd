using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using EasyDdd.Kernel.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyDdd.Kernel.Kafka
{
    public record KafkaDomainEventPublisherOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions();
        public string TypeNameHeader { get; set; } = "TypeName";
        public Func<AggregateIdentifier, string> TopicNamingStrategy { get; set; } = AggregateIdentifierExtensions.DefaultTopicNamingStrategy;
    }

    public class KafkaDomainEventPublisher : IDomainEventPublisher
    {
        private readonly IProducer<string, string> _producer;
        private readonly IOptions<KafkaDomainEventPublisherOptions> _options;
        private readonly ILogger<KafkaDomainEventPublisher> _logger;

        public KafkaDomainEventPublisher(IOptions<KafkaDomainEventPublisherOptions> options, IProducer<string, string> producer, ILogger<KafkaDomainEventPublisher> logger)
        {
            _producer = producer;
            _options = options;
            _logger = logger;
        }

        public async Task PublishEvents(IReadOnlyList<DomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            foreach (var domainEvent in domainEvents)
            {
                var eventType = domainEvent.GetType();
                var eventTypeName = eventType.FullName ?? string.Empty;
                var aggregateIdentifier = domainEvent.GetAggregateIdentifier();
                var topicName = _options.Value.TopicNamingStrategy(aggregateIdentifier);

                _logger.LogInformation("Handling event {EventTypeName} for aggregate {AggregateIdentifier}.", eventTypeName, aggregateIdentifier);

                var eventJson = JsonSerializer.Serialize(domainEvent, eventType, _options.Value.JsonSerializerOptions);

                var message = new Message<string, string>
                {
                    Key = aggregateIdentifier.Identifier,
                    Value = eventJson,
                    Headers = new Headers { { _options.Value.TypeNameHeader, Encoding.ASCII.GetBytes(eventTypeName) } }
                };

                await _producer.ProduceAsync(topicName, message, cancellationToken);

                _logger.LogInformation("{EventTypeName} published to {TopicName} topic successfully.", eventTypeName, topicName);
            }
        }
    }
}
