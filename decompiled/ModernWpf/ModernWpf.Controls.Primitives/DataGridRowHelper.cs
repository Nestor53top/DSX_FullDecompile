using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class DataGridRowHelper
{
	private static readonly DependencyPropertyKey AreRowDetailsFrozenPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AreRowDetailsFrozen", typeof(bool), typeof(DataGridRowHelper), new PropertyMetadata((object)false));

	public static readonly DependencyProperty AreRowDetailsFrozenProperty = AreRowDetailsFrozenPropertyKey.DependencyProperty;

	internal static readonly DependencyProperty AreRowDetailsFrozenInternalProperty = DependencyProperty.RegisterAttached("AreRowDetailsFrozenInternal", typeof(bool), typeof(DataGridRowHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnAreRowDetailsFrozenInternalChanged)));

	private static readonly DependencyPropertyKey HeadersVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HeadersVisibility", typeof(DataGridHeadersVisibility), typeof(DataGridRowHelper), new PropertyMetadata((object)(DataGridHeadersVisibility)3));

	public static readonly DependencyProperty HeadersVisibilityProperty = HeadersVisibilityPropertyKey.DependencyProperty;

	internal static readonly DependencyProperty HeadersVisibilityInternalProperty = DependencyProperty.RegisterAttached("HeadersVisibilityInternal", typeof(DataGridHeadersVisibility), typeof(DataGridRowHelper), new PropertyMetadata((object)(DataGridHeadersVisibility)3, new PropertyChangedCallback(OnHeadersVisibilityInternalChanged)));

	public static bool GetAreRowDetailsFrozen(DataGridRow row)
	{
		return (bool)((DependencyObject)row).GetValue(AreRowDetailsFrozenProperty);
	}

	private static void SetAreRowDetailsFrozen(DataGridRow row, bool value)
	{
		((DependencyObject)row).SetValue(AreRowDetailsFrozenPropertyKey, (object)value);
	}

	private static void OnAreRowDetailsFrozenInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		SetAreRowDetailsFrozen((DataGridRow)d, (bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	public static DataGridHeadersVisibility GetHeadersVisibility(DataGridRow row)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (DataGridHeadersVisibility)((DependencyObject)row).GetValue(HeadersVisibilityProperty);
	}

	private static void SetHeadersVisibility(DataGridRow row, DataGridHeadersVisibility value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)row).SetValue(HeadersVisibilityPropertyKey, (object)value);
	}

	private static void OnHeadersVisibilityInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		SetHeadersVisibility((DataGridRow)d, (DataGridHeadersVisibility)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}
}
