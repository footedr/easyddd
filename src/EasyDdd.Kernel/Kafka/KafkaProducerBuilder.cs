using Microsoft.Extensions.DependencyInjection;

namespace EasyDdd.Kernel.Kafka
{
    public class KafkaProducerBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public KafkaProducerBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public KafkaProducerBuilder AddDomainEventPublisher(Action<KafkaDomainEventPublisherOptions>? action = null)
        {
            if (action == null)
            {
                action = _ => { };
            }

            _serviceCollection.Configure(action);

            _serviceCollection.AddScoped<IDomainEventPublisher, KafkaDomainEventPublisher>();

            return this;
        }
    }
}