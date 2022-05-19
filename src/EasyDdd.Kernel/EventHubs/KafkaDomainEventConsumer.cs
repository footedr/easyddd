using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Threading.Tasks.Task;

namespace EasyDdd.Kernel.EventHubs;

public class KafkaDomainEventConsumer : BackgroundService
{
	private readonly DomainEventConsumerConfiguration _configuration;
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly ILogger<KafkaDomainEventConsumer> _logger;

	public KafkaDomainEventConsumer(DomainEventConsumerConfiguration configuration,
		IServiceScopeFactory serviceScopeFactory,
		ILogger<KafkaDomainEventConsumer> logger)
	{
		_configuration = configuration;
		_serviceScopeFactory = serviceScopeFactory;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Run(() => Consume(stoppingToken), stoppingToken);
	}

	private async Task Consume(CancellationToken cancellationToken)
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		var conf = new ConsumerConfig
		{
			GroupId = _configuration.ConsumerGroup,
			BootstrapServers = _configuration.Endpoint,
			AutoOffsetReset = AutoOffsetReset.Earliest,
			AllowAutoCreateTopics = true,
			PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin,
			/*
			 * The auto offset commit capability in the .NET Client is actually quite flexible. As outlined above, by default, the offsets to be commited to Kafka are
			 * updated immediately prior to the Consume method deliverying messages to the application. However, you can prevent this from happening by setting the
			 * EnableAutoOffsetStore config property to false. You can then use the StoreOffsets method to specify the offsets you would like the background thread to commit,
			 * and you can call this precisely when you want. This approach is preferred over the synchronous commit approach outlined in the previous section.
			 */
			EnableAutoCommit = true,
			EnableAutoOffsetStore = false
		};

		if (_configuration is DomainEventConsumerWithSaslConfiguration configWithSasl)
		{
			conf.SecurityProtocol = SecurityProtocol.SaslSsl;
			conf.SaslMechanism = SaslMechanism.Plain;
			conf.SaslUsername = "$ConnectionString";
			conf.SaslPassword = configWithSasl.ConnectionString;
		}

		using var builder = new ConsumerBuilder<Ignore, string>(conf).Build();
		builder.Subscribe(_configuration.TopicName);

		_logger.LogInformation("Subscribed to EventHub topic: {TopicName}.", _configuration.TopicName);

		try
		{
			while (true)
			{
				var consumerResult = builder.Consume(cancellationToken);
				_logger.LogInformation("Message: {Message} received from {TopicOffset}", consumerResult.Message.Value, consumerResult.TopicPartitionOffset);

				using var scope = _serviceScopeFactory.CreateScope();

				var eventTypeHeader = consumerResult.Message.Headers.SingleOrDefault(h => h.Key.Equals(EventHubConstants.EventTypeHeaderName));
				if (eventTypeHeader is null)
				{
					throw new Exception("Error processing shipments topic event. Missing EventType header.");
				}

				var eventType = Encoding.ASCII.GetString(eventTypeHeader.GetValueBytes());
				
				var eventDataType = assemblies
					.Select(assembly => assembly.GetType(eventType, false, true))
					.FirstOrDefault(type => type is not null);

				if (eventDataType is null)
				{
					throw new Exception($"Error processing shipments topic event. Cannot find type matching name: {eventDataType}");
				}

				var domainEvent = JsonSerializer.Deserialize(consumerResult.Message.Value, eventDataType, _configuration.JsonSerializerOptions);

				if (domainEvent is null)
				{
					_logger.LogError($"Event type ${eventDataType} not processed.  Data deserialized to null.");
					continue;
				}

				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
				await mediator.Publish(domainEvent, cancellationToken);

				builder.StoreOffset(consumerResult);
			}
		}
		catch (Exception exception)
		{
			_logger.LogError("Exception occurred, closing Kafka consumer. Exception: {ExceptionMessage}", exception.Message);
			builder.Close();
		}
	}
}