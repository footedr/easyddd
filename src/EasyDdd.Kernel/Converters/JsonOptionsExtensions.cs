using System;
using System.Text.Json;

namespace EasyDdd.Kernel.Converters;

public delegate bool TryCreate<TValue, TObject>(TValue value, out TObject @object, out string? errorMessage);

public static class JsonSerializerOptionsExtensions
{
	public static void AddStringValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<string?, TObject?> func, bool convertEmptyStringToNull = true)
		where TObject : class, ISimpleValueObject<string>
	{
		options.AddStringValueConverter(func, x => x.Value, convertEmptyStringToNull);
	}

	public static void AddStringValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<string?, TObject?> func, Func<TObject, string?> valueAccessor, bool convertEmptyStringToNull = true)
		where TObject : class
	{
		options.Converters.Add(new StringValueConverter<TObject>(func, valueAccessor, convertEmptyStringToNull));
	}

	public static void AddIntValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<int?, TObject?> func)
		where TObject : class, ISimpleValueObject<int>
	{
		options.AddIntValueConverter(func, x => x.Value);
	}

	public static void AddIntValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<int?, TObject?> func, Func<TObject, int?> valueAccessor)
		where TObject : class
	{
		options.Converters.Add(new IntValueConverter<TObject>(func, valueAccessor));
	}

	public static void AddDecimalValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<decimal?, TObject?> func)
		where TObject : class, ISimpleValueObject<decimal>
	{
		options.AddDecimalValueConverter(func, x => x.Value);
	}

	public static void AddDecimalValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<decimal?, TObject?> func, Func<TObject, decimal?> valueAccessor)
		where TObject : class
	{
		options.Converters.Add(new DecimalValueConverter<TObject>(func, valueAccessor));
	}

	public static void AddDoubleValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<double?, TObject?> func)
		where TObject : class, ISimpleValueObject<double>
	{
		options.AddDoubleValueConverter(func, x => x.Value);
	}

	public static void AddDoubleValueConverter<TObject>(this JsonSerializerOptions options, TryCreate<double?, TObject?> func, Func<TObject, double?> valueAccessor)
		where TObject : class
	{
		options.Converters.Add(new DoubleValueConverter<TObject>(func, valueAccessor));
	}
}