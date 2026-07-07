using System.Windows;
using System.Windows.Media;

namespace ModernWpf.Controls;

public class IconSource : DependencyObject
{
	public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(IconSource));

	public Brush Foreground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(ForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ForegroundProperty, (object)value);
		}
	}
}
