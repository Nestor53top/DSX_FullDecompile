using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModernWpf.Controls.Primitives;

public static class TextBoxHelper
{
	private const string ButtonStatesGroup = "ButtonStates";

	private const string ButtonVisibleState = "ButtonVisible";

	private const string ButtonCollapsedState = "ButtonCollapsed";

	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));

	private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasText", typeof(bool), typeof(TextBoxHelper), (PropertyMetadata)null);

	public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

	public static readonly DependencyProperty IsDeleteButtonProperty = DependencyProperty.RegisterAttached("IsDeleteButton", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsDeleteButtonChanged)));

	public static readonly DependencyProperty IsDeleteButtonVisibleProperty = DependencyProperty.RegisterAttached("IsDeleteButtonVisible", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsDeleteButtonVisibleChanged)));

	public static bool GetIsEnabled(TextBox textBox)
	{
		return (bool)((DependencyObject)textBox).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(TextBox textBox, bool value)
	{
		((DependencyObject)textBox).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		TextBox val = (TextBox)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((FrameworkElement)val).Loaded += new RoutedEventHandler(OnLoaded);
			((TextBoxBase)val).TextChanged += new TextChangedEventHandler(OnTextChanged);
			UpdateHasText(val);
		}
		else
		{
			((FrameworkElement)val).Loaded -= new RoutedEventHandler(OnLoaded);
			((TextBoxBase)val).TextChanged -= new TextChangedEventHandler(OnTextChanged);
			((DependencyObject)val).ClearValue(HasTextPropertyKey);
		}
	}

	public static bool GetHasText(TextBox textBox)
	{
		return (bool)((DependencyObject)textBox).GetValue(HasTextProperty);
	}

	private static void SetHasText(TextBox textBox, bool value)
	{
		((DependencyObject)textBox).SetValue(HasTextPropertyKey, (object)value);
	}

	private static void UpdateHasText(TextBox textBox)
	{
		SetHasText(textBox, !string.IsNullOrEmpty(textBox.Text));
	}

	public static bool GetIsDeleteButton(Button button)
	{
		return (bool)((DependencyObject)button).GetValue(IsDeleteButtonProperty);
	}

	public static void SetIsDeleteButton(Button button, bool value)
	{
		((DependencyObject)button).SetValue(IsDeleteButtonProperty, (object)value);
	}

	private static void OnIsDeleteButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		Button val = (Button)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).OldValue)
		{
			((ButtonBase)val).Click -= new RoutedEventHandler(OnDeleteButtonClick);
		}
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((ButtonBase)val).Click += new RoutedEventHandler(OnDeleteButtonClick);
		}
	}

	public static bool GetIsDeleteButtonVisible(TextBox textBox)
	{
		return (bool)((DependencyObject)textBox).GetValue(IsDeleteButtonVisibleProperty);
	}

	public static void SetIsDeleteButtonVisible(TextBox textBox, bool value)
	{
		((DependencyObject)textBox).SetValue(IsDeleteButtonVisibleProperty, (object)value);
	}

	private static void OnIsDeleteButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		UpdateVisualStates((TextBox)d, (bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	private static void OnLoaded(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0011: Expected O, but got Unknown
		TextBox val = (TextBox)sender;
		UpdateVisualStates(val, GetIsDeleteButtonVisible(val));
	}

	private static void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateHasText((TextBox)sender);
	}

	private static void OnDeleteButtonClick(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DependencyObject templatedParent = ((FrameworkElement)(Button)sender).TemplatedParent;
		TextBox val = (TextBox)(object)((templatedParent is TextBox) ? templatedParent : null);
		if (val != null)
		{
			((DependencyObject)val).SetCurrentValue(TextBox.TextProperty, (object)null);
		}
	}

	private static void UpdateVisualStates(TextBox textBox, bool isDeleteButtonVisible)
	{
		VisualStateManager.GoToState((FrameworkElement)(object)textBox, isDeleteButtonVisible ? "ButtonVisible" : "ButtonCollapsed", true);
	}
}
