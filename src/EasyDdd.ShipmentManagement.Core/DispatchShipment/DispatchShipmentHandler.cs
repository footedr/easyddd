using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core.Specifications;
using EasyDdd.Kernel;
using Microsoft.Extensions.Logging;

namespace EasyDdd.ShipmentManagement.Core.DispatchShipment;

public class DispatchShipmentHandler : CommandHandler<DispatchShipmentCommand, Dispatch>
{
	private readonly NodaTime.IClock _clock;
	private readonly IDispatchNumberService _dispatchNumberService;
	private readonly ILogger<DispatchShipmentHandler> _logger;
	private readonly IRepository<Shipment> _shipmentRepo;

	public DispatchShipmentHandler(IRepository<Shipment> shipmentRepo,
		IDispatchNumberService dispatchNumberService,
		NodaTime.IClock clock,
		ILogger<DispatchShipmentHandler> logger)
	{
		_shipmentRepo = shipmentRepo;
		_dispatchNumberService = dispatchNumberService;
		_clock = clock;
		_logger = logger;
	}

	public override async Task<Dispatch> Handle(DispatchShipmentCommand command, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Received command: {CommandName} from user: {UserIdentifier}.", nameof(command), command.User);

		var shipment = (await _shipmentRepo.FindAsync(new ShipmentIdSpecification(command.ShipmentId))
				.ConfigureAwait(false))
			.SingleOrDefault();

		if (shipment == null)
		{
			_logger.LogError("Unable to dispatch shipment: {ShipmentId}. Shipment not found.", command.ShipmentId);
			throw new NotFoundException<Shipment>(command.ShipmentId);
		}

		var dispatchNumber = await _dispatchNumberService.ReserveNumber();

		shipment.Dispatch(dispatchNumber, command.DispatchRequest, command.User.Identity?.Name, _clock.GetCurrentInstant());

		await _shipmentRepo.SaveAsync(shipment).ConfigureAwait(false);

		if (shipment.DispatchInfo == null)
		{
			_logger.LogError("Failed to dispatch shipment: {ShipmentId}.", shipment.Identifier);
			throw new Exception($"Failed to dispatch shipment: {command.ShipmentId}.");
		}

		_logger.LogInformation("Shipment: {ShipmentId} dispatched successfully with dispatch# {DispatchNumber}.", command.ShipmentId, shipment.DispatchInfo.DispatchNumber);

		return shipment.DispatchInfo;
	}
}