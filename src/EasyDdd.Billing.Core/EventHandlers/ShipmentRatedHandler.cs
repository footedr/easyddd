using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentRatedHandler : ExternalEventHandler<ShipmentRated>
	{
		private readonly ILogger<ShipmentRatedHandler> _logger;

		public ShipmentRatedHandler(ILogger<ShipmentRatedHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Handle(ShipmentRated notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentRated), notification.ShipmentIdentifier);

			await Task.CompletedTask;
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentRated(string ShipmentIdentifier, Rate CarrierRate)
		: ExternalEvent
	{
	}

	public record Rate(Carrier Carrier,
		decimal FuelCharge,
		decimal DiscountAmount,
		decimal ChargeTotal,
		decimal Total,
		IReadOnlyList<Charge> Charges);

	public record Charge(decimal Amount, string Description);

	public record Carrier(string Name, string Code);
}