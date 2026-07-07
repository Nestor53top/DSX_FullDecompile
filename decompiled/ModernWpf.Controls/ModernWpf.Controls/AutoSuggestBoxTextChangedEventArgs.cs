using System;
using System.Windows;

namespace ModernWpf.Controls;

public sealed class AutoSuggestBoxTextChangedEventArgs : DependencyObject
{
	public static readonly DependencyProperty ReasonProperty = DependencyProperty.Register("Reason", typeof(AutoSuggestionBoxTextChangeReason), typeof(AutoSuggestBoxTextChangedEventArgs), new PropertyMetadata((object)AutoSuggestionBoxTextChangeReason.ProgrammaticChange));

	private readonly WeakReference<AutoSuggestBox> m_source;

	private readonly string m_value;

	public AutoSuggestionBoxTextChangeReason Reason
	{
		get
		{
			return (AutoSuggestionBoxTextChangeReason)((DependencyObject)this).GetValue(ReasonProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ReasonProperty, (object)value);
		}
	}

	public AutoSuggestBoxTextChangedEventArgs()
	{
	}

	internal AutoSuggestBoxTextChangedEventArgs(AutoSuggestBox source, string value)
	{
		m_source = new WeakReference<AutoSuggestBox>(source);
		m_value = value;
	}

	public bool CheckCurrent()
	{
		if (m_source != null && m_source.TryGetTarget(out var target))
		{
			return target.Text == m_value;
		}
		return false;
	}
}
