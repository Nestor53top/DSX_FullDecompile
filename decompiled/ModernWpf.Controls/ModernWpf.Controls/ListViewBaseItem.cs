using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class ListViewBaseItem : ListBoxItem
{
	public static readonly DependencyProperty UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(ListViewBaseItem));

	public static readonly DependencyProperty FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(ListViewBaseItem));

	public static readonly DependencyProperty CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(ListViewBaseItem));

	private bool m_isPressed;

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

	private ListViewBase ParentListViewBase => ItemsControl.ItemsControlFromItemContainer((DependencyObject)(object)this) as ListViewBase;

	protected ListViewBaseItem()
	{
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		UpdateMultiSelectStates(ParentListViewBase, useTransitions: false);
	}

	protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).Handled)
		{
			m_isPressed = true;
		}
		((ListBoxItem)this).OnMouseLeftButtonDown(e);
	}

	protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).Handled)
		{
			HandleMouseUp(e);
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

	protected override void OnKeyDown(KeyEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		((UIElement)this).OnKeyDown(e);
		if ((int)e.Key == 6)
		{
			OnClick();
			((RoutedEventArgs)e).Handled = true;
		}
	}

	internal void SubscribeToMultiSelectEnabledChanged(ListViewBase parent)
	{
		parent.MultiSelectEnabledChanged += OnMultiSelectEnabledChanged;
		UpdateMultiSelectStates(parent);
	}

	internal void UnsubscribeFromMultiSelectEnabledChanged(ListViewBase parent)
	{
		parent.MultiSelectEnabledChanged -= OnMultiSelectEnabledChanged;
		UpdateMultiSelectStates(parent);
	}

	private void OnMultiSelectEnabledChanged(object sender, EventArgs e)
	{
		UpdateMultiSelectStates((ListViewBase)sender);
	}

	private void UpdateMultiSelectStates(ListViewBase parent, bool useTransitions = true)
	{
		if (parent != null)
		{
			bool flag = parent.MultiSelectEnabled && parent.IsMultiSelectCheckBoxEnabled;
			VisualStateManager.GoToState((FrameworkElement)(object)this, flag ? "MultiSelectEnabled" : "MultiSelectDisabled", useTransitions);
		}
	}

	private void HandleMouseUp(MouseButtonEventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (m_isPressed)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(default(Point), ((UIElement)this).RenderSize);
			if (((Rect)(ref val)).Contains(((MouseEventArgs)e).GetPosition((IInputElement)(object)this)))
			{
				OnClick();
			}
		}
	}

	private void OnClick()
	{
		ParentListViewBase?.NotifyListItemClicked(this);
	}
}
