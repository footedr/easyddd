using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentStatusUpdatedHandler : ExternalEventHandler<ShipmentStatusUpdated>
	{
		private readonly ILogger<ShipmentStatusUpdatedHandler> _logger;

		public ShipmentStatusUpdatedHandler(ILogger<ShipmentStatusUpdatedHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Handle(ShipmentStatusUpdated notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentStatusUpdated), notification.ShipmentIdentifier);

			await Task.CompletedTask;
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentStatusUpdated(string ShipmentIdentifier, ShipmentStatus OldStatus, ShipmentStatus NewStatus)
		: ExternalEvent
	{
	}

	public record ShipmentStatus(string Description, string Code);
}