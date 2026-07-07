using System;
using System.Windows;
using System.Windows.Media;

namespace ModernWpf;

internal static class Helper
{
	public static bool IsAnimationsEnabled
	{
		get
		{
			if (SystemParameters.ClientAreaAnimation)
			{
				return RenderCapability.Tier > 0;
			}
			return false;
		}
	}

	public static bool TryGetTransformToDevice(Visual visual, out Matrix value)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		PresentationSource val = PresentationSource.FromVisual(visual);
		if (val != null)
		{
			value = val.CompositionTarget.TransformToDevice;
			return true;
		}
		value = default(Matrix);
		return false;
	}

	public static Vector GetOffset(UIElement element1, InterestPoint interestPoint1, UIElement element2, InterestPoint interestPoint2, Rect element2Bounds)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Point val = element1.TranslatePoint(GetPoint(element1, interestPoint1), element2);
		if (((Rect)(ref element2Bounds)).IsEmpty)
		{
			return val - GetPoint(element2, interestPoint2);
		}
		return val - GetPoint(element2Bounds, interestPoint2);
	}

	private static Point GetPoint(UIElement element, InterestPoint interestPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetPoint(new Rect(element.RenderSize), interestPoint);
	}

	private static Point GetPoint(Rect rect, InterestPoint interestPoint)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		return (Point)(interestPoint switch
		{
			InterestPoint.TopLeft => ((Rect)(ref rect)).TopLeft, 
			InterestPoint.TopRight => ((Rect)(ref rect)).TopRight, 
			InterestPoint.BottomLeft => ((Rect)(ref rect)).BottomLeft, 
			InterestPoint.BottomRight => ((Rect)(ref rect)).BottomRight, 
			InterestPoint.Center => new Point(((Rect)(ref rect)).Left + ((Rect)(ref rect)).Width / 2.0, ((Rect)(ref rect)).Top + ((Rect)(ref rect)).Height / 2.0), 
			_ => throw new ArgumentOutOfRangeException("interestPoint"), 
		});
	}

	public static bool HasDefaultValue(this DependencyObject d, DependencyProperty dp)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		ValueSource valueSource = DependencyPropertyHelper.GetValueSource(d, dp);
		return (int)((ValueSource)(ref valueSource)).BaseValueSource == 1;
	}

	public static bool HasNonDefaultValue(this DependencyObject d, DependencyProperty dp)
	{
		return !d.HasDefaultValue(dp);
	}

	public static bool HasLocalValue(this DependencyObject d, DependencyProperty dp)
	{
		return d.ReadLocalValue(dp) != DependencyProperty.UnsetValue;
	}
}
