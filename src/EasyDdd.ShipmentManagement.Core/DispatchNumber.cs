using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core;

public class DispatchNumber : SimpleValueObject<string>
{
	private const string DispatchIdentifierPattern = "^DSP[1-9][0-9]{3,}?$";

	private DispatchNumber(string value) : base(value)
	{
	}

	public static bool TryCreate(string? value, [NotNullWhen(true)] out DispatchNumber? dispatchIdentifier, [NotNullWhen(false)] out string? errorMessage)
	{
		var normalized = value?.ToUpperInvariant()?.Trim();

		// A dispatch identifer must start with 'DSP', then a number greater than 1000.
		if (normalized == null || !Regex.IsMatch(normalized, DispatchIdentifierPattern))
		{
			dispatchIdentifier = null;
			errorMessage = $"Invalid dispatch identifier: {value ?? "NULL"}";
			return false;
		}


		dispatchIdentifier = new DispatchNumber(normalized);
		errorMessage = null;
		return true;
	}

	public static DispatchNumber Create(string? value)
	{
		if (!TryCreate(value, out var dispatchIdentifier, out var errorMessage)) throw new FormatException(errorMessage);

		return dispatchIdentifier;
	}

	public static implicit operator DispatchNumber(string value)
	{
		return Create(value);
	}

	public static implicit operator string(DispatchNumber id)
	{
		return id.Value;
	}
}