using System;
using System.ComponentModel;
using System.Globalization;

namespace NuGet;

internal class SemanticVersionTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string version && SemanticVersion.TryParse(version, out var value2))
		{
			return value2;
		}
		return null;
	}
}
