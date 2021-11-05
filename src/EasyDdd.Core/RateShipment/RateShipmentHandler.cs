using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Core.RateShipment
{
	public class RateShipmentHandler : IRequestHandler<RateShipmentCommand, Rate>
	{
		private readonly ILogger<RateShipmentHandler> _logger;
		private readonly IRepository<Shipment> _shipmentRepo;

		public RateShipmentHandler(IRepository<Shipment> shipmentRepo, ILogger<RateShipmentHandler> logger)
		{
			_shipmentRepo = shipmentRepo;
			_logger = logger;
		}

		public async Task<Rate> Handle(RateShipmentCommand command, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received command: {CommandName} from user: {UserIdentifier}.", nameof(command), command.User);

			var shipment = (await _shipmentRepo.FindAsync(new ShipmentIdSpecification(command.ShipmentIdentifier))
					.ConfigureAwait(false))
				.SingleOrDefault();

			if (shipment == null)
			{
				_logger.LogError("Unable to rate shipment: {ShipmentIdentifier}. Shipment not found.", command.ShipmentIdentifier);
				throw new NotFoundException($"Shipment with id: {command.ShipmentIdentifier} was not found.");
			}

			shipment.Rate(command.RateRequest);

			await _shipmentRepo.SaveAsync(shipment);

			if (shipment.CarrierRate == null)
			{
				_logger.LogError("Failed to rate shipment: {ShipmentIdentifier}.", shipment.Identifier);
				throw new Exception($"Failed to rate shipment: {command.ShipmentIdentifier}.");
			}

			_logger.LogInformation("Shipment: {ShipmentIdentifier} rated successfully.", command.ShipmentIdentifier);
			return shipment.CarrierRate;
		}
	}
}