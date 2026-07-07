using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernWpf.Controls.Primitives;

public static class DataGridHelper
{
	private class ColumnStylesHelper
	{
		private readonly DataGrid _dataGrid;

		public ColumnStylesHelper(DataGrid dataGrid)
		{
			_dataGrid = dataGrid;
		}

		public void Attach()
		{
			_dataGrid.Columns.CollectionChanged += OnColumnsCollectionChanged;
			foreach (DataGridColumn column in _dataGrid.Columns)
			{
				BindColumnStyleProperties(column);
			}
		}

		public void Detach()
		{
			_dataGrid.Columns.CollectionChanged -= OnColumnsCollectionChanged;
			foreach (DataGridColumn column in _dataGrid.Columns)
			{
				ClearColumnStyleProperties(column);
			}
		}

		private void OnColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems == null)
			{
				return;
			}
			foreach (object newItem in e.NewItems)
			{
				BindColumnStyleProperties((DataGridColumn)((newItem is DataGridColumn) ? newItem : null));
			}
		}

		private void BindColumnStyleProperties(DataGridColumn column)
		{
			DataGridTextColumn val = (DataGridTextColumn)(object)((column is DataGridTextColumn) ? column : null);
			if (val != null)
			{
				Bind((DependencyObject)(object)val, DataGridBoundColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, TextColumnElementStyleProperty);
				Bind((DependencyObject)(object)val, DataGridBoundColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, TextColumnEditingElementStyleProperty);
				Bind((DependencyObject)(object)val, DataGridTextColumn.FontSizeProperty, (DependencyObject)(object)_dataGrid, TextColumnFontSizeProperty);
				return;
			}
			DataGridCheckBoxColumn val2 = (DataGridCheckBoxColumn)(object)((column is DataGridCheckBoxColumn) ? column : null);
			if (val2 != null)
			{
				Bind((DependencyObject)(object)val2, DataGridBoundColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, CheckBoxColumnElementStyleProperty);
				Bind((DependencyObject)(object)val2, DataGridBoundColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, CheckBoxColumnEditingElementStyleProperty);
				return;
			}
			DataGridComboBoxColumn val3 = (DataGridComboBoxColumn)(object)((column is DataGridComboBoxColumn) ? column : null);
			if (val3 != null)
			{
				Bind((DependencyObject)(object)val3, DataGridComboBoxColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, ComboBoxColumnElementStyleProperty);
				Bind((DependencyObject)(object)val3, DataGridComboBoxColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, ComboBoxColumnEditingElementStyleProperty);
				return;
			}
			DataGridHyperlinkColumn val4 = (DataGridHyperlinkColumn)(object)((column is DataGridHyperlinkColumn) ? column : null);
			if (val4 != null)
			{
				Bind((DependencyObject)(object)val4, DataGridBoundColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, HyperlinkColumnElementStyleProperty);
				Bind((DependencyObject)(object)val4, DataGridBoundColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, HyperlinkColumnEditingElementStyleProperty);
			}
		}

		private void ClearColumnStyleProperties(DataGridColumn column)
		{
			DataGridTextColumn val = (DataGridTextColumn)(object)((column is DataGridTextColumn) ? column : null);
			if (val != null)
			{
				Clear((DependencyObject)(object)val, DataGridBoundColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, TextColumnElementStyleProperty);
				Clear((DependencyObject)(object)val, DataGridBoundColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, TextColumnEditingElementStyleProperty);
				Clear((DependencyObject)(object)val, DataGridTextColumn.FontSizeProperty, (DependencyObject)(object)_dataGrid, TextColumnFontSizeProperty);
				return;
			}
			DataGridCheckBoxColumn val2 = (DataGridCheckBoxColumn)(object)((column is DataGridCheckBoxColumn) ? column : null);
			if (val2 != null)
			{
				Clear((DependencyObject)(object)val2, DataGridBoundColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, CheckBoxColumnElementStyleProperty);
				Clear((DependencyObject)(object)val2, DataGridBoundColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, CheckBoxColumnEditingElementStyleProperty);
				return;
			}
			DataGridComboBoxColumn val3 = (DataGridComboBoxColumn)(object)((column is DataGridComboBoxColumn) ? column : null);
			if (val3 != null)
			{
				Clear((DependencyObject)(object)val3, DataGridComboBoxColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, ComboBoxColumnElementStyleProperty);
				Clear((DependencyObject)(object)val3, DataGridComboBoxColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, ComboBoxColumnEditingElementStyleProperty);
				return;
			}
			DataGridHyperlinkColumn val4 = (DataGridHyperlinkColumn)(object)((column is DataGridHyperlinkColumn) ? column : null);
			if (val4 != null)
			{
				Clear((DependencyObject)(object)val4, DataGridBoundColumn.ElementStyleProperty, (DependencyObject)(object)_dataGrid, HyperlinkColumnElementStyleProperty);
				Clear((DependencyObject)(object)val4, DataGridBoundColumn.EditingElementStyleProperty, (DependencyObject)(object)_dataGrid, HyperlinkColumnEditingElementStyleProperty);
			}
		}

		private static void Bind(DependencyObject target, DependencyProperty targetDP, DependencyObject source, DependencyProperty sourceDP)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			if (target.ReadLocalValue(targetDP) == DependencyProperty.UnsetValue)
			{
				BindingOperations.SetBinding(target, targetDP, (BindingBase)new Binding
				{
					Path = new PropertyPath((object)sourceDP),
					Source = source
				});
			}
		}

		private static void Clear(DependencyObject target, DependencyProperty targetDP, DependencyObject source, DependencyProperty sourceDP)
		{
			Binding binding = BindingOperations.GetBinding(target, targetDP);
			if (binding != null && binding.Source == source)
			{
				target.ClearValue(targetDP);
			}
		}
	}

	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DataGridHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsEnabledChanged)));

	public static readonly DependencyProperty TextColumnElementStyleProperty = DependencyProperty.RegisterAttached("TextColumnElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty TextColumnEditingElementStyleProperty = DependencyProperty.RegisterAttached("TextColumnEditingElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty TextColumnFontSizeProperty = DependencyProperty.RegisterAttached("TextColumnFontSize", typeof(double), typeof(DataGridHelper), new PropertyMetadata((object)SystemFonts.MessageFontSize));

	public static readonly DependencyProperty CheckBoxColumnElementStyleProperty = DependencyProperty.RegisterAttached("CheckBoxColumnElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty CheckBoxColumnEditingElementStyleProperty = DependencyProperty.RegisterAttached("CheckBoxColumnEditingElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty ComboBoxColumnElementStyleProperty = DependencyProperty.RegisterAttached("ComboBoxColumnElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty ComboBoxColumnEditingElementStyleProperty = DependencyProperty.RegisterAttached("ComboBoxColumnEditingElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty HyperlinkColumnElementStyleProperty = DependencyProperty.RegisterAttached("HyperlinkColumnElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty HyperlinkColumnEditingElementStyleProperty = DependencyProperty.RegisterAttached("HyperlinkColumnEditingElementStyle", typeof(Style), typeof(DataGridHelper));

	public static readonly DependencyProperty UseModernColumnStylesProperty = DependencyProperty.RegisterAttached("UseModernColumnStyles", typeof(bool), typeof(DataGridHelper), new PropertyMetadata(new PropertyChangedCallback(OnUseModernColumnStylesChanged)));

	private static readonly DependencyProperty ColumnStylesHelperProperty = DependencyProperty.RegisterAttached("ColumnStylesHelper", typeof(ColumnStylesHelper), typeof(DataGridHelper), new PropertyMetadata(new PropertyChangedCallback(OnColumnStylesHelperChanged)));

	public static bool GetIsEnabled(DataGrid dataGrid)
	{
		return (bool)((DependencyObject)dataGrid).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(DataGrid dataGrid, bool value)
	{
		((DependencyObject)dataGrid).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		DataGrid val = (DataGrid)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			val.LoadingRow += OnLoadingRow;
		}
		else
		{
			val.LoadingRow -= OnLoadingRow;
		}
	}

	private static void OnLoadingRow(object sender, DataGridRowEventArgs e)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		DataGridRow row = e.Row;
		if (((DependencyObject)row).ReadLocalValue(DataGridRowHelper.AreRowDetailsFrozenInternalProperty) == DependencyProperty.UnsetValue)
		{
			((FrameworkElement)row).SetBinding(DataGridRowHelper.AreRowDetailsFrozenInternalProperty, (BindingBase)new Binding
			{
				Path = new PropertyPath((object)DataGrid.AreRowDetailsFrozenProperty),
				Source = sender
			});
		}
		if (((DependencyObject)row).ReadLocalValue(DataGridRowHelper.HeadersVisibilityInternalProperty) == DependencyProperty.UnsetValue)
		{
			((FrameworkElement)row).SetBinding(DataGridRowHelper.HeadersVisibilityInternalProperty, (BindingBase)new Binding
			{
				Path = new PropertyPath((object)DataGrid.HeadersVisibilityProperty),
				Source = sender
			});
		}
	}

	public static Style GetTextColumnElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(TextColumnElementStyleProperty);
	}

	public static void SetTextColumnElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(TextColumnElementStyleProperty, (object)value);
	}

	public static Style GetTextColumnEditingElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(TextColumnEditingElementStyleProperty);
	}

	public static void SetTextColumnEditingElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(TextColumnEditingElementStyleProperty, (object)value);
	}

	public static double GetTextColumnFontSize(DataGrid dataGrid)
	{
		return (double)((DependencyObject)dataGrid).GetValue(TextColumnFontSizeProperty);
	}

	public static void SetTextColumnFontSize(DataGrid dataGrid, double value)
	{
		((DependencyObject)dataGrid).SetValue(TextColumnFontSizeProperty, (object)value);
	}

	public static Style GetCheckBoxColumnElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(CheckBoxColumnElementStyleProperty);
	}

	public static void SetCheckBoxColumnElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(CheckBoxColumnElementStyleProperty, (object)value);
	}

	public static Style GetCheckBoxColumnEditingElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(CheckBoxColumnEditingElementStyleProperty);
	}

	public static void SetCheckBoxColumnEditingElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(CheckBoxColumnEditingElementStyleProperty, (object)value);
	}

	public static Style GetComboBoxColumnElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(ComboBoxColumnElementStyleProperty);
	}

	public static void SetComboBoxColumnElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(ComboBoxColumnElementStyleProperty, (object)value);
	}

	public static Style GetComboBoxColumnEditingElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(ComboBoxColumnEditingElementStyleProperty);
	}

	public static void SetComboBoxColumnEditingElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(ComboBoxColumnEditingElementStyleProperty, (object)value);
	}

	public static Style GetHyperlinkColumnElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(HyperlinkColumnElementStyleProperty);
	}

	public static void SetHyperlinkColumnElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(HyperlinkColumnElementStyleProperty, (object)value);
	}

	public static Style GetHyperlinkColumnEditingElementStyle(DataGrid dataGrid)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)dataGrid).GetValue(HyperlinkColumnEditingElementStyleProperty);
	}

	public static void SetHyperlinkColumnEditingElementStyle(DataGrid dataGrid, Style value)
	{
		((DependencyObject)dataGrid).SetValue(HyperlinkColumnEditingElementStyleProperty, (object)value);
	}

	public static bool GetUseModernColumnStyles(DataGrid dataGrid)
	{
		return (bool)((DependencyObject)dataGrid).GetValue(UseModernColumnStylesProperty);
	}

	public static void SetUseModernColumnStyles(DataGrid dataGrid, bool value)
	{
		((DependencyObject)dataGrid).SetValue(UseModernColumnStylesProperty, (object)value);
	}

	private static void OnUseModernColumnStylesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		DataGrid val = (DataGrid)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((DependencyObject)val).SetValue(ColumnStylesHelperProperty, (object)new ColumnStylesHelper(val));
		}
		else
		{
			((DependencyObject)val).ClearValue(ColumnStylesHelperProperty);
		}
	}

	private static void OnColumnStylesHelperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is ColumnStylesHelper columnStylesHelper)
		{
			columnStylesHelper.Detach();
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue is ColumnStylesHelper columnStylesHelper2)
		{
			columnStylesHelper2.Attach();
		}
	}
}
