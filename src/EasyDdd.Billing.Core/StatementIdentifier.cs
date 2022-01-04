using System.Diagnostics.CodeAnalysis;
using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core;

public class StatementIdentifier : SimpleValueObject<string>
{
	private StatementIdentifier(string value) : base(value)
	{
	}

	public static bool TryCreate(string? value, [NotNullWhen(true)] out StatementIdentifier? identifier, [NotNullWhen(false)] out string? errorMessage)
	{
		var normalized = value?.ToUpperInvariant()?.Trim();

		if (string.IsNullOrEmpty(normalized))
		{
			identifier = null;
			errorMessage = $"Invalid statement identifier: {value ?? "NULL"}";
			return false;
		}


		identifier = new StatementIdentifier(normalized);
		errorMessage = null;
		return true;
	}

	public static StatementIdentifier Create(string? value)
	{
		if (!TryCreate(value, out var identifier, out var errorMessage)) throw new FormatException(errorMessage);

		return identifier;
	}

	public static implicit operator StatementIdentifier(string value)
	{
		return Create(value);
	}

	public static implicit operator string(StatementIdentifier identifier)
	{
		return identifier.Value;
	}
}