using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public static class SliderAutoToolTipHelper
{
	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(SliderAutoToolTipHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));

	private static readonly DependencyProperty OriginalCustomPopupPlacementCallbackProperty = DependencyProperty.RegisterAttached("OriginalCustomPopupPlacementCallback", typeof(CustomPopupPlacementCallback), typeof(SliderAutoToolTipHelper));

	public static bool GetIsEnabled(ToolTip toolTip)
	{
		return (bool)((DependencyObject)toolTip).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(ToolTip toolTip, bool value)
	{
		((DependencyObject)toolTip).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		ToolTip toolTip = (ToolTip)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			UIElement placementTarget = toolTip.PlacementTarget;
			Thumb val = (Thumb)(object)((placementTarget is Thumb) ? placementTarget : null);
			if (val != null)
			{
				DependencyObject templatedParent = ((FrameworkElement)val).TemplatedParent;
				Slider slider = (Slider)(object)((templatedParent is Slider) ? templatedParent : null);
				if (slider != null)
				{
					SetOriginalCustomPopupPlacementCallback(toolTip, toolTip.CustomPopupPlacementCallback);
					toolTip.CustomPopupPlacementCallback = (CustomPopupPlacementCallback)((Size popupSize, Size targetSize, Point offset) => PositionAutoToolTip(slider, toolTip, popupSize, targetSize));
				}
			}
			((UIElement)toolTip).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnToolTipIsVisibleChanged);
		}
		else
		{
			if (((DependencyObject)toolTip).ReadLocalValue(OriginalCustomPopupPlacementCallbackProperty) != DependencyProperty.UnsetValue)
			{
				toolTip.CustomPopupPlacementCallback = GetOriginalCustomPopupPlacementCallback(toolTip);
				((DependencyObject)toolTip).ClearValue(OriginalCustomPopupPlacementCallbackProperty);
			}
			((UIElement)toolTip).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnToolTipIsVisibleChanged);
		}
	}

	private static CustomPopupPlacementCallback GetOriginalCustomPopupPlacementCallback(ToolTip toolTip)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (CustomPopupPlacementCallback)((DependencyObject)toolTip).GetValue(OriginalCustomPopupPlacementCallbackProperty);
	}

	private static void SetOriginalCustomPopupPlacementCallback(ToolTip toolTip, CustomPopupPlacementCallback value)
	{
		((DependencyObject)toolTip).SetValue(OriginalCustomPopupPlacementCallbackProperty, (object)value);
	}

	private static void OnToolTipIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ToolTip val = (ToolTip)sender;
		UIElement placementTarget = val.PlacementTarget;
		Thumb val2 = (Thumb)(object)((placementTarget is Thumb) ? placementTarget : null);
		if (val2 != null)
		{
			if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
			{
				((FrameworkElement)val2).SizeChanged += new SizeChangedEventHandler(OnThumbSizeChanged);
				UpdatePlacementRectangle(val, ((UIElement)val2).RenderSize);
			}
			else
			{
				((FrameworkElement)val2).SizeChanged -= new SizeChangedEventHandler(OnThumbSizeChanged);
				((DependencyObject)val).ClearValue(ToolTip.PlacementRectangleProperty);
			}
		}
	}

	private static void OnThumbSizeChanged(object sender, SizeChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		object toolTip = ((FrameworkElement)(Thumb)sender).ToolTip;
		ToolTip val = (ToolTip)((toolTip is ToolTip) ? toolTip : null);
		if (val != null)
		{
			UpdatePlacementRectangle(val, e.NewSize);
		}
	}

	private static void UpdatePlacementRectangle(ToolTip toolTip, Size targetSize)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		toolTip.PlacementRectangle = new Rect(new Point(-20.0, -20.0), new Point(((Size)(ref targetSize)).Width + 20.0, ((Size)(ref targetSize)).Height + 20.0));
	}

	private static CustomPopupPlacement[] PositionAutoToolTip(Slider slider, ToolTip autoToolTip, Size popupSize, Size targetSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		AutoToolTipPlacement autoToolTipPlacement = slider.AutoToolTipPlacement;
		Point val = default(Point);
		PopupPrimaryAxis val2;
		if ((int)autoToolTipPlacement != 1)
		{
			if ((int)autoToolTipPlacement != 2)
			{
				return (CustomPopupPlacement[])(object)new CustomPopupPlacement[0];
			}
			if ((int)slider.Orientation == 0)
			{
				((Point)(ref val))._002Ector((((Size)(ref targetSize)).Width - ((Size)(ref popupSize)).Width) * 0.5, ((Size)(ref targetSize)).Height);
				val2 = (PopupPrimaryAxis)1;
			}
			else
			{
				((Point)(ref val))._002Ector(((Size)(ref targetSize)).Width, (((Size)(ref targetSize)).Height - ((Size)(ref popupSize)).Height) * 0.5);
				val2 = (PopupPrimaryAxis)2;
			}
		}
		else if ((int)slider.Orientation == 0)
		{
			((Point)(ref val))._002Ector((((Size)(ref targetSize)).Width - ((Size)(ref popupSize)).Width) * 0.5, 0.0 - ((Size)(ref popupSize)).Height);
			val2 = (PopupPrimaryAxis)1;
		}
		else
		{
			((Point)(ref val))._002Ector(0.0 - ((Size)(ref popupSize)).Width, (((Size)(ref targetSize)).Height - ((Size)(ref popupSize)).Height) * 0.5);
			val2 = (PopupPrimaryAxis)2;
		}
		if (Helper.TryGetTransformToDevice((Visual)(object)autoToolTip, out var value))
		{
			Vector offset = VisualTreeHelper.GetOffset((Visual)(object)autoToolTip);
			val -= ((Matrix)(ref value)).Transform(offset);
		}
		return (CustomPopupPlacement[])(object)new CustomPopupPlacement[1]
		{
			new CustomPopupPlacement(val, val2)
		};
	}
}
