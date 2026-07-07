using System;

namespace ModernWpf.Controls;

public sealed class NavigationViewDisplayModeChangedEventArgs : EventArgs
{
	public NavigationViewDisplayMode DisplayMode { get; internal set; }

	internal NavigationViewDisplayModeChangedEventArgs()
	{
	}
}
