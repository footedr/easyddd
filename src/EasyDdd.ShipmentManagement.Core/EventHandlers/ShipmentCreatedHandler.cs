using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MediatR;

namespace EasyDdd.ShipmentManagement.Core.EventHandlers;

public class ShipmentCreatedHandler : INotificationHandler<ShipmentCreated>
{
	public async Task Handle(ShipmentCreated @event, CancellationToken cancellationToken)
	{
		var url = "easyddd.servicebus.windows.net:9093";
		var connectionString = "Endpoint=sb://easyddd.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=q/9OUbmCYzjDjZgpFLoEU2wBItMRvEUmU3bmq2zmFRQ=";
		var topicName = "shipments";

		var producerConfig = new ProducerConfig(new Dictionary<string, string>
		{
			{ "bootstrap.servers", url },
			{ "security.protocol", "SASL_SSL" },
			{ "sasl.mechanism", "PLAIN" },
			//{ "sasl.jaas.config", $"org.apache.kafka.common.security.plain.PlainLoginModule required username=\"$ConnectionString\" password=\"{connectionString}\"" }
			{ "sasl.username", "$ConnectionString" },
			{ "sasl.password", connectionString }
		});

		using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

		var eventJson = JsonSerializer.Serialize(@event);

		var message = new Message<string, string>
		{
			Key = @event.Shipment.Identifier,
			Value = eventJson
		};

		await producer.ProduceAsync(topicName, message, cancellationToken);
	}
}