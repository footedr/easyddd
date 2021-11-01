using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.Core
{
	public record ShipmentCreated(Shipment Shipment) : DomainEvent
	{
	}

	public record ShipmentDetailAdded(string ShipmentIdentifier, ShipmentDetail Detail)
		: DomainEvent
	{
	}

	public record ShipmentDetailUpdated(string ShipmentIdentifier, ShipmentDetail Detail)
		: DomainEvent
	{
	}

	public record RateQuoteDeselected(string ShipmentIdentifier, LocalDateTime? Updated)
		: DomainEvent
	{
	}

	public record ShipmentRated(string ShipmentIdentifier, Rate CarrierRate)
		: DomainEvent
	{
	}

	public record ShipmentStatusUpdated(string ShipmentIdentifier, ShipmentStatus OldStatus, ShipmentStatus NewStatus)
		: DomainEvent
	{
	}
}