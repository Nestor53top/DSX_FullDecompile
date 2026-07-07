using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class ProgressRing : Control
{
	private const string s_ActiveStateName = "Active";

	private const string s_InactiveStateName = "Inactive";

	private const string s_SmallStateName = "Small";

	private const string s_LargeStateName = "Large";

	public static readonly DependencyProperty IsActiveProperty;

	private static readonly DependencyPropertyKey TemplateSettingsPropertyKey;

	public static readonly DependencyProperty TemplateSettingsProperty;

	public bool IsActive
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsActiveProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsActiveProperty, (object)value);
		}
	}

	public ProgressRingTemplateSettings TemplateSettings => (ProgressRingTemplateSettings)((DependencyObject)this).GetValue(TemplateSettingsProperty);

	static ProgressRing()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsActivePropertyChanged)));
		TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(ProgressRingTemplateSettings), typeof(ProgressRing), (PropertyMetadata)null);
		TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ProgressRing)));
	}

	public ProgressRing()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		((DependencyObject)this).SetValue(TemplateSettingsPropertyKey, (object)new ProgressRingTemplateSettings());
		((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
	}

	private static void OnIsActivePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ProgressRing)(object)sender).OnIsActivePropertyChanged(args);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new ProgressRingAutomationPeer(this);
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		ChangeVisualState();
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		ApplyTemplateSettings();
		ChangeVisualState();
	}

	private void OnIsActivePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		ChangeVisualState();
	}

	private void ChangeVisualState()
	{
		VisualStateManager.GoToState((FrameworkElement)(object)this, IsActive ? "Active" : "Inactive", true);
		VisualStateManager.GoToState((FrameworkElement)(object)this, (TemplateSettings.MaxSideLength < 60.0) ? "Small" : "Large", true);
	}

	private void ApplyTemplateSettings()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		ProgressRingTemplateSettings templateSettings = TemplateSettings;
		(double, double, double) tuple = calcSettings();
		double item = tuple.Item1;
		double item2 = tuple.Item2;
		double item3 = tuple.Item3;
		templateSettings.EllipseDiameter = item2;
		Thickness ellipseOffset = default(Thickness);
		((Thickness)(ref ellipseOffset))._002Ector(0.0, item3, 0.0, 0.0);
		templateSettings.EllipseOffset = ellipseOffset;
		templateSettings.MaxSideLength = item;
		(double, double, double) calcSettings()
		{
			double width;
			if (((FrameworkElement)this).ActualWidth != 0.0)
			{
				width = Math.Min(((FrameworkElement)this).ActualWidth, ((FrameworkElement)this).ActualHeight);
				double num = init();
				double num2 = width * 0.1 + num;
				double item4 = width * 0.5 - num2;
				return (width, num2, item4);
			}
			return (0.0, 0.0, 0.0);
			double init()
			{
				if (width <= 40.0)
				{
					return 1.0;
				}
				return 0.0;
			}
		}
	}
}
