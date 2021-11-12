using System;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.Kernel;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Core.CreateShipment
{
	public class CreateShipmentHandler : CommandHandler<CreateShipmentCommand, Shipment>
	{
		private readonly ILogger<CreateShipmentHandler> _logger;
		private readonly IRepository<Shipment> _shipmentRepository;
		private readonly IClock _clock;

		public CreateShipmentHandler(IRepository<Shipment> shipmentRepository, 
			IClock clock,
			ILogger<CreateShipmentHandler> logger)
		{
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

			var shipment = new Shipment(command.Shipment.ReadyWindow, command.Shipment.Shipper, command.Shipment.Consignee, command.Shipment.Details, _clock.GetCurrentInstant(), command.User.Identity.Name);
			await _shipmentRepository.SaveAsync(shipment);

			_logger.LogInformation("Shipment #{ShipmentId} created for user: {User}.", shipment.Identifier, command.User.Identity?.Name);

			return shipment;
		}
	}
}