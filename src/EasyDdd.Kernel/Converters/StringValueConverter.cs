using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyDdd.Kernel.Converters;

public class StringValueConverter<TValue> : JsonConverter<TValue> where TValue : class
{
	private readonly bool _convertEmptyStringToNull;
	private readonly TryCreate<string?, TValue?> _func;
	private readonly Func<TValue, string?> _valueAccessor;

	public StringValueConverter(TryCreate<string?, TValue?> func, Func<TValue, string?> valueAccessor, bool convertEmptyStringToNull = true)
	{
		_func = func;
		_valueAccessor = valueAccessor;
		_convertEmptyStringToNull = convertEmptyStringToNull;
	}

	public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var stringValue = reader.GetString();
		if (stringValue == null || _convertEmptyStringToNull && string.IsNullOrWhiteSpace(stringValue)) return default!;

		if (_func(stringValue, out var typedValue, out var errorMessage))
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

		writer.WriteStringValue(value);
	}
}