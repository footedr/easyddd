using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyDdd.Kernel.Converters;

public class DecimalValueConverter<TValue> : JsonConverter<TValue> where TValue : class
{
	private readonly TryCreate<decimal?, TValue?> _tryCreate;
	private readonly Func<TValue, decimal?> _valueAccessor;

	public DecimalValueConverter(TryCreate<decimal?, TValue?> tryCreate, Func<TValue, decimal?> valueAccessor)
	{
		_tryCreate = tryCreate;
		_valueAccessor = valueAccessor;
	}

	public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var intValue = reader.TokenType == JsonTokenType.Null ? (decimal?)null : reader.GetDecimal();
		if (intValue == null) return default!;

		if (_tryCreate(intValue, out var typedValue, out var errorMessage))
			return typedValue!;
		throw new JsonException(errorMessage);
	}

	public override void Write(Utf8JsonWriter writer, TValue? typedValue, JsonSerializerOptions options)
	{
		if (typedValue == null)
		{
			writer.WriteNullValue();
			return;
		}

		var value = _valueAccessor(typedValue);
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}

		writer.WriteNumberValue(value.Value);
	}
}