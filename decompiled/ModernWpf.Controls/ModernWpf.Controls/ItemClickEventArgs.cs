using System.Windows;

namespace ModernWpf.Controls;

public sealed class ItemClickEventArgs : RoutedEventArgs
{
	public object ClickedItem { get; internal set; }
}
