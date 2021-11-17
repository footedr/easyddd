using System;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core;

public record Dispatch(DispatchNumber DispatchNumber,
	string PickupNumber,
	LocalDateTime DispatchDateTime,
	string CreatedBy,
	Instant Created)
{
	[Obsolete("This is used by EF and is necessary due to issues w/ nested record types.")]
	private Dispatch() : this(default!, default!, default!, default!, default!)
	{
	}

	public string? PickupNote { get; init; }
	public string? ReferenceNumber { get; init; }
}