using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventGrid;

public class EventGridDomainEventProducer : IDomainEventProducer
{
	private readonly EventGridPublisherConfiguration _configuration;
	private readonly ILogger<EventGridDomainEventProducer> _logger;

	public EventGridDomainEventProducer(EventGridPublisherConfiguration configuration,
		ILogger<EventGridDomainEventProducer> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public async Task Produce(IReadOnlyList<DomainEvent> domainEvents, CancellationToken cancellationToken)
	{
		var hostname = _configuration.Hostname ?? string.Empty;
		var azureKeyCredential = _configuration.Key ?? string.Empty;

		var client = new EventGridPublisherClient(new Uri(hostname), new AzureKeyCredential(azureKeyCredential));
		var events = new List<EventGridEvent>();

		foreach (var domainEvent in domainEvents)
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

			events.Add(eventGridEvent);
		}
		
		await client.SendEventsAsync(events, cancellationToken);
		
		_logger.LogInformation($"Published event batch to EventGrid.");
	}
}