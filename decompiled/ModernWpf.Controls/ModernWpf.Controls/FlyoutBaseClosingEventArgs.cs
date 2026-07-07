using System;

namespace ModernWpf.Controls;

internal sealed class FlyoutBaseClosingEventArgs : EventArgs
{
	public bool Cancel
	{
		get
		{
			return false;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	internal FlyoutBaseClosingEventArgs()
	{
	}
}
