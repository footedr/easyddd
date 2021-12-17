using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyDdd.Kernel.EventGrid
{
	public class TimeSpanJsonConverter : JsonConverter<TimeSpan?>
	{
		public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			if (value == null)
			{
				return null;
			}

			if (TimeSpan.TryParse(value, out var timeSpan))
			{
				return timeSpan;
			}
			else
			{
				throw new JsonException($"Invalid time span '{value}'");
			}
		}

		public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
		{
			if (value == null)
			{
				writer.WriteNullValue();
			}
			else
			{
				writer.WriteStringValue($"{value.Value.Hours}:{value.Value.Minutes}:{value.Value.Seconds}");
			}
		}
	}
}
