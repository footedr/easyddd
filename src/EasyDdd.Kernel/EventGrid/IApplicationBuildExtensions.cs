using System;
using System.Text.Json;
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
			var config = new EventGridPublisherConfiguration
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

			services.AddScoped(serviceProvider =>
			{
				var logger = serviceProvider.GetRequiredService<ILogger<EventGridDomainEventProducer>>();
				return new EventGridDomainEventProducer(config, logger);
			});
			return services;
        }
	}
}
