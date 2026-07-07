using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls;

[EditorBrowsable(EditorBrowsableState.Never)]
public class FontIconFallback : Control
{
	public static readonly DependencyProperty DataProperty;

	public Geometry Data
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Geometry)((DependencyObject)this).GetValue(DataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DataProperty, (object)value);
		}
	}

	static FontIconFallback()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(FontIconFallback), (PropertyMetadata)null);
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIconFallback), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(FontIconFallback)));
		UIElement.FocusableProperty.OverrideMetadata(typeof(FontIconFallback), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
	}
}
