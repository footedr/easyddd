using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public class KafkaDomainEventProducer : IDomainEventProducer
{
	private readonly IProducer<string, string> _producer;
	private readonly JsonSerializerOptions _jsonSerializerOptions;
	private readonly ILogger<KafkaDomainEventProducer> _logger;

	public KafkaDomainEventProducer(IProducer<string, string> producer,
		JsonSerializerOptions jsonSerializerOptions,
		ILogger<KafkaDomainEventProducer> logger)
	{
		_producer = producer;
		_jsonSerializerOptions = jsonSerializerOptions;
		_logger = logger;
	}

	public async Task Produce(IReadOnlyList<DomainEvent> domainEvents, CancellationToken cancellationToken)
	{
		foreach (var domainEvent in domainEvents)
		{
			_logger.LogInformation("Handling event: {EventType}, pushing to EventHub {TopicName} topic.", nameof(DomainEvent), domainEvent.Topic);

			var eventJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _jsonSerializerOptions);

			var message = new Message<string, string>
			{
				Key = domainEvent.Topic.Key,
				Value = eventJson,
				Headers = new Headers { { EventHubConstants.EventTypeHeaderName, Encoding.ASCII.GetBytes(domainEvent.EventType) } }
			};
			
			await _producer.ProduceAsync(domainEvent.Topic.Name, message, cancellationToken);

			_logger.LogInformation("{EventType} published to {TopicName} topic successfully.", nameof(DomainEvent), domainEvent.Topic);
		}
	}
}