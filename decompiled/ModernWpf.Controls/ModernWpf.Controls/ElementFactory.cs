using System;
using System.Windows;

namespace ModernWpf.Controls;

public class ElementFactory : DependencyObject, IElementFactoryShim
{
	public UIElement GetElement(ElementFactoryGetArgs args)
	{
		return GetElementCore(args);
	}

	public void RecycleElement(ElementFactoryRecycleArgs args)
	{
		RecycleElementCore(args);
	}

	protected virtual UIElement GetElementCore(ElementFactoryGetArgs args)
	{
		throw new NotImplementedException();
	}

	protected virtual void RecycleElementCore(ElementFactoryRecycleArgs args)
	{
		throw new NotImplementedException();
	}
}
