using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

internal static class CustomPopupPlacementHelper
{
	public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached("Placement", typeof(CustomPlacementMode), typeof(CustomPopupPlacementHelper), new PropertyMetadata((object)CustomPlacementMode.Top));

	public static CustomPlacementMode GetPlacement(DependencyObject element)
	{
		return (CustomPlacementMode)element.GetValue(PlacementProperty);
	}

	public static void SetPlacement(DependencyObject element, CustomPlacementMode value)
	{
		element.SetValue(PlacementProperty, (object)value);
	}

	internal static CustomPopupPlacement[] PositionPopup(CustomPlacementMode placement, Size popupSize, Size targetSize, Point offset, FrameworkElement child = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Matrix value = default(Matrix);
		if (child != null)
		{
			Helper.TryGetTransformToDevice((Visual)(object)child, out value);
		}
		CustomPopupPlacement val = CalculatePopupPlacement(placement, popupSize, targetSize, offset, child, value);
		CustomPopupPlacement? val2 = null;
		CustomPlacementMode? alternativePlacementMode = GetAlternativePlacementMode(placement);
		if (alternativePlacementMode.HasValue)
		{
			val2 = CalculatePopupPlacement(alternativePlacementMode.Value, popupSize, targetSize, offset, child, value);
		}
		if (!val2.HasValue)
		{
			return (CustomPopupPlacement[])(object)new CustomPopupPlacement[1] { val };
		}
		return (CustomPopupPlacement[])(object)new CustomPopupPlacement[2] { val, val2.Value };
	}

	private static CustomPopupPlacement CalculatePopupPlacement(CustomPlacementMode placement, Size popupSize, Size targetSize, Point offset, FrameworkElement child = null, Matrix transformToDevice = default(Matrix))
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		Point val = default(Point);
		PopupPrimaryAxis val2;
		switch (placement)
		{
		case CustomPlacementMode.Top:
			((Point)(ref val))._002Ector((((Size)(ref targetSize)).Width - ((Size)(ref popupSize)).Width) / 2.0, 0.0 - ((Size)(ref popupSize)).Height);
			val2 = (PopupPrimaryAxis)1;
			break;
		case CustomPlacementMode.Bottom:
			((Point)(ref val))._002Ector((((Size)(ref targetSize)).Width - ((Size)(ref popupSize)).Width) / 2.0, ((Size)(ref targetSize)).Height);
			val2 = (PopupPrimaryAxis)1;
			break;
		case CustomPlacementMode.Left:
			((Point)(ref val))._002Ector(0.0 - ((Size)(ref popupSize)).Width, (((Size)(ref targetSize)).Height - ((Size)(ref popupSize)).Height) / 2.0);
			val2 = (PopupPrimaryAxis)2;
			break;
		case CustomPlacementMode.Right:
			((Point)(ref val))._002Ector(((Size)(ref targetSize)).Width, (((Size)(ref targetSize)).Height - ((Size)(ref popupSize)).Height) / 2.0);
			val2 = (PopupPrimaryAxis)2;
			break;
		case CustomPlacementMode.Full:
			((Point)(ref val))._002Ector((((Size)(ref targetSize)).Width - ((Size)(ref popupSize)).Width) / 2.0, (((Size)(ref targetSize)).Height - ((Size)(ref popupSize)).Height) / 2.0);
			val2 = (PopupPrimaryAxis)0;
			break;
		case CustomPlacementMode.TopEdgeAlignedLeft:
			((Point)(ref val))._002Ector(0.0, 0.0 - ((Size)(ref popupSize)).Height);
			val2 = (PopupPrimaryAxis)1;
			break;
		case CustomPlacementMode.TopEdgeAlignedRight:
			((Point)(ref val))._002Ector(((Size)(ref targetSize)).Width - ((Size)(ref popupSize)).Width, 0.0 - ((Size)(ref popupSize)).Height);
			val2 = (PopupPrimaryAxis)1;
			break;
		case CustomPlacementMode.BottomEdgeAlignedLeft:
			((Point)(ref val))._002Ector(0.0, ((Size)(ref targetSize)).Height);
			val2 = (PopupPrimaryAxis)1;
			break;
		case CustomPlacementMode.BottomEdgeAlignedRight:
			((Point)(ref val))._002Ector(((Size)(ref targetSize)).Width - ((Size)(ref popupSize)).Width, ((Size)(ref targetSize)).Height);
			val2 = (PopupPrimaryAxis)1;
			break;
		case CustomPlacementMode.LeftEdgeAlignedTop:
			((Point)(ref val))._002Ector(0.0 - ((Size)(ref popupSize)).Width, 0.0);
			val2 = (PopupPrimaryAxis)2;
			break;
		case CustomPlacementMode.LeftEdgeAlignedBottom:
			((Point)(ref val))._002Ector(0.0 - ((Size)(ref popupSize)).Width, ((Size)(ref targetSize)).Height - ((Size)(ref popupSize)).Height);
			val2 = (PopupPrimaryAxis)2;
			break;
		case CustomPlacementMode.RightEdgeAlignedTop:
			((Point)(ref val))._002Ector(((Size)(ref targetSize)).Width, 0.0);
			val2 = (PopupPrimaryAxis)2;
			break;
		case CustomPlacementMode.RightEdgeAlignedBottom:
			((Point)(ref val))._002Ector(((Size)(ref targetSize)).Width, ((Size)(ref targetSize)).Height - ((Size)(ref popupSize)).Height);
			val2 = (PopupPrimaryAxis)2;
			break;
		default:
			throw new ArgumentOutOfRangeException("placement");
		}
		if (child != null)
		{
			Vector val3 = VisualTreeHelper.GetOffset((Visual)(object)child);
			if (transformToDevice != default(Matrix))
			{
				val3 = ((Matrix)(ref transformToDevice)).Transform(val3);
			}
			val -= val3;
		}
		return new CustomPopupPlacement(val, val2);
	}

	private static CustomPlacementMode? GetAlternativePlacementMode(CustomPlacementMode placement)
	{
		return placement switch
		{
			CustomPlacementMode.Top => CustomPlacementMode.Bottom, 
			CustomPlacementMode.Bottom => CustomPlacementMode.Top, 
			CustomPlacementMode.Left => CustomPlacementMode.Right, 
			CustomPlacementMode.Right => CustomPlacementMode.Left, 
			CustomPlacementMode.Full => null, 
			CustomPlacementMode.TopEdgeAlignedLeft => CustomPlacementMode.BottomEdgeAlignedLeft, 
			CustomPlacementMode.TopEdgeAlignedRight => CustomPlacementMode.BottomEdgeAlignedRight, 
			CustomPlacementMode.BottomEdgeAlignedLeft => CustomPlacementMode.TopEdgeAlignedLeft, 
			CustomPlacementMode.BottomEdgeAlignedRight => CustomPlacementMode.TopEdgeAlignedRight, 
			CustomPlacementMode.LeftEdgeAlignedTop => CustomPlacementMode.RightEdgeAlignedTop, 
			CustomPlacementMode.LeftEdgeAlignedBottom => CustomPlacementMode.RightEdgeAlignedBottom, 
			CustomPlacementMode.RightEdgeAlignedTop => CustomPlacementMode.RightEdgeAlignedTop, 
			CustomPlacementMode.RightEdgeAlignedBottom => CustomPlacementMode.LeftEdgeAlignedBottom, 
			_ => null, 
		};
	}
}
