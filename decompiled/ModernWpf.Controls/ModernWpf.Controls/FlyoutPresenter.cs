using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class FlyoutPresenter : ContentControl
{
	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty IsDefaultShadowEnabledProperty;

	public CornerRadius CornerRadius
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (CornerRadius)((DependencyObject)this).GetValue(CornerRadiusProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CornerRadiusProperty, (object)value);
		}
	}

	public bool IsDefaultShadowEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsDefaultShadowEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsDefaultShadowEnabledProperty, (object)value);
		}
	}

	static FlyoutPresenter()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(FlyoutPresenter));
		IsDefaultShadowEnabledProperty = DependencyProperty.Register("IsDefaultShadowEnabled", typeof(bool), typeof(FlyoutPresenter), new PropertyMetadata((object)true));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(FlyoutPresenter)));
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		((UIElement)this).OnKeyDown(e);
		if ((int)e.Key == 13)
		{
			DependencyObject parent = ((FrameworkElement)this).Parent;
			Popup val = (Popup)(object)((parent is Popup) ? parent : null);
			if (val != null && val.IsOpen)
			{
				((DependencyObject)val).SetCurrentValue(Popup.IsOpenProperty, (object)false);
				((RoutedEventArgs)e).Handled = true;
			}
		}
	}

	protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((Visual)this).OnDpiChanged(oldDpi, newDpi);
		CacheMode cacheMode = ((UIElement)this).CacheMode;
		BitmapCache val = (BitmapCache)(object)((cacheMode is BitmapCache) ? cacheMode : null);
		if (val != null)
		{
			val.RenderAtScale = ((DpiScale)(ref newDpi)).PixelsPerDip;
		}
	}
}
