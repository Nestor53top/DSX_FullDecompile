using System;
using System.Windows;
using System.Windows.Input;

namespace ModernWpf.Input;

internal class GettingFocusHelper : IDisposable
{
	private UIElement _owner;

	private bool _ignoreGotFocus;

	public event TypedEventHandler<UIElement, GettingFocusEventArgs> GettingFocus;

	public GettingFocusHelper(UIElement owner)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		_owner = owner;
		_owner.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewGotKeyboardFocus);
	}

	public void Dispose()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if (_owner != null)
		{
			_owner.PreviewGotKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnPreviewGotKeyboardFocus);
			_owner = null;
		}
	}

	private void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		if (_ignoreGotFocus)
		{
			return;
		}
		TypedEventHandler<UIElement, GettingFocusEventArgs> typedEventHandler = this.GettingFocus;
		if (typedEventHandler == null)
		{
			return;
		}
		try
		{
			_ignoreGotFocus = true;
			GettingFocusEventArgs e2 = new GettingFocusEventArgs(e);
			typedEventHandler((UIElement)((sender is UIElement) ? sender : null), e2);
			if (e2.Cancel)
			{
				((RoutedEventArgs)e).Handled = true;
			}
		}
		finally
		{
			_ignoreGotFocus = false;
		}
	}
}
