using System.Windows;
using System.Windows.Media;

namespace ModernWpf.Controls;

public class FontIconSource : IconSource
{
	private const string c_fontIconSourceDefaultFontFamily = "Segoe MDL2 Assets";

	public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(FontIconSource), new PropertyMetadata((object)new FontFamily("Segoe MDL2 Assets")));

	public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(FontIconSource), new PropertyMetadata((object)20.0));

	public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(FontIconSource), new PropertyMetadata((object)FontStyles.Normal));

	public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(FontIconSource), new PropertyMetadata((object)FontWeights.Normal));

	public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(FontIconSource), new PropertyMetadata((object)string.Empty));

	public FontFamily FontFamily
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (FontFamily)((DependencyObject)this).GetValue(FontFamilyProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FontFamilyProperty, (object)value);
		}
	}

	public double FontSize
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(FontSizeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FontSizeProperty, (object)value);
		}
	}

	public FontStyle FontStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (FontStyle)((DependencyObject)this).GetValue(FontStyleProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FontStyleProperty, (object)value);
		}
	}

	public FontWeight FontWeight
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (FontWeight)((DependencyObject)this).GetValue(FontWeightProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FontWeightProperty, (object)value);
		}
	}

	public string Glyph
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(GlyphProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(GlyphProperty, (object)value);
		}
	}
}
