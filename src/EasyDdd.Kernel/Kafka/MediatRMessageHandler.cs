using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyDdd.Kernel.Kafka
{
    public record MediatRMessageHandlerOptions
    {
        public TypeList<INotification> NotificationTypes { get; } = new TypeList<INotification>();
        public string TypeNameHeader { get; set; } = "TypeName";
        public JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions();
    }

    public class MediatRMessageHandler : IKafkaMessageHandler<string, string>
    {
        private readonly IOptions<MediatRMessageHandlerOptions> _options;
        private readonly IMediator _mediator;
        private readonly ILogger<MediatRMessageHandler> _logger;

        public MediatRMessageHandler(IOptions<MediatRMessageHandlerOptions> options, IMediator mediator, ILogger<MediatRMessageHandler> logger)
        {
            _options = options;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task HandleMessage(Message<string, string> message, CancellationToken cancellationToken)
        {
            var eventTypeHeader = message.Headers.SingleOrDefault(h => h.Key.Equals(_options.Value.TypeNameHeader));
            if (eventTypeHeader is null)
            {
                _logger.LogWarning("Unable to handle message, missing {TypeNameHeader} header", _options.Value.TypeNameHeader);
                return;
            }

            var typeName = Encoding.ASCII.GetString(eventTypeHeader.GetValueBytes());
            var notificationType = _options.Value.NotificationTypes
                .FirstOrDefault(t => t.FullName?.Equals(typeName, StringComparison.OrdinalIgnoreCase) ?? false);

            if (notificationType is null)
            {
                _logger.LogInformation("Notification type with name {NotificationTypeName} not found, message not handled", typeName);
                return;
            }

            _logger.LogInformation("Handling {NotificationTypeName} message", typeName);

            var notification = JsonSerializer.Deserialize(message.Value, notificationType, _options.Value.JsonSerializerOptions);
            if (notification is null)
            {
                _logger.LogWarning("Unable to handle message, value deserialized to null");
                return;
            }

            await _mediator.Publish(notification, cancellationToken);
        }
    }
}
