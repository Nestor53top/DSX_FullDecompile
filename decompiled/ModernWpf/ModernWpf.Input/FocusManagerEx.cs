using System.Windows;
using System.Windows.Input;

namespace ModernWpf.Input;

internal static class FocusManagerEx
{
	public static UIElement FindNextFocusableElement(FocusNavigationDirection focusNavigationDirection)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		IInputElement focusedElement = Keyboard.FocusedElement;
		UIElement val = (UIElement)(object)((focusedElement is UIElement) ? focusedElement : null);
		if (val != null)
		{
			DependencyObject obj = val.PredictFocus(focusNavigationDirection);
			return (UIElement)(object)((obj is UIElement) ? obj : null);
		}
		return null;
	}
}
