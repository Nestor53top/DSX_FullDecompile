using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf;

namespace WPFCustomMessageBox;

internal class CustomMessageBoxWindow : Window, IComponentConnector
{
	internal struct AccentPolicy
	{
		public AccentState AccentState;

		public int AccentFlags;

		public int GradientColor;

		public int AnimationId;
	}

	internal struct WindowCompositionAttributeData
	{
		public WindowCompositionAttribute Attribute;

		public IntPtr Data;

		public int SizeOfData;
	}

	internal enum WindowCompositionAttribute
	{
		WCA_ACCENT_POLICY = 19
	}

	internal enum AccentState
	{
		ACCENT_DISABLED = 1,
		ACCENT_ENABLE_GRADIENT = 0,
		ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
		ACCENT_ENABLE_BLURBEHIND = 3,
		ACCENT_INVALID_STATE = 4
	}

	internal CustomMessageBoxWindow customMBWindow;

	internal Border borderBackground;

	internal Image Image_MessageBox;

	internal TextBlock TextBlock_Message;

	internal Button Button_Cancel;

	internal Label Label_Cancel;

	internal Button Button_No;

	internal Label Label_No;

	internal Button Button_Yes;

	internal Label Label_Yes;

	internal Button Button_OK;

	internal Label Label_Ok;

	private bool _contentLoaded;

	internal string Caption
	{
		get
		{
			return ((Window)this).Title;
		}
		set
		{
			((Window)this).Title = value;
		}
	}

	internal string Message
	{
		get
		{
			return TextBlock_Message.Text;
		}
		set
		{
			TextBlock_Message.Text = value;
		}
	}

	internal string OkButtonText
	{
		get
		{
			return ((ContentControl)Label_Ok).Content.ToString();
		}
		set
		{
			((ContentControl)Label_Ok).Content = value.TryAddKeyboardAccellerator();
		}
	}

	internal string CancelButtonText
	{
		get
		{
			return ((ContentControl)Label_Cancel).Content.ToString();
		}
		set
		{
			((ContentControl)Label_Cancel).Content = value.TryAddKeyboardAccellerator();
		}
	}

	internal string YesButtonText
	{
		get
		{
			return ((ContentControl)Label_Yes).Content.ToString();
		}
		set
		{
			((ContentControl)Label_Yes).Content = value.TryAddKeyboardAccellerator();
		}
	}

	internal string NoButtonText
	{
		get
		{
			return ((ContentControl)Label_No).Content.ToString();
		}
		set
		{
			((ContentControl)Label_No).Content = value.TryAddKeyboardAccellerator();
		}
	}

	public MessageBoxResult Result { get; set; }

	public string DisplayType { get; set; }

	[DllImport("user32.dll")]
	internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

	internal CustomMessageBoxWindow(string message)
	{
		InitializeComponent();
		Message = message;
		((UIElement)Image_MessageBox).Visibility = (Visibility)2;
		DisplayButtons((MessageBoxButton)0);
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0020: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		BrushConverter val = new BrushConverter();
		Brush background = (Brush)((TypeConverter)val).ConvertFromString("#CCFFFFFF");
		Brush background2 = (Brush)((TypeConverter)val).ConvertFromString("#CC000000");
		SolidColorBrush foreground = new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
		SolidColorBrush foreground2 = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte)0, (byte)0, (byte)0));
		EnableBlur();
		if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Dark)
		{
			borderBackground.Background = background2;
			TextBlock_Message.Foreground = (Brush)(object)foreground;
			((Control)Label_Cancel).Foreground = (Brush)(object)foreground;
			((Control)Label_No).Foreground = (Brush)(object)foreground;
			((Control)Label_Yes).Foreground = (Brush)(object)foreground;
			((Control)Label_Ok).Foreground = (Brush)(object)foreground;
		}
		else
		{
			borderBackground.Background = background;
			TextBlock_Message.Foreground = (Brush)(object)foreground2;
			((Control)Label_Cancel).Foreground = (Brush)(object)foreground2;
			((Control)Label_No).Foreground = (Brush)(object)foreground2;
			((Control)Label_Yes).Foreground = (Brush)(object)foreground2;
			((Control)Label_Ok).Foreground = (Brush)(object)foreground2;
		}
		SystemSounds.Asterisk.Play();
	}

	internal void EnableBlur()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		WindowInteropHelper val = new WindowInteropHelper((Window)(object)this);
		AccentPolicy structure = new AccentPolicy
		{
			AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
		};
		int num = Marshal.SizeOf(structure);
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
		WindowCompositionAttributeData data = new WindowCompositionAttributeData
		{
			Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
			SizeOfData = num,
			Data = intPtr
		};
		SetWindowCompositionAttribute(val.Handle, ref data);
		Marshal.FreeHGlobal(intPtr);
	}

	internal CustomMessageBoxWindow(string message, string caption)
	{
		InitializeComponent();
		Message = message;
		Caption = caption;
		((UIElement)Image_MessageBox).Visibility = (Visibility)2;
		DisplayButtons((MessageBoxButton)0);
	}

	internal CustomMessageBoxWindow(string message, string caption, MessageBoxButton button)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
		Message = message;
		Caption = caption;
		((UIElement)Image_MessageBox).Visibility = (Visibility)2;
		DisplayButtons(button);
	}

	internal CustomMessageBoxWindow(string message, string caption, MessageBoxImage image)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
		Message = message;
		Caption = caption;
		DisplayImage(image);
		DisplayButtons((MessageBoxButton)0);
	}

	internal CustomMessageBoxWindow(string message, string caption, MessageBoxButton button, MessageBoxImage image)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
		Message = message;
		Caption = caption;
		((UIElement)Image_MessageBox).Visibility = (Visibility)2;
		DisplayButtons(button);
		DisplayImage(image);
	}

	private void DisplayButtons(MessageBoxButton button)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		switch (button - 1)
		{
		case 0:
			((UIElement)Button_OK).Visibility = (Visibility)0;
			((UIElement)Button_OK).Focus();
			((UIElement)Button_Cancel).Visibility = (Visibility)0;
			((UIElement)Button_Yes).Visibility = (Visibility)2;
			((UIElement)Button_No).Visibility = (Visibility)2;
			DisplayType = "OKCancel";
			break;
		case 3:
			((UIElement)Button_Yes).Visibility = (Visibility)0;
			((UIElement)Button_Yes).Focus();
			((UIElement)Button_No).Visibility = (Visibility)0;
			((UIElement)Button_OK).Visibility = (Visibility)2;
			((UIElement)Button_Cancel).Visibility = (Visibility)2;
			DisplayType = "YesNo";
			break;
		case 2:
			((UIElement)Button_Yes).Visibility = (Visibility)0;
			((UIElement)Button_Yes).Focus();
			((UIElement)Button_No).Visibility = (Visibility)0;
			((UIElement)Button_Cancel).Visibility = (Visibility)0;
			((UIElement)Button_OK).Visibility = (Visibility)2;
			DisplayType = "YesNoCancel";
			break;
		default:
			((UIElement)Button_OK).Visibility = (Visibility)0;
			((UIElement)Button_OK).Focus();
			((UIElement)Button_Yes).Visibility = (Visibility)2;
			((UIElement)Button_No).Visibility = (Visibility)2;
			((UIElement)Button_Cancel).Visibility = (Visibility)2;
			DisplayType = "Default";
			break;
		}
	}

	private void DisplayImage(MessageBoxImage image)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		Icon icon;
		if ((int)image <= 32)
		{
			if ((int)image != 16)
			{
				if ((int)image != 32)
				{
					goto IL_003d;
				}
				icon = SystemIcons.Question;
			}
			else
			{
				icon = SystemIcons.Hand;
			}
		}
		else if ((int)image != 48)
		{
			if ((int)image != 64)
			{
				goto IL_003d;
			}
			icon = SystemIcons.Information;
		}
		else
		{
			icon = SystemIcons.Exclamation;
		}
		goto IL_0043;
		IL_0043:
		Image_MessageBox.Source = icon.ToImageSource();
		((UIElement)Image_MessageBox).Visibility = (Visibility)0;
		return;
		IL_003d:
		icon = SystemIcons.Information;
		goto IL_0043;
	}

	private void Button_OK_Click(object sender, RoutedEventArgs e)
	{
		Result = (MessageBoxResult)1;
		((Window)this).Close();
	}

	private void Button_Cancel_Click(object sender, RoutedEventArgs e)
	{
		Result = (MessageBoxResult)2;
		((Window)this).Close();
	}

	private void Button_Yes_Click(object sender, RoutedEventArgs e)
	{
		Result = (MessageBoxResult)6;
		((Window)this).Close();
	}

	private void Button_No_Click(object sender, RoutedEventArgs e)
	{
		Result = (MessageBoxResult)7;
		((Window)this).Close();
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.ChangedButton == 0)
		{
			((Window)this).DragMove();
		}
	}

	private void customMBWindow_Closing(object sender, CancelEventArgs e)
	{
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/DualSenseX;component/message%20box/custommessageboxwindow.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			customMBWindow = (CustomMessageBoxWindow)target;
			((FrameworkElement)customMBWindow).Loaded += new RoutedEventHandler(Window_Loaded);
			((Window)customMBWindow).Closing += customMBWindow_Closing;
			((UIElement)customMBWindow).MouseDown += new MouseButtonEventHandler(Window_MouseDown);
			break;
		case 2:
			borderBackground = (Border)target;
			break;
		case 3:
			Image_MessageBox = (Image)target;
			break;
		case 4:
			TextBlock_Message = (TextBlock)target;
			break;
		case 5:
			Button_Cancel = (Button)target;
			((ButtonBase)Button_Cancel).Click += new RoutedEventHandler(Button_Cancel_Click);
			break;
		case 6:
			Label_Cancel = (Label)target;
			break;
		case 7:
			Button_No = (Button)target;
			((ButtonBase)Button_No).Click += new RoutedEventHandler(Button_No_Click);
			break;
		case 8:
			Label_No = (Label)target;
			break;
		case 9:
			Button_Yes = (Button)target;
			((ButtonBase)Button_Yes).Click += new RoutedEventHandler(Button_Yes_Click);
			break;
		case 10:
			Label_Yes = (Label)target;
			break;
		case 11:
			Button_OK = (Button)target;
			((ButtonBase)Button_OK).Click += new RoutedEventHandler(Button_OK_Click);
			break;
		case 12:
			Label_Ok = (Label)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
