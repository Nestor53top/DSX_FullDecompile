using System.Windows;
using System.Windows.Media.Animation;

namespace ModernWpf.Controls.Primitives;

public static class OpeningAnimationHelper
{
	public static readonly DependencyProperty StoryboardProperty = DependencyProperty.RegisterAttached("Storyboard", typeof(Storyboard), typeof(OpeningAnimationHelper), new PropertyMetadata(new PropertyChangedCallback(OnStoryboardChanged)));

	public static Storyboard GetStoryboard(FrameworkElement element)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Storyboard)((DependencyObject)element).GetValue(StoryboardProperty);
	}

	public static void SetStoryboard(FrameworkElement element, Storyboard value)
	{
		((DependencyObject)element).SetValue(StoryboardProperty, (object)value);
	}

	private static void OnStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue != null)
		{
			val.Loaded -= new RoutedEventHandler(OnElementLoaded);
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue != null)
		{
			val.Loaded += new RoutedEventHandler(OnElementLoaded);
		}
	}

	private static void OnElementLoaded(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)sender;
		if (((UIElement)val).IsVisible && Helper.IsAnimationsEnabled && !DesignMode.DesignModeEnabled)
		{
			Storyboard storyboard = GetStoryboard(val);
			if (storyboard != null)
			{
				storyboard.Begin();
			}
		}
	}
}
