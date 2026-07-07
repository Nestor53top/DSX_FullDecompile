using System;

namespace Nefarius.ViGEm.Client.Targets.DualShock4;

public class DualShock4FeedbackReceivedEventArgs : EventArgs
{
	public byte LargeMotor { get; }

	public byte SmallMotor { get; }

	public LightbarColor LightbarColor { get; }

	public DualShock4FeedbackReceivedEventArgs(byte largeMotor, byte smallMotor, LightbarColor color)
	{
		LargeMotor = largeMotor;
		SmallMotor = smallMotor;
		LightbarColor = color;
	}
}
