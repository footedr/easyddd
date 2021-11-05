using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using EasyDdd.Kernel;

namespace EasyDdd.Core;

public class Carrier : SimpleValueObject<string>
{
	private Carrier(string code, string name) : base(code)
	{
		Name = name;
		Code = code;
	}

	public static Carrier DaytonFreight { get; } = new("DAFG", "Dayton Freight Lines");
	public static Carrier Southeastern { get; } = new("SEFL", "Southeastern");
	public static Carrier Pyle { get; } = new("PYLE", "A. Duie Pyle");
	public static Carrier Manitoulin { get; } = new("MANI", "Manitoulin Transport");
	public static Carrier Midwest { get; } = new("MME", "Midwest Motor Express");
	public static Carrier OakHarbor { get; } = new("OAKH", "Oak Harbor Freight Lines");

	public string Name { get; }
	public string Code { get; }

	public static IReadOnlyList<Carrier> AllCarriers { get; } = new[]
	{
		DaytonFreight, Southeastern, Pyle, Manitoulin, Midwest, OakHarbor
	};

	public static bool TryCreate(string? carrierCode, [NotNullWhen(true)] out Carrier? carrier, [NotNullWhen(false)] out string? errorMessage)
	{
		carrier = AllCarriers.SingleOrDefault(x => x.Value.Equals(carrierCode, StringComparison.OrdinalIgnoreCase));

		if (carrier == null)
		{
			errorMessage = $"Unrecognized carrier: '{carrierCode ?? "(NULL)"}'";
			return false;
		}

		errorMessage = null;
		return true;
	}

	public static Carrier Create(string carrierCode)
	{
		if (!TryCreate(carrierCode, out var carrier, out var errorMessage)) throw new FormatException(errorMessage);

		return carrier;
	}

	public static implicit operator Carrier(string carrierCode)
	{
		return Create(carrierCode);
	}

	public static implicit operator string(Carrier carrier)
	{
		return carrier.Code;
	}

	protected override ITuple AsTuple()
	{
		return (Code, Name);
	}
}