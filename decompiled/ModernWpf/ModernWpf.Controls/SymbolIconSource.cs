using System.Windows;

namespace ModernWpf.Controls;

public class SymbolIconSource : IconSource
{
	public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(Symbol), typeof(SymbolIconSource), new PropertyMetadata((object)Symbol.Emoji));

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
}
