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
}