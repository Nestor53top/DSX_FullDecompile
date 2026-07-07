using System.Windows;

namespace ModernWpf.Controls;

public interface IElementFactoryShim
{
	UIElement GetElement(ElementFactoryGetArgs args);

	void RecycleElement(ElementFactoryRecycleArgs context);
}
