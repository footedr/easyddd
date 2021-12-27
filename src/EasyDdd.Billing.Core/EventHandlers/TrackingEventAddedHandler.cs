using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class TrackingEventAddedHandler : ExternalEventHandler<TrackingEventAdded>
	{
		private readonly ILogger<TrackingEventAddedHandler> _logger;

		public TrackingEventAddedHandler(ILogger<TrackingEventAddedHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Handle(TrackingEventAdded @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(TrackingEventAdded), @event.ShipmentIdentifier);

			await Task.CompletedTask;
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