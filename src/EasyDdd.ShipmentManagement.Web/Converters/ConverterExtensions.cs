using System.Text.Json;
using System.Text.Json.Serialization;
using EasyDdd.Kernel.Converters;
using EasyDdd.ShipmentManagement.Core;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace EasyDdd.ShipmentManagement.Web.Converters;

public static class ConverterExtensions
{
	public static JsonSerializerOptions ConfigureConverters(this JsonSerializerOptions options)
	{
		options.Converters.Add(new JsonStringEnumConverter());
		options.ConfigureForNodaTime(DateTimeZoneProviders.Bcl);
		options.AddStringValueConverter<ShipmentId>(ShipmentId.TryCreate);
		options.AddStringValueConverter<FreightClass>(FreightClass.TryCreate);
		options.AddStringValueConverter<PackagingType>(PackagingType.TryCreate);
		options.AddStringValueConverter<DispatchNumber>(DispatchNumber.TryCreate);

		return options;
	}
}