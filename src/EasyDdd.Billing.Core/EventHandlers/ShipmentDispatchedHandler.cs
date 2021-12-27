using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentDispatchedHandler : ExternalEventHandler<ShipmentDispatched>
	{
		private readonly ILogger<ShipmentDispatchedHandler> _logger;
		private readonly IRepository<Shipment> _repository;

		public ShipmentDispatchedHandler(ILogger<ShipmentDispatchedHandler> logger,
			IRepository<Shipment> repository)
		{
			_logger = logger;
			_repository = repository;
		}

		public override async Task Handle(ShipmentDispatched @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentDispatched), @event.ShipmentIdentifier);

			var shipment = (await _repository.FindAsync(new ShipmentByIdSpecification(@event.ShipmentIdentifier))
					.ConfigureAwait(false))
				.SingleOrDefault();
			if (shipment == null)
			{
				_logger.LogError("Attempted to update dispatch info for shipment with id: {ShipmentId}. Shipment was not found.", @event.ShipmentIdentifier);
				return;
			}

			var dispatchInfo = new DispatchInfo(@event.DispatchInfo.DispatchNumber, @event.DispatchInfo.PickupNumber, @event.DispatchInfo.DispatchDateTime)
			{
				PickupNote = @event.DispatchInfo.PickupNote,
				ReferenceNumber = @event.DispatchInfo.ReferenceNumber
			};

			shipment.UpdateDispatchInfo(dispatchInfo);

			await _repository.SaveAsync(shipment);

			_logger.LogInformation("Dispatch info for shipment with id: {ShipmentId} was updated successfully.", @event.ShipmentIdentifier);
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentDispatched(string ShipmentIdentifier, Dispatch DispatchInfo)
		: ExternalEvent
	{
	}

	public record Dispatch(string DispatchNumber,
		string PickupNumber,
		LocalDateTime DispatchDateTime,
		string CreatedBy,
		Instant Created)
	{
		public string? PickupNote { get; init; }
		public string? ReferenceNumber { get; init; }
	}
}