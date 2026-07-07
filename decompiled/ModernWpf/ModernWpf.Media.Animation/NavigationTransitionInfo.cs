using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ModernWpf.Media.Animation;

public class NavigationTransitionInfo : DependencyObject
{
	internal static readonly TimeSpan ExitDuration;

	internal static readonly TimeSpan EnterDuration;

	internal static readonly TimeSpan MaxMoveDuration;

	internal static readonly KeySpline AccelerateKeySpline;

	internal static readonly KeySpline DecelerateKeySpline;

	internal static readonly PropertyPath OpacityPath;

	internal static readonly PropertyPath TranslateXPath;

	internal static readonly PropertyPath TranslateYPath;

	internal static readonly PropertyPath ScaleXPath;

	internal static readonly PropertyPath ScaleYPath;

	static NavigationTransitionInfo()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		ExitDuration = TimeSpan.FromMilliseconds(150.0);
		EnterDuration = TimeSpan.FromMilliseconds(300.0);
		MaxMoveDuration = TimeSpan.FromMilliseconds(500.0);
		OpacityPath = new PropertyPath((object)UIElement.OpacityProperty);
		TranslateXPath = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)", Array.Empty<object>());
		TranslateYPath = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)", Array.Empty<object>());
		ScaleXPath = new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)", Array.Empty<object>());
		ScaleYPath = new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)", Array.Empty<object>());
		AccelerateKeySpline = new KeySpline(0.7, 0.0, 1.0, 0.5);
		((Freezable)AccelerateKeySpline).Freeze();
		DecelerateKeySpline = new KeySpline(0.1, 0.9, 0.2, 1.0);
		((Freezable)DecelerateKeySpline).Freeze();
	}

	protected NavigationTransitionInfo()
	{
	}

	internal virtual NavigationAnimation GetEnterAnimation(FrameworkElement element, bool movingBackwards)
	{
		return null;
	}

	internal virtual NavigationAnimation GetExitAnimation(FrameworkElement element, bool movingBackwards)
	{
		return null;
	}
}
