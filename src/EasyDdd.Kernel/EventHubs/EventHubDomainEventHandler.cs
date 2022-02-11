using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public class EventHubDomainEventHandler : DomainEventHandler<DomainEvent>
{
	private readonly EventHubDomainEventPublisherConfiguration _configuration;
	private readonly ILogger<EventHubDomainEventHandler> _logger;

	public EventHubDomainEventHandler(EventHubDomainEventPublisherConfiguration configuration,
		ILogger<EventHubDomainEventHandler> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public override async Task Handle(DomainEvent @event, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Handling event: {EventType}, pushing to EventHub {TopicName} topic.", nameof(DomainEvent), @event.Topic);

		var producerConfig = new ProducerConfig(new Dictionary<string, string>
		{
			{ "bootstrap.servers", _configuration.Endpoint },
			{ "security.protocol", "SASL_SSL" },
			{ "sasl.mechanism", "PLAIN" },
			{ "sasl.username", "$ConnectionString" },
			{ "sasl.password", _configuration.ConnectionString }
		});

		using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

		var eventJson = JsonSerializer.Serialize(@event, @event.GetType(), _configuration.JsonSerializerOptions);

		var message = new Message<string, string>
		{
			Key = @event.Topic.Key,
			Value = eventJson,
			Headers = new Headers { { EventHubConstants.EventTypeHeaderName, Encoding.ASCII.GetBytes(@event.EventType) } }
		};

		await producer.ProduceAsync(@event.Topic.Name, message, cancellationToken);

		_logger.LogInformation("{EventType} published to {TopicName} topic successfully.", nameof(DomainEvent), @event.Topic);
	}
}