using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Kernel.EventGrid;

public class EventGridDomainEventProducer : IDomainEventProducer
{
	private readonly EventGridDomainEventPublisherConfiguration _configuration;
	private readonly ILogger<EventGridDomainEventProducer> _logger;

	public EventGridDomainEventProducer(EventGridDomainEventPublisherConfiguration configuration,
		ILogger<EventGridDomainEventProducer> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public async Task Produce(DomainEvent domainEvent, CancellationToken cancellationToken)
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