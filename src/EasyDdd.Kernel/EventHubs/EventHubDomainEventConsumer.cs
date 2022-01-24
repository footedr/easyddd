using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public class EventHubDomainEventConsumer : IHostedService
{
	private readonly EventHubDomainEventConsumerConfiguration _configuration;
	private readonly ILogger<EventHubDomainEventConsumer> _logger;

	public EventHubDomainEventConsumer(EventHubDomainEventConsumerConfiguration configuration,
		ILogger<EventHubDomainEventConsumer> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		var conf = new ConsumerConfig
		{
			GroupId = _configuration.ConsumerGroup,
			BootstrapServers = _configuration.Endpoint,
			AutoOffsetReset = AutoOffsetReset.Earliest,
			SaslPassword = _configuration.ConnectionString,
			SaslUsername = "$ConnectionString",
			SecurityProtocol = SecurityProtocol.SaslSsl,
			SaslMechanism = SaslMechanism.Plain
		};

		using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
		{
			builder.Subscribe(_configuration.TopicName);

			try
			{
				while (true)
				{
					var consumer = builder.Consume(cancellationToken);
					_logger.LogInformation($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
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