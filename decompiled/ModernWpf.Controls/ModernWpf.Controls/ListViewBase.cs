using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class ListViewBase : ListBox
{
	public static readonly DependencyProperty IsItemClickEnabledProperty;

	public static readonly DependencyProperty IsSelectionEnabledProperty;

	public static readonly DependencyProperty IsMultiSelectCheckBoxEnabledProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty FocusVisualMarginProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	private bool m_multiSelectEnabled;

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

	public bool IsSelectionEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsSelectionEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsSelectionEnabledProperty, (object)value);
		}
	}

	public bool IsMultiSelectCheckBoxEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsMultiSelectCheckBoxEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsMultiSelectCheckBoxEnabledProperty, (object)value);
		}
	}

	public bool UseSystemFocusVisuals
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(UseSystemFocusVisualsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UseSystemFocusVisualsProperty, (object)value);
		}
	}

	public Thickness FocusVisualMargin
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Thickness)((DependencyObject)this).GetValue(FocusVisualMarginProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FocusVisualMarginProperty, (object)value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (CornerRadius)((DependencyObject)this).GetValue(CornerRadiusProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CornerRadiusProperty, (object)value);
		}
	}

	internal bool MultiSelectEnabled
	{
		get
		{
			return m_multiSelectEnabled;
		}
		set
		{
			if (m_multiSelectEnabled != value)
			{
				m_multiSelectEnabled = value;
				this.MultiSelectEnabledChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public event ItemClickEventHandler ItemClick;

	internal event EventHandler MultiSelectEnabledChanged;

	static ListViewBase()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		IsItemClickEnabledProperty = DependencyProperty.Register("IsItemClickEnabled", typeof(bool), typeof(ListViewBase), new PropertyMetadata((object)false));
		IsSelectionEnabledProperty = DependencyProperty.Register("IsSelectionEnabled", typeof(bool), typeof(ListViewBase), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsSelectionEnabledChanged)));
		IsMultiSelectCheckBoxEnabledProperty = DependencyProperty.Register("IsMultiSelectCheckBoxEnabled", typeof(bool), typeof(ListViewBase), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsMultiSelectCheckBoxEnabledChanged)));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(ListViewBase));
		FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(ListViewBase));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(ListViewBase));
		ListBox.SelectionModeProperty.OverrideMetadata(typeof(ListViewBase), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectionModePropertyChanged)));
	}

	protected ListViewBase()
	{
		UpdateMultiSelectEnabled();
	}

	private static void OnIsSelectionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ListViewBase listViewBase = (ListViewBase)(object)d;
		listViewBase.UpdateMultiSelectEnabled();
		if (!(bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue && ((ListBox)listViewBase).SelectedItems.Count > 0)
		{
			((ListBox)listViewBase).UnselectAll();
		}
	}

	private static void OnIsMultiSelectCheckBoxEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ListViewBase)(object)d).UpdateMultiSelectEnabled();
	}

	protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
	{
		((ListBox)this).PrepareContainerForItemOverride(element, item);
		if (element is ListViewBaseItem listViewBaseItem)
		{
			listViewBaseItem.SubscribeToMultiSelectEnabledChanged(this);
		}
	}

	protected override void ClearContainerForItemOverride(DependencyObject element, object item)
	{
		((Selector)this).ClearContainerForItemOverride(element, item);
		if (element is ListViewBaseItem listViewBaseItem)
		{
			listViewBaseItem.UnsubscribeFromMultiSelectEnabledChanged(this);
		}
	}

	protected override void OnSelectionChanged(SelectionChangedEventArgs e)
	{
		if (IsSelectionEnabled)
		{
			((ListBox)this).OnSelectionChanged(e);
		}
		else if (((ListBox)this).SelectedItems.Count > 0)
		{
			((ListBox)this).UnselectAll();
		}
	}

	internal void NotifyListItemClicked(ListViewBaseItem item)
	{
		if (IsItemClickEnabled)
		{
			object clickedItem = ((ItemsControl)this).ItemContainerGenerator.ItemFromContainer((DependencyObject)(object)item);
			this.ItemClick?.Invoke(this, new ItemClickEventArgs
			{
				ClickedItem = clickedItem
			});
		}
	}

	private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ListViewBase)(object)d).UpdateMultiSelectEnabled();
	}

	private void UpdateMultiSelectEnabled()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		MultiSelectEnabled = IsSelectionEnabled && (int)((ListBox)this).SelectionMode == 1 && IsMultiSelectCheckBoxEnabled;
	}
}
