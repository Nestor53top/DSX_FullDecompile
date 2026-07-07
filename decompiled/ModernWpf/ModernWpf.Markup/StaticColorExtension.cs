using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ModernWpf.Markup;

public class StaticColorExtension : StaticResourceExtension
{
	public StaticColorExtension()
	{
	}

	public StaticColorExtension(object resourceKey)
		: base(resourceKey)
	{
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		object result = ((StaticResourceExtension)this).ProvideValue(serviceProvider);
		object? obj = serviceProvider?.GetService(typeof(IProvideValueTarget));
		IProvideValueTarget val = (IProvideValueTarget)((obj is IProvideValueTarget) ? obj : null);
		if (val != null)
		{
			object targetObject = val.TargetObject;
			SolidColorBrush val2 = (SolidColorBrush)((targetObject is SolidColorBrush) ? targetObject : null);
			if (val2 != null)
			{
				ThemeResourceHelper.SetColorKey(val2, ((StaticResourceExtension)this).ResourceKey);
			}
		}
		return result;
	}
}
