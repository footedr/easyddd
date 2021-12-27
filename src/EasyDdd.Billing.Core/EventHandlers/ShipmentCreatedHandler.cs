using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

		public override async Task Handle(ShipmentCreated notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentCreated), notification.Shipment.Identifier);

			var shipment = new Shipment(notification.Shipment.Identifier,
				new Address(notification.Shipment.Shipper.Address.Line1,
					notification.Shipment.Shipper.Address.City,
					notification.Shipment.Shipper.Address.StateAbbreviation,
					notification.Shipment.Shipper.Address.PostalCode),
				new Address(notification.Shipment.Consignee.Address.Line1,
					notification.Shipment.Consignee.Address.City,
					notification.Shipment.Consignee.Address.StateAbbreviation,
					notification.Shipment.Consignee.Address.PostalCode),
				notification.Shipment.Status,
				notification.Shipment.CreatedBy,
				notification.Shipment.Details.Select(d => new ShipmentDetail(d.Class, d.Weight, d.HandlingUnitCount, d.IsHazardous)).ToList());

			await Task.CompletedTask;
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