using System.Windows;

namespace ModernWpf.Controls;

public sealed class BackRequestedEventArgs : RoutedEventArgs
{
	internal BackRequestedEventArgs()
		: base(TitleBar.BackRequestedEvent)
	{
	}

	internal BackRequestedEventArgs(object source)
		: base(TitleBar.BackRequestedEvent, source)
	{
	}
}
