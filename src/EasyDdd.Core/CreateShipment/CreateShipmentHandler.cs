using System.Threading;
using System.Threading.Tasks;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Core.CreateShipment
{
	public class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, Shipment>
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

		public async Task<Shipment> Handle(CreateShipmentCommand command, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Handling CreateShipmentCommand for user: {User}.", command.User.Identity?.Name);

			var shipment = new Shipment(command.Shipment.ReadyWindow, command.Shipment.Shipper, command.Shipment.Consignee, command.Shipment.Details, _clock.GetCurrentInstant());
			await _shipmentRepository.SaveAsync(shipment);

			_logger.LogInformation("Shipment #{ShipmentId} created for user: {User}.", shipment.Identifier, command.User.Identity?.Name);

			return shipment;
		}
	}
}