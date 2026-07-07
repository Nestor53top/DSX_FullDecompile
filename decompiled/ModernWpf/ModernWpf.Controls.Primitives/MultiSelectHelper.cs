using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class MultiSelectHelper
{
	private const string MultiSelectStatesGroup = "MultiSelectStates";

	private const string MultiSelectDisabledState = "MultiSelectDisabled";

	private const string MultiSelectEnabledState = "MultiSelectEnabled";

	public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.RegisterAttached("SelectionMode", typeof(SelectionMode), typeof(MultiSelectHelper), new PropertyMetadata((object)(SelectionMode)0, new PropertyChangedCallback(OnSelectionModeChanged)));

	public static SelectionMode GetSelectionMode(ListBoxItem container)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (SelectionMode)((DependencyObject)container).GetValue(SelectionModeProperty);
	}

	public static void SetSelectionMode(ListBoxItem container, SelectionMode value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)container).SetValue(SelectionModeProperty, (object)value);
	}

	private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ListBoxItem val = (ListBoxItem)d;
		UpdateVisualState(val, (SelectionMode)((DependencyPropertyChangedEventArgs)(ref e)).NewValue, ((UIElement)val).IsVisible);
	}

	private static void UpdateVisualState(ListBoxItem control, SelectionMode selectionMode, bool useTransitions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		bool flag = (int)selectionMode == 1;
		VisualStateManager.GoToState((FrameworkElement)(object)control, flag ? "MultiSelectEnabled" : "MultiSelectDisabled", useTransitions);
	}
}
