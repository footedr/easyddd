using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using EasyDdd.Kernel;

namespace EasyDdd.Core;

public class TrackingEventType : ValueObject, ISimpleValueObject<string>
{
	[Obsolete("This is used by EF and is necessary due to issues w/ nested record types.")]
	private TrackingEventType()
	{
		Code = default!;
		Description = default!;
	}

	private TrackingEventType(string code, string description, ShipmentStatus? correspondingStatus)
	{
		Code = code;
		Description = description;
		CorrespondingStatus = correspondingStatus;
	}

	public static TrackingEventType PickedUp { get; } = new("PU", "Picked Up", ShipmentStatus.InTransit);
	public static TrackingEventType OverTheRoad { get; } = new("OTR", "Over the Road", ShipmentStatus.InTransit);
	public static TrackingEventType OutForDelivery { get; } = new("OFD", "Out for Delivery", ShipmentStatus.InTransit);
	public static TrackingEventType Delivered { get; } = new("DLV", "Delivered", ShipmentStatus.Delivered);
	public static TrackingEventType Other { get; } = new("OTH", "Other", default);

	public string Code { get; }
	public string Description { get; }
	public ShipmentStatus? CorrespondingStatus { get; }

	public static IReadOnlyList<TrackingEventType> All { get; } = new[]
	{
		PickedUp,
		OverTheRoad,
		OutForDelivery,
		Delivered,
		Other
	};

	string ISimpleValueObject<string>.Value => Code;

	protected override ITuple AsTuple()
	{
		return (Code, Description, CorrespondingStatus);
	}

	public static bool TryCreate(string? code, [NotNullWhen(true)] out TrackingEventType? trackingEventType,
		[NotNullWhen(false)] out string? errorMessage)
	{
		trackingEventType = All.SingleOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

		if (trackingEventType is null)
		{
			errorMessage = $"Unrecognized tracking event type: '{code ?? "(NULL)"}'";
			return false;
		}

		errorMessage = null;
		return true;
	}

	public static TrackingEventType Create(string code)
	{
		if (!TryCreate(code, out var trackingEventType, out var errorMessage)) throw new FormatException(errorMessage);

		return trackingEventType;
	}

	public static implicit operator TrackingEventType(string code)
	{
		return Create(code);
	}

	public static implicit operator string(TrackingEventType trackingEventType)
	{
		return trackingEventType.Code;
	}
}