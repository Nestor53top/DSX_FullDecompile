using System.Windows;

namespace ModernWpf.Controls.Primitives;

public class BindingProxy : Freezable
{
	public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BindingProxy));

	public object Value
	{
		get
		{
			return ((DependencyObject)this).GetValue(ValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ValueProperty, value);
		}
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new BindingProxy();
	}
}
