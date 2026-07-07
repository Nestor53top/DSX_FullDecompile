using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public sealed class AutoSuggestBox : ItemsControl
{
	private const string c_popupName = "SuggestionsPopup";

	private const string c_popupBorderName = "SuggestionsContainer";

	private const string c_textBoxName = "TextBox";

	private const string c_textBoxBorderName = "BorderElement";

	private const string c_controlCornerRadiusKey = "ControlCornerRadius";

	private const string c_overlayCornerRadiusKey = "OverlayCornerRadius";

	private TextBox m_textBox;

	private Button m_queryButton;

	private Popup m_suggestionsPopup;

	private AutoSuggestBoxListView m_suggestionsList;

	private PopupRepositionHelper m_popupRepositionHelper;

	private string m_searchText = string.Empty;

	private readonly DispatcherTimer m_delayTimer;

	private AutoSuggestionBoxTextChangeReason? m_textChangeReason;

	private bool m_ignoreTextBoxTextChange;

	private bool m_ignoreSelectionChange;

	public static readonly DependencyProperty UpdateTextOnSelectProperty;

	public static readonly DependencyProperty TextMemberPathProperty;

	public static readonly DependencyProperty TextBoxStyleProperty;

	public static readonly DependencyProperty TextProperty;

	public static readonly DependencyProperty PlaceholderTextProperty;

	public static readonly DependencyProperty MaxSuggestionListHeightProperty;

	public static readonly DependencyProperty IsSuggestionListOpenProperty;

	public static readonly DependencyProperty HeaderProperty;

	public static readonly DependencyProperty QueryIconProperty;

	public static readonly DependencyProperty DescriptionProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	public bool UpdateTextOnSelect
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(UpdateTextOnSelectProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UpdateTextOnSelectProperty, (object)value);
		}
	}

	public string TextMemberPath
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(TextMemberPathProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TextMemberPathProperty, (object)value);
		}
	}

	public Style TextBoxStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(TextBoxStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TextBoxStyleProperty, (object)value);
		}
	}

	public string Text
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(TextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TextProperty, (object)value);
		}
	}

	public string PlaceholderText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(PlaceholderTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PlaceholderTextProperty, (object)value);
		}
	}

	public double MaxSuggestionListHeight
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaxSuggestionListHeightProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxSuggestionListHeightProperty, (object)value);
		}
	}

	public bool IsSuggestionListOpen
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsSuggestionListOpenProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsSuggestionListOpenProperty, (object)value);
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

	public IconElement QueryIcon
	{
		get
		{
			return (IconElement)((DependencyObject)this).GetValue(QueryIconProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(QueryIconProperty, (object)value);
		}
	}

	public object Description
	{
		get
		{
			return ((DependencyObject)this).GetValue(DescriptionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DescriptionProperty, value);
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

	public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;

	public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged;

	public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

	static AutoSuggestBox()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00ba: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		UpdateTextOnSelectProperty = DependencyProperty.Register("UpdateTextOnSelect", typeof(bool), typeof(AutoSuggestBox), new PropertyMetadata((object)true));
		TextMemberPathProperty = DependencyProperty.Register("TextMemberPath", typeof(string), typeof(AutoSuggestBox), new PropertyMetadata((object)string.Empty));
		TextBoxStyleProperty = DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(AutoSuggestBox), (PropertyMetadata)null);
		TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AutoSuggestBox), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnTextPropertyChanged), new CoerceValueCallback(CoerceText)));
		PlaceholderTextProperty = ControlHelper.PlaceholderTextProperty.AddOwner(typeof(AutoSuggestBox));
		MaxSuggestionListHeightProperty = DependencyProperty.Register("MaxSuggestionListHeight", typeof(double), typeof(AutoSuggestBox), new PropertyMetadata((object)double.PositiveInfinity));
		IsSuggestionListOpenProperty = DependencyProperty.Register("IsSuggestionListOpen", typeof(bool), typeof(AutoSuggestBox), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsSuggestionListOpenPropertyChanged)));
		HeaderProperty = ControlHelper.HeaderProperty.AddOwner(typeof(AutoSuggestBox));
		QueryIconProperty = DependencyProperty.Register("QueryIcon", typeof(IconElement), typeof(AutoSuggestBox), new PropertyMetadata((object)null, new PropertyChangedCallback(OnQueryIconPropertyChanged)));
		DescriptionProperty = ControlHelper.DescriptionProperty.AddOwner(typeof(AutoSuggestBox));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(AutoSuggestBox));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(AutoSuggestBox));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoSuggestBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(AutoSuggestBox)));
	}

	public AutoSuggestBox()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		m_delayTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(150.0)
		};
		m_delayTimer.Tick += OnDelayTimerTick;
	}

	public override void OnApplyTemplate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Expected O, but got Unknown
		if (m_textBox != null)
		{
			((TextBoxBase)m_textBox).TextChanged -= new TextChangedEventHandler(OnTextBoxTextChanged);
			((UIElement)m_textBox).PreviewKeyDown -= new KeyEventHandler(OnTextBoxPreviewKeyDown);
		}
		if (m_queryButton != null)
		{
			((ButtonBase)m_queryButton).Click -= new RoutedEventHandler(OnQueryButtonClick);
			((DependencyObject)m_queryButton).ClearValue(ContentControl.ContentProperty);
			m_queryButton = null;
		}
		if (m_suggestionsPopup != null)
		{
			m_suggestionsPopup.Opened -= OnSuggestionsPopupOpened;
			m_suggestionsPopup.Closed -= OnSuggestionsPopupClosed;
			((DependencyObject)m_suggestionsPopup).ClearValue(Popup.PlacementTargetProperty);
		}
		if (m_popupRepositionHelper != null)
		{
			m_popupRepositionHelper.Dispose();
			m_popupRepositionHelper = null;
		}
		if (m_suggestionsList != null)
		{
			((FrameworkElement)m_suggestionsList).Loaded -= new RoutedEventHandler(OnSuggestionsListLoaded);
			((Selector)m_suggestionsList).SelectionChanged -= new SelectionChangedEventHandler(OnSuggestionsListSelectionChanged);
			m_suggestionsList.ItemClick -= OnSuggestionsListItemClick;
		}
		((FrameworkElement)this).OnApplyTemplate();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("TextBox");
		m_textBox = (TextBox)(object)((templateChild is TextBox) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("SuggestionsPopup");
		m_suggestionsPopup = (Popup)(object)((templateChild2 is Popup) ? templateChild2 : null);
		m_suggestionsList = ((FrameworkElement)this).GetTemplateChild("SuggestionsList") as AutoSuggestBoxListView;
		if (m_textBox != null)
		{
			((FrameworkElement)m_textBox).ApplyTemplate();
			m_queryButton = ((Control)(object)m_textBox).GetTemplateChild<Button>("QueryButton");
			((TextBoxBase)m_textBox).TextChanged += new TextChangedEventHandler(OnTextBoxTextChanged);
			((UIElement)m_textBox).PreviewKeyDown += new KeyEventHandler(OnTextBoxPreviewKeyDown);
			UpdateTextBox();
		}
		if (m_queryButton != null)
		{
			((ButtonBase)m_queryButton).Click += new RoutedEventHandler(OnQueryButtonClick);
			OnQueryIconChanged(null, QueryIcon);
		}
		if (m_suggestionsPopup != null)
		{
			m_suggestionsPopup.Opened += OnSuggestionsPopupOpened;
			m_suggestionsPopup.Closed += OnSuggestionsPopupClosed;
			m_popupRepositionHelper = new PopupRepositionHelper(m_suggestionsPopup, (UIElement)(object)this);
			if (m_textBox != null)
			{
				FrameworkElement templateChild3 = ((Control)(object)m_textBox).GetTemplateChild<FrameworkElement>("BorderElement");
				if (templateChild3 != null)
				{
					m_suggestionsPopup.PlacementTarget = (UIElement)(object)templateChild3;
				}
			}
		}
		if (m_suggestionsList != null)
		{
			((FrameworkElement)m_suggestionsList).Loaded += new RoutedEventHandler(OnSuggestionsListLoaded);
			((Selector)m_suggestionsList).SelectionChanged += new SelectionChangedEventHandler(OnSuggestionsListSelectionChanged);
			m_suggestionsList.ItemClick += OnSuggestionsListItemClick;
		}
	}

	protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
	{
		((ItemsControl)this).OnItemsChanged(e);
		OpenOrCloseSuggestionListIfFocused();
	}

	protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
	{
		((ItemsControl)this).OnItemsSourceChanged(oldValue, newValue);
		ClearSelection();
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		((FrameworkElement)this).OnGotFocus(e);
		TextBox textBox = m_textBox;
		if (textBox != null)
		{
			((UIElement)textBox).Focus();
		}
	}

	protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)this).OnIsKeyboardFocusWithinChanged(e);
		if (!(bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			CloseSuggestionList();
		}
	}

	private void OnTextChanged(DependencyPropertyChangedEventArgs args)
	{
		m_delayTimer.Stop();
		m_delayTimer.Tag = null;
		OpenOrCloseSuggestionListIfFocused();
		if (m_textChangeReason != AutoSuggestionBoxTextChangeReason.SuggestionChosen)
		{
			UpdateSearchText((string)((DependencyPropertyChangedEventArgs)(ref args)).NewValue);
		}
		UpdateTextBox();
		m_delayTimer.Tag = m_textChangeReason ?? AutoSuggestionBoxTextChangeReason.ProgrammaticChange;
		m_delayTimer.Start();
	}

	private void OnIsSuggestionListOpenChanged(DependencyPropertyChangedEventArgs args)
	{
		if (!(bool)((DependencyPropertyChangedEventArgs)(ref args)).NewValue)
		{
			UpdateSearchText(Text);
			ClearSelection();
		}
	}

	private void OnQueryIconChanged(DependencyPropertyChangedEventArgs args)
	{
		OnQueryIconChanged(((DependencyPropertyChangedEventArgs)(ref args)).OldValue as IconElement, ((DependencyPropertyChangedEventArgs)(ref args)).NewValue as IconElement);
	}

	private void OnQueryIconChanged(IconElement oldQueryIcon, IconElement newQueryIcon)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006f: Expected O, but got Unknown
		if (oldQueryIcon != null)
		{
			((DependencyObject)oldQueryIcon).ClearValue(IconElement.ForegroundProperty);
			if (newQueryIcon is SymbolIcon)
			{
				((DependencyObject)oldQueryIcon).ClearValue(SymbolIcon.FontSizeProperty);
			}
		}
		if (newQueryIcon != null && m_queryButton != null && newQueryIcon is SymbolIcon)
		{
			((FrameworkElement)newQueryIcon).SetBinding(SymbolIcon.FontSizeProperty, (BindingBase)new Binding
			{
				Path = new PropertyPath((object)TextElement.FontSizeProperty),
				RelativeSource = new RelativeSource
				{
					AncestorType = typeof(ContentPresenter)
				}
			});
		}
		UpdateQueryButton();
	}

	private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
	{
		if (!m_ignoreTextBoxTextChange)
		{
			UpdateTextValue(m_textBox.Text, AutoSuggestionBoxTextChangeReason.UserInput);
		}
	}

	private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (IsSuggestionListOpen)
		{
			Key key = e.Key;
			if ((int)key <= 13)
			{
				if ((int)key != 6)
				{
					if ((int)key == 13 && IsSuggestionListOpen)
					{
						UpdateTextValue(m_searchText);
						TryMoveCaretToEnd();
						CloseSuggestionList();
						((RoutedEventArgs)e).Handled = true;
					}
				}
				else if (TryCommitChosenSuggestion() || TryCommitTextBoxText())
				{
					((RoutedEventArgs)e).Handled = true;
				}
			}
			else if ((int)key != 24)
			{
				if ((int)key == 26 && (1 & Keyboard.Modifiers) == 0)
				{
					if (!TryMoveCaretToEnd())
					{
						SelectedIndexIncrement();
					}
					((RoutedEventArgs)e).Handled = true;
				}
			}
			else
			{
				SelectedIndexDecrement();
				((RoutedEventArgs)e).Handled = true;
			}
		}
		else if ((int)e.Key == 6 && TryCommitTextBoxText())
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private void OnQueryButtonClick(object sender, RoutedEventArgs e)
	{
		TryCommitTextBoxText();
	}

	private void OnSuggestionsPopupOpened(object sender, EventArgs e)
	{
		UpdateCornerRadius(isPopupOpen: true);
	}

	private void OnSuggestionsPopupClosed(object sender, EventArgs e)
	{
		UpdateCornerRadius(isPopupOpen: false);
	}

	private void OnSuggestionsListLoaded(object sender, RoutedEventArgs e)
	{
		ClearSelection();
		m_suggestionsList.ScrollToTop();
	}

	private void OnSuggestionsListSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (m_ignoreSelectionChange || !IsSuggestionListOpen)
		{
			return;
		}
		object selectedItem = ((Selector)m_suggestionsList).SelectedItem;
		if (selectedItem != null)
		{
			((ListBox)m_suggestionsList).ScrollIntoView(selectedItem);
			this.SuggestionChosen?.Invoke(this, new AutoSuggestBoxSuggestionChosenEventArgs
			{
				SelectedItem = selectedItem
			});
			if (UpdateTextOnSelect)
			{
				object selectedValue = ((Selector)m_suggestionsList).SelectedValue;
				if (selectedValue != null)
				{
					UpdateTextValue(selectedValue.ToString(), AutoSuggestionBoxTextChangeReason.SuggestionChosen);
				}
			}
		}
		else
		{
			m_suggestionsList.ScrollToTop();
			UpdateTextValue(m_searchText);
		}
		if (m_textBox != null)
		{
			m_textBox.CaretIndex = m_textBox.Text.Length;
		}
	}

	private void OnSuggestionsListItemClick(object sender, ItemClickEventArgs e)
	{
		((Selector)m_suggestionsList).SelectedItem = e.ClickedItem;
		TryCommitChosenSuggestion();
	}

	private void OnDelayTimerTick(object sender, EventArgs e)
	{
		m_delayTimer.Stop();
		if (m_delayTimer.Tag is AutoSuggestionBoxTextChangeReason reason)
		{
			m_delayTimer.Tag = null;
			this.TextChanged?.Invoke(this, new AutoSuggestBoxTextChangedEventArgs(this, Text)
			{
				Reason = reason
			});
		}
	}

	private void UpdateTextValue(string value, AutoSuggestionBoxTextChangeReason reason = AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
	{
		if (Text != value)
		{
			m_textChangeReason = reason;
			((DependencyObject)this).SetCurrentValue(TextProperty, (object)value);
			m_textChangeReason = null;
		}
	}

	private void UpdateSearchText(string value)
	{
		if (m_searchText != value)
		{
			m_searchText = value;
		}
	}

	private void UpdateTextBox()
	{
		if (m_textBox != null)
		{
			string text = Text;
			if (m_textBox.Text != text)
			{
				m_ignoreTextBoxTextChange = true;
				m_textBox.Text = text;
				m_ignoreTextBoxTextChange = false;
			}
		}
	}

	private void UpdateQueryButton()
	{
		if (m_queryButton != null)
		{
			IconElement queryIcon = QueryIcon;
			((ContentControl)m_queryButton).Content = queryIcon;
			((UIElement)m_queryButton).Visibility = (Visibility)((queryIcon == null) ? 2 : 0);
		}
	}

	private void OpenSuggestionList()
	{
		if (!IsSuggestionListOpen)
		{
			((DependencyObject)this).SetCurrentValue(IsSuggestionListOpenProperty, (object)true);
		}
	}

	private void CloseSuggestionList()
	{
		if (IsSuggestionListOpen)
		{
			((DependencyObject)this).SetCurrentValue(IsSuggestionListOpenProperty, (object)false);
		}
	}

	private void OpenOrCloseSuggestionListIfFocused()
	{
		if (((UIElement)this).IsKeyboardFocusWithin)
		{
			if (((ItemsControl)this).HasItems)
			{
				OpenSuggestionList();
			}
			else
			{
				CloseSuggestionList();
			}
		}
	}

	private void SelectedIndexIncrement()
	{
		if (m_suggestionsList != null)
		{
			int selectedIndex = ((Selector)m_suggestionsList).SelectedIndex;
			((Selector)m_suggestionsList).SelectedIndex = ((selectedIndex + 1 >= ((CollectionView)((ItemsControl)m_suggestionsList).Items).Count) ? (-1) : (selectedIndex + 1));
		}
	}

	private void SelectedIndexDecrement()
	{
		if (m_suggestionsList != null)
		{
			int selectedIndex = ((Selector)m_suggestionsList).SelectedIndex;
			if (selectedIndex >= 0)
			{
				AutoSuggestBoxListView suggestionsList = m_suggestionsList;
				int selectedIndex2 = ((Selector)suggestionsList).SelectedIndex;
				((Selector)suggestionsList).SelectedIndex = selectedIndex2 - 1;
			}
			else if (selectedIndex == -1)
			{
				((Selector)m_suggestionsList).SelectedIndex = ((CollectionView)((ItemsControl)m_suggestionsList).Items).Count - 1;
			}
		}
	}

	private void ClearSelection()
	{
		if (m_suggestionsList != null)
		{
			m_ignoreSelectionChange = true;
			((DependencyObject)m_suggestionsList).ClearValue(Selector.SelectedItemProperty);
			((DependencyObject)m_suggestionsList).ClearValue(Selector.SelectedIndexProperty);
			m_ignoreSelectionChange = false;
		}
	}

	private bool TryCommitChosenSuggestion()
	{
		if (IsSuggestionListOpen && m_textBox != null && m_suggestionsList != null)
		{
			object selectedItem = ((Selector)m_suggestionsList).SelectedItem;
			if (selectedItem != null)
			{
				CloseSuggestionList();
				SubmitQuery(m_textBox.Text, selectedItem);
				return true;
			}
		}
		return false;
	}

	private bool TryCommitTextBoxText()
	{
		if (m_textBox != null)
		{
			bool isSuggestionListOpen = IsSuggestionListOpen;
			SubmitQuery(m_textBox.Text, null);
			if (isSuggestionListOpen)
			{
				CloseSuggestionList();
			}
			return true;
		}
		return false;
	}

	private void SubmitQuery(string queryText, object chosenSuggestion)
	{
		this.QuerySubmitted?.Invoke(this, new AutoSuggestBoxQuerySubmittedEventArgs
		{
			QueryText = queryText,
			ChosenSuggestion = chosenSuggestion
		});
	}

	private bool TryMoveCaretToEnd()
	{
		if (m_textBox != null)
		{
			int length = m_textBox.Text.Length;
			if (m_textBox.CaretIndex < length)
			{
				m_textBox.CaretIndex = length;
				return true;
			}
		}
		return false;
	}

	private void UpdateCornerRadius(bool isPopupOpen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		CornerRadius val = CornerRadius;
		CornerRadius val2 = (CornerRadius)ResourceLookup("OverlayCornerRadius");
		if (isPopupOpen)
		{
			bool num = IsPopupOpenDown();
			CornerRadiusFilterConverter cornerRadiusFilterConverter = new CornerRadiusFilterConverter();
			CornerRadiusFilterKind filterKind = ((!num) ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom);
			val2 = cornerRadiusFilterConverter.Convert(val2, filterKind);
			CornerRadiusFilterKind filterKind2 = (num ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom);
			val = cornerRadiusFilterConverter.Convert(val, filterKind2);
		}
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("SuggestionsContainer");
		Border val3 = (Border)(object)((templateChild is Border) ? templateChild : null);
		if (val3 != null)
		{
			val3.CornerRadius = val2;
		}
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("TextBox");
		TextBox val4 = (TextBox)(object)((templateChild2 is TextBox) ? templateChild2 : null);
		if (val4 != null)
		{
			ControlHelper.SetCornerRadius((Control)(object)val4, val);
		}
	}

	private bool IsPopupOpenDown()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("SuggestionsContainer");
		Border val = (Border)(object)((templateChild is Border) ? templateChild : null);
		if (val != null)
		{
			DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("TextBox");
			TextBox val2 = (TextBox)(object)((templateChild2 is TextBox) ? templateChild2 : null);
			if (val2 != null)
			{
				Point val3 = ((UIElement)val).TranslatePoint(new Point(0.0, 0.0), (UIElement)(object)val2);
				num = ((Point)(ref val3)).Y;
			}
		}
		return num >= 0.0;
	}

	private object ResourceLookup(object key)
	{
		return ((FrameworkElement)this).TryFindResource(key);
	}

	private static void OnTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((AutoSuggestBox)(object)sender).OnTextChanged(args);
	}

	private static object CoerceText(DependencyObject d, object baseValue)
	{
		return baseValue ?? string.Empty;
	}

	private static void OnIsSuggestionListOpenPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((AutoSuggestBox)(object)sender).OnIsSuggestionListOpenChanged(args);
	}

	private static void OnQueryIconPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((AutoSuggestBox)(object)sender).OnQueryIconChanged(args);
	}
}
