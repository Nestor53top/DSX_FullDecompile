using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls;

public class FontIcon : IconElement
{
	public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(FontIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)new FontFamily("Segoe MDL2 Assets"), new PropertyChangedCallback(OnFontFamilyChanged)));

	public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(FontIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)20.0, new PropertyChangedCallback(OnFontSizeChanged)));

	public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(FontIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)FontStyles.Normal, new PropertyChangedCallback(OnFontStyleChanged)));

	public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(FontIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)FontWeights.Normal, new PropertyChangedCallback(OnFontWeightChanged)));

	public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(FontIcon), (PropertyMetadata)new FrameworkPropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnGlyphChanged)));

	private TextBlock _textBlock;

	[Bindable(true)]
	[Category("Appearance")]
	[Localizability(/*Could not decode attribute arguments.*/)]
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

	[TypeConverter(typeof(FontSizeConverter))]
	[Bindable(true)]
	[Category("Appearance")]
	[Localizability(/*Could not decode attribute arguments.*/)]
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

	[Bindable(true)]
	[Category("Appearance")]
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

	[Bindable(true)]
	[Category("Appearance")]
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

	private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		FontIcon fontIcon = (FontIcon)(object)d;
		if (fontIcon._textBlock != null)
		{
			fontIcon._textBlock.FontFamily = (FontFamily)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
	}

	private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		FontIcon fontIcon = (FontIcon)(object)d;
		if (fontIcon._textBlock != null)
		{
			fontIcon._textBlock.FontSize = (double)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
	}

	private static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		FontIcon fontIcon = (FontIcon)(object)d;
		if (fontIcon._textBlock != null)
		{
			fontIcon._textBlock.FontStyle = (FontStyle)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
	}

	private static void OnFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		FontIcon fontIcon = (FontIcon)(object)d;
		if (fontIcon._textBlock != null)
		{
			fontIcon._textBlock.FontWeight = (FontWeight)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
	}

	private static void OnGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		FontIcon fontIcon = (FontIcon)(object)d;
		if (fontIcon._textBlock != null)
		{
			fontIcon._textBlock.Text = (string)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
	}

	private protected override void InitializeChildren()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		_textBlock = new TextBlock
		{
			Style = null,
			HorizontalAlignment = (HorizontalAlignment)3,
			VerticalAlignment = (VerticalAlignment)1,
			TextAlignment = (TextAlignment)2,
			FontFamily = FontFamily,
			FontSize = FontSize,
			FontStyle = FontStyle,
			FontWeight = FontWeight,
			Text = Glyph
		};
		if (base.ShouldInheritForegroundFromVisualParent)
		{
			_textBlock.Foreground = base.VisualParentForeground;
		}
		base.Children.Add((UIElement)(object)_textBlock);
	}

	private protected override void OnShouldInheritForegroundFromVisualParentChanged()
	{
		if (_textBlock != null)
		{
			if (base.ShouldInheritForegroundFromVisualParent)
			{
				_textBlock.Foreground = base.VisualParentForeground;
			}
			else
			{
				((DependencyObject)_textBlock).ClearValue(TextBlock.ForegroundProperty);
			}
		}
	}

	private protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (base.ShouldInheritForegroundFromVisualParent && _textBlock != null)
		{
			_textBlock.Foreground = (Brush)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
	}
}
