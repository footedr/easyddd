using System.ComponentModel;
using System.Globalization;
using EasyDdd.ShipmentManagement.Core;

namespace EasyDdd.ShipmentManagement.Web.Converters;

public class ShipmentIdTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		return ShipmentId.Create(value as string);
	}
}