using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Windows;

namespace ModernWpf.Markup;

public class ThemeResouceExtensionConverter : TypeConverter
{
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(InstanceDescriptor))
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is ThemeResourceExtension themeResourceExtension))
			{
				throw new ArgumentException(string.Format("{0} must be of type {1}", value, "ThemeResourceExtension"), "value");
			}
			return new InstanceDescriptor(typeof(ThemeResourceExtension).GetConstructor(new Type[1] { typeof(object) }), new object[1] { ((DynamicResourceExtension)themeResourceExtension).ResourceKey });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
