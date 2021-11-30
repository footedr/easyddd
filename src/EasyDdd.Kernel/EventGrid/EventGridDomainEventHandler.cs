using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

namespace EasyDdd.Kernel.EventGrid
{
    public class EventGridDomainEventHandler : DomainEventHandler<DomainEvent>
    {
        private readonly EventGridDomainEventPublisherConfiguration _configuration;
        private readonly IClock _clock;
        private readonly ILogger<EventGridDomainEventHandler> _logger;

        public EventGridDomainEventHandler(EventGridDomainEventPublisherConfiguration configuration, IClock clock, ILogger<EventGridDomainEventHandler> logger)
        {
            _configuration = configuration;
            _clock = clock;
            _logger = logger;
        }

        public override async Task Handle(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var eventGridEvent = new EventGridEvent(
                domainEvent.EventId.ToString(),
                _configuration.Subject,
                JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _configuration.JsonOptions),
                _configuration.EventNameResolver?.Invoke(domainEvent),
                _clock.Now().UtcDateTime,
                _configuration.DataVersion);

            if (_configuration.Filter != null && !_configuration.Filter(domainEvent))
            {
                _logger.LogInformation("Event {EventType} {EventId} excluded by filter", eventGridEvent.EventType, eventGridEvent.Id);
                return;
            }

            var headers = new Dictionary<string, List<string>> { { "aeg-sas-key", new List<string> { _configuration.Key! } } };
            var credentials = new TokenCredentials(_configuration.Key);
            var client = new EventGridClient(credentials);
            await client.PublishEventsWithHttpMessagesAsync(_configuration.Hostname, new[] { eventGridEvent }, headers, cancellationToken);

            _logger.LogInformation("Published event {EventType} {EventId}", eventGridEvent.EventType, eventGridEvent.Id);
        }
    }
}
