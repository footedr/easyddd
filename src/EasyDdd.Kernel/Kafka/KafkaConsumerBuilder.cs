using Microsoft.Extensions.DependencyInjection;

namespace EasyDdd.Kernel.Kafka
{
    public class KafkaConsumerBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public KafkaConsumerBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public KafkaConsumerBuilder AddMediatRMessageHandler(Action<MediatRMessageHandlerOptions>? action = null)
        {
            if (action == null)
            {
                action = _ => { };
            }

            _serviceCollection.Configure(action);

            _serviceCollection.PostConfigure<MediatRMessageHandlerOptions>(options =>
            {
                if (!options.NotificationTypes.Any())
                {
                    options.NotificationTypes.AddImplementationsFromAllAssemblies();
                }
            });

            _serviceCollection.AddTransient<IKafkaMessageHandler<string, string>, MediatRMessageHandler>();

            return this;
        }

        public KafkaConsumerBuilder AddDeadLetterExceptionHandler()
        {
            _serviceCollection.AddSingleton<IKafkaMessageExceptionHandler<string, string>, DeadLetterExceptionHandler<string, string>>();

            return this;
        }
    }
}
