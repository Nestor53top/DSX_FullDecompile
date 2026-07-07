using System.Windows;

namespace ModernWpf.Input;

internal interface IGettingFocusEventArgs2
{
	bool TrySetNewFocusedElement(DependencyObject element);
}
