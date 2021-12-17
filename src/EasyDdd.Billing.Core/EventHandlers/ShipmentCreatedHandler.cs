using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentCreatedHandler : ExternalEventHandler<ShipmentCreated>
	{
		private readonly ILogger<ShipmentCreatedHandler> _logger;

		public ShipmentCreatedHandler(ILogger<ShipmentCreatedHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Handle(ShipmentCreated notification, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment #{ShipmentId}.", nameof(ShipmentCreated), notification.Shipment.Identifier);

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