using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.UI.Xaml.Markup;

namespace DualSenseX;

[MarkupExtensionReturnType]
public class IconImageExtension : StaticResourceExtension
{
	private static readonly FontFamily fontFamily = new FontFamily("Segoe MDL2 Assets");

	public int SymbolCode { get; set; }

	public double SymbolSize { get; set; } = 16.0;

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0075: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		TextBlock visual = new TextBlock
		{
			FontFamily = fontFamily,
			Text = char.ConvertFromUtf32(SymbolCode)
		};
		VisualBrush brush = new VisualBrush
		{
			Visual = (Visual)(object)visual,
			Stretch = (Stretch)2
		};
		return (object)new DrawingImage((Drawing)new GeometryDrawing
		{
			Brush = (Brush)(object)brush,
			Geometry = (Geometry)new RectangleGeometry(new Rect(0.0, 0.0, SymbolSize, SymbolSize))
		});
	}
}
