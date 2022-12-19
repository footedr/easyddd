using System.ComponentModel;
using System.Globalization;
using EasyDdd.Billing.Core;

namespace EasyDdd.Billing.Web.Converters;

public class StatementIdentifierConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		return StatementIdentifier.Create(value as string);
	}
}