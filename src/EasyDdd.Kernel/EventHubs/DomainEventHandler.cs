using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public class DomainEventHandler : DomainEventHandler<DomainEvent>
{
	private readonly DomainEventPublisherConfiguration _configuration;
	private readonly ILogger<DomainEventHandler> _logger;

	public DomainEventHandler(DomainEventPublisherConfiguration configuration,
		ILogger<DomainEventHandler> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public override async Task Handle(DomainEvent @event, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Handling event: {EventType}, pushing to EventHub {TopicName} topic.", nameof(DomainEvent), @event.Topic);

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