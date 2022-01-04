using System;
using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentCreated(Shipment Shipment) 
		: DomainEvent
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

	public record ShipmentRated(string ShipmentIdentifier, Rate CarrierRate)
		: DomainEvent
	{
	}

	public record ShipmentStatusUpdated(string ShipmentIdentifier, ShipmentStatus OldStatus, ShipmentStatus NewStatus)
		: DomainEvent
	{
	}

	public record ShipmentDispatched(string ShipmentIdentifier, Dispatch DispatchInfo)
		: DomainEvent
	{
	}

	public record TrackingEventAdded(string ShipmentIdentifier, TrackingEvent TrackingEvent)
		: DomainEvent
	{
	}

	public record ShipmentDelivered(string ShipmentIdentifier, LocalDateTime DeliveredAt, DateTimeOffset Occurred)
		: DomainEvent
	{
	}
}