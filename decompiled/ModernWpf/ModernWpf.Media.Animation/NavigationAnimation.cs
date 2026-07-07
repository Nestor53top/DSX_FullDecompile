using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWpf.Media.Animation;

internal class NavigationAnimation
{
	private static readonly BitmapCache _defaultBitmapCache;

	private readonly FrameworkElement _element;

	private readonly Storyboard _storyboard;

	private ClockState _currentState = (ClockState)2;

	public event EventHandler Completed;

	static NavigationAnimation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		_defaultBitmapCache = new BitmapCache();
		((Freezable)_defaultBitmapCache).Freeze();
	}

	public NavigationAnimation(FrameworkElement element, Storyboard storyboard)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_element = element;
		_storyboard = storyboard;
		((Timeline)_storyboard).CurrentStateInvalidated += OnCurrentStateInvalidated;
		((Timeline)_storyboard).Completed += OnCompleted;
	}

	public void Begin()
	{
		if (!(((UIElement)_element).CacheMode is BitmapCache))
		{
			((DependencyObject)_element).SetCurrentValue(UIElement.CacheModeProperty, (object)GetBitmapCache());
		}
		_storyboard.Begin(_element, true);
	}

	public void Stop()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)_currentState != 2)
		{
			_storyboard.Stop(_element);
		}
		((DependencyObject)_element).InvalidateProperty(UIElement.CacheModeProperty);
		((DependencyObject)_element).InvalidateProperty(UIElement.RenderTransformProperty);
		((DependencyObject)_element).InvalidateProperty(UIElement.RenderTransformOriginProperty);
	}

	private void OnCurrentStateInvalidated(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Clock val = (Clock)((sender is Clock) ? sender : null);
		if (val != null)
		{
			_currentState = val.CurrentState;
		}
	}

	private void OnCompleted(object sender, EventArgs e)
	{
		this.Completed?.Invoke(this, EventArgs.Empty);
	}

	private BitmapCache GetBitmapCache()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		DpiScale dpi = VisualTreeHelper.GetDpi((Visual)(object)_element);
		return new BitmapCache(((DpiScale)(ref dpi)).PixelsPerDip);
	}
}
