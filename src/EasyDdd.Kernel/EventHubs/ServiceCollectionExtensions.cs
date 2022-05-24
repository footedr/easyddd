using System.Collections.Generic;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventHubs;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKafkaDomainEventProducer(this IServiceCollection services,
		KafkaPublisherConfiguration configuration)
	{
		services.AddSingleton(serviceProvider =>
		{
			var producerConfig = new ProducerConfig(new Dictionary<string, string>
			{
				{ "bootstrap.servers", configuration.Endpoint }
			});

			if (configuration is KafkaPublisherWithSaslConfiguration configWithSasl)
			{
				producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
				producerConfig.SaslMechanism = SaslMechanism.Plain;
				producerConfig.SaslUsername = "$ConnectionString";
				producerConfig.SaslPassword = configWithSasl.ConnectionString;
			}
			
			return new ProducerBuilder<string, string>(producerConfig).Build();
		});

		services.AddScoped(serviceProvider =>
		{
			var producer = serviceProvider.GetRequiredService<IProducer<string, string>>();
			var logger = serviceProvider.GetRequiredService<ILogger<KafkaDomainEventProducer>>();
			return new KafkaDomainEventProducer(producer, configuration.JsonSerializerOptions, logger);
		});

		return services;
	}

	public static IServiceCollection AddKafkaDomainEventConsumer(this IServiceCollection services,
		KafkaConsumerConfiguration configuration)
	{
		services.AddSingleton<IHostedService, KafkaDomainEventConsumer>(serviceProvider =>
		{
			var logger = serviceProvider.GetRequiredService<ILogger<KafkaDomainEventConsumer>>();
			var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

			return new KafkaDomainEventConsumer(configuration, scopeFactory, logger);
		});
		return services;
	}
}