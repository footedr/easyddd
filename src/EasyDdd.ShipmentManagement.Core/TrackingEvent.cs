using System;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core;

public record TrackingEvent(TrackingEventType Type, LocalDateTime Occurred, Instant CreatedAt, string CreatedBy)
{
	[Obsolete("This is used by EF and is necessary due to issues w/ nested record types.")]
	public TrackingEvent() : this(default!, default!, default!, default!)
	{
	}

    public string? Comments { get; init; }
}