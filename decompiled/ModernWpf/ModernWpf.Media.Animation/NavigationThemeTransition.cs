using System.Windows;
using System.Windows.Markup;

namespace ModernWpf.Media.Animation;

[ContentProperty("DefaultNavigationTransitionInfo")]
public sealed class NavigationThemeTransition : Transition
{
	public static readonly DependencyProperty DefaultNavigationTransitionInfoProperty = DependencyProperty.Register("DefaultNavigationTransitionInfo", typeof(NavigationTransitionInfo), typeof(NavigationThemeTransition), (PropertyMetadata)null);

	public NavigationTransitionInfo DefaultNavigationTransitionInfo
	{
		get
		{
			return (NavigationTransitionInfo)((DependencyObject)this).GetValue(DefaultNavigationTransitionInfoProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DefaultNavigationTransitionInfoProperty, (object)value);
		}
	}
}
