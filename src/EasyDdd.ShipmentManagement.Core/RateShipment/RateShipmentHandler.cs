using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core.Specifications;
using EasyDdd.Kernel;
using Microsoft.Extensions.Logging;

namespace EasyDdd.ShipmentManagement.Core.RateShipment
{
	public class RateShipmentHandler : CommandHandler<RateShipmentCommand, Rate>
	{
		private readonly ILogger<RateShipmentHandler> _logger;
		private readonly IRepository<Shipment> _shipmentRepo;

		public RateShipmentHandler(IRepository<Shipment> shipmentRepo, ILogger<RateShipmentHandler> logger)
		{
			_shipmentRepo = shipmentRepo;
			_logger = logger;
		}

		public override async Task<Rate> Handle(RateShipmentCommand command, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received command: {CommandName} from user: {UserIdentifier}.", nameof(command), command.User);

			var shipment = (await _shipmentRepo.FindAsync(new ShipmentIdSpecification(command.ShipmentId))
					.ConfigureAwait(false))
				.SingleOrDefault();

			if (shipment == null)
			{
				_logger.LogError("Unable to rate shipment: {ShipmentId}. Shipment not found.", command.ShipmentId);
				throw new NotFoundException($"Shipment with id: {command.ShipmentId} was not found.");
			}

			shipment.Rate(command.RateRequest);

			await _shipmentRepo.SaveAsync(shipment).ConfigureAwait(false);

			if (shipment.CarrierRate == null)
			{
				_logger.LogError("Failed to rate shipment: {ShipmentId}.", shipment.Identifier);
				throw new Exception($"Failed to rate shipment: {command.ShipmentId}.");
			}

			_logger.LogInformation("Shipment: {ShipmentId} rated successfully.", command.ShipmentId);
			return shipment.CarrierRate;
		}
	}
}