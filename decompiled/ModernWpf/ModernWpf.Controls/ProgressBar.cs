using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
[TemplatePart(Name = "DeterminateProgressBarIndicator", Type = typeof(Rectangle))]
[TemplatePart(Name = "IndeterminateProgressBarIndicator", Type = typeof(Rectangle))]
[TemplatePart(Name = "IndeterminateProgressBarIndicator2", Type = typeof(Rectangle))]
[TemplateVisualState(GroupName = "CommonStates", Name = "Determinate")]
[TemplateVisualState(GroupName = "CommonStates", Name = "Indeterminate")]
public class ProgressBar : RangeBase
{
	public static readonly DependencyProperty IsIndeterminateProperty;

	public static readonly DependencyProperty ShowErrorProperty;

	public static readonly DependencyProperty ShowPausedProperty;

	private static readonly DependencyPropertyKey TemplateSettingsPropertyKey;

	public static readonly DependencyProperty TemplateSettingsProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	private Grid m_layoutRoot;

	private Rectangle m_determinateProgressBarIndicator;

	private Rectangle m_indeterminateProgressBarIndicator;

	private Rectangle m_indeterminateProgressBarIndicator2;

	private const string s_LayoutRootName = "LayoutRoot";

	private const string s_DeterminateProgressBarIndicatorName = "DeterminateProgressBarIndicator";

	private const string s_IndeterminateProgressBarIndicatorName = "IndeterminateProgressBarIndicator";

	private const string s_IndeterminateProgressBarIndicator2Name = "IndeterminateProgressBarIndicator2";

	private const string s_ErrorStateName = "Error";

	private const string s_PausedStateName = "Paused";

	private const string s_IndeterminateStateName = "Indeterminate";

	private const string s_IndeterminateErrorStateName = "IndeterminateError";

	private const string s_IndeterminatePausedStateName = "IndeterminatePaused";

	private const string s_DeterminateStateName = "Determinate";

	private const string s_UpdatingStateName = "Updating";

	private const string s_UpdatingWithErrorStateName = "UpdatingError";

	public bool IsIndeterminate
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsIndeterminateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsIndeterminateProperty, (object)value);
		}
	}

	public bool ShowError
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(ShowErrorProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowErrorProperty, (object)value);
		}
	}

	public bool ShowPaused
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(ShowPausedProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowPausedProperty, (object)value);
		}
	}

	public ProgressBarTemplateSettings TemplateSettings
	{
		get
		{
			return (ProgressBarTemplateSettings)((DependencyObject)this).GetValue(TemplateSettingsProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(TemplateSettingsPropertyKey, (object)value);
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

	static ProgressBar()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_0161: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_018c: Expected O, but got Unknown
		IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProgressBar), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsIndeterminatePropertyChanged)));
		ShowErrorProperty = DependencyProperty.Register("ShowError", typeof(bool), typeof(ProgressBar), new PropertyMetadata(new PropertyChangedCallback(OnShowErrorPropertyChanged)));
		ShowPausedProperty = DependencyProperty.Register("ShowPaused", typeof(bool), typeof(ProgressBar), new PropertyMetadata(new PropertyChangedCallback(OnShowPausedPropertyChanged)));
		TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(ProgressBarTemplateSettings), typeof(ProgressBar), (PropertyMetadata)null);
		TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(ProgressBar));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ProgressBar)));
		Control.PaddingProperty.OverrideMetadata(typeof(ProgressBar), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPaddingChanged)));
		Control.BackgroundProperty.OverrideMetadata(typeof(ProgressBar), (PropertyMetadata)new FrameworkPropertyMetadata
		{
			CoerceValueCallback = new CoerceValueCallback(CoerceBrush)
		});
		Control.ForegroundProperty.OverrideMetadata(typeof(ProgressBar), (PropertyMetadata)new FrameworkPropertyMetadata
		{
			CoerceValueCallback = new CoerceValueCallback(CoerceBrush)
		});
	}

	public ProgressBar()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		((DependencyObject)this).SetValue(TemplateSettingsPropertyKey, (object)new ProgressBarTemplateSettings());
	}

	private static void OnIsIndeterminatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ProgressBar)(object)sender).OnIsIndeterminatePropertyChanged(args);
	}

	private static void OnShowErrorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ProgressBar)(object)sender).OnShowErrorPropertyChanged(args);
	}

	private static void OnShowPausedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ProgressBar)(object)sender).OnShowPausedPropertyChanged(args);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new ProgressBarAutomationPeer(this);
	}

	public override void OnApplyTemplate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		ThemeManager.RemoveActualThemeChangedHandler((FrameworkElement)(object)this, new RoutedEventHandler(OnActualThemeChanged));
		((FrameworkElement)this).OnApplyTemplate();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("LayoutRoot");
		m_layoutRoot = (Grid)(object)((templateChild is Grid) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("DeterminateProgressBarIndicator");
		m_determinateProgressBarIndicator = (Rectangle)(object)((templateChild2 is Rectangle) ? templateChild2 : null);
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("IndeterminateProgressBarIndicator");
		m_indeterminateProgressBarIndicator = (Rectangle)(object)((templateChild3 is Rectangle) ? templateChild3 : null);
		DependencyObject templateChild4 = ((FrameworkElement)this).GetTemplateChild("IndeterminateProgressBarIndicator2");
		m_indeterminateProgressBarIndicator2 = (Rectangle)(object)((templateChild4 is Rectangle) ? templateChild4 : null);
		UpdateStates();
		ThemeManager.AddActualThemeChangedHandler((FrameworkElement)(object)this, new RoutedEventHandler(OnActualThemeChanged));
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		SetProgressBarIndicatorWidth();
		UpdateWidthBasedTemplateSettings();
		ReapplyIndeterminateStoryboard();
	}

	protected override void OnValueChanged(double oldValue, double newValue)
	{
		((RangeBase)this).OnValueChanged(oldValue, newValue);
		OnIndicatorWidthComponentChanged();
	}

	protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
	{
		((RangeBase)this).OnMinimumChanged(oldMinimum, newMinimum);
		OnIndicatorWidthComponentChanged();
	}

	protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
	{
		((RangeBase)this).OnMaximumChanged(oldMaximum, newMaximum);
		OnIndicatorWidthComponentChanged();
	}

	private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ProgressBar)(object)d).OnIndicatorWidthComponentChanged();
	}

	private void OnIndicatorWidthComponentChanged()
	{
		SetProgressBarIndicatorWidth();
	}

	private void OnIsIndeterminatePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		SetProgressBarIndicatorWidth();
		UpdateStates();
		ReapplyIndeterminateStoryboard();
	}

	private void OnShowPausedPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateStates();
	}

	private void OnShowErrorPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		UpdateStates();
	}

	private void UpdateStates(bool useTransitions = true)
	{
		if (IsIndeterminate)
		{
			if (ShowError)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "IndeterminateError", true);
			}
			else if (ShowPaused)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "IndeterminatePaused", true);
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "Indeterminate", true);
			}
			UpdateWidthBasedTemplateSettings();
		}
		else if (ShowError)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Error", true);
		}
		else if (ShowPaused)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Paused", true);
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Determinate", true);
		}
	}

	private void SetProgressBarIndicatorWidth()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		ProgressBarTemplateSettings templateSettings = TemplateSettings;
		Grid layoutRoot = m_layoutRoot;
		if (layoutRoot == null)
		{
			return;
		}
		Rectangle determinateProgressBarIndicator = m_determinateProgressBarIndicator;
		if (determinateProgressBarIndicator == null)
		{
			return;
		}
		double actualWidth = ((FrameworkElement)layoutRoot).ActualWidth;
		double actualWidth2 = ((FrameworkElement)determinateProgressBarIndicator).ActualWidth;
		double maximum = ((RangeBase)this).Maximum;
		double minimum = ((RangeBase)this).Minimum;
		Thickness padding = ((Control)this).Padding;
		if (ShowError)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "UpdatingError", true);
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Updating", true);
		}
		if (IsIndeterminate)
		{
			((FrameworkElement)m_determinateProgressBarIndicator).Width = 0.0;
			if (m_indeterminateProgressBarIndicator != null)
			{
				((FrameworkElement)m_indeterminateProgressBarIndicator).Width = actualWidth * 0.4;
			}
			if (m_indeterminateProgressBarIndicator2 != null)
			{
				((FrameworkElement)m_indeterminateProgressBarIndicator2).Width = actualWidth * 0.6;
			}
		}
		else if (Math.Abs(maximum - minimum) > double.Epsilon)
		{
			double num = (actualWidth - (((Thickness)(ref padding)).Left + ((Thickness)(ref padding)).Right)) / (maximum - minimum) * (((RangeBase)this).Value - minimum);
			double num2 = num - actualWidth2;
			templateSettings.IndicatorLengthDelta = 0.0 - num2;
			((FrameworkElement)m_determinateProgressBarIndicator).Width = num;
		}
		else
		{
			((FrameworkElement)m_determinateProgressBarIndicator).Width = 0.0;
		}
		UpdateStates();
	}

	private void UpdateWidthBasedTemplateSettings()
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		ProgressBarTemplateSettings templateSettings = TemplateSettings;
		double num;
		double num2;
		if (m_layoutRoot != null)
		{
			num = ((FrameworkElement)m_layoutRoot).ActualWidth;
			num2 = ((FrameworkElement)m_layoutRoot).ActualHeight;
		}
		else
		{
			num = 0.0;
			num2 = 0.0;
		}
		double num3 = num * 0.4;
		double num4 = num * 0.6;
		templateSettings.ContainerAnimationStartPosition = num3 * -1.0;
		templateSettings.ContainerAnimationEndPosition = num3 * 3.0;
		templateSettings.Container2AnimationStartPosition = num4 * -1.5;
		templateSettings.Container2AnimationEndPosition = num4 * 1.66;
		templateSettings.ContainerAnimationMidPosition = num * 0.2;
		Thickness padding = ((Control)this).Padding;
		RectangleGeometry val = new RectangleGeometry(new Rect(((Thickness)(ref padding)).Left, ((Thickness)(ref padding)).Top, num - (((Thickness)(ref padding)).Right + ((Thickness)(ref padding)).Left), num2 - (((Thickness)(ref padding)).Bottom + ((Thickness)(ref padding)).Top)));
		if (m_indeterminateProgressBarIndicator != null)
		{
			val.RadiusX = m_indeterminateProgressBarIndicator.RadiusX;
			val.RadiusY = m_indeterminateProgressBarIndicator.RadiusY;
		}
		templateSettings.ClipRect = val;
		templateSettings.EllipseAnimationEndPosition = 1.0 / 3.0 * num;
		templateSettings.EllipseAnimationWellPosition = 2.0 / 3.0 * num;
		if (num <= 180.0)
		{
			templateSettings.EllipseDiameter = 4.0;
			templateSettings.EllipseOffset = 4.0;
		}
		else if (num <= 280.0)
		{
			templateSettings.EllipseDiameter = 5.0;
			templateSettings.EllipseOffset = 7.0;
		}
		else
		{
			templateSettings.EllipseDiameter = 6.0;
			templateSettings.EllipseOffset = 9.0;
		}
	}

	private static object CoerceBrush(DependencyObject d, object baseValue)
	{
		Brush val = (Brush)((baseValue is Brush) ? baseValue : null);
		if (val != null && !((Freezable)val).IsFrozen)
		{
			Brush obj = val.CloneCurrentValue();
			((Freezable)obj).Freeze();
			return obj;
		}
		return baseValue;
	}

	private void OnActualThemeChanged(object sender, RoutedEventArgs e)
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke(RefreshStates, (DispatcherPriority)7);
	}

	private void RefreshStates()
	{
		VisualStateManager.GoToState((FrameworkElement)(object)this, "Updating", false);
		UpdateStates(useTransitions: false);
	}

	private void ReapplyIndeterminateStoryboard()
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
		{
			if (IsIndeterminate)
			{
				RefreshStates();
			}
		}, (DispatcherPriority)7);
	}
}
