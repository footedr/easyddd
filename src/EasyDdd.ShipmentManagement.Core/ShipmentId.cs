using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core;

public class ShipmentId : SimpleValueObject<string>
{
	private const string ShipmentIdPattern = "^TMS[1-9][0-9]{3,}?$";

	private ShipmentId(string value) : base(value)
	{
	}

	public static bool TryCreate(string? value, [NotNullWhen(true)] out ShipmentId? shipmentId, [NotNullWhen(false)] out string? errorMessage)
	{
		var normalized = value?.ToUpperInvariant()?.Trim();

		// A shipment id must start with 'TMS', then a number greater than 1000.
		if (normalized == null || !Regex.IsMatch(normalized, ShipmentIdPattern))
		{
			shipmentId = null;
			errorMessage = $"Invalid shipment id: {value ?? "NULL"}";
			return false;
		}


		shipmentId = new ShipmentId(normalized);
		errorMessage = null;
		return true;
	}

	public static ShipmentId Create(string? value)
	{
		if (!TryCreate(value, out var shipmentId, out var errorMessage)) throw new FormatException(errorMessage);

		return shipmentId;
	}

	public static implicit operator ShipmentId(string value)
	{
		return Create(value);
	}

	public static implicit operator string(ShipmentId id)
	{
		return id.Value;
	}
}