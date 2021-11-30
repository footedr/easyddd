using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
    public class ShipmentDeliveredHandler : ExternalEventHandler<ShipmentDelivered>
    {
		private readonly ILogger<ShipmentDeliveredHandler> _logger;

		public ShipmentDeliveredHandler(ILogger<ShipmentDeliveredHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Handle(ShipmentDelivered @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment# {ShipmentIdentifier}.", nameof(@event), @event.ShipmentIdentifier);

			await Task.CompletedTask;
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentDelivered(string ShipmentIdentifier, LocalDateTime DeliveredAt, DateTimeOffset Occurred)
		: ExternalEvent
	{
	}
}