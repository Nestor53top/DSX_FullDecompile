using System.Windows;

namespace WPFCustomMessageBox;

public static class CustomMessageBox
{
	public static MessageBoxResult Show(string messageBoxText)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow customMessageBoxWindow = new CustomMessageBoxWindow(messageBoxText);
		((Window)customMessageBoxWindow).ShowDialog();
		return customMessageBoxWindow.Result;
	}

	public static MessageBoxResult Show(string messageBoxText, string caption)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow customMessageBoxWindow = new CustomMessageBoxWindow(messageBoxText, caption);
		((Window)customMessageBoxWindow).ShowDialog();
		return customMessageBoxWindow.Result;
	}

	public static MessageBoxResult Show(Window owner, string messageBoxText)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow customMessageBoxWindow = new CustomMessageBoxWindow(messageBoxText);
		((Window)customMessageBoxWindow).Owner = owner;
		((Window)customMessageBoxWindow).ShowDialog();
		return customMessageBoxWindow.Result;
	}

	public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow customMessageBoxWindow = new CustomMessageBoxWindow(messageBoxText, caption);
		((Window)customMessageBoxWindow).Owner = owner;
		((Window)customMessageBoxWindow).ShowDialog();
		return customMessageBoxWindow.Result;
	}

	public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow customMessageBoxWindow = new CustomMessageBoxWindow(messageBoxText, caption, button);
		((Window)customMessageBoxWindow).ShowDialog();
		return customMessageBoxWindow.Result;
	}

	public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow customMessageBoxWindow = new CustomMessageBoxWindow(messageBoxText, caption, button, icon);
		((Window)customMessageBoxWindow).ShowDialog();
		return customMessageBoxWindow.Result;
	}

	public static MessageBoxResult ShowOK(string messageBoxText, string caption, string okButtonText)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)0)
		{
			OkButtonText = okButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}

	public static MessageBoxResult ShowOK(string messageBoxText, string caption, string okButtonText, MessageBoxImage icon)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)0, icon)
		{
			OkButtonText = okButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}

	public static MessageBoxResult ShowOKCancel(string messageBoxText, string caption, string okButtonText, string cancelButtonText)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)1)
		{
			OkButtonText = okButtonText,
			CancelButtonText = cancelButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}

	public static MessageBoxResult ShowOKCancel(string messageBoxText, string caption, string okButtonText, string cancelButtonText, MessageBoxImage icon)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)1, icon)
		{
			OkButtonText = okButtonText,
			CancelButtonText = cancelButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}

	public static MessageBoxResult ShowYesNo(string messageBoxText, string caption, string yesButtonText, string noButtonText)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)4)
		{
			YesButtonText = yesButtonText,
			NoButtonText = noButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}

	public static MessageBoxResult ShowYesNo(string messageBoxText, string caption, string yesButtonText, string noButtonText, MessageBoxImage icon)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)4, icon)
		{
			YesButtonText = yesButtonText,
			NoButtonText = noButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}

	public static MessageBoxResult ShowYesNoCancel(string messageBoxText, string caption, string yesButtonText, string noButtonText, string cancelButtonText)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)3)
		{
			YesButtonText = yesButtonText,
			NoButtonText = noButtonText,
			CancelButtonText = cancelButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}

	public static MessageBoxResult ShowYesNoCancel(string messageBoxText, string caption, string yesButtonText, string noButtonText, string cancelButtonText, MessageBoxImage icon)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageBoxWindow obj = new CustomMessageBoxWindow(messageBoxText, caption, (MessageBoxButton)3, icon)
		{
			YesButtonText = yesButtonText,
			NoButtonText = noButtonText,
			CancelButtonText = cancelButtonText
		};
		((Window)obj).ShowDialog();
		return obj.Result;
	}
}
