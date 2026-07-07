using System.Windows;

namespace ModernWpf.Input;

internal sealed class TappedRoutedEventArgs : RoutedEventArgs
{
	internal int Timestamp { get; set; }
}
