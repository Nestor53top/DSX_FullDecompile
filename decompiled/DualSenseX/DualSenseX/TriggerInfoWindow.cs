using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;

namespace DualSenseX;

public class TriggerInfoWindow : Window, IComponentConnector
{
	internal TriggerInfoWindow Window;

	internal Grid InitialGrid;

	internal Label ConnectionTypeLabel;

	internal Label StatusBatteryLevelIcon;

	internal Label StatusBatteryLevelLabel;

	internal Label ConnectionStatusLabel;

	internal Label ActiveTriggerStaticLabel;

	internal Label ActiveTriggerLabel;

	internal Label EmulationTypeLabel;

	private bool _contentLoaded;

	[DllImport("user32.dll")]
	internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

	public TriggerInfoWindow()
	{
		InitializeComponent();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
	}

	public void ToggleVisibility(bool IsVisibile)
	{
		if (IsVisibile)
		{
			((UIElement)this).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)this).Visibility = (Visibility)1;
		}
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

	internal void DisableBlur()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		WindowInteropHelper val = new WindowInteropHelper((Window)(object)this);
		AccentPolicy structure = new AccentPolicy
		{
			AccentState = AccentState.ACCENT_DISABLED
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

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
	}

	private void Window_MouseEnter(object sender, MouseEventArgs e)
	{
	}

	private void Window_MouseLeave(object sender, MouseEventArgs e)
	{
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/DualSenseX;component/triggerinfowindow.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			Window = (TriggerInfoWindow)target;
			((FrameworkElement)Window).Loaded += new RoutedEventHandler(Window_Loaded);
			((UIElement)Window).MouseEnter += new MouseEventHandler(Window_MouseEnter);
			((UIElement)Window).MouseLeave += new MouseEventHandler(Window_MouseLeave);
			break;
		case 2:
			InitialGrid = (Grid)target;
			break;
		case 3:
			ConnectionTypeLabel = (Label)target;
			break;
		case 4:
			StatusBatteryLevelIcon = (Label)target;
			break;
		case 5:
			StatusBatteryLevelLabel = (Label)target;
			break;
		case 6:
			ConnectionStatusLabel = (Label)target;
			break;
		case 7:
			ActiveTriggerStaticLabel = (Label)target;
			break;
		case 8:
			ActiveTriggerLabel = (Label)target;
			break;
		case 9:
			EmulationTypeLabel = (Label)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
