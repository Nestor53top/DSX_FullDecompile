using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace Nefarius.ViGEm.Client.Targets;

public interface IDualShock4Controller : IVirtualGamepad
{
	event DualShock4FeedbackReceivedEventHandler FeedbackReceived;

	void SetButtonState(DualShock4Button button, bool pressed);

	void SetDPadDirection(DualShock4DPadDirection direction);

	void SetAxisValue(DualShock4Axis axis, byte value);

	void SetSliderValue(DualShock4Slider slider, byte value);

	void SubmitRawReport(byte[] buffer);
}
