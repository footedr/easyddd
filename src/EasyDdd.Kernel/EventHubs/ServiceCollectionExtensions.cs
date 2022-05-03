using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDomainEventProducer(this IServiceCollection services,
		DomainEventPublisherConfiguration configuration)
	{
		services.AddTransient<IDomainEventProducer, EventHubDomainEventProducer>(serviceProvider =>
		{
			var logger = serviceProvider.GetRequiredService<ILogger<EventHubDomainEventProducer>>();
			return new EventHubDomainEventProducer(configuration, logger);
		});

		return services;
	}

	public static IServiceCollection AddDomainEventConsumer(this IServiceCollection services,
		DomainEventConsumerConfiguration configuration)
	{
		services.AddSingleton<IHostedService, DomainEventConsumer>(serviceProvider =>
		{
			var logger = serviceProvider.GetRequiredService<ILogger<DomainEventConsumer>>();
			var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

			return new DomainEventConsumer(configuration, scopeFactory, logger);
		});
		return services;
	}
}