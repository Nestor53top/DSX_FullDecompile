namespace WindowsInput;

public interface IInputSimulator
{
	IKeyboardSimulator Keyboard { get; }

	IMouseSimulator Mouse { get; }

	IInputDeviceStateAdaptor InputDeviceState { get; }
}
