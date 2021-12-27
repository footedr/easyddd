using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentCreatedHandler : ExternalEventHandler<ShipmentCreated>
	{
		private readonly ILogger<ShipmentCreatedHandler> _logger;
		private readonly IRepository<Shipment> _repository;

		public ShipmentCreatedHandler(ILogger<ShipmentCreatedHandler> logger,
			IRepository<Shipment> repository)
		{
			_logger = logger;
			_repository = repository;
		}

		public override async Task Handle(ShipmentCreated @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentCreated), @event.Shipment.Identifier);

			var shipment = new Shipment(@event.Shipment.Identifier,
				new Address(@event.Shipment.Shipper.Address.Line1,
					@event.Shipment.Shipper.Address.City,
					@event.Shipment.Shipper.Address.StateAbbreviation,
					@event.Shipment.Shipper.Address.PostalCode),
				new Address(@event.Shipment.Consignee.Address.Line1,
					@event.Shipment.Consignee.Address.City,
					@event.Shipment.Consignee.Address.StateAbbreviation,
					@event.Shipment.Consignee.Address.PostalCode),
				@event.Shipment.Status,
				@event.Shipment.CreatedBy,
				@event.Shipment.Details.Select(d => new ShipmentDetail(d.Class, d.Weight, d.HandlingUnitCount, d.IsHazardous)).ToList());

			await _repository.SaveAsync(shipment);

			_logger.LogInformation("Shipment with id: {ShipmentId} has been saved to Billing schema.", shipment.Identifier);
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentCreated(Shipment Shipment) : ExternalEvent;

	public record Shipment(string Identifier,
		AppointmentWindow ReadyWindow,
		Location Shipper,
		Location Consignee,
		IReadOnlyList<ShipmentDetail> Details,
		Instant CreatedAt,
		string Status,
		string CreatedBy);

	public record AppointmentWindow(LocalDate Date, LocalTime Start, LocalTime End);

	public record Location(Address Address, Contact Contact);

	public record Address(string Line1, string City, string StateAbbreviation, string PostalCode);

	public record Contact(string Name)
	{
		public string? Phone { get; init; }
		public string? Email { get; init; }
	}

	public record ShipmentDetail(string Description, string Class, int Weight, int HandlingUnitCount, string PackagingType, bool IsHazardous);
}