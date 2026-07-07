using System;

namespace ModernWpf.Controls;

public class NumberBoxValueChangedEventArgs : EventArgs
{
	public double OldValue { get; }

	public double NewValue { get; }

	public NumberBoxValueChangedEventArgs(double oldValue, double newValue)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
