using System.Windows;

namespace ModernWpf.Controls;

public interface IElementFactory
{
	UIElement GetElement(ElementFactoryGetArgs args);

	void RecycleElement(ElementFactoryRecycleArgs args);
}
