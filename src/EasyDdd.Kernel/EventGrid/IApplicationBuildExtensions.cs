using System;
using System.Linq;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventGrid
{
    public static class IApplicationBuildExtensions
    {
        public static IApplicationBuilder UseEventGrid(this IApplicationBuilder app, string route, string apiKey, JsonSerializerOptions? jsonOptions = null)
        {
            app.MapWhen(
                context => context.Request.Path.ToString().Equals(route, StringComparison.OrdinalIgnoreCase),
                appBranch => appBranch.UseMiddleware<EventGridMiddleware>(
                    new EventGridConfiguration(apiKey, jsonOptions)));

            return app;
        }

        public static IServiceCollection AddEventGridDomainEventProducer(this IServiceCollection services, 
			string hostname, 
			string key, 
			Func<DomainEvent, bool>? filter = null, 
			string subject = "eventgridevent", 
			string dataVersion = "1.0", 
			Func<DomainEvent, string>? eventNameResolver = null, 
			JsonSerializerOptions? jsonOptions = null)
        {
			var config = new EventGridDomainEventPublisherConfiguration
			{
				Hostname = hostname,
				Key = key,
				Subject = subject,
				DataVersion = dataVersion,
				Filter = filter,
				EventNameResolver = eventNameResolver,
				JsonOptions = jsonOptions
			};

			config.EventNameResolver ??= domainEvent => domainEvent.GetType().ToString().ToLowerInvariant();

			services.AddScoped<IDomainEventProducer, EventGridDomainEventProducer>(serviceProvider =>
			{
				var logger = serviceProvider.GetRequiredService<ILogger<EventGridDomainEventProducer>>();
				return new EventGridDomainEventProducer(config, logger);
			});
			return services;
        }

        public static IServiceCollection AddEventGridDomainEventHandler(this IServiceCollection services, 
			string hostname, 
			string key, 
			Func<DomainEvent, bool>? filter = null, 
			string subject = "eventgridevent", 
			string dataVersion = "1.0", 
			Func<DomainEvent, string>? eventNameResolver = null, 
			JsonSerializerOptions? jsonOptions = null)
        {
            var config = new EventGridDomainEventPublisherConfiguration
            {
                Hostname = hostname,
                Key = key,
                Subject = subject,
                DataVersion = dataVersion,
                Filter = filter,
                EventNameResolver = eventNameResolver,
                JsonOptions = jsonOptions
            };

            if (config.EventNameResolver == null)
            {
                config.EventNameResolver = domainEvent => domainEvent?.GetType().ToString().ToLowerInvariant() ?? string.Empty;
            }

            var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(DomainEvent).IsAssignableFrom(type))
                .Distinct();

            foreach (var eventType in eventTypes)
            {
                services.AddTransient(typeof(INotificationHandler<>).MakeGenericType(eventType), serviceProvider =>
                {
                    var clock = serviceProvider.GetRequiredService<IClock>();
                    var logger = serviceProvider.GetRequiredService<ILogger<EventGridDomainEventHandler>>();
                    return new EventGridDomainEventHandler(config, clock, logger);
                });
            }

            return services;
        }
    }
}
