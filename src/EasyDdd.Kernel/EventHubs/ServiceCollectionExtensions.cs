using System;
using System.Linq;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEventHubDomainEventProducer(this IServiceCollection services, 
		string endpoint, 
		string connectionString,
		JsonSerializerOptions jsonSerializerOptions)
	{
		var config = new EventHubDomainEventPublisherConfiguration
		{
			ConnectionString = connectionString,
			Endpoint = endpoint,
			JsonSerializerOptions = jsonSerializerOptions
		};
		
		var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => typeof(DomainEvent).IsAssignableFrom(type))
			.Distinct();

		foreach (var eventType in eventTypes)
		{
			services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(eventType), serviceProvider =>
			{
				var logger = serviceProvider.GetRequiredService<ILogger<EventHubDomainEventHandler>>();

				return new EventHubDomainEventHandler(config, logger);
			});
		}

		return services;
	}

	public static IServiceCollection AddEventHubDomainEventConsumer(this IServiceCollection services,
		string endpoint,
		string connectionString,
		string topicName,
		string consumerGroup,
		JsonSerializerOptions jsonSerializerOptions)
	{
		services.AddSingleton<IHostedService, EventHubDomainEventConsumer>(serviceProvider =>
		{
			var config = new EventHubDomainEventConsumerConfiguration
			{
				ConnectionString = connectionString,
				ConsumerGroup = consumerGroup,
				Endpoint = endpoint,
				TopicName = topicName,
				JsonSerializerOptions = jsonSerializerOptions
			};

			var logger = serviceProvider.GetRequiredService<ILogger<EventHubDomainEventConsumer>>();
			var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

			return new EventHubDomainEventConsumer(config, scopeFactory, logger);
		});
		return services;
	}
}