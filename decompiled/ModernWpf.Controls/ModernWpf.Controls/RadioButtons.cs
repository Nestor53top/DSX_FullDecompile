using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[ContentProperty("Items")]
[TemplatePart(Name = "InnerRepeater", Type = typeof(ItemsRepeater))]
public class RadioButtons : Control
{
	public static readonly DependencyProperty ItemsSourceProperty;

	public static readonly DependencyProperty ItemsProperty;

	public static readonly DependencyProperty ItemTemplateProperty;

	public static readonly DependencyProperty SelectedIndexProperty;

	public static readonly DependencyProperty SelectedItemProperty;

	public static readonly DependencyProperty MaxColumnsProperty;

	public static readonly DependencyProperty HeaderProperty;

	public static readonly DependencyProperty HeaderTemplateProperty;

	public static readonly RoutedEvent SelectionChangedEvent;

	private int m_selectedIndex = -1;

	private bool m_currentlySelecting;

	private bool m_blockSelecting = true;

	private bool m_currentlySettingFocus;

	private ItemsRepeater m_repeater;

	private RadioButtonsElementFactory m_radioButtonsElementFactory;

	private bool m_testHooksEnabled;

	private const string s_repeaterName = "InnerRepeater";

	public IEnumerable ItemsSource
	{
		get
		{
			return (IEnumerable)((DependencyObject)this).GetValue(ItemsSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemsSourceProperty, (object)value);
		}
	}

	public IList Items => (IList)((DependencyObject)this).GetValue(ItemsProperty);

	public object ItemTemplate
	{
		get
		{
			return ((DependencyObject)this).GetValue(ItemTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemTemplateProperty, value);
		}
	}

	public int SelectedIndex
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(SelectedIndexProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectedIndexProperty, (object)value);
		}
	}

	public object SelectedItem
	{
		get
		{
			return ((DependencyObject)this).GetValue(SelectedItemProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectedItemProperty, value);
		}
	}

	public int MaxColumns
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(MaxColumnsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxColumnsProperty, (object)value);
		}
	}

	public object Header
	{
		get
		{
			return ((DependencyObject)this).GetValue(HeaderProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HeaderProperty, value);
		}
	}

	public DataTemplate HeaderTemplate
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplate)((DependencyObject)this).GetValue(HeaderTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HeaderTemplateProperty, (object)value);
		}
	}

	public event SelectionChangedEventHandler SelectionChanged
	{
		add
		{
			((UIElement)this).AddHandler(SelectionChangedEvent, (Delegate)(object)value);
		}
		remove
		{
			((UIElement)this).RemoveHandler(SelectionChangedEvent, (Delegate)(object)value);
		}
	}

	static RadioButtons()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(RadioButtons), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));
		ItemsProperty = DependencyProperty.Register("Items", typeof(IList), typeof(RadioButtons), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemsPropertyChanged)));
		ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(object), typeof(RadioButtons), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemTemplateChanged)));
		SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(RadioButtons), (PropertyMetadata)new FrameworkPropertyMetadata((object)(-1), (FrameworkPropertyMetadataOptions)1280, new PropertyChangedCallback(OnSelectedIndexPropertyChanged)));
		SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(RadioButtons), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, (FrameworkPropertyMetadataOptions)256, new PropertyChangedCallback(OnSelectedItemPropertyChanged)));
		MaxColumnsProperty = DependencyProperty.Register("MaxColumns", typeof(int), typeof(RadioButtons), (PropertyMetadata)new FrameworkPropertyMetadata((object)1));
		HeaderProperty = ControlHelper.HeaderProperty.AddOwner(typeof(RadioButtons));
		HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(RadioButtons), (PropertyMetadata)null);
		SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", (RoutingStrategy)2, typeof(SelectionChangedEventHandler), typeof(RadioButtons));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioButtons), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(RadioButtons)));
	}

	public RadioButtons()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		ObservableCollection<object> observableCollection = new ObservableCollection<object>();
		((DependencyObject)this).SetValue(ItemsProperty, (object)observableCollection);
		((UIElement)this).PreviewKeyDown += new KeyEventHandler(OnChildPreviewKeyDown);
		m_radioButtonsElementFactory = new RadioButtonsElementFactory();
		((UIElement)this).IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
	}

	private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((RadioButtons)(object)d).UpdateItemsSource();
	}

	private static void OnItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((RadioButtons)(object)d).UpdateItemsSource();
	}

	private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((RadioButtons)(object)d).UpdateItemTemplate();
	}

	private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((RadioButtons)(object)d).UpdateSelectedIndex();
	}

	private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((RadioButtons)(object)d).UpdateSelectedItem();
	}

	public override void OnApplyTemplate()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		((FrameworkElement)this).OnApplyTemplate();
		if (m_repeater != null)
		{
			m_repeater.ElementPrepared -= OnRepeaterElementPrepared;
			m_repeater.ElementClearing -= OnRepeaterElementClearing;
			m_repeater.ElementIndexChanged -= OnRepeaterElementIndexChanged;
			((FrameworkElement)m_repeater).Loaded -= new RoutedEventHandler(OnRepeaterLoaded);
			if (m_repeater.Layout is ColumnMajorUniformToLargestGridLayout columnMajorUniformToLargestGridLayout)
			{
				((DependencyObject)columnMajorUniformToLargestGridLayout).ClearValue(ColumnMajorUniformToLargestGridLayout.MaxColumnsProperty);
			}
		}
		m_repeater = ((FrameworkElement)this).GetTemplateChild("InnerRepeater") as ItemsRepeater;
		if (m_repeater != null)
		{
			m_repeater.ItemTemplate = m_radioButtonsElementFactory;
			m_repeater.ElementPrepared += OnRepeaterElementPrepared;
			m_repeater.ElementClearing += OnRepeaterElementClearing;
			m_repeater.ElementIndexChanged += OnRepeaterElementIndexChanged;
			((FrameworkElement)m_repeater).Loaded += new RoutedEventHandler(OnRepeaterLoaded);
			if (m_repeater.Layout is ColumnMajorUniformToLargestGridLayout columnMajorUniformToLargestGridLayout2)
			{
				BindingOperations.SetBinding((DependencyObject)(object)columnMajorUniformToLargestGridLayout2, ColumnMajorUniformToLargestGridLayout.MaxColumnsProperty, (BindingBase)new Binding
				{
					Path = new PropertyPath((object)MaxColumnsProperty),
					Source = this
				});
			}
		}
		UpdateItemsSource();
		UpdateVisualStateForIsEnabledChange();
	}

	protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs args)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Invalid comparison between Unknown and I4
		if (!m_currentlySettingFocus)
		{
			ItemsRepeater repeater = m_repeater;
			if (repeater != null)
			{
				IInputElement oldFocus = args.OldFocus;
				if (oldFocus != null)
				{
					UIElement val = (UIElement)(object)((oldFocus is UIElement) ? oldFocus : null);
					if (val != null)
					{
						DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)val);
						if ((object)repeater != parent)
						{
							UIElement val2 = repeater.TryGetElement(m_selectedIndex);
							if (val2 != null)
							{
								try
								{
									m_currentlySettingFocus = true;
									if (val2.Focus())
									{
										((RoutedEventArgs)args).Handled = true;
									}
								}
								finally
								{
									m_currentlySettingFocus = false;
								}
							}
						}
						else if ((((KeyboardEventArgs)args).KeyboardDevice.Modifiers & 2) != 2)
						{
							IInputElement newFocus = args.NewFocus;
							UIElement val3 = (UIElement)(object)((newFocus is UIElement) ? newFocus : null);
							if (val3 != null)
							{
								Select(repeater.GetElementIndex(val3));
							}
						}
					}
				}
			}
		}
		((UIElement)this).OnPreviewGotKeyboardFocus(args);
	}

	private void OnRepeaterLoaded(object sender, RoutedEventArgs args)
	{
		if (m_repeater != null)
		{
			if (m_testHooksEnabled)
			{
				AttachToLayoutChanged();
			}
			m_blockSelecting = false;
			if (SelectedIndex == -1 && SelectedItem != null)
			{
				UpdateSelectedItem();
			}
			else
			{
				UpdateSelectedIndex();
			}
			OnRepeaterCollectionChanged(null, null);
		}
	}

	private void OnChildPreviewKeyDown(object sender, KeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected I4, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		Key key = args.Key;
		switch (key - 23)
		{
		case 3:
			if (MoveFocusNext())
			{
				((RoutedEventArgs)args).Handled = true;
			}
			else
			{
				((RoutedEventArgs)args).Handled = HandleEdgeCaseFocus(first: false, ((RoutedEventArgs)args).OriginalSource);
			}
			break;
		case 1:
			if (MoveFocusPrevious())
			{
				((RoutedEventArgs)args).Handled = true;
			}
			else
			{
				((RoutedEventArgs)args).Handled = HandleEdgeCaseFocus(first: true, ((RoutedEventArgs)args).OriginalSource);
			}
			break;
		case 2:
		{
			object originalSource2 = ((RoutedEventArgs)args).OriginalSource;
			UIElement val2 = (UIElement)((originalSource2 is UIElement) ? originalSource2 : null);
			if (val2 != null)
			{
				if (val2.MoveFocus(new TraversalRequest((FocusNavigationDirection)5)))
				{
					((RoutedEventArgs)args).Handled = true;
				}
				else
				{
					((RoutedEventArgs)args).Handled = HandleEdgeCaseFocus(first: false, ((RoutedEventArgs)args).OriginalSource);
				}
			}
			break;
		}
		case 0:
		{
			object originalSource = ((RoutedEventArgs)args).OriginalSource;
			UIElement val = (UIElement)((originalSource is UIElement) ? originalSource : null);
			if (val != null)
			{
				if (val.MoveFocus(new TraversalRequest((FocusNavigationDirection)4)))
				{
					((RoutedEventArgs)args).Handled = true;
				}
				else
				{
					((RoutedEventArgs)args).Handled = HandleEdgeCaseFocus(first: true, ((RoutedEventArgs)args).OriginalSource);
				}
			}
			break;
		}
		}
	}

	private bool HandleEdgeCaseFocus(bool first, object source)
	{
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			UIElement val = (UIElement)((source is UIElement) ? source : null);
			if (val != null)
			{
				int num = calculateIndex();
				if (repeater.GetElementIndex(val) == num)
				{
					return true;
				}
			}
		}
		return false;
		int calculateIndex()
		{
			if (first)
			{
				return 0;
			}
			ItemsSourceView itemsSourceView = repeater.ItemsSourceView;
			if (itemsSourceView != null)
			{
				return itemsSourceView.Count - 1;
			}
			return -1;
		}
	}

	private void OnRepeaterElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		UIElement element = args.Element;
		if (element == null)
		{
			return;
		}
		ToggleButton val = (ToggleButton)(object)((element is ToggleButton) ? element : null);
		if (val != null)
		{
			val.Checked += new RoutedEventHandler(OnChildChecked);
			val.Unchecked += new RoutedEventHandler(OnChildUnchecked);
			if (val.IsChecked == true)
			{
				m_blockSelecting = false;
				Select(args.Index);
			}
		}
	}

	private void OnRepeaterElementClearing(ItemsRepeater sender, ItemsRepeaterElementClearingEventArgs args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		UIElement element = args.Element;
		if (element != null)
		{
			ToggleButton val = (ToggleButton)(object)((element is ToggleButton) ? element : null);
			if (val != null)
			{
				val.Checked -= new RoutedEventHandler(OnChildChecked);
				val.Unchecked -= new RoutedEventHandler(OnChildUnchecked);
			}
			ToggleButton val2 = (ToggleButton)(object)((element is ToggleButton) ? element : null);
			if (val2 != null && val2.IsChecked == true)
			{
				Select(-1);
			}
		}
	}

	private void OnRepeaterElementIndexChanged(ItemsRepeater sender, ItemsRepeaterElementIndexChangedEventArgs args)
	{
		UIElement element = args.Element;
		if (element != null)
		{
			ToggleButton val = (ToggleButton)(object)((element is ToggleButton) ? element : null);
			if (val != null && val.IsChecked == true)
			{
				Select(args.NewIndex);
			}
		}
	}

	private void OnRepeaterCollectionChanged(object sender, object args)
	{
	}

	private void Select(int index)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		if (!m_blockSelecting && !m_currentlySelecting && m_selectedIndex != index)
		{
			try
			{
				m_currentlySelecting = true;
				int selectedIndex = m_selectedIndex;
				m_selectedIndex = index;
				object dataAtIndex = GetDataAtIndex(m_selectedIndex, containerIsChecked: true);
				object dataAtIndex2 = GetDataAtIndex(selectedIndex, containerIsChecked: false);
				((DependencyObject)this).SetCurrentValue(SelectedIndexProperty, (object)m_selectedIndex);
				((DependencyObject)this).SetCurrentValue(SelectedItemProperty, dataAtIndex);
				((UIElement)this).RaiseEvent((RoutedEventArgs)new SelectionChangedEventArgs(SelectionChangedEvent, (IList)new object[1] { dataAtIndex2 }, (IList)new object[1] { dataAtIndex }));
			}
			finally
			{
				m_currentlySelecting = false;
			}
		}
	}

	private object GetDataAtIndex(int index, bool containerIsChecked)
	{
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			UIElement val = repeater.TryGetElement(index);
			if (val != null)
			{
				ToggleButton val2 = (ToggleButton)(object)((val is ToggleButton) ? val : null);
				if (val2 != null)
				{
					((DependencyObject)val2).SetCurrentValue(ToggleButton.IsCheckedProperty, (object)containerIsChecked);
				}
			}
			if (index >= 0)
			{
				ItemsSourceView itemsSourceView = repeater.ItemsSourceView;
				if (itemsSourceView != null && index < itemsSourceView.Count)
				{
					return itemsSourceView.GetAt(index);
				}
			}
		}
		return null;
	}

	private void OnChildChecked(object sender, RoutedEventArgs args)
	{
		if (m_currentlySelecting)
		{
			return;
		}
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			UIElement val = (UIElement)((sender is UIElement) ? sender : null);
			if (val != null)
			{
				Select(repeater.GetElementIndex(val));
			}
		}
	}

	private void OnChildUnchecked(object sender, RoutedEventArgs args)
	{
		if (m_currentlySelecting)
		{
			return;
		}
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			UIElement val = (UIElement)((sender is UIElement) ? sender : null);
			if (val != null && m_selectedIndex == repeater.GetElementIndex(val))
			{
				Select(-1);
			}
		}
	}

	private bool MoveFocusNext()
	{
		return MoveFocus(1);
	}

	private bool MoveFocusPrevious()
	{
		return MoveFocus(-1);
	}

	private bool MoveFocus(int indexIncrement)
	{
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			IInputElement focusedElement = Keyboard.FocusedElement;
			UIElement val = (UIElement)(object)((focusedElement is UIElement) ? focusedElement : null);
			if (val != null)
			{
				int elementIndex = repeater.GetElementIndex(val);
				if (elementIndex >= 0)
				{
					elementIndex += indexIncrement;
					for (int count = repeater.ItemsSourceView.Count; elementIndex >= 0 && elementIndex < count; elementIndex += indexIncrement)
					{
						UIElement val2 = repeater.TryGetElement(elementIndex);
						if (val2 != null)
						{
							Control val3 = (Control)(object)((val2 is Control) ? val2 : null);
							if (val3 != null && ((UIElement)val3).Focus())
							{
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		UpdateVisualStateForIsEnabledChange();
	}

	public UIElement ContainerFromIndex(int index)
	{
		return m_repeater?.TryGetElement(index);
	}

	private void UpdateItemsSource()
	{
		Select(-1);
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			ItemsSourceView itemsSourceView = repeater.ItemsSourceView;
			if (itemsSourceView != null)
			{
				itemsSourceView.CollectionChanged -= OnRepeaterCollectionChanged;
			}
			repeater.ItemsSource = GetItemsSource();
			ItemsSourceView itemsSourceView2 = repeater.ItemsSourceView;
			if (itemsSourceView2 != null)
			{
				itemsSourceView2.CollectionChanged += OnRepeaterCollectionChanged;
			}
		}
	}

	private object GetItemsSource()
	{
		IEnumerable itemsSource = ItemsSource;
		if (itemsSource != null)
		{
			return itemsSource;
		}
		return Items;
	}

	private void UpdateSelectedIndex()
	{
		if (!m_currentlySelecting)
		{
			Select(SelectedIndex);
		}
	}

	private void UpdateSelectedItem()
	{
		if (m_currentlySelecting)
		{
			return;
		}
		ItemsRepeater repeater = m_repeater;
		if (repeater != null)
		{
			ItemsSourceView itemsSourceView = repeater.ItemsSourceView;
			if (itemsSourceView != null)
			{
				Select(itemsSourceView.IndexOf(SelectedItem));
			}
		}
	}

	private void UpdateItemTemplate()
	{
		m_radioButtonsElementFactory.UserElementFactory(ItemTemplate);
	}

	private void UpdateVisualStateForIsEnabledChange()
	{
		VisualStateManager.GoToState((FrameworkElement)(object)this, ((UIElement)this).IsEnabled ? "Normal" : "Disabled", false);
	}

	internal void SetTestHooksEnabled(bool enabled)
	{
		if (m_testHooksEnabled != enabled)
		{
			m_testHooksEnabled = enabled;
			if (enabled)
			{
				AttachToLayoutChanged();
			}
			else
			{
				DetatchFromLayoutChanged();
			}
		}
	}

	private void OnLayoutChanged(ColumnMajorUniformToLargestGridLayout sender, object args)
	{
		RadioButtonsTestHooks.NotifyLayoutChanged(this);
	}

	internal int GetRows()
	{
		return GetLayout()?.GetRows() ?? (-1);
	}

	internal int GetColumns()
	{
		return GetLayout()?.GetColumns() ?? (-1);
	}

	internal int GetLargerColumns()
	{
		return GetLayout()?.GetLargerColumns() ?? (-1);
	}

	private void AttachToLayoutChanged()
	{
		ColumnMajorUniformToLargestGridLayout layout = GetLayout();
		if (layout != null)
		{
			layout.SetTestHooksEnabled(enabled: true);
			layout.LayoutChanged += OnLayoutChanged;
		}
	}

	private void DetatchFromLayoutChanged()
	{
		ColumnMajorUniformToLargestGridLayout layout = GetLayout();
		if (layout != null)
		{
			layout.SetTestHooksEnabled(enabled: false);
			layout.LayoutChanged -= OnLayoutChanged;
		}
	}

	private ColumnMajorUniformToLargestGridLayout GetLayout()
	{
		if (m_repeater != null && m_repeater.Layout is ColumnMajorUniformToLargestGridLayout result)
		{
			return result;
		}
		return null;
	}
}
