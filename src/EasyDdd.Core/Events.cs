using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.Core
{
	public record ShipmentCreated(Shipment Shipment) : DomainEvent
	{
	}

	public record LineItemAdded(string ShipmentIdentifier, ShipmentDetailLine LineItem)
		: DomainEvent
	{
	}

	public record RateQuoteDeselected(string ShipmentIdentifier, LocalDateTime? Updated)
		: DomainEvent
	{
	}
}