using System;
using System.Windows;
using System.Windows.Media;

namespace Standard;

internal static class DpiHelper
{
	[ThreadStatic]
	private static Matrix _transformToDevice;

	[ThreadStatic]
	private static Matrix _transformToDip;

	public static Point LogicalPixelsToDevice(Point logicalPoint, double dpiScaleX, double dpiScaleY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		_transformToDevice = Matrix.Identity;
		((Matrix)(ref _transformToDevice)).Scale(dpiScaleX, dpiScaleY);
		return ((Matrix)(ref _transformToDevice)).Transform(logicalPoint);
	}

	public static Point DevicePixelsToLogical(Point devicePoint, double dpiScaleX, double dpiScaleY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_transformToDip = Matrix.Identity;
		((Matrix)(ref _transformToDip)).Scale(1.0 / dpiScaleX, 1.0 / dpiScaleY);
		return ((Matrix)(ref _transformToDip)).Transform(devicePoint);
	}

	public static Rect LogicalRectToDevice(Rect logicalRectangle, double dpiScaleX, double dpiScaleY)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Point val = LogicalPixelsToDevice(new Point(((Rect)(ref logicalRectangle)).Left, ((Rect)(ref logicalRectangle)).Top), dpiScaleX, dpiScaleY);
		Point val2 = LogicalPixelsToDevice(new Point(((Rect)(ref logicalRectangle)).Right, ((Rect)(ref logicalRectangle)).Bottom), dpiScaleX, dpiScaleY);
		return new Rect(val, val2);
	}

	public static Rect DeviceRectToLogical(Rect deviceRectangle, double dpiScaleX, double dpiScaleY)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Point val = DevicePixelsToLogical(new Point(((Rect)(ref deviceRectangle)).Left, ((Rect)(ref deviceRectangle)).Top), dpiScaleX, dpiScaleY);
		Point val2 = DevicePixelsToLogical(new Point(((Rect)(ref deviceRectangle)).Right, ((Rect)(ref deviceRectangle)).Bottom), dpiScaleX, dpiScaleY);
		return new Rect(val, val2);
	}

	public static Size LogicalSizeToDevice(Size logicalSize, double dpiScaleX, double dpiScaleY)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Point val = LogicalPixelsToDevice(new Point(((Size)(ref logicalSize)).Width, ((Size)(ref logicalSize)).Height), dpiScaleX, dpiScaleY);
		Size result = default(Size);
		((Size)(ref result)).Width = ((Point)(ref val)).X;
		((Size)(ref result)).Height = ((Point)(ref val)).Y;
		return result;
	}

	public static Size DeviceSizeToLogical(Size deviceSize, double dpiScaleX, double dpiScaleY)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Point val = DevicePixelsToLogical(new Point(((Size)(ref deviceSize)).Width, ((Size)(ref deviceSize)).Height), dpiScaleX, dpiScaleY);
		return new Size(((Point)(ref val)).X, ((Point)(ref val)).Y);
	}

	public static Thickness LogicalThicknessToDevice(Thickness logicalThickness, double dpiScaleX, double dpiScaleY)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Point val = LogicalPixelsToDevice(new Point(((Thickness)(ref logicalThickness)).Left, ((Thickness)(ref logicalThickness)).Top), dpiScaleX, dpiScaleY);
		Point val2 = LogicalPixelsToDevice(new Point(((Thickness)(ref logicalThickness)).Right, ((Thickness)(ref logicalThickness)).Bottom), dpiScaleX, dpiScaleY);
		return new Thickness(((Point)(ref val)).X, ((Point)(ref val)).Y, ((Point)(ref val2)).X, ((Point)(ref val2)).Y);
	}
}
