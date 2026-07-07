using WindowsInput.Native;

namespace WindowsInput;

internal interface IInputMessageDispatcher
{
	void DispatchInput(INPUT[] inputs);
}
