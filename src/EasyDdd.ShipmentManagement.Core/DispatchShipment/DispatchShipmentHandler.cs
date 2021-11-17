using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core.Specifications;
using EasyDdd.Kernel;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core.DispatchShipment;

public class DispatchShipmentHandler : CommandHandler<DispatchShipmentCommand, Dispatch>
{
	private readonly IClock _clock;
	private readonly IDispatchNumberService _dispatchNumberService;
	private readonly ILogger<DispatchShipmentHandler> _logger;
	private readonly IRepository<Shipment> _shipmentRepo;

	public DispatchShipmentHandler(IRepository<Shipment> shipmentRepo,
		IDispatchNumberService dispatchNumberService,
		IClock clock,
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

		var shipment = (await _shipmentRepo.FindAsync(new ShipmentIdSpecification(command.ShipmentIdentifier))
				.ConfigureAwait(false))
			.SingleOrDefault();

		if (shipment == null)
		{
			_logger.LogError("Unable to dispatch shipment: {ShipmentIdentifier}. Shipment not found.", command.ShipmentIdentifier);
			throw new NotFoundException($"Shipment with id: {command.ShipmentIdentifier} was not found.");
		}

		var dispatchNumber = await _dispatchNumberService.ReserveNumber();

		shipment.Dispatch(dispatchNumber, command.DispatchRequest, command.User.Identity?.Name, _clock.GetCurrentInstant());

		await _shipmentRepo.SaveAsync(shipment);

		if (shipment.DispatchInfo == null)
		{
			_logger.LogError("Failed to dispatch shipment: {ShipmentIdentifier}.", shipment.Identifier);
			throw new Exception($"Failed to dispatch shipment: {command.ShipmentIdentifier}.");
		}

		_logger.LogInformation("Shipment: {ShipmentIdentifier} dispatched successfully with dispatch# {DispatchNumber}.", command.ShipmentIdentifier, shipment.DispatchInfo.DispatchNumber);

		return shipment.DispatchInfo;
	}
}