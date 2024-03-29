﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.Kernel;
using Microsoft.Extensions.Logging;

namespace EasyDdd.ShipmentManagement.Core.CreateShipment
{
	public class CreateShipmentHandler : CommandHandler<CreateShipmentCommand, Shipment>
	{
		private readonly ILogger<CreateShipmentHandler> _logger;
		private readonly IShipmentIdService _shipmentIdService;
		private readonly IRepository<Shipment> _shipmentRepository;
		private readonly NodaTime.IClock _clock;

		public CreateShipmentHandler(IShipmentIdService shipmentIdService,
			IRepository<Shipment> shipmentRepository, 
			NodaTime.IClock clock,
			ILogger<CreateShipmentHandler> logger)
		{
			_shipmentIdService = shipmentIdService;
			_shipmentRepository = shipmentRepository;
			_clock = clock;
			_logger = logger;
		}

		public override async Task<Shipment> Handle(CreateShipmentCommand command, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Handling CreateShipmentCommand for user: {User}.", command.User.Identity?.Name);

			if (command.User.Identity?.Name == null)
			{
				throw new ArgumentNullException(nameof(command.User.Identity), "Creating a shipment requires a valid user.");
			}

			var shipmentId = await _shipmentIdService.ReserveId().ConfigureAwait(false);
			var shipment = new Shipment(shipmentId,
				command.Shipment.ReadyWindow,
				command.Shipment.Shipper,
				command.Shipment.Consignee,
				command.Shipment.Details,
				_clock.GetCurrentInstant(),
				command.User.Identity.Name);

			await _shipmentRepository.SaveAsync(shipment).ConfigureAwait(false);

			_logger.LogInformation("Shipment #{ShipmentId} created for user: {User}.", shipment.Identifier, command.User.Identity?.Name);

			return shipment;
		}
	}
}