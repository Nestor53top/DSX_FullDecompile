using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public class MenuPopup : Popup
{
	private static readonly DependencyPropertyKey IsSuspendingAnimationPropertyKey = DependencyProperty.RegisterReadOnly("IsSuspendingAnimation", typeof(bool), typeof(MenuPopup), (PropertyMetadata)null);

	public static readonly DependencyProperty IsSuspendingAnimationProperty = IsSuspendingAnimationPropertyKey.DependencyProperty;

	public bool IsSuspendingAnimation
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsSuspendingAnimationProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(IsSuspendingAnimationPropertyKey, (object)value);
		}
	}

	protected override void OnOpened(EventArgs e)
	{
		((Popup)this).OnOpened(e);
		((DependencyObject)this).ClearValue(IsSuspendingAnimationPropertyKey);
	}

	protected override void OnClosed(EventArgs e)
	{
		((Popup)this).OnClosed(e);
		((DependencyObject)this).ClearValue(IsSuspendingAnimationPropertyKey);
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property == Popup.IsOpenProperty)
		{
			OnIsOpenChanged(e);
		}
		((FrameworkElement)this).OnPropertyChanged(e);
	}

	private void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
	{
		if (!(bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			return;
		}
		Window window = Window.GetWindow((DependencyObject)(object)this);
		if (window != null)
		{
			IInputElement focusedElement = FocusManager.GetFocusedElement((DependencyObject)(object)window);
			if (focusedElement is TextBoxBase || focusedElement is PasswordBox)
			{
				IsSuspendingAnimation = true;
			}
		}
	}
}
