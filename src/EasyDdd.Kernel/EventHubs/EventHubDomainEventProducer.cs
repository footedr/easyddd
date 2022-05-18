using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public class EventHubDomainEventProducer : IDomainEventProducer
{
	private readonly DomainEventPublisherConfiguration _configuration;
	private readonly ILogger<EventHubDomainEventProducer> _logger;

	public EventHubDomainEventProducer(DomainEventPublisherConfiguration configuration,
		ILogger<EventHubDomainEventProducer> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public async Task Produce(IReadOnlyList<DomainEvent> domainEvents, CancellationToken cancellationToken)
	{
		var producerConfig = new ProducerConfig(new Dictionary<string, string>
		{
			{ "bootstrap.servers", _configuration.Endpoint }
		});

		if (_configuration is DomainEventPublisherWithSaslConfiguration configWithSasl)
		{
			producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
			producerConfig.SaslMechanism = SaslMechanism.Plain;
			producerConfig.SaslUsername = "$ConnectionString";
			producerConfig.SaslPassword = configWithSasl.ConnectionString;
		}

		using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

		foreach (var domainEvent in domainEvents)
		{
			_logger.LogInformation("Handling event: {EventType}, pushing to EventHub {TopicName} topic.", nameof(DomainEvent), domainEvent.Topic);

			var eventJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _configuration.JsonSerializerOptions);

			var message = new Message<string, string>
			{
				Key = domainEvent.Topic.Key,
				Value = eventJson,
				Headers = new Headers { { EventHubConstants.EventTypeHeaderName, Encoding.ASCII.GetBytes(domainEvent.EventType) } }
			};

			await producer.ProduceAsync(domainEvent.Topic.Name, message, cancellationToken);

			_logger.LogInformation("{EventType} published to {TopicName} topic successfully.", nameof(DomainEvent), domainEvent.Topic);
		}
	}
}