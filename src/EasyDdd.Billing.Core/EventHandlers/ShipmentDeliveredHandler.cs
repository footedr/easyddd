using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
    public class ShipmentDeliveredHandler : ExternalEventHandler<ShipmentDelivered>
    {
		private readonly ILogger<ShipmentDeliveredHandler> _logger;
		private readonly IRepository<Shipment> _shipmentRepository;

		public ShipmentDeliveredHandler(ILogger<ShipmentDeliveredHandler> logger,
			IRepository<Shipment> shipmentRepository)
		{
			_logger = logger;
			_shipmentRepository = shipmentRepository;
		}

		public override async Task Handle(ShipmentDelivered @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment# {ShipmentIdentifier}.", nameof(ShipmentDelivered), @event.ShipmentIdentifier);

			var shipment = (await _shipmentRepository.FindAsync(new ShipmentByIdSpecification(@event.ShipmentIdentifier))
					.ConfigureAwait(false))
				.SingleOrDefault();

			if (shipment == null)
			{
				_logger.LogError("Attempting to deliver shipment with id: {ShipmentId}, but shipment was not found.", @event.ShipmentIdentifier);
				return;
			}

			shipment.UpdateDeliveryDate(@event.DeliveredAt);

			await _shipmentRepository.SaveAsync(shipment);

			_logger.LogInformation("Shipment with id: {ShipmentId} was delivered successfully.", @event.ShipmentIdentifier);
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