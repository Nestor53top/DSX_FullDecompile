using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

internal static class SelectorHelper
{
	internal static bool ItemGetIsSelectable(object item)
	{
		if (item != null)
		{
			return !(item is Separator);
		}
		return false;
	}

	internal static bool UiGetIsSelectable(DependencyObject o)
	{
		if (o != null)
		{
			if (!ItemGetIsSelectable(o))
			{
				return false;
			}
			ItemsControl val = ItemsControl.ItemsControlFromItemContainer(o);
			if (val != null)
			{
				object obj = val.ItemContainerGenerator.ItemFromContainer(o);
				if (obj != o)
				{
					return ItemGetIsSelectable(obj);
				}
				return true;
			}
		}
		return false;
	}
}
