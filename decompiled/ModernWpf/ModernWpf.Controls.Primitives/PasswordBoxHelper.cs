using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public class PasswordBoxHelper : DependencyObject
{
	private const string ButtonStatesGroup = "ButtonStates";

	private const string ButtonVisibleState = "ButtonVisible";

	private const string ButtonCollapsedState = "ButtonCollapsed";

	private static readonly CommandBinding TextBoxCutBinding;

	private static readonly CommandBinding TextBoxCopyBinding;

	private readonly PasswordBox _passwordBox;

	private bool _hideRevealButton;

	public static readonly DependencyProperty PasswordRevealModeProperty;

	public static readonly DependencyProperty IsEnabledProperty;

	private static readonly DependencyPropertyKey PlaceholderTextVisibilityPropertyKey;

	public static readonly DependencyProperty PlaceholderTextVisibilityProperty;

	private static readonly DependencyProperty HelperInstanceProperty;

	private TextBox TextBox { get; set; }

	private PasswordRevealMode PasswordRevealMode => GetPasswordRevealMode(_passwordBox);

	static PasswordBoxHelper()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		PasswordRevealModeProperty = DependencyProperty.RegisterAttached("PasswordRevealMode", typeof(PasswordRevealMode), typeof(PasswordBoxHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)PasswordRevealMode.Peek, new PropertyChangedCallback(OnPasswordRevealModeChanged)));
		IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PasswordBoxHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));
		PlaceholderTextVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("PlaceholderTextVisibility", typeof(Visibility), typeof(PasswordBoxHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)(Visibility)0));
		PlaceholderTextVisibilityProperty = PlaceholderTextVisibilityPropertyKey.DependencyProperty;
		HelperInstanceProperty = DependencyProperty.RegisterAttached("HelperInstance", typeof(PasswordBoxHelper), typeof(PasswordBoxHelper), new PropertyMetadata(new PropertyChangedCallback(OnHelperInstanceChanged)));
		TextBoxCutBinding = new CommandBinding((ICommand)ApplicationCommands.Cut);
		TextBoxCutBinding.CanExecute += new CanExecuteRoutedEventHandler(OnDisabledCommandCanExecute);
		TextBoxCopyBinding = new CommandBinding((ICommand)ApplicationCommands.Copy);
		TextBoxCopyBinding.CanExecute += new CanExecuteRoutedEventHandler(OnDisabledCommandCanExecute);
	}

	public PasswordBoxHelper(PasswordBox passwordBox)
	{
		_passwordBox = passwordBox;
	}

	public static PasswordRevealMode GetPasswordRevealMode(PasswordBox passwordBox)
	{
		return (PasswordRevealMode)((DependencyObject)passwordBox).GetValue(PasswordRevealModeProperty);
	}

	public static void SetPasswordRevealMode(PasswordBox passwordBox, PasswordRevealMode value)
	{
		((DependencyObject)passwordBox).SetValue(PasswordRevealModeProperty, (object)value);
	}

	private static void OnPasswordRevealModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		GetHelperInstance((PasswordBox)d)?.UpdateVisualState(useTransitions: true);
	}

	public static bool GetIsEnabled(PasswordBox passwordBox)
	{
		return (bool)((DependencyObject)passwordBox).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(PasswordBox passwordBox, bool value)
	{
		((DependencyObject)passwordBox).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		PasswordBox val = (PasswordBox)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			SetHelperInstance(val, new PasswordBoxHelper(val));
		}
		else
		{
			((DependencyObject)val).ClearValue(HelperInstanceProperty);
		}
	}

	public static Visibility GetPlaceholderTextVisibility(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Visibility)((DependencyObject)control).GetValue(PlaceholderTextVisibilityProperty);
	}

	private static void SetPlaceholderTextVisibility(Control control, Visibility value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)control).SetValue(PlaceholderTextVisibilityPropertyKey, (object)value);
	}

	private static PasswordBoxHelper GetHelperInstance(PasswordBox passwordBox)
	{
		return (PasswordBoxHelper)((DependencyObject)passwordBox).GetValue(HelperInstanceProperty);
	}

	private static void SetHelperInstance(PasswordBox passwordBox, PasswordBoxHelper value)
	{
		((DependencyObject)passwordBox).SetValue(HelperInstanceProperty, (object)value);
	}

	private static void OnHelperInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is PasswordBoxHelper passwordBoxHelper)
		{
			passwordBoxHelper.Detach();
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue is PasswordBoxHelper passwordBoxHelper2)
		{
			passwordBoxHelper2.Attach();
		}
	}

	private static void OnDisabledCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = false;
		((RoutedEventArgs)e).Handled = true;
	}

	private void Attach()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		_passwordBox.PasswordChanged += new RoutedEventHandler(OnPasswordChanged);
		((UIElement)_passwordBox).GotFocus += new RoutedEventHandler(OnGotFocus);
		((UIElement)_passwordBox).LostFocus += new RoutedEventHandler(OnLostFocus);
		if (((FrameworkElement)_passwordBox).IsLoaded)
		{
			OnApplyTemplate();
		}
		else
		{
			((FrameworkElement)_passwordBox).Loaded += new RoutedEventHandler(OnLoaded);
		}
	}

	private void Detach()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		_passwordBox.PasswordChanged -= new RoutedEventHandler(OnPasswordChanged);
		((UIElement)_passwordBox).GotFocus -= new RoutedEventHandler(OnGotFocus);
		((UIElement)_passwordBox).LostFocus -= new RoutedEventHandler(OnLostFocus);
		((FrameworkElement)_passwordBox).Loaded -= new RoutedEventHandler(OnLoaded);
		if (TextBox != null)
		{
			((UIElement)TextBox).CommandBindings.Remove(TextBoxCutBinding);
			((UIElement)TextBox).CommandBindings.Remove(TextBoxCopyBinding);
			((TextBoxBase)TextBox).TextChanged -= new TextChangedEventHandler(OnTextBoxTextChanged);
			TextBox = null;
		}
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		((FrameworkElement)_passwordBox).Loaded -= new RoutedEventHandler(OnLoaded);
		OnApplyTemplate();
	}

	private void OnApplyTemplate()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		((FrameworkElement)_passwordBox).ApplyTemplate();
		TextBox = ((Control)(object)_passwordBox).GetTemplateChild<TextBox>("TextBox");
		if (TextBox != null)
		{
			((TextBoxBase)TextBox).IsUndoEnabled = false;
			SpellCheck.SetIsEnabled((TextBoxBase)(object)TextBox, false);
			((UIElement)TextBox).CommandBindings.Add(TextBoxCutBinding);
			((UIElement)TextBox).CommandBindings.Add(TextBoxCopyBinding);
			((TextBoxBase)TextBox).TextChanged += new TextChangedEventHandler(OnTextBoxTextChanged);
			UpdateTextBox();
		}
		UpdateVisualState(useTransitions: false);
	}

	private void OnGotFocus(object sender, RoutedEventArgs e)
	{
		if (PasswordRevealMode == PasswordRevealMode.Visible && TextBox != null && e.OriginalSource == _passwordBox)
		{
			((UIElement)TextBox).Focus();
			e.Handled = true;
		}
		if (!string.IsNullOrEmpty(_passwordBox.Password))
		{
			_hideRevealButton = true;
		}
		UpdateVisualState(useTransitions: true);
	}

	private void OnLostFocus(object sender, RoutedEventArgs e)
	{
		UpdateVisualState(useTransitions: true);
	}

	private void OnPasswordChanged(object sender, RoutedEventArgs e)
	{
		bool flag = !string.IsNullOrEmpty(_passwordBox.Password);
		if (!flag)
		{
			_hideRevealButton = false;
		}
		SetPlaceholderTextVisibility((Control)(object)_passwordBox, (Visibility)(flag ? 2 : 0));
		UpdateTextBox();
		UpdateVisualState(useTransitions: true);
	}

	private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (PasswordRevealMode == PasswordRevealMode.Visible)
		{
			_passwordBox.Password = ((TextBox)sender).Text;
		}
	}

	private void UpdateTextBox()
	{
		if (TextBox != null)
		{
			TextBox.Text = _passwordBox.Password;
		}
	}

	private void UpdateVisualState(bool useTransitions)
	{
		bool flag = false;
		if (((UIElement)_passwordBox).IsFocused)
		{
			switch (PasswordRevealMode)
			{
			case PasswordRevealMode.Peek:
				flag = !_hideRevealButton && !string.IsNullOrEmpty(_passwordBox.Password);
				break;
			case PasswordRevealMode.Hidden:
			case PasswordRevealMode.Visible:
				flag = false;
				break;
			}
		}
		VisualStateManager.GoToState((FrameworkElement)(object)_passwordBox, flag ? "ButtonVisible" : "ButtonCollapsed", useTransitions);
	}
}
