using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWpf.Controls.Primitives;

public class PopupEx : Popup
{
	internal bool SuppressFadeAnimation { get; set; }

	internal event EventHandler Closing;

	internal event EventHandler IsOpenChanged;

	static PopupEx()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		Popup.IsOpenProperty.OverrideMetadata(typeof(PopupEx), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsOpenPropertyChanged)));
	}

	protected override void OnOpened(EventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		((Popup)this).OnOpened(e);
		if ((int)((Popup)this).PopupAnimation == 1 && SuppressFadeAnimation)
		{
			StopAnimation();
		}
	}

	protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		((Popup)this).OnPreviewMouseLeftButtonDown(e);
		if (!((Popup)this).IsOpen)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
	{
		((Popup)this).OnPreviewMouseRightButtonDown(e);
		if (!((Popup)this).IsOpen)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
	{
		((Popup)this).OnPreviewMouseLeftButtonUp(e);
		if (!((Popup)this).IsOpen)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
	{
		((Popup)this).OnPreviewMouseRightButtonUp(e);
		if (!((Popup)this).IsOpen)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((PopupEx)(object)d).OnIsOpenChanged();
	}

	private void OnIsOpenChanged()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		this.IsOpenChanged?.Invoke(this, EventArgs.Empty);
		if (!((Popup)this).IsOpen)
		{
			if ((int)((Popup)this).PopupAnimation == 1 && SuppressFadeAnimation)
			{
				StopAnimation();
			}
			this.Closing?.Invoke(this, EventArgs.Empty);
		}
	}

	private void StopAnimation()
	{
		UIElement child = ((Popup)this).Child;
		FrameworkElement val = (FrameworkElement)(object)((child is FrameworkElement) ? child : null);
		if (val != null)
		{
			DependencyObject obj = FindPopupRoot((DependencyObject)(object)val);
			FrameworkElement val2 = (FrameworkElement)(object)((obj is FrameworkElement) ? obj : null);
			if (val2 != null)
			{
				((UIElement)val2).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)null);
			}
		}
	}

	private static DependencyObject FindPopupRoot(DependencyObject child)
	{
		DependencyObject parent = VisualTreeHelper.GetParent(child);
		if (parent == null)
		{
			return child;
		}
		return FindPopupRoot(parent);
	}
}
