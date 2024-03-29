﻿using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core
{
    public abstract record ShipmentDomainEvent(ShipmentId ShipmentId) : DomainEvent
    {
        public static string BoundedContextName => "ShipmentManagement";
        public static string CollectionName => "Shipments";

        public override AggregateIdentifier GetAggregateIdentifier() => new (BoundedContextName, CollectionName, ShipmentId.Value);
    }

	public record ShipmentCreated(Shipment Shipment) : ShipmentDomainEvent(Shipment.Identifier)
	{ 
	}

	public record ShipmentDetailAdded(string ShipmentIdentifier, ShipmentDetail Detail)
		: ShipmentDomainEvent(ShipmentIdentifier)
	{
	}

	public record ShipmentDetailUpdated(string ShipmentIdentifier, ShipmentDetail Detail)
		: ShipmentDomainEvent(ShipmentIdentifier)
	{
	}

	public record ShipmentRated(string ShipmentIdentifier, Rate CarrierRate)
		: ShipmentDomainEvent(ShipmentIdentifier)
	{
	}

	public record ShipmentStatusUpdated(string ShipmentIdentifier, ShipmentStatus OldStatus, ShipmentStatus NewStatus)
		: ShipmentDomainEvent(ShipmentIdentifier)
	{
	}

	public record ShipmentDispatched(string ShipmentIdentifier, Dispatch DispatchInfo)
		: ShipmentDomainEvent(ShipmentIdentifier)
	{
	}

	public record TrackingEventAdded(string ShipmentIdentifier, TrackingEvent TrackingEvent)
		: ShipmentDomainEvent(ShipmentIdentifier)
	{
	}

	public record ShipmentDelivered(string ShipmentIdentifier, LocalDateTime DeliveredAt, DateTimeOffset Occurred)
		: ShipmentDomainEvent(ShipmentIdentifier)
	{
	}
}