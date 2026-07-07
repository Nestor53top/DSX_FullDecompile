using System.Windows;
using System.Windows.Input;

namespace ModernWpf.Input;

internal class GettingFocusEventArgs : IGettingFocusEventArgs2
{
	private KeyboardFocusChangedEventArgs _args;

	public DependencyObject NewFocusedElement { get; set; }

	public bool Handled { get; set; }

	public bool Cancel { get; set; }

	public FocusInputDeviceKind InputDevice { get; }

	public DependencyObject OldFocusedElement { get; }

	internal GettingFocusEventArgs(KeyboardFocusChangedEventArgs args)
	{
		_args = args;
		InputDevice mostRecentInputDevice = InputManager.Current.MostRecentInputDevice;
		InputDevice = ((mostRecentInputDevice is MouseDevice) ? FocusInputDeviceKind.Mouse : ((mostRecentInputDevice is TouchDevice) ? FocusInputDeviceKind.Touch : ((mostRecentInputDevice is StylusDevice) ? FocusInputDeviceKind.Pen : ((mostRecentInputDevice is TabletDevice) ? FocusInputDeviceKind.Pen : ((!(mostRecentInputDevice is KeyboardDevice)) ? FocusInputDeviceKind.Mouse : FocusInputDeviceKind.Keyboard)))));
		IInputElement oldFocus = args.OldFocus;
		OldFocusedElement = (DependencyObject)(object)((oldFocus is DependencyObject) ? oldFocus : null);
		IInputElement newFocus = args.NewFocus;
		NewFocusedElement = (DependencyObject)(object)((newFocus is DependencyObject) ? newFocus : null);
	}

	public bool TrySetNewFocusedElement(DependencyObject element)
	{
		IInputElement val = (IInputElement)(object)((element is IInputElement) ? element : null);
		if (val != null && Keyboard.Focus(val) == val)
		{
			Cancel = true;
			return true;
		}
		return false;
	}
}
