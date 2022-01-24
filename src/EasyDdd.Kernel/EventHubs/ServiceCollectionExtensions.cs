using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEventHubDomainEventHandler(this IServiceCollection services, 
		string endpoint, 
		string connectionString)
	{
		services.AddTransient<INotificationHandler<DomainEvent>, EventHubDomainEventHandler>(serviceProvider =>
		{
			var config = new EventHubDomainEventPublisherConfiguration
			{
				ConnectionString = connectionString,
				Endpoint = endpoint
			};

			var logger = serviceProvider.GetRequiredService<ILogger<EventHubDomainEventHandler>>();

			return new EventHubDomainEventHandler(config, logger);
		});

		return services;
	}

	public static IServiceCollection AddEventHubDomainEventConsumer(this IServiceCollection services,
		string endpoint,
		string connectionString,
		string topicName,
		string consumerGroup)
	{
		services.AddSingleton<IHostedService, EventHubDomainEventConsumer>(serviceProvider =>
		{
			var config = new EventHubDomainEventConsumerConfiguration
			{
				ConnectionString = connectionString,
				ConsumerGroup = consumerGroup,
				Endpoint = endpoint,
				TopicName = topicName
			};

			var logger = serviceProvider.GetRequiredService<ILogger<EventHubDomainEventConsumer>>();

			return new EventHubDomainEventConsumer(config, logger);
		});
		return services;
	}
}