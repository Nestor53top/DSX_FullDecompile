using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace Nefarius.ViGEm.Client.Targets;

public interface IXbox360Controller : IVirtualGamepad
{
	int UserIndex { get; }

	event Xbox360FeedbackReceivedEventHandler FeedbackReceived;

	void SetButtonState(Xbox360Button button, bool pressed);

	void SetAxisValue(Xbox360Axis axis, short value);

	void SetSliderValue(Xbox360Slider slider, byte value);
}
