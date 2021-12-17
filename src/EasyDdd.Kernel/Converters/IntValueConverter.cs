using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyDdd.Kernel.Converters;

public class IntValueConverter<TValue> : JsonConverter<TValue> where TValue : class
{
	private readonly TryCreate<int?, TValue?> _func;
	private readonly Func<TValue, int?> _valueAccessor;

	public IntValueConverter(TryCreate<int?, TValue?> func, Func<TValue, int?> valueAccessor)
	{
		_func = func;
		_valueAccessor = valueAccessor;
	}

	public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var intValue = reader.TokenType == JsonTokenType.Null ? (int?)null : reader.GetInt32();
		if (intValue == null) return default!;

		if (_func(intValue, out var typedValue, out var errorMessage))
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