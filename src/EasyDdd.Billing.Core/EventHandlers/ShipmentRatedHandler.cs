using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentRatedHandler : ExternalEventHandler<ShipmentRated>
	{
		private readonly ILogger<ShipmentRatedHandler> _logger;
		private readonly IRepository<Shipment> _repository;

		public ShipmentRatedHandler(ILogger<ShipmentRatedHandler> logger,
			IRepository<Shipment> repository)
		{
			_logger = logger;
			_repository = repository;
		}

		public override async Task Handle(ShipmentRated @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentRated), @event.ShipmentIdentifier);

			var shipment = (await _repository.FindAsync(new ShipmentByIdSpecification(@event.ShipmentIdentifier))
				.ConfigureAwait(false)).SingleOrDefault();

			if (shipment == null)
			{
				_logger.LogError("Attempted to update rate info for shipment with id: {ShipmentId}. Shipment was not found.", @event.ShipmentIdentifier);
				return;
			}

			var carrier = new Carrier(@event.CarrierRate.Carrier.Name, @event.CarrierRate.Carrier.Code);

			shipment.UpdateRateInfo(carrier, @event.CarrierRate.ChargeTotal);

			await _repository.SaveAsync(shipment);

			_logger.LogInformation("Rating information for shipment with id: {ShipmentId} has been updated successfully.", @event.ShipmentIdentifier);
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