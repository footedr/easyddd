using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyDdd.Kernel.Converters;

public class DoubleValueConverter<TValue> : JsonConverter<TValue> where TValue : class
{
	private readonly TryCreate<double?, TValue?> _tryCreate;
	private readonly Func<TValue, double?> _valueAccessor;

	public DoubleValueConverter(TryCreate<double?, TValue?> tryCreate, Func<TValue, double?> valueAccessor)
	{
		_tryCreate = tryCreate;
		_valueAccessor = valueAccessor;
	}

	public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var doubleValue = reader.TokenType == JsonTokenType.Null ? (double?)null : reader.GetDouble();
		if (doubleValue == null) return default!;

		if (_tryCreate(doubleValue, out var typedValue, out var errorMessage))
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