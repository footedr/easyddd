using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace EasyDdd.Billing.Web;

public class ShipmentsConsumerHandler : IHostedService
{
	private const string Topic = "shipments";
	private const string ConsumerGroup = "billing";
	private readonly IOptions<KafkaOptions> _kafkaConfig;

	public ShipmentsConsumerHandler(IOptions<KafkaOptions> kafkaConfig)
	{
		_kafkaConfig = kafkaConfig;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		var conf = new ConsumerConfig
		{
			GroupId = ConsumerGroup,
			BootstrapServers = _kafkaConfig.Value.Url,
			AutoOffsetReset = AutoOffsetReset.Earliest,
			SaslPassword = _kafkaConfig.Value.ConnectionString,
			SaslUsername = "$ConnectionString",
			SecurityProtocol = SecurityProtocol.SaslSsl,
			SaslMechanism = SaslMechanism.Plain
		};

		using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
		{
			builder.Subscribe(Topic);

			try
			{
				while (true)
				{
					var consumer = builder.Consume(cancellationToken);
					Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
				}
			}
			catch (Exception)
			{
				builder.Close();
			}
		}

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}