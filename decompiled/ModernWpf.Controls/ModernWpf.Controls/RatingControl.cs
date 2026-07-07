using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class RatingControl : Control
{
	private const double c_horizontalScaleAnimationCenterPoint = 0.5;

	private const double c_verticalScaleAnimationCenterPoint = 0.8;

	private static readonly Thickness c_focusVisualMargin;

	private const int c_defaultRatingFontSizeForRendering = 32;

	private const int c_defaultItemSpacing = 8;

	private const double c_defaultCaptionTopMargin = 22.0;

	private const double c_noValueSetSentinel = -1.0;

	private TextBlock m_captionTextBlock;

	private StackPanel m_backgroundStackPanel;

	private StackPanel m_foregroundStackPanel;

	private bool m_isPointerOver;

	private bool m_isPointerDown;

	private double m_mousePercentage;

	private RatingInfoType m_infoType = RatingInfoType.Font;

	public static readonly DependencyProperty CaptionProperty;

	public static readonly DependencyProperty InitialSetValueProperty;

	public static readonly DependencyProperty IsClearEnabledProperty;

	public static readonly DependencyProperty IsReadOnlyProperty;

	public static readonly DependencyProperty ItemInfoProperty;

	public static readonly DependencyProperty MaxRatingProperty;

	public static readonly DependencyProperty PlaceholderValueProperty;

	public static readonly DependencyProperty ValueProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	private double RenderingRatingFontSize => 32.0;

	private double ActualRatingFontSize => RenderingRatingFontSize / 2.0;

	private double ItemSpacing => 8.0;

	public string Caption
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(CaptionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CaptionProperty, (object)value);
		}
	}

	public int InitialSetValue
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(InitialSetValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InitialSetValueProperty, (object)value);
		}
	}

	public bool IsClearEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsClearEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsClearEnabledProperty, (object)value);
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsReadOnlyProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsReadOnlyProperty, (object)value);
		}
	}

	public RatingItemInfo ItemInfo
	{
		get
		{
			return (RatingItemInfo)((DependencyObject)this).GetValue(ItemInfoProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemInfoProperty, (object)value);
		}
	}

	public int MaxRating
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(MaxRatingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxRatingProperty, (object)value);
		}
	}

	public double PlaceholderValue
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(PlaceholderValueProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PlaceholderValueProperty, (object)value);
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

	public event TypedEventHandler<RatingControl, object> ValueChanged;

	static RatingControl()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Expected O, but got Unknown
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Expected O, but got Unknown
		c_focusVisualMargin = new Thickness(-8.0, -7.0, -8.0, 0.0);
		CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(RatingControl), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnCaptionPropertyChanged)));
		InitialSetValueProperty = DependencyProperty.Register("InitialSetValue", typeof(int), typeof(RatingControl), new PropertyMetadata((object)1, new PropertyChangedCallback(OnInitialSetValuePropertyChanged)));
		IsClearEnabledProperty = DependencyProperty.Register("IsClearEnabled", typeof(bool), typeof(RatingControl), new PropertyMetadata((object)true, new PropertyChangedCallback(OnIsClearEnabledPropertyChanged)));
		IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(RatingControl), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsReadOnlyPropertyChanged)));
		ItemInfoProperty = DependencyProperty.Register("ItemInfo", typeof(RatingItemInfo), typeof(RatingControl), new PropertyMetadata(new PropertyChangedCallback(OnItemInfoPropertyChanged)));
		MaxRatingProperty = DependencyProperty.Register("MaxRating", typeof(int), typeof(RatingControl), new PropertyMetadata((object)5, new PropertyChangedCallback(OnMaxRatingPropertyChanged)));
		PlaceholderValueProperty = DependencyProperty.Register("PlaceholderValue", typeof(double), typeof(RatingControl), new PropertyMetadata((object)(-1.0), new PropertyChangedCallback(OnPlaceholderValuePropertyChanged)));
		ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(RatingControl), new PropertyMetadata((object)(-1.0), new PropertyChangedCallback(OnValuePropertyChanged)));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(RatingControl));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(RatingControl)));
		Control.FontFamilyProperty.OverrideMetadata(typeof(RatingControl), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFontFamilyPropertyChanged)));
	}

	private void UpdateCaptionMargins()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		TextBlock captionTextBlock = m_captionTextBlock;
		if (captionTextBlock != null)
		{
			Thickness margin = ((FrameworkElement)captionTextBlock).Margin;
			((Thickness)(ref margin)).Top = 22.0 - ActualRatingFontSize * 0.8;
			((FrameworkElement)captionTextBlock).Margin = margin;
		}
	}

	public override void OnApplyTemplate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		((FrameworkElement)this).OnApplyTemplate();
		RecycleEvents();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("Caption");
		TextBlock val = (TextBlock)(object)((templateChild is TextBlock) ? templateChild : null);
		if (val != null)
		{
			m_captionTextBlock = val;
			((FrameworkElement)val).SizeChanged += new SizeChangedEventHandler(OnCaptionSizeChanged);
			UpdateCaptionMargins();
		}
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("RatingBackgroundStackPanel");
		StackPanel val2 = (StackPanel)(object)((templateChild2 is StackPanel) ? templateChild2 : null);
		if (val2 != null)
		{
			m_backgroundStackPanel = val2;
			((UIElement)val2).LostMouseCapture += new MouseEventHandler(OnPointerCaptureLostBackgroundStackPanel);
			((UIElement)val2).MouseMove += new MouseEventHandler(OnPointerMovedOverBackgroundStackPanel);
			((UIElement)val2).MouseEnter += new MouseEventHandler(OnPointerEnteredBackgroundStackPanel);
			((UIElement)val2).MouseLeave += new MouseEventHandler(OnPointerExitedBackgroundStackPanel);
			((UIElement)val2).MouseDown += new MouseButtonEventHandler(OnPointerPressedBackgroundStackPanel);
			((UIElement)val2).MouseUp += new MouseButtonEventHandler(OnPointerReleasedBackgroundStackPanel);
		}
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("RatingForegroundStackPanel");
		m_foregroundStackPanel = (StackPanel)(object)((templateChild3 is StackPanel) ? templateChild3 : null);
		((DependencyObject)this).SetValue(FocusVisualHelper.FocusVisualMarginProperty, (object)c_focusVisualMargin);
		((UIElement)this).IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
		StampOutRatingItems();
	}

	private double CoerceValueBetweenMinAndMax(double value)
	{
		if (value < 0.0)
		{
			value = -1.0;
		}
		else if (value <= 1.0)
		{
			value = 1.0;
		}
		else if (value > (double)MaxRating)
		{
			value = MaxRating;
		}
		return value;
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new RatingControlAutomationPeer(this);
	}

	private void StampOutRatingItems()
	{
		if (m_backgroundStackPanel != null && m_foregroundStackPanel != null)
		{
			((Panel)m_backgroundStackPanel).Children.Clear();
			if (IsItemInfoPresentAndFontInfo())
			{
				PopulateStackPanelWithItems("BackgroundGlyphDefaultTemplate", m_backgroundStackPanel, RatingControlStates.Unset);
			}
			else if (IsItemInfoPresentAndImageInfo())
			{
				PopulateStackPanelWithItems("BackgroundImageDefaultTemplate", m_backgroundStackPanel, RatingControlStates.Unset);
			}
			else if (IsItemInfoPresentAndPathInfo())
			{
				PopulateStackPanelWithItems("BackgroundPathDefaultTemplate", m_backgroundStackPanel, RatingControlStates.Unset);
			}
			((Panel)m_foregroundStackPanel).Children.Clear();
			if (IsItemInfoPresentAndFontInfo())
			{
				PopulateStackPanelWithItems("ForegroundGlyphDefaultTemplate", m_foregroundStackPanel, RatingControlStates.Set);
			}
			else if (IsItemInfoPresentAndImageInfo())
			{
				PopulateStackPanelWithItems("ForegroundImageDefaultTemplate", m_foregroundStackPanel, RatingControlStates.Set);
			}
			else if (IsItemInfoPresentAndPathInfo())
			{
				PopulateStackPanelWithItems("ForegroundPathDefaultTemplate", m_foregroundStackPanel, RatingControlStates.Set);
			}
			UpdateRatingItemsAppearance();
		}
	}

	private void ReRenderCaption()
	{
		if (m_captionTextBlock != null)
		{
			ResetControlWidth();
		}
	}

	private void UpdateRatingItemsAppearance()
	{
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		if (m_foregroundStackPanel == null)
		{
			return;
		}
		double placeholderValue = PlaceholderValue;
		double value = Value;
		double num = 0.0;
		if (m_isPointerOver)
		{
			num = Math.Ceiling(m_mousePercentage * (double)MaxRating);
			if (value == -1.0)
			{
				if (placeholderValue == -1.0)
				{
					VisualStateManager.GoToState((FrameworkElement)(object)this, "PointerOverPlaceholder", false);
					CustomizeStackPanel(m_foregroundStackPanel, RatingControlStates.PointerOverPlaceholder);
				}
				else
				{
					VisualStateManager.GoToState((FrameworkElement)(object)this, "PointerOverUnselected", false);
					CustomizeStackPanel(m_foregroundStackPanel, RatingControlStates.PointerOverPlaceholder);
				}
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "PointerOverSet", false);
				CustomizeStackPanel(m_foregroundStackPanel, RatingControlStates.PointerOverSet);
			}
		}
		else if (value > -1.0)
		{
			num = value;
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Set", false);
			CustomizeStackPanel(m_foregroundStackPanel, RatingControlStates.Set);
		}
		else if (placeholderValue > -1.0)
		{
			num = placeholderValue;
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Placeholder", false);
			CustomizeStackPanel(m_foregroundStackPanel, RatingControlStates.Placeholder);
		}
		if (!((UIElement)this).IsEnabled)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Disabled", false);
			CustomizeStackPanel(m_foregroundStackPanel, RatingControlStates.Disabled);
		}
		int num2 = 0;
		foreach (object child in ((Panel)m_foregroundStackPanel).Children)
		{
			double num3 = RenderingRatingFontSize;
			if ((double)(num2 + 1) > num)
			{
				num3 = ((!((double)num2 < num)) ? 0.0 : (num3 * (num - Math.Floor(num))));
			}
			RectangleGeometry clip = new RectangleGeometry(new Rect(0.0, 0.0, num3, RenderingRatingFontSize));
			((UIElement)child).Clip = (Geometry)(object)clip;
			num2++;
		}
		ResetControlWidth();
	}

	private void ApplyScaleExpressionAnimation(UIElement uiElement, int starIndex)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		Transform renderTransform = uiElement.RenderTransform;
		ScaleTransform val = (ScaleTransform)(object)((renderTransform is ScaleTransform) ? renderTransform : null);
		if (val == null)
		{
			val = (ScaleTransform)(object)(uiElement.RenderTransform = (Transform)new ScaleTransform());
		}
		val.ScaleX = 0.5;
		val.ScaleY = 0.5;
		val.CenterX = 16.0;
		val.CenterY = 25.6;
	}

	private void PopulateStackPanelWithItems(string templateName, StackPanel stackPanel, RatingControlStates state)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		DataTemplate val = (DataTemplate)Application.Current.FindResource((object)templateName);
		for (int i = 0; i < MaxRating; i++)
		{
			DependencyObject obj = ((FrameworkTemplate)val).LoadContent();
			UIElement val2 = (UIElement)(object)((obj is UIElement) ? obj : null);
			if (val2 != null)
			{
				CustomizeRatingItem(val2, state);
				((Panel)stackPanel).Children.Add(val2);
				ApplyScaleExpressionAnimation(val2, i);
			}
		}
	}

	private void CustomizeRatingItem(UIElement ui, RatingControlStates type)
	{
		if (IsItemInfoPresentAndFontInfo())
		{
			TextBlock val = (TextBlock)(object)((ui is TextBlock) ? ui : null);
			if (val != null)
			{
				val.FontFamily = ((Control)this).FontFamily;
				val.Text = GetAppropriateGlyph(type);
			}
		}
		else if (IsItemInfoPresentAndImageInfo())
		{
			Image val2 = (Image)(object)((ui is Image) ? ui : null);
			if (val2 != null)
			{
				val2.Source = GetAppropriateImageSource(type);
				((FrameworkElement)val2).Width = RenderingRatingFontSize;
				((FrameworkElement)val2).Height = RenderingRatingFontSize;
			}
		}
		else if (IsItemInfoPresentAndPathInfo() && ui is FontIconFallback fontIconFallback)
		{
			fontIconFallback.Data = GetAppropriatePathData(type);
		}
	}

	private void CustomizeStackPanel(StackPanel stackPanel, RatingControlStates state)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		foreach (UIElement child in ((Panel)stackPanel).Children)
		{
			UIElement ui = child;
			CustomizeRatingItem(ui, state);
		}
	}

	private bool IsItemInfoPresentAndFontInfo()
	{
		return m_infoType == RatingInfoType.Font;
	}

	private bool IsItemInfoPresentAndImageInfo()
	{
		return m_infoType == RatingInfoType.Image;
	}

	private bool IsItemInfoPresentAndPathInfo()
	{
		return m_infoType == RatingInfoType.Path;
	}

	private string GetAppropriateGlyph(RatingControlStates type)
	{
		IsItemInfoPresentAndFontInfo();
		RatingItemFontInfo ratingItemFontInfo = (RatingItemFontInfo)ItemInfo;
		return type switch
		{
			RatingControlStates.Disabled => GetNextGlyphIfNull(ratingItemFontInfo.DisabledGlyph, RatingControlStates.Set), 
			RatingControlStates.PointerOverSet => GetNextGlyphIfNull(ratingItemFontInfo.PointerOverGlyph, RatingControlStates.Set), 
			RatingControlStates.PointerOverPlaceholder => GetNextGlyphIfNull(ratingItemFontInfo.PointerOverPlaceholderGlyph, RatingControlStates.Placeholder), 
			RatingControlStates.Placeholder => GetNextGlyphIfNull(ratingItemFontInfo.PlaceholderGlyph, RatingControlStates.Set), 
			RatingControlStates.Unset => GetNextGlyphIfNull(ratingItemFontInfo.UnsetGlyph, RatingControlStates.Set), 
			RatingControlStates.Null => string.Empty, 
			_ => ratingItemFontInfo.Glyph, 
		};
	}

	private string GetNextGlyphIfNull(string glyph, RatingControlStates fallbackType)
	{
		if (string.IsNullOrEmpty(glyph))
		{
			if (fallbackType == RatingControlStates.Null)
			{
				return string.Empty;
			}
			return GetAppropriateGlyph(fallbackType);
		}
		return glyph;
	}

	private ImageSource GetAppropriateImageSource(RatingControlStates type)
	{
		IsItemInfoPresentAndImageInfo();
		RatingItemImageInfo ratingItemImageInfo = (RatingItemImageInfo)ItemInfo;
		return (ImageSource)(type switch
		{
			RatingControlStates.Disabled => GetNextImageIfNull(ratingItemImageInfo.DisabledImage, RatingControlStates.Set), 
			RatingControlStates.PointerOverSet => GetNextImageIfNull(ratingItemImageInfo.PointerOverImage, RatingControlStates.Set), 
			RatingControlStates.PointerOverPlaceholder => GetNextImageIfNull(ratingItemImageInfo.PointerOverPlaceholderImage, RatingControlStates.Placeholder), 
			RatingControlStates.Placeholder => GetNextImageIfNull(ratingItemImageInfo.PlaceholderImage, RatingControlStates.Set), 
			RatingControlStates.Unset => GetNextImageIfNull(ratingItemImageInfo.UnsetImage, RatingControlStates.Set), 
			RatingControlStates.Null => null, 
			_ => ratingItemImageInfo.Image, 
		});
	}

	private ImageSource GetNextImageIfNull(ImageSource image, RatingControlStates fallbackType)
	{
		if (image == null)
		{
			if (fallbackType == RatingControlStates.Null)
			{
				return null;
			}
			return GetAppropriateImageSource(fallbackType);
		}
		return image;
	}

	private Geometry GetAppropriatePathData(RatingControlStates type)
	{
		IsItemInfoPresentAndPathInfo();
		RatingItemPathInfo ratingItemPathInfo = (RatingItemPathInfo)ItemInfo;
		return (Geometry)(type switch
		{
			RatingControlStates.Disabled => GetNextGeometryIfNull(ratingItemPathInfo.DisabledData, RatingControlStates.Set), 
			RatingControlStates.PointerOverSet => GetNextGeometryIfNull(ratingItemPathInfo.PointerOverData, RatingControlStates.Set), 
			RatingControlStates.PointerOverPlaceholder => GetNextGeometryIfNull(ratingItemPathInfo.PointerOverPlaceholderData, RatingControlStates.Placeholder), 
			RatingControlStates.Placeholder => GetNextGeometryIfNull(ratingItemPathInfo.PlaceholderData, RatingControlStates.Set), 
			RatingControlStates.Unset => GetNextGeometryIfNull(ratingItemPathInfo.UnsetData, RatingControlStates.Set), 
			RatingControlStates.Null => null, 
			_ => ratingItemPathInfo.Data, 
		});
	}

	private Geometry GetNextGeometryIfNull(Geometry geometry, RatingControlStates fallbackType)
	{
		if (geometry == null)
		{
			if (fallbackType == RatingControlStates.Null)
			{
				return null;
			}
			return GetAppropriatePathData(fallbackType);
		}
		return geometry;
	}

	private void ResetControlWidth()
	{
		double width = CalculateTotalRatingControlWidth();
		((FrameworkElement)this).Width = width;
	}

	private void ChangeRatingBy(double change, bool originatedFromMouse)
	{
		if (change == 0.0)
		{
			return;
		}
		double num = 0.0;
		double value = Value;
		if (value != -1.0)
		{
			if ((double)(int)Value != Value)
			{
				num = ((change != -1.0) ? ((double)(int)Value + change) : ((double)(int)Value));
			}
			else
			{
				num = value;
				num += change;
			}
		}
		else
		{
			num = InitialSetValue;
		}
		SetRatingTo(num, originatedFromMouse);
	}

	private void SetRatingTo(double newRating, bool originatedFromMouse)
	{
		double num = 0.0;
		double value = Value;
		num = Math.Min(newRating, MaxRating);
		num = Math.Max(num, 0.0);
		if (value > -1.0 || num != 0.0)
		{
			if (!IsClearEnabled && num <= 0.0)
			{
				((DependencyObject)this).SetCurrentValue(ValueProperty, (object)1.0);
			}
			else if (num == value && IsClearEnabled && (num != (double)MaxRating || originatedFromMouse))
			{
				((DependencyObject)this).SetCurrentValue(ValueProperty, (object)(-1.0));
			}
			else if (num > 0.0)
			{
				((DependencyObject)this).SetCurrentValue(ValueProperty, (object)num);
			}
			else
			{
				((DependencyObject)this).SetCurrentValue(ValueProperty, (object)(-1.0));
			}
			this.ValueChanged?.Invoke(this, null);
		}
	}

	private void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		DependencyProperty property = ((DependencyPropertyChangedEventArgs)(ref args)).Property;
		if (property == MaxRatingProperty)
		{
			int num = (int)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
			int num2 = Math.Max(1, num);
			if (Value > (double)num2)
			{
				Value = num2;
			}
			if (PlaceholderValue > (double)num2)
			{
				PlaceholderValue = num2;
			}
			if (num2 != num)
			{
				((DependencyObject)this).SetValue(property, (object)num2);
				return;
			}
		}
		else if (property == PlaceholderValueProperty || property == ValueProperty)
		{
			double num3 = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
			double num4 = CoerceValueBetweenMinAndMax(num3);
			if (num3 != num4)
			{
				((DependencyObject)this).SetValue(property, (object)num4);
				return;
			}
		}
		if (property == CaptionProperty)
		{
			OnCaptionChanged(args);
		}
		else if (property == InitialSetValueProperty)
		{
			OnInitialSetValueChanged(args);
		}
		else if (property == IsClearEnabledProperty)
		{
			OnIsClearEnabledChanged(args);
		}
		else if (property == IsReadOnlyProperty)
		{
			OnIsReadOnlyChanged(args);
		}
		else if (property == ItemInfoProperty)
		{
			OnItemInfoChanged(args);
		}
		else if (property == MaxRatingProperty)
		{
			OnMaxRatingChanged(args);
		}
		else if (property == PlaceholderValueProperty)
		{
			OnPlaceholderValueChanged(args);
		}
		else if (property == ValueProperty)
		{
			OnValueChanged(args);
		}
	}

	private void OnCaptionChanged(DependencyPropertyChangedEventArgs args)
	{
		ReRenderCaption();
	}

	private static void OnFontFamilyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		((RatingControl)(object)sender).OnFontFamilyChanged();
	}

	private void OnFontFamilyChanged()
	{
		if (m_backgroundStackPanel != null)
		{
			for (int i = 0; i < MaxRating; i++)
			{
				UIElement obj = ((Panel)m_backgroundStackPanel).Children[i];
				TextBlock val = (TextBlock)(object)((obj is TextBlock) ? obj : null);
				if (val != null)
				{
					CustomizeRatingItem((UIElement)(object)val, RatingControlStates.Unset);
				}
				UIElement obj2 = ((Panel)m_foregroundStackPanel).Children[i];
				TextBlock val2 = (TextBlock)(object)((obj2 is TextBlock) ? obj2 : null);
				if (val2 != null)
				{
					CustomizeRatingItem((UIElement)(object)val2, RatingControlStates.Set);
				}
			}
		}
		UpdateRatingItemsAppearance();
	}

	private void OnInitialSetValueChanged(DependencyPropertyChangedEventArgs args)
	{
	}

	private void OnIsClearEnabledChanged(DependencyPropertyChangedEventArgs args)
	{
	}

	private void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs args)
	{
	}

	private void OnItemInfoChanged(DependencyPropertyChangedEventArgs args)
	{
		bool flag = false;
		if (ItemInfo == null)
		{
			m_infoType = RatingInfoType.None;
		}
		else if (ItemInfo is RatingItemFontInfo)
		{
			if (m_infoType != RatingInfoType.Font && m_backgroundStackPanel != null)
			{
				m_infoType = RatingInfoType.Font;
				StampOutRatingItems();
				flag = true;
			}
		}
		else if (ItemInfo is RatingItemPathInfo)
		{
			if (m_infoType != RatingInfoType.Path)
			{
				m_infoType = RatingInfoType.Path;
				StampOutRatingItems();
				flag = true;
			}
		}
		else if (m_infoType != RatingInfoType.Image)
		{
			m_infoType = RatingInfoType.Image;
			StampOutRatingItems();
			flag = true;
		}
		if (m_backgroundStackPanel != null && !flag)
		{
			for (int i = 0; i < MaxRating; i++)
			{
				CustomizeRatingItem(((Panel)m_backgroundStackPanel).Children[i], RatingControlStates.Unset);
				CustomizeRatingItem(((Panel)m_foregroundStackPanel).Children[i], RatingControlStates.Set);
			}
		}
		UpdateRatingItemsAppearance();
	}

	private void OnMaxRatingChanged(DependencyPropertyChangedEventArgs args)
	{
		StampOutRatingItems();
	}

	private void OnPlaceholderValueChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateRatingItemsAppearance();
	}

	private void OnValueChanged(DependencyPropertyChangedEventArgs args)
	{
		AutomationPeer val = UIElementAutomationPeer.FromElement((UIElement)(object)this);
		if (val != null)
		{
			((RatingControlAutomationPeer)(object)val).RaisePropertyChangedEvent(Value);
		}
		UpdateRatingItemsAppearance();
	}

	private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
	{
		UpdateRatingItemsAppearance();
	}

	private void OnCaptionSizeChanged(object sender, SizeChangedEventArgs args)
	{
		ResetControlWidth();
	}

	private void OnPointerCaptureLostBackgroundStackPanel(object sender, MouseEventArgs args)
	{
		PointerExitedImpl(args, resetScaleAnimation: false);
	}

	private void OnPointerMovedOverBackgroundStackPanel(object sender, MouseEventArgs args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!IsReadOnly)
		{
			Point position = args.GetPosition((IInputElement)(object)m_backgroundStackPanel);
			double x = ((Point)(ref position)).X;
			m_mousePercentage = x / CalculateActualRatingWidth();
			UpdateRatingItemsAppearance();
			((RoutedEventArgs)args).Handled = true;
		}
	}

	private void OnPointerEnteredBackgroundStackPanel(object sender, MouseEventArgs args)
	{
		if (!IsReadOnly)
		{
			m_isPointerOver = true;
			((RoutedEventArgs)args).Handled = true;
		}
	}

	private void OnPointerExitedBackgroundStackPanel(object sender, MouseEventArgs args)
	{
		PointerExitedImpl(args);
	}

	private void PointerExitedImpl(MouseEventArgs args, bool resetScaleAnimation = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.GetPosition((IInputElement)(object)m_backgroundStackPanel);
		if (resetScaleAnimation)
		{
			m_isPointerOver = false;
		}
		if (!m_isPointerDown)
		{
			UpdateRatingItemsAppearance();
		}
		((RoutedEventArgs)args).Handled = true;
	}

	private void OnPointerPressedBackgroundStackPanel(object sender, MouseButtonEventArgs args)
	{
		if (!IsReadOnly)
		{
			m_isPointerDown = true;
			((UIElement)m_backgroundStackPanel).CaptureMouse();
		}
	}

	private void OnPointerReleasedBackgroundStackPanel(object sender, MouseButtonEventArgs args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!IsReadOnly)
		{
			Point position = ((MouseEventArgs)args).GetPosition((IInputElement)(object)m_backgroundStackPanel);
			double num = ((Point)(ref position)).X / CalculateActualRatingWidth();
			SetRatingTo(Math.Ceiling(num * (double)MaxRating), originatedFromMouse: true);
		}
		if (m_isPointerDown)
		{
			m_isPointerDown = false;
			UpdateRatingItemsAppearance();
		}
		((UIElement)m_backgroundStackPanel).ReleaseMouseCapture();
		((UIElement)this).Focus();
	}

	private double CalculateTotalRatingControlWidth()
	{
		double num = CalculateActualRatingWidth();
		string obj = (string)((DependencyObject)this).GetValue(CaptionProperty);
		double num2 = 0.0;
		if (obj.Length > 0)
		{
			num2 = ItemSpacing;
		}
		double num3 = 0.0;
		if (m_captionTextBlock != null)
		{
			num3 = ((FrameworkElement)m_captionTextBlock).ActualWidth;
		}
		return num + num2 + num3;
	}

	private double CalculateActualRatingWidth()
	{
		return (double)MaxRating * ActualRatingFontSize + (double)(MaxRating - 1) * ItemSpacing;
	}

	protected override void OnKeyDown(KeyEventArgs eventArgs)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected I4, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (((RoutedEventArgs)eventArgs).Handled)
		{
			return;
		}
		if (!IsReadOnly)
		{
			bool handled = false;
			Key val = eventArgs.Key;
			double num = 1.0;
			if ((int)((FrameworkElement)this).FlowDirection == 1)
			{
				num *= -1.0;
			}
			Key key = eventArgs.Key;
			if ((int)key == 24)
			{
				val = (Key)25;
				num = 1.0;
			}
			else if ((int)key == 26)
			{
				val = (Key)23;
				num = 1.0;
			}
			switch (val - 21)
			{
			case 2:
				ChangeRatingBy(-1.0 * num, originatedFromMouse: false);
				handled = true;
				break;
			case 4:
				ChangeRatingBy(1.0 * num, originatedFromMouse: false);
				handled = true;
				break;
			case 1:
				SetRatingTo(0.0, originatedFromMouse: false);
				handled = true;
				break;
			case 0:
				SetRatingTo(MaxRating, originatedFromMouse: false);
				handled = true;
				break;
			}
			((RoutedEventArgs)eventArgs).Handled = handled;
		}
		((UIElement)this).OnKeyDown(eventArgs);
	}

	private void RecycleEvents()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		StackPanel backgroundStackPanel = m_backgroundStackPanel;
		if (backgroundStackPanel != null)
		{
			((UIElement)backgroundStackPanel).LostMouseCapture -= new MouseEventHandler(OnPointerCaptureLostBackgroundStackPanel);
			((UIElement)backgroundStackPanel).MouseMove -= new MouseEventHandler(OnPointerMovedOverBackgroundStackPanel);
			((UIElement)backgroundStackPanel).MouseEnter -= new MouseEventHandler(OnPointerEnteredBackgroundStackPanel);
			((UIElement)backgroundStackPanel).MouseLeave -= new MouseEventHandler(OnPointerExitedBackgroundStackPanel);
			((UIElement)backgroundStackPanel).MouseDown -= new MouseButtonEventHandler(OnPointerPressedBackgroundStackPanel);
			((UIElement)backgroundStackPanel).MouseUp -= new MouseButtonEventHandler(OnPointerReleasedBackgroundStackPanel);
		}
		TextBlock captionTextBlock = m_captionTextBlock;
		if (captionTextBlock != null)
		{
			((FrameworkElement)captionTextBlock).SizeChanged -= new SizeChangedEventHandler(OnCaptionSizeChanged);
		}
	}

	private static void OnCaptionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnInitialSetValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnIsClearEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnIsReadOnlyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnItemInfoPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnMaxRatingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnPlaceholderValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((RatingControl)(object)sender).PrivateOnPropertyChanged(args);
	}
}
