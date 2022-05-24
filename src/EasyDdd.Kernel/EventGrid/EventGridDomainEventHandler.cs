using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventGrid;

public class EventGridDomainEventHandler : DomainEventHandler<DomainEvent>
{
	private readonly IClock _clock;
	private readonly EventGridPublisherConfiguration _configuration;
	private readonly ILogger<EventGridDomainEventHandler> _logger;

	public EventGridDomainEventHandler(EventGridPublisherConfiguration configuration, 
		IClock clock, 
		ILogger<EventGridDomainEventHandler> logger)
	{
		_configuration = configuration;
		_clock = clock;
		_logger = logger;
	}

	public override async Task Handle(DomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var eventGridEvent = new EventGridEvent(_configuration.Subject,
			_configuration.EventNameResolver?.Invoke(domainEvent),
			_configuration.DataVersion,
			JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _configuration.JsonOptions));

		if (_configuration.Filter != null && !_configuration.Filter(domainEvent))
		{
			_logger.LogInformation($"Event {eventGridEvent.EventType} {eventGridEvent.Id} excluded by filter");
			return;
		}

		var hostname = _configuration.Hostname ?? string.Empty;
		var azureKeyCredential = _configuration.Key ?? string.Empty;

		var client = new EventGridPublisherClient(new Uri(hostname), new AzureKeyCredential(azureKeyCredential));
		
		await client.SendEventAsync(eventGridEvent, cancellationToken);

		_logger.LogInformation($"Published event {eventGridEvent.EventType} {eventGridEvent.Id}");
	}
}