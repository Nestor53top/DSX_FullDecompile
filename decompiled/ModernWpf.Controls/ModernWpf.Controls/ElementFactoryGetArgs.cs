using System.Windows;

namespace ModernWpf.Controls;

public sealed class ElementFactoryGetArgs
{
	public UIElement Parent { get; set; }

	public object Data { get; set; }

	internal int Index { get; set; }
}
