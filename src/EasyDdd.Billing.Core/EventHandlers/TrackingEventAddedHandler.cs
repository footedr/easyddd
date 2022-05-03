using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class TrackingEventAddedHandler : ExternalEventHandler<TrackingEventAdded>
	{
		private readonly IRepository<Shipment> _shipmentRepository;
		private readonly ILogger<TrackingEventAddedHandler> _logger;

		public TrackingEventAddedHandler(IRepository<Shipment> shipmentRepository,
			ILogger<TrackingEventAddedHandler> logger)
		{
			_shipmentRepository = shipmentRepository;
			_logger = logger;
		}

		public override async Task Handle(TrackingEventAdded @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(TrackingEventAdded), @event.ShipmentIdentifier);

			var shipment = (await _shipmentRepository.FindAsync(new ShipmentByIdSpecification(@event.ShipmentIdentifier)))
				.FirstOrDefault();
			if (shipment is null)
			{
				_logger.LogError("Unable to update latest tracking event for shipment #{ShipmentId}. Shipment not found.", @event.ShipmentIdentifier);
				return;
			}

			var latestTrackingEvent = new TrackingEvent(@event.TrackingEvent.Type.Description, @event.TrackingEvent.Occurred)
			{
				Comments = @event.TrackingEvent.Comments
			};

			shipment.UpdateLatestTrackingEvent(latestTrackingEvent);

			_logger.LogInformation("Updated shipment# {ShipmentId} with latest tracking event: {LatestTrackingEvent}.", shipment.Identifier, latestTrackingEvent.Type);
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record TrackingEventAdded(string ShipmentIdentifier, TrackingEvent TrackingEvent)
		: ExternalEvent
	{
	}

	public record TrackingEvent(TrackingEventType Type, LocalDateTime Occurred, Instant CreatedAt, string CreatedBy)
	{
		public string? Comments { get; init; }
	}

	public record TrackingEventType(string Description, string Code);
}