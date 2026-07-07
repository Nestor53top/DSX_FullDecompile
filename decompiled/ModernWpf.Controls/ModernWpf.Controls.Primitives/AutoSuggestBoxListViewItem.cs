using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public class AutoSuggestBoxListViewItem : ListViewItem
{
	private bool m_isPressed;

	private AutoSuggestBoxListView ParentListView => ParentSelector as AutoSuggestBoxListView;

	internal Selector ParentSelector
	{
		get
		{
			ItemsControl obj = ItemsControl.ItemsControlFromItemContainer((DependencyObject)(object)this);
			return (Selector)(object)((obj is Selector) ? obj : null);
		}
	}

	protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).Handled)
		{
			((RoutedEventArgs)e).Handled = true;
			m_isPressed = true;
		}
		((ListBoxItem)this).OnMouseLeftButtonDown(e);
	}

	protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).Handled)
		{
			((RoutedEventArgs)e).Handled = true;
			HandleMouseUp((MouseButton)0);
			m_isPressed = false;
		}
		((UIElement)this).OnMouseLeftButtonUp(e);
	}

	protected override void OnMouseLeave(MouseEventArgs e)
	{
		if (!((RoutedEventArgs)e).Handled)
		{
			m_isPressed = false;
		}
		((ListBoxItem)this).OnMouseLeave(e);
	}

	private void HandleMouseUp(MouseButton mouseButton)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (m_isPressed && SelectorHelper.UiGetIsSelectable((DependencyObject)(object)this) && ((UIElement)this).Focus())
		{
			ParentListView?.NotifyListItemClicked(this, mouseButton);
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		((UIElement)this).OnKeyDown(e);
		if ((int)e.Key == 6 && SelectorHelper.UiGetIsSelectable((DependencyObject)(object)this) && ((UIElement)this).Focus())
		{
			ParentListView?.NotifyListItemClicked(this);
			((RoutedEventArgs)e).Handled = true;
		}
	}
}
