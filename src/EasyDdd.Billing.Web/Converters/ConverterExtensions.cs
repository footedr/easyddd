﻿using System.Text.Json;
using System.Text.Json.Serialization;
using EasyDdd.Billing.Core;
using EasyDdd.Kernel.Converters;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace EasyDdd.Billing.Web.Converters;

public static class ConverterExtensions
{
	public static JsonSerializerOptions ConfigureConverters(this JsonSerializerOptions options)
	{
		options.Converters.Add(new JsonStringEnumConverter());
		options.ConfigureForNodaTime(DateTimeZoneProviders.Bcl);
		options.AddStringValueConverter<StatementIdentifier>(StatementIdentifier.TryCreate);
		
		return options;
	}
}