using System.Windows;
using System.Windows.Controls.Primitives;

namespace ModernWpf.Controls.Primitives;

public static class ScrollBarThumbHelper
{
	public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(ScrollBarThumbHelper), new PropertyMetadata((object)false));

	public static bool GetIsExpanded(Thumb thumb)
	{
		return (bool)((DependencyObject)thumb).GetValue(IsExpandedProperty);
	}

	public static void SetIsExpanded(Thumb thumb, bool value)
	{
		((DependencyObject)thumb).SetValue(IsExpandedProperty, (object)value);
	}
}
