using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentDispatchedHandler : ExternalEventHandler<ShipmentDispatched>
	{
		private readonly ILogger<ShipmentDispatchedHandler> _logger;

		public ShipmentDispatchedHandler(ILogger<ShipmentDispatchedHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Handle(ShipmentDispatched notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentDispatched), notification.ShipmentIdentifier);

			await Task.CompletedTask;
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentDispatched(string ShipmentIdentifier, Dispatch DispatchInfo)
		: ExternalEvent
	{
	}

	public record Dispatch(string DispatchNumber,
		string PickupNumber,
		LocalDateTime DispatchDateTime,
		string CreatedBy,
		Instant Created)
	{
		public string? PickupNote { get; init; }
		public string? ReferenceNumber { get; init; }
	}
}