using System.Windows;

namespace ModernWpf.Controls;

public class RatingItemFontInfo : RatingItemInfo
{
	public static readonly DependencyProperty DisabledGlyphProperty = DependencyProperty.Register("DisabledGlyph", typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));

	public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));

	public static readonly DependencyProperty PlaceholderGlyphProperty = DependencyProperty.Register("PlaceholderGlyph", typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));

	public static readonly DependencyProperty PointerOverGlyphProperty = DependencyProperty.Register("PointerOverGlyph", typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));

	public static readonly DependencyProperty PointerOverPlaceholderGlyphProperty = DependencyProperty.Register("PointerOverPlaceholderGlyph", typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));

	public static readonly DependencyProperty UnsetGlyphProperty = DependencyProperty.Register("UnsetGlyph", typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));

	public string DisabledGlyph
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(DisabledGlyphProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DisabledGlyphProperty, (object)value);
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

	public string PlaceholderGlyph
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(PlaceholderGlyphProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PlaceholderGlyphProperty, (object)value);
		}
	}

	public string PointerOverGlyph
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(PointerOverGlyphProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PointerOverGlyphProperty, (object)value);
		}
	}

	public string PointerOverPlaceholderGlyph
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(PointerOverPlaceholderGlyphProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PointerOverPlaceholderGlyphProperty, (object)value);
		}
	}

	public string UnsetGlyph
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(UnsetGlyphProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UnsetGlyphProperty, (object)value);
		}
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new RatingItemFontInfo();
	}
}
