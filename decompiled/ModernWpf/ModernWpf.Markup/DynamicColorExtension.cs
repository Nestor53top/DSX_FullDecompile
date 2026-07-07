using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ModernWpf.Markup;

[TypeConverter(typeof(DynamicColorExtensionConverter))]
public class DynamicColorExtension : DynamicResourceExtension
{
	public DynamicColorExtension()
	{
	}

	public DynamicColorExtension(object resourceKey)
		: base(resourceKey)
	{
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		object result = ((DynamicResourceExtension)this).ProvideValue(serviceProvider);
		object? obj = serviceProvider?.GetService(typeof(IProvideValueTarget));
		IProvideValueTarget val = (IProvideValueTarget)((obj is IProvideValueTarget) ? obj : null);
		if (val != null)
		{
			object targetObject = val.TargetObject;
			SolidColorBrush val2 = (SolidColorBrush)((targetObject is SolidColorBrush) ? targetObject : null);
			if (val2 != null)
			{
				ThemeResourceHelper.SetColorKey(val2, ((DynamicResourceExtension)this).ResourceKey);
			}
		}
		return result;
	}
}
