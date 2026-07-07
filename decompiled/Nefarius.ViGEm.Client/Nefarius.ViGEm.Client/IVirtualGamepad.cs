namespace Nefarius.ViGEm.Client;

public interface IVirtualGamepad
{
	int ButtonCount { get; }

	int AxisCount { get; }

	int SliderCount { get; }

	bool AutoSubmitReport { get; set; }

	void Connect();

	void Disconnect();

	void SetButtonState(int index, bool pressed);

	void SetAxisValue(int index, short value);

	void SetSliderValue(int index, byte value);

	void ResetReport();

	void SubmitReport();
}
