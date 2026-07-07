namespace WindowsInput;

public class InputSimulator : IInputSimulator
{
	private readonly IKeyboardSimulator _keyboardSimulator;

	private readonly IMouseSimulator _mouseSimulator;

	private readonly IInputDeviceStateAdaptor _inputDeviceState;

	public IKeyboardSimulator Keyboard => _keyboardSimulator;

	public IMouseSimulator Mouse => _mouseSimulator;

	public IInputDeviceStateAdaptor InputDeviceState => _inputDeviceState;

	public InputSimulator(IKeyboardSimulator keyboardSimulator, IMouseSimulator mouseSimulator, IInputDeviceStateAdaptor inputDeviceStateAdaptor)
	{
		_keyboardSimulator = keyboardSimulator;
		_mouseSimulator = mouseSimulator;
		_inputDeviceState = inputDeviceStateAdaptor;
	}

	public InputSimulator()
	{
		_keyboardSimulator = new KeyboardSimulator(this);
		_mouseSimulator = new MouseSimulator(this);
		_inputDeviceState = new WindowsInputDeviceStateAdaptor();
	}
}
