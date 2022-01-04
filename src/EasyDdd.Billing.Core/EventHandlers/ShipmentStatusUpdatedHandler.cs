using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentStatusUpdatedHandler : ExternalEventHandler<ShipmentStatusUpdated>
	{
		private readonly ILogger<ShipmentStatusUpdatedHandler> _logger;
		private readonly IRepository<Shipment> _repository;

		public ShipmentStatusUpdatedHandler(ILogger<ShipmentStatusUpdatedHandler> logger,
			IRepository<Shipment> repository)
		{
			_logger = logger;
			_repository = repository;
		}

		public override async Task Handle(ShipmentStatusUpdated @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentStatusUpdated), @event.ShipmentIdentifier);

			var shipment = (await _repository.FindAsync(new ShipmentByIdSpecification(@event.ShipmentIdentifier))
					.ConfigureAwait(false))
				.SingleOrDefault();

			if (shipment == null)
			{
				_logger.LogError("Attempted to update the status for shipment with id: {ShipmentId}, but shipment was not found.", @event.ShipmentIdentifier);
				return;
			}

			shipment.UpdateStatus(@event.NewStatus);

			await _repository.SaveAsync(shipment);

			_logger.LogInformation("Updated status for shipment with id: {ShipmentId}. Previous status was: {OldStatus}. New status is: {NewStatus}.", 
				@event.ShipmentIdentifier, 
				@event.OldStatus, 
				@event.NewStatus);
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentStatusUpdated(string ShipmentIdentifier, string OldStatus, string NewStatus)
		: ExternalEvent
	{
	}
}