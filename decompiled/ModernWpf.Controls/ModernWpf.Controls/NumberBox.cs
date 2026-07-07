using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[TemplatePart(Name = "DownSpinButton", Type = typeof(RepeatButton))]
[TemplatePart(Name = "UpSpinButton", Type = typeof(RepeatButton))]
[TemplatePart(Name = "InputBox", Type = typeof(TextBox))]
[TemplatePart(Name = "UpDownPopup", Type = typeof(Popup))]
[TemplatePart(Name = "PopupDownSpinButton", Type = typeof(RepeatButton))]
[TemplatePart(Name = "PopupUpSpinButton", Type = typeof(RepeatButton))]
public class NumberBox : Control
{
	private const string c_numberBoxHeaderName = "HeaderContentPresenter";

	private const string c_numberBoxDownButtonName = "DownSpinButton";

	private const string c_numberBoxUpButtonName = "UpSpinButton";

	private const string c_numberBoxTextBoxName = "InputBox";

	private const string c_numberBoxPopupButtonName = "PopupButton";

	private const string c_numberBoxPopupName = "UpDownPopup";

	private const string c_numberBoxPopupDownButtonName = "PopupDownSpinButton";

	private const string c_numberBoxPopupUpButtonName = "PopupUpSpinButton";

	private const string c_numberBoxPopupContentRootName = "PopupContentRoot";

	private const double c_popupShadowDepth = 16.0;

	private const string c_numberBoxPopupShadowDepthName = "NumberBoxPopupShadowDepth";

	private static readonly ResourceAccessor ResourceAccessor;

	private bool m_valueUpdating;

	private bool m_textUpdating;

	private DefaultNumberRounder m_displayRounder = new DefaultNumberRounder();

	private TextBox m_textBox;

	private ContentPresenter m_headerPresenter;

	private Popup m_popup;

	private PopupRepositionHelper m_popupRepositionHelper;

	public static readonly DependencyProperty MinimumProperty;

	public static readonly DependencyProperty MaximumProperty;

	public static readonly DependencyProperty ValueProperty;

	public static readonly DependencyProperty SmallChangeProperty;

	public static readonly DependencyProperty LargeChangeProperty;

	public static readonly DependencyProperty TextProperty;

	public static readonly DependencyProperty HeaderProperty;

	public static readonly DependencyProperty HeaderTemplateProperty;

	public static readonly DependencyProperty PlaceholderTextProperty;

	public static readonly DependencyProperty SelectionBrushProperty;

	public static readonly DependencyProperty TextAlignmentProperty;

	public static readonly DependencyProperty DescriptionProperty;

	public static readonly DependencyProperty ValidationModeProperty;

	public static readonly DependencyProperty SpinButtonPlacementModeProperty;

	public static readonly DependencyProperty IsWrapEnabledProperty;

	public static readonly DependencyProperty AcceptsExpressionProperty;

	public static readonly DependencyProperty NumberFormatterProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	public double Minimum
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MinimumProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MinimumProperty, (object)value);
		}
	}

	public double Maximum
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaximumProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaximumProperty, (object)value);
		}
	}

	public double Value
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ValueProperty, (object)value);
		}
	}

	public double SmallChange
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(SmallChangeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SmallChangeProperty, (object)value);
		}
	}

	public double LargeChange
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(LargeChangeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LargeChangeProperty, (object)value);
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

	public Brush SelectionBrush
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(SelectionBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SelectionBrushProperty, (object)value);
		}
	}

	public TextAlignment TextAlignment
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (TextAlignment)((DependencyObject)this).GetValue(TextAlignmentProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(TextAlignmentProperty, (object)value);
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

	public NumberBoxValidationMode ValidationMode
	{
		get
		{
			return (NumberBoxValidationMode)((DependencyObject)this).GetValue(ValidationModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ValidationModeProperty, (object)value);
		}
	}

	public NumberBoxSpinButtonPlacementMode SpinButtonPlacementMode
	{
		get
		{
			return (NumberBoxSpinButtonPlacementMode)((DependencyObject)this).GetValue(SpinButtonPlacementModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SpinButtonPlacementModeProperty, (object)value);
		}
	}

	public bool IsWrapEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsWrapEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsWrapEnabledProperty, (object)value);
		}
	}

	public bool AcceptsExpression
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(AcceptsExpressionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AcceptsExpressionProperty, (object)value);
		}
	}

	public INumberBoxNumberFormatter NumberFormatter
	{
		get
		{
			return (INumberBoxNumberFormatter)((DependencyObject)this).GetValue(NumberFormatterProperty);
		}
		set
		{
			ValidateNumberFormatter(value);
			((DependencyObject)this).SetValue(NumberFormatterProperty, (object)value);
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

	public event TypedEventHandler<NumberBox, NumberBoxValueChangedEventArgs> ValueChanged;

	static NumberBox()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Expected O, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Expected O, but got Unknown
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Expected O, but got Unknown
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Expected O, but got Unknown
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Expected O, but got Unknown
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Expected O, but got Unknown
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Expected O, but got Unknown
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Expected O, but got Unknown
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Expected O, but got Unknown
		ResourceAccessor = new ResourceAccessor(typeof(NumberBox));
		MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(NumberBox), new PropertyMetadata((object)double.MinValue, new PropertyChangedCallback(OnMinimumPropertyChanged)));
		MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(NumberBox), new PropertyMetadata((object)double.MaxValue, new PropertyChangedCallback(OnMaximumPropertyChanged)));
		ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(NumberBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)double.NaN, (FrameworkPropertyMetadataOptions)1280, new PropertyChangedCallback(OnValuePropertyChanged)));
		SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(NumberBox), new PropertyMetadata((object)1.0, new PropertyChangedCallback(OnSmallChangePropertyChanged)));
		LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(NumberBox), new PropertyMetadata((object)10.0));
		TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(NumberBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)string.Empty, (FrameworkPropertyMetadataOptions)1280, new PropertyChangedCallback(OnTextPropertyChanged)));
		HeaderProperty = ControlHelper.HeaderProperty.AddOwner(typeof(NumberBox), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeaderPropertyChanged)));
		HeaderTemplateProperty = ControlHelper.HeaderTemplateProperty.AddOwner(typeof(NumberBox), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeaderTemplatePropertyChanged)));
		PlaceholderTextProperty = ControlHelper.PlaceholderTextProperty.AddOwner(typeof(NumberBox));
		SelectionBrushProperty = DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(NumberBox));
		TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(NumberBox), new PropertyMetadata((object)(TextAlignment)0));
		DescriptionProperty = ControlHelper.DescriptionProperty.AddOwner(typeof(NumberBox));
		ValidationModeProperty = DependencyProperty.Register("ValidationMode", typeof(NumberBoxValidationMode), typeof(NumberBox), new PropertyMetadata(new PropertyChangedCallback(OnValidationModePropertyChanged)));
		SpinButtonPlacementModeProperty = DependencyProperty.Register("SpinButtonPlacementMode", typeof(NumberBoxSpinButtonPlacementMode), typeof(NumberBox), new PropertyMetadata((object)NumberBoxSpinButtonPlacementMode.Hidden, new PropertyChangedCallback(OnSpinButtonPlacementModePropertyChanged)));
		IsWrapEnabledProperty = DependencyProperty.Register("IsWrapEnabled", typeof(bool), typeof(NumberBox), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsWrapEnabledPropertyChanged)));
		AcceptsExpressionProperty = DependencyProperty.Register("AcceptsExpression", typeof(bool), typeof(NumberBox), new PropertyMetadata((object)false));
		NumberFormatterProperty = DependencyProperty.Register("NumberFormatter", typeof(INumberBoxNumberFormatter), typeof(NumberBox), new PropertyMetadata(new PropertyChangedCallback(OnNumberFormatterPropertyChanged)));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(NumberBox));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(NumberBox)));
	}

	public NumberBox()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		((DependencyObject)this).SetCurrentValue(NumberFormatterProperty, (object)GetRegionalSettingsAwareDecimalFormatter());
		((UIElement)this).MouseWheel += new MouseWheelEventHandler(OnNumberBoxScroll);
		((UIElement)this).GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnNumberBoxGotFocus);
		((UIElement)this).LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnNumberBoxLostFocus);
		SetDefaultInputScope();
	}

	private void SetDefaultInputScope()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		InputScopeName value = new InputScopeName((InputScopeNameValue)29);
		InputScope val = new InputScope();
		val.Names.Add(value);
		((DependencyObject)this).SetValue(FrameworkElement.InputScopeProperty, (object)val);
	}

	private INumberBoxNumberFormatter GetRegionalSettingsAwareDecimalFormatter()
	{
		return new DefaultNumberBoxNumberFormatter();
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new NumberBoxAutomationPeer(this);
	}

	public override void OnApplyTemplate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		((FrameworkElement)this).OnApplyTemplate();
		string localizedStringResource = ResourceAccessor.GetLocalizedStringResource("NumberBoxDownSpinButtonName");
		string localizedStringResource2 = ResourceAccessor.GetLocalizedStringResource("NumberBoxUpSpinButtonName");
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("DownSpinButton");
		RepeatButton val = (RepeatButton)(object)((templateChild is RepeatButton) ? templateChild : null);
		if (val != null)
		{
			((ButtonBase)val).Click += new RoutedEventHandler(OnSpinDownClick);
			if (string.IsNullOrEmpty(AutomationProperties.GetName((DependencyObject)(object)val)))
			{
				AutomationProperties.SetName((DependencyObject)(object)val, localizedStringResource);
			}
		}
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("UpSpinButton");
		RepeatButton val2 = (RepeatButton)(object)((templateChild2 is RepeatButton) ? templateChild2 : null);
		if (val2 != null)
		{
			((ButtonBase)val2).Click += new RoutedEventHandler(OnSpinUpClick);
			if (string.IsNullOrEmpty(AutomationProperties.GetName((DependencyObject)(object)val2)))
			{
				AutomationProperties.SetName((DependencyObject)(object)val2, localizedStringResource2);
			}
		}
		UpdateHeaderPresenterState();
		m_textBox = GetTextBox();
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("UpDownPopup");
		m_popup = (Popup)(object)((templateChild3 is Popup) ? templateChild3 : null);
		if (m_popup != null)
		{
			if (((DependencyObject)(object)m_popup).HasDefaultValue(Popup.PlacementTargetProperty))
			{
				m_popup.PlacementTarget = (UIElement)(object)m_textBox;
			}
			m_popupRepositionHelper = new PopupRepositionHelper(m_popup, (UIElement)(object)this);
		}
		DependencyObject templateChild4 = ((FrameworkElement)this).GetTemplateChild("PopupDownSpinButton");
		RepeatButton val3 = (RepeatButton)(object)((templateChild4 is RepeatButton) ? templateChild4 : null);
		if (val3 != null)
		{
			((ButtonBase)val3).Click += new RoutedEventHandler(OnSpinDownClick);
		}
		DependencyObject templateChild5 = ((FrameworkElement)this).GetTemplateChild("PopupUpSpinButton");
		RepeatButton val4 = (RepeatButton)(object)((templateChild5 is RepeatButton) ? templateChild5 : null);
		if (val4 != null)
		{
			((ButtonBase)val4).Click += new RoutedEventHandler(OnSpinUpClick);
		}
		((UIElement)this).IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
		UpdateSpinButtonPlacement();
		UpdateSpinButtonEnabled();
		UpdateVisualStateForIsEnabledChange();
		if (((DependencyObject)this).ReadLocalValue(ValueProperty) == DependencyProperty.UnsetValue && ((DependencyObject)this).ReadLocalValue(TextProperty) != DependencyProperty.UnsetValue)
		{
			UpdateValueToText();
		}
		else
		{
			UpdateTextToValue();
		}
		TextBox GetTextBox()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			DependencyObject templateChild6 = ((FrameworkElement)this).GetTemplateChild("InputBox");
			TextBox val5 = (TextBox)(object)((templateChild6 is TextBox) ? templateChild6 : null);
			if (val5 != null)
			{
				if (SharedHelpers.IsRS3OrHigher())
				{
					((UIElement)val5).PreviewKeyDown += new KeyEventHandler(OnNumberBoxKeyDown);
				}
				else
				{
					((UIElement)val5).KeyDown += new KeyEventHandler(OnNumberBoxKeyDown);
				}
				((UIElement)val5).KeyUp += new KeyEventHandler(OnNumberBoxKeyUp);
			}
			return val5;
		}
	}

	private void OnValuePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		if (m_valueUpdating)
		{
			return;
		}
		double num = (double)((DependencyPropertyChangedEventArgs)(ref args)).OldValue;
		try
		{
			m_valueUpdating = true;
			CoerceValue();
			double value = Value;
			if (value != num && (!double.IsNaN(value) || !double.IsNaN(num)))
			{
				NumberBoxValueChangedEventArgs args2 = new NumberBoxValueChangedEventArgs(num, value);
				this.ValueChanged?.Invoke(this, args2);
				if (UIElementAutomationPeer.FromElement((UIElement)(object)this) is NumberBoxAutomationPeer numberBoxAutomationPeer)
				{
					numberBoxAutomationPeer.RaiseValueChangedEvent(num, value);
				}
			}
			UpdateTextToValue();
			UpdateSpinButtonEnabled();
		}
		finally
		{
			m_valueUpdating = false;
		}
	}

	private void OnMinimumPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		CoerceMaximum();
		CoerceValue();
		UpdateSpinButtonEnabled();
	}

	private void OnMaximumPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		CoerceMinimum();
		CoerceValue();
		UpdateSpinButtonEnabled();
	}

	private void OnSmallChangePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateSpinButtonEnabled();
	}

	private void OnIsWrapEnabledPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateSpinButtonEnabled();
	}

	private void OnNumberFormatterPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateTextToValue();
	}

	private void ValidateNumberFormatter(INumberBoxNumberFormatter value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
	}

	private void OnSpinButtonPlacementModePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateSpinButtonPlacement();
	}

	private void OnTextPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		if (!m_textUpdating)
		{
			UpdateValueToText();
		}
	}

	private void UpdateValueToText()
	{
		if (m_textBox != null)
		{
			m_textBox.Text = Text;
			ValidateInput();
		}
	}

	private void OnHeaderPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateHeaderPresenterState();
	}

	private void OnHeaderTemplatePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateHeaderPresenterState();
	}

	private void OnValidationModePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		ValidateInput();
		UpdateSpinButtonEnabled();
	}

	private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
	{
		UpdateVisualStateForIsEnabledChange();
	}

	private void UpdateVisualStateForIsEnabledChange()
	{
		VisualStateManager.GoToState((FrameworkElement)(object)this, ((UIElement)this).IsEnabled ? "Normal" : "Disabled", false);
	}

	private void OnNumberBoxGotFocus(object sender, RoutedEventArgs e)
	{
		if (m_textBox != null)
		{
			((TextBoxBase)m_textBox).SelectAll();
		}
		if (SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Compact && m_popup != null)
		{
			m_popup.IsOpen = true;
		}
	}

	private void OnNumberBoxLostFocus(object sender, RoutedEventArgs e)
	{
		ValidateInput();
		if (m_popup != null)
		{
			m_popup.IsOpen = false;
		}
	}

	private void CoerceMinimum()
	{
		double maximum = Maximum;
		if (Minimum > maximum)
		{
			((DependencyObject)this).SetCurrentValue(MinimumProperty, (object)maximum);
		}
	}

	private void CoerceMaximum()
	{
		double minimum = Minimum;
		if (Maximum < minimum)
		{
			((DependencyObject)this).SetCurrentValue(MaximumProperty, (object)minimum);
		}
	}

	private void CoerceValue()
	{
		double value = Value;
		if (!double.IsNaN(value) && !IsInBounds(value) && ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
		{
			double maximum = Maximum;
			if (value > maximum)
			{
				((DependencyObject)this).SetCurrentValue(ValueProperty, (object)maximum);
			}
			else
			{
				((DependencyObject)this).SetCurrentValue(ValueProperty, (object)Minimum);
			}
		}
	}

	private void ValidateInput()
	{
		if (m_textBox == null)
		{
			return;
		}
		string text = m_textBox.Text.Trim();
		if (string.IsNullOrEmpty(text))
		{
			((DependencyObject)this).SetCurrentValue(ValueProperty, (object)double.NaN);
			return;
		}
		INumberBoxNumberFormatter numberFormatter = NumberFormatter;
		double? num = (AcceptsExpression ? NumberBoxParser.Compute(text, numberFormatter) : numberFormatter.ParseDouble(text));
		if (!num.HasValue)
		{
			if (ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
			{
				UpdateTextToValue();
			}
		}
		else if (num.Value == Value)
		{
			UpdateTextToValue();
		}
		else
		{
			((DependencyObject)this).SetCurrentValue(ValueProperty, (object)num.Value);
		}
	}

	private void OnSpinDownClick(object sender, RoutedEventArgs args)
	{
		StepValue(0.0 - SmallChange);
	}

	private void OnSpinUpClick(object sender, RoutedEventArgs args)
	{
		StepValue(SmallChange);
	}

	private void OnNumberBoxKeyDown(object sender, KeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		Key key = args.Key;
		if ((int)key <= 20)
		{
			if ((int)key != 19)
			{
				if ((int)key == 20)
				{
					StepValue(0.0 - LargeChange);
					((RoutedEventArgs)args).Handled = true;
				}
			}
			else
			{
				StepValue(LargeChange);
				((RoutedEventArgs)args).Handled = true;
			}
		}
		else if ((int)key != 24)
		{
			if ((int)key == 26)
			{
				StepValue(0.0 - SmallChange);
				((RoutedEventArgs)args).Handled = true;
			}
		}
		else
		{
			StepValue(SmallChange);
			((RoutedEventArgs)args).Handled = true;
		}
	}

	private void OnNumberBoxKeyUp(object sender, KeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		Key key = args.Key;
		if ((int)key != 6)
		{
			if ((int)key == 13)
			{
				UpdateTextToValue();
				((RoutedEventArgs)args).Handled = true;
			}
		}
		else
		{
			ValidateInput();
			((RoutedEventArgs)args).Handled = true;
		}
	}

	private void OnNumberBoxScroll(object sender, MouseWheelEventArgs args)
	{
		if (m_textBox != null && ((UIElement)m_textBox).IsFocused)
		{
			int delta = args.Delta;
			if (delta > 0)
			{
				StepValue(SmallChange);
			}
			else if (delta < 0)
			{
				StepValue(0.0 - SmallChange);
			}
			((RoutedEventArgs)args).Handled = true;
		}
	}

	private void StepValue(double change)
	{
		ValidateInput();
		double value = Value;
		if (double.IsNaN(value))
		{
			return;
		}
		value += change;
		if (IsWrapEnabled)
		{
			double maximum = Maximum;
			double minimum = Minimum;
			if (value > maximum)
			{
				value = minimum;
			}
			else if (value < minimum)
			{
				value = maximum;
			}
		}
		((DependencyObject)this).SetCurrentValue(ValueProperty, (object)value);
		MoveCaretToTextEnd();
	}

	private void UpdateTextToValue()
	{
		if (m_textBox != null)
		{
			string text = string.Empty;
			double value = Value;
			if (!double.IsNaN(value))
			{
				double value2 = m_displayRounder.RoundDouble(value);
				text = NumberFormatter.FormatDouble(value2);
			}
			m_textBox.Text = text;
			try
			{
				m_textUpdating = true;
				((DependencyObject)this).SetCurrentValue(TextProperty, (object)text);
			}
			finally
			{
				m_textUpdating = false;
			}
		}
	}

	private void UpdateSpinButtonPlacement()
	{
		switch (SpinButtonPlacementMode)
		{
		case NumberBoxSpinButtonPlacementMode.Inline:
			VisualStateManager.GoToState((FrameworkElement)(object)this, "SpinButtonsVisible", false);
			break;
		case NumberBoxSpinButtonPlacementMode.Compact:
			VisualStateManager.GoToState((FrameworkElement)(object)this, "SpinButtonsPopup", false);
			break;
		default:
			VisualStateManager.GoToState((FrameworkElement)(object)this, "SpinButtonsCollapsed", false);
			break;
		}
	}

	private void UpdateSpinButtonEnabled()
	{
		double value = Value;
		bool flag = false;
		bool flag2 = false;
		if (!double.IsNaN(value))
		{
			if (IsWrapEnabled || ValidationMode != NumberBoxValidationMode.InvalidInputOverwritten)
			{
				flag = true;
				flag2 = true;
			}
			else
			{
				if (value < Maximum)
				{
					flag = true;
				}
				if (value > Minimum)
				{
					flag2 = true;
				}
			}
		}
		VisualStateManager.GoToState((FrameworkElement)(object)this, flag ? "UpSpinButtonEnabled" : "UpSpinButtonDisabled", false);
		VisualStateManager.GoToState((FrameworkElement)(object)this, flag2 ? "DownSpinButtonEnabled" : "DownSpinButtonDisabled", false);
	}

	private bool IsInBounds(double value)
	{
		if (value >= Minimum)
		{
			return value <= Maximum;
		}
		return false;
	}

	private void UpdateHeaderPresenterState()
	{
		bool flag = false;
		object header = Header;
		if (header != null)
		{
			if (header is string value)
			{
				if (!string.IsNullOrEmpty(value))
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
		}
		if (HeaderTemplate != null)
		{
			flag = true;
		}
		if (flag && m_headerPresenter == null)
		{
			DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("HeaderContentPresenter");
			ContentPresenter val = (ContentPresenter)(object)((templateChild is ContentPresenter) ? templateChild : null);
			if (val != null)
			{
				m_headerPresenter = val;
			}
		}
		if (m_headerPresenter != null)
		{
			((UIElement)m_headerPresenter).Visibility = (Visibility)((!flag) ? 2 : 0);
		}
	}

	private void MoveCaretToTextEnd()
	{
		TextBox textBox = m_textBox;
		if (textBox != null)
		{
			textBox.Select(textBox.Text.Length, 0);
		}
	}

	private static void OnMinimumPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnMinimumPropertyChanged(args);
	}

	private static void OnMaximumPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnMaximumPropertyChanged(args);
	}

	private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnValuePropertyChanged(args);
	}

	private static void OnSmallChangePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnSmallChangePropertyChanged(args);
	}

	private static void OnTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnTextPropertyChanged(args);
	}

	private static void OnHeaderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnHeaderPropertyChanged(args);
	}

	private static void OnHeaderTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnHeaderTemplatePropertyChanged(args);
	}

	private static void OnValidationModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnValidationModePropertyChanged(args);
	}

	private static void OnSpinButtonPlacementModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnSpinButtonPlacementModePropertyChanged(args);
	}

	private static void OnIsWrapEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NumberBox)(object)sender).OnIsWrapEnabledPropertyChanged(args);
	}

	private static void OnNumberFormatterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		NumberBox numberBox = (NumberBox)(object)sender;
		INumberBoxNumberFormatter numberBoxNumberFormatter2;
		INumberBoxNumberFormatter numberBoxNumberFormatter = (numberBoxNumberFormatter2 = (INumberBoxNumberFormatter)((DependencyPropertyChangedEventArgs)(ref args)).NewValue);
		numberBox.ValidateNumberFormatter(numberBoxNumberFormatter2);
		if (numberBoxNumberFormatter != numberBoxNumberFormatter2)
		{
			sender.SetCurrentValue(((DependencyPropertyChangedEventArgs)(ref args)).Property, (object)numberBoxNumberFormatter2);
		}
		else
		{
			numberBox.OnNumberFormatterPropertyChanged(args);
		}
	}
}
