using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core.Specifications;
using EasyDdd.Kernel;
using Microsoft.Extensions.Logging;

namespace EasyDdd.ShipmentManagement.Core.Tracking;

public class AddTrackingEventHandler : CommandHandler<AddTrackingEventCommand, TrackingEvent>
{
	private readonly NodaTime.IClock _clock;
	private readonly ILogger<AddTrackingEventHandler> _logger;
	private readonly IRepository<Shipment> _shipmentRepo;

	public AddTrackingEventHandler(IRepository<Shipment> shipmentRepo, NodaTime.IClock clock, ILogger<AddTrackingEventHandler> logger)
	{
		_shipmentRepo = shipmentRepo;
		_clock = clock;
		_logger = logger;
	}

	public override async Task<TrackingEvent> Handle(AddTrackingEventCommand command, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Received command: {CommandName} from user: {UserIdentifier}.", nameof(command), command.User);

		var shipment = (await _shipmentRepo.FindAsync(new ShipmentIdSpecification(command.ShipmentIdentifier))
				.ConfigureAwait(false))
			.SingleOrDefault();

		if (shipment is null)
		{
			_logger.LogError("Unable to add tracking event for shipment: {ShipmentIdentifier}. Shipment not found.", command.ShipmentIdentifier);
			throw new NotFoundException($"Shipment with id: {command.ShipmentIdentifier} was not found.");
		}

		shipment.AddTrackingEvent(command.TrackingEventRequest, command.User.Identity?.Name, _clock.GetCurrentInstant());

		await _shipmentRepo.SaveAsync(shipment);

		_logger.LogInformation("Tracking event successfully added to shipment# {ShipmentIdentifier}.", command.ShipmentIdentifier);

		return shipment.TrackingHistory.Last();
	}
}