using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace ModernWpf.Controls.Primitives;

public static class RichTextBoxHelper
{
	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(RichTextBoxHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsEnabledChanged)));

	private static readonly DependencyPropertyKey IsEmptyPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsEmpty", typeof(bool), typeof(RichTextBoxHelper), new PropertyMetadata((object)false));

	public static readonly DependencyProperty IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;

	public static bool GetIsEnabled(RichTextBox richTextBox)
	{
		return (bool)((DependencyObject)richTextBox).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(RichTextBox richTextBox, bool value)
	{
		((DependencyObject)richTextBox).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		RichTextBox val = (RichTextBox)d;
		_ = (bool)((DependencyPropertyChangedEventArgs)(ref e)).OldValue;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((TextBoxBase)val).TextChanged += new TextChangedEventHandler(OnTextChanged);
			UpdateIsEmpty(val);
		}
		else
		{
			((TextBoxBase)val).TextChanged -= new TextChangedEventHandler(OnTextChanged);
			((DependencyObject)val).ClearValue(IsEmptyPropertyKey);
		}
	}

	public static bool GetIsEmpty(RichTextBox richTextBox)
	{
		return (bool)((DependencyObject)richTextBox).GetValue(IsEmptyProperty);
	}

	private static void SetIsEmpty(RichTextBox richTextBox, bool value)
	{
		((DependencyObject)richTextBox).SetValue(IsEmptyPropertyKey, (object)value);
	}

	private static void UpdateIsEmpty(RichTextBox rtb)
	{
		bool flag;
		if (((TextElementCollection<Block>)(object)rtb.Document.Blocks).Count == 0)
		{
			flag = true;
		}
		else
		{
			TextPointer nextInsertionPosition = rtb.Document.ContentStart.GetNextInsertionPosition((LogicalDirection)1);
			TextPointer nextInsertionPosition2 = rtb.Document.ContentEnd.GetNextInsertionPosition((LogicalDirection)0);
			flag = nextInsertionPosition.CompareTo(nextInsertionPosition2) == 0;
		}
		if (GetIsEmpty(rtb) != flag)
		{
			SetIsEmpty(rtb, flag);
		}
	}

	private static void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateIsEmpty((RichTextBox)sender);
	}
}
