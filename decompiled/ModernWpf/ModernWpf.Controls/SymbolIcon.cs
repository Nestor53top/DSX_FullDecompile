using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls;

public sealed class SymbolIcon : IconElement
{
	public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata((object)Symbol.Emoji, new PropertyChangedCallback(OnSymbolChanged)));

	internal static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(SymbolIcon), new PropertyMetadata((object)20.0, new PropertyChangedCallback(OnFontSizeChanged)));

	private TextBlock _textBlock;

	public Symbol Symbol
	{
		get
		{
			return (Symbol)((DependencyObject)this).GetValue(SymbolProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SymbolProperty, (object)value);
		}
	}

	internal double FontSize
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

	public SymbolIcon()
	{
	}

	public SymbolIcon(Symbol symbol)
	{
		Symbol = symbol;
	}

	private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SymbolIcon)(object)d).OnSymbolChanged(e);
	}

	private void OnSymbolChanged(DependencyPropertyChangedEventArgs e)
	{
		if (_textBlock != null)
		{
			_textBlock.Text = ConvertToString((Symbol)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		}
	}

	private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((SymbolIcon)(object)d).OnFontSizeChanged(e);
	}

	private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
	{
		if (_textBlock != null)
		{
			_textBlock.FontSize = (double)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
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
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		_textBlock = new TextBlock
		{
			Style = null,
			HorizontalAlignment = (HorizontalAlignment)3,
			VerticalAlignment = (VerticalAlignment)1,
			TextAlignment = (TextAlignment)2,
			FontSize = FontSize,
			FontStyle = FontStyles.Normal,
			FontWeight = FontWeights.Normal,
			Text = ConvertToString(Symbol)
		};
		((FrameworkElement)_textBlock).SetResourceReference(TextBlock.FontFamilyProperty, (object)"SymbolThemeFontFamily");
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

	private static string ConvertToString(Symbol symbol)
	{
		return char.ConvertFromUtf32((int)symbol).ToString();
	}
}
