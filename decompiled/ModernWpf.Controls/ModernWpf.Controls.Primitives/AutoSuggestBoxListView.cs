using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public class AutoSuggestBoxListView : ListView
{
	public static readonly DependencyProperty IsItemClickEnabledProperty;

	private ScrollViewer m_scrollHost;

	public bool IsItemClickEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsItemClickEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsItemClickEnabledProperty, (object)value);
		}
	}

	public event ItemClickEventHandler ItemClick;

	static AutoSuggestBoxListView()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		IsItemClickEnabledProperty = DependencyProperty.Register("IsItemClickEnabled", typeof(bool), typeof(AutoSuggestBoxListView), new PropertyMetadata((object)false));
		ListBox.SelectionModeProperty.OverrideMetadata(typeof(AutoSuggestBoxListView), (PropertyMetadata)new FrameworkPropertyMetadata((object)(SelectionMode)0));
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		m_scrollHost = ((DependencyObject)(object)this).FindDescendant<ScrollViewer>();
	}

	protected override bool IsItemItsOwnContainerOverride(object item)
	{
		return item is AutoSuggestBoxListViewItem;
	}

	protected override DependencyObject GetContainerForItemOverride()
	{
		return (DependencyObject)(object)new AutoSuggestBoxListViewItem();
	}

	internal void NotifyListItemClicked(AutoSuggestBoxListViewItem item, MouseButton? mouseButton = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		if (IsItemClickEnabled)
		{
			OnItemClick(item);
		}
		if ((int)((ListBox)this).SelectionMode == 0)
		{
			if (!((ListBoxItem)item).IsSelected)
			{
				((DependencyObject)item).SetCurrentValue(Selector.IsSelectedProperty, (object)true);
			}
			else if (mouseButton.HasValue && (Keyboard.Modifiers & 2) == 2)
			{
				((DependencyObject)item).SetCurrentValue(Selector.IsSelectedProperty, (object)false);
			}
			return;
		}
		throw new NotImplementedException();
	}

	internal void ScrollToTop()
	{
		ScrollViewer scrollHost = m_scrollHost;
		if (scrollHost != null)
		{
			scrollHost.ScrollToTop();
		}
	}

	private void OnItemClick(AutoSuggestBoxListViewItem lvi)
	{
		object obj = ((ItemsControl)this).ItemContainerGenerator.ItemFromContainer((DependencyObject)(object)lvi);
		if (obj != null)
		{
			this.ItemClick?.Invoke(this, new ItemClickEventArgs
			{
				ClickedItem = obj
			});
		}
	}
}
