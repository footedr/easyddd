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

public class DomainEventConsumerConfiguration
{
	public DomainEventConsumerConfiguration(string topicName, string consumerGroup, string endpoint, JsonSerializerOptions jsonSerializerOptions)
	{
		TopicName = topicName;
		ConsumerGroup = consumerGroup;
		Endpoint = endpoint;
		JsonSerializerOptions = jsonSerializerOptions;
	}

	public string TopicName { get; }
	public string ConsumerGroup { get; }
	public string Endpoint { get; }
	public JsonSerializerOptions JsonSerializerOptions { get; }
}

public class DomainEventConsumerWithSaslConfiguration : DomainEventConsumerConfiguration
{
	public DomainEventConsumerWithSaslConfiguration(string topicName, string consumerGroup, string endpoint, string connectionString, JsonSerializerOptions jsonSerializerOptions)
		: base(topicName, consumerGroup, endpoint, jsonSerializerOptions)
	{
		ConnectionString = connectionString;
	}
	public string ConnectionString { get; }
}

public class DomainEventConsumer : BackgroundService
{
	private readonly DomainEventConsumerConfiguration _configuration;
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly ILogger<DomainEventConsumer> _logger;

	public DomainEventConsumer(DomainEventConsumerConfiguration configuration,
		IServiceScopeFactory serviceScopeFactory,
		ILogger<DomainEventConsumer> logger)
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
			AutoOffsetReset = AutoOffsetReset.Earliest
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
				var consumer = builder.Consume(cancellationToken);
				_logger.LogInformation("Message: {Message} received from {TopicOffset}", consumer.Message.Value, consumer.TopicPartitionOffset);

				using var scope = _serviceScopeFactory.CreateScope();

				var eventTypeHeader = consumer.Message.Headers.SingleOrDefault(h => h.Key.Equals(EventHubConstants.EventTypeHeaderName));
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

				var domainEvent = JsonSerializer.Deserialize(consumer.Message.Value, eventDataType, _configuration.JsonSerializerOptions);

				if (domainEvent is null)
				{
					_logger.LogError($"Event type ${eventDataType} not processed.  Data deserialized to null.");
					continue;
				}

				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
				await mediator.Publish(domainEvent, cancellationToken);
			}
		}
		catch (Exception exception)
		{
			_logger.LogError("Exception occurred, closing Kafka consumer. Exception: {ExceptionMessage}", exception.Message);
			builder.Close();
		}
	}
}