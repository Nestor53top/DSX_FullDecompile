using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using EasyLocalization.Localization;
using HidSharp;
using Microsoft.Win32;
using ModernWpf.Controls;
using WPFCustomMessageBox;

namespace DualSenseX;

public class LoadPS5Skd
{
	public HidDevice something;

	public DuelSense_Base_Updated dualSenseUpdated;

	public string ConnectionType;

	public string ControllerID;

	private ContentDialogExample VigemDriverNotFoundDialog = new ContentDialogExample(LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_Uh-oh"), LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_YouDontHaveViGEmBusDriverse"), LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadAndInstall"), "", LocalizationManager.Instance.GetValue("AppNotificationPopUp_Cancel"));

	private DispatcherTimer dispatcherTimer = new DispatcherTimer();

	private ProgressBarDownloadDialog DownloadLatesDialog = new ProgressBarDownloadDialog(LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadingAndInstalling"), "", "", "", "", 0);

	private Process ffmpeg = new Process();

	public async void ConnectControllerEmulation()
	{
		if (GlobalVar.DualSense.GetMaxInputReportLength() == 64 && GlobalVar.DualSense != null)
		{
			try
			{
				if (new ManagementObjectSearcher((ObjectQuery)new SelectQuery("Win32_SystemDriver")
				{
					Condition = "Name = 'ViGEmBus'"
				}).Get().Count > 0)
				{
					ConnectionType = "USB";
					dualSenseUpdated = new DualSense_USB_Updated(GlobalVar.DualSense);
					NormalTrigger(leftTrigger: false, rightTrigger: false, bothTriggers: true);
					return;
				}
				GlobalVar.IsControllerConnectedStatus = false;
				GlobalVar.DriverNotInstalled = true;
				if (((FrameworkElement)VigemDriverNotFoundDialog).IsLoaded)
				{
					return;
				}
				GlobalVar.CheckIfMinimizedNoDrivers = true;
				VigemDriverNotFoundDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_Uh-oh");
				VigemDriverNotFoundDialog.BodyText.Text = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_YouDontHaveViGEmBusDriverse");
				VigemDriverNotFoundDialog.PrimaryButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadAndInstall");
				VigemDriverNotFoundDialog.CloseButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_Cancel");
				((UIElement)VigemDriverNotFoundDialog.Rectangle2).Visibility = (Visibility)1;
				((FrameworkElement)VigemDriverNotFoundDialog.Rectangle1).Height = 0.0;
				((FrameworkElement)VigemDriverNotFoundDialog.Rectangle3).Height = 0.0;
				((FrameworkElement)VigemDriverNotFoundDialog.whatsnewTextblock).Height = 0.0;
				((FrameworkElement)VigemDriverNotFoundDialog.installNowTextblock).Height = 0.0;
				VigemDriverNotFoundDialog.ScrollViewer.HorizontalScrollBarVisibility = (ScrollBarVisibility)0;
				if (await VigemDriverNotFoundDialog.ShowAsync() == ContentDialogResult.Primary)
				{
					if (Environment.Is64BitOperatingSystem)
					{
						WebClient webClient = new WebClient();
						webClient.DownloadFileCompleted += Completed64Bit;
						webClient.DownloadFileAsync(new Uri("https://github.com/ViGEm/ViGEmBus/releases/download/setup-v1.17.333/ViGEmBusSetup_x64.msi"), "C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
						if (((UIElement)DownloadLatesDialog).IsVisible)
						{
							return;
						}
						DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadingAndInstalling");
						((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = LocalizationManager.Instance.GetValue("GamsPage_GameListStatusTwo_PleaseWait");
						((UIElement)DownloadLatesDialog.ProgressValueLabel).Opacity = 0.0;
						((UIElement)DownloadLatesDialog.ProgressBar2).Opacity = 0.0;
						switch (await DownloadLatesDialog.ShowAsync())
						{
						case ContentDialogResult.Primary:
							await Task.Delay(500);
							File.Delete("C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
							if ((int)CustomMessageBox.ShowOK("ViGEm installation Requires a restart.", "DualSenseX", "Restart App", (MessageBoxImage)48) == 1)
							{
								Application.Restart();
								Environment.Exit(0);
							}
							break;
						}
						return;
					}
					WebClient webClient2 = new WebClient();
					webClient2.DownloadFileCompleted += Completed32Bit;
					webClient2.DownloadFileAsync(new Uri("https://github.com/ViGEm/ViGEmBus/releases/download/setup-v1.17.333/ViGEmBusSetup_x86.msi"), "C:\\Temp\\DualSenseX\\ViGEmBusSetup_x86.msi");
					if (((UIElement)DownloadLatesDialog).IsVisible)
					{
						return;
					}
					DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadingAndInstalling");
					((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = LocalizationManager.Instance.GetValue("GamsPage_GameListStatusTwo_PleaseWait");
					((UIElement)DownloadLatesDialog.ProgressValueLabel).Opacity = 0.0;
					((UIElement)DownloadLatesDialog.ProgressBar2).Opacity = 0.0;
					switch (await DownloadLatesDialog.ShowAsync())
					{
					case ContentDialogResult.Primary:
						await Task.Delay(500);
						File.Delete("C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
						if ((int)CustomMessageBox.ShowOK("ViGEm installation Requires a restart.", "DualSenseX", "Restart App", (MessageBoxImage)48) == 1)
						{
							Application.Restart();
							Environment.Exit(0);
						}
						break;
					}
					return;
				}
				_ = 2;
			}
			catch (Exception ex)
			{
				if (!ex.Message.Contains("VigemBusNotFoundException"))
				{
					return;
				}
				GlobalVar.DriverNotInstalled = true;
				if (((FrameworkElement)VigemDriverNotFoundDialog).IsLoaded)
				{
					return;
				}
				GlobalVar.CheckIfMinimizedNoDrivers = true;
				VigemDriverNotFoundDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_Uh-oh");
				VigemDriverNotFoundDialog.BodyText.Text = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_YouDontHaveViGEmBusDriverse");
				VigemDriverNotFoundDialog.PrimaryButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadAndInstall");
				VigemDriverNotFoundDialog.CloseButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_Cancel");
				((UIElement)VigemDriverNotFoundDialog.Rectangle2).Visibility = (Visibility)1;
				((FrameworkElement)VigemDriverNotFoundDialog.Rectangle1).Height = 0.0;
				((FrameworkElement)VigemDriverNotFoundDialog.Rectangle3).Height = 0.0;
				((FrameworkElement)VigemDriverNotFoundDialog.whatsnewTextblock).Height = 0.0;
				((FrameworkElement)VigemDriverNotFoundDialog.installNowTextblock).Height = 0.0;
				VigemDriverNotFoundDialog.ScrollViewer.HorizontalScrollBarVisibility = (ScrollBarVisibility)0;
				if (await VigemDriverNotFoundDialog.ShowAsync() == ContentDialogResult.Primary)
				{
					if (Environment.Is64BitOperatingSystem)
					{
						WebClient webClient3 = new WebClient();
						webClient3.DownloadFileCompleted += Completed64Bit;
						webClient3.DownloadFileAsync(new Uri("https://github.com/ViGEm/ViGEmBus/releases/download/setup-v1.17.333/ViGEmBusSetup_x64.msi"), "C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
						if (((UIElement)DownloadLatesDialog).IsVisible)
						{
							return;
						}
						DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadingAndInstalling");
						((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = LocalizationManager.Instance.GetValue("GamsPage_GameListStatusTwo_PleaseWait");
						((UIElement)DownloadLatesDialog.ProgressValueLabel).Opacity = 0.0;
						((UIElement)DownloadLatesDialog.ProgressBar2).Opacity = 0.0;
						switch (await DownloadLatesDialog.ShowAsync())
						{
						case ContentDialogResult.Primary:
							await Task.Delay(500);
							File.Delete("C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
							if ((int)CustomMessageBox.ShowOK("ViGEm installation Requires a restart.", "DualSenseX", "Restart App", (MessageBoxImage)48) == 1)
							{
								Application.Restart();
								Environment.Exit(0);
							}
							break;
						}
						return;
					}
					WebClient webClient4 = new WebClient();
					webClient4.DownloadFileCompleted += Completed32Bit;
					webClient4.DownloadFileAsync(new Uri("https://github.com/ViGEm/ViGEmBus/releases/download/setup-v1.17.333/ViGEmBusSetup_x86.msi"), "C:\\Temp\\DualSenseX\\ViGEmBusSetup_x86.msi");
					if (((UIElement)DownloadLatesDialog).IsVisible)
					{
						return;
					}
					DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadingAndInstalling");
					((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = LocalizationManager.Instance.GetValue("GamsPage_GameListStatusTwo_PleaseWait");
					((UIElement)DownloadLatesDialog.ProgressValueLabel).Opacity = 0.0;
					((UIElement)DownloadLatesDialog.ProgressBar2).Opacity = 0.0;
					switch (await DownloadLatesDialog.ShowAsync())
					{
					case ContentDialogResult.Primary:
						await Task.Delay(500);
						File.Delete("C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
						if ((int)CustomMessageBox.ShowOK("ViGEm installation Requires a restart.", "DualSenseX", "Restart App", (MessageBoxImage)48) == 1)
						{
							Application.Restart();
							Environment.Exit(0);
						}
						break;
					}
					return;
				}
				_ = 2;
			}
			return;
		}
		try
		{
			if (new ManagementObjectSearcher((ObjectQuery)new SelectQuery("Win32_SystemDriver")
			{
				Condition = "Name = 'ViGEmBus'"
			}).Get().Count > 0)
			{
				ConnectionType = "Bluetooth";
				dualSenseUpdated = new DualSense_Bluetooth_Updated(GlobalVar.DualSense);
				NormalTrigger(leftTrigger: false, rightTrigger: false, bothTriggers: true);
				return;
			}
			GlobalVar.IsControllerConnectedStatus = false;
			GlobalVar.DriverNotInstalled = true;
			if (((FrameworkElement)VigemDriverNotFoundDialog).IsLoaded)
			{
				return;
			}
			GlobalVar.CheckIfMinimizedNoDrivers = true;
			VigemDriverNotFoundDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_Uh-oh");
			VigemDriverNotFoundDialog.BodyText.Text = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_YouDontHaveViGEmBusDriverse");
			VigemDriverNotFoundDialog.PrimaryButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadAndInstall");
			VigemDriverNotFoundDialog.CloseButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_Cancel");
			((UIElement)VigemDriverNotFoundDialog.Rectangle2).Visibility = (Visibility)1;
			((FrameworkElement)VigemDriverNotFoundDialog.Rectangle1).Height = 0.0;
			((FrameworkElement)VigemDriverNotFoundDialog.Rectangle3).Height = 0.0;
			((FrameworkElement)VigemDriverNotFoundDialog.whatsnewTextblock).Height = 0.0;
			((FrameworkElement)VigemDriverNotFoundDialog.installNowTextblock).Height = 0.0;
			VigemDriverNotFoundDialog.ScrollViewer.HorizontalScrollBarVisibility = (ScrollBarVisibility)0;
			if (await VigemDriverNotFoundDialog.ShowAsync() == ContentDialogResult.Primary)
			{
				if (Environment.Is64BitOperatingSystem)
				{
					WebClient webClient5 = new WebClient();
					webClient5.DownloadFileCompleted += Completed64Bit;
					webClient5.DownloadFileAsync(new Uri("https://github.com/ViGEm/ViGEmBus/releases/download/setup-v1.17.333/ViGEmBusSetup_x64.msi"), "C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
					if (((UIElement)DownloadLatesDialog).IsVisible)
					{
						return;
					}
					DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadingAndInstalling");
					((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = LocalizationManager.Instance.GetValue("GamsPage_GameListStatusTwo_PleaseWait");
					((UIElement)DownloadLatesDialog.ProgressValueLabel).Opacity = 0.0;
					((UIElement)DownloadLatesDialog.ProgressBar2).Opacity = 0.0;
					switch (await DownloadLatesDialog.ShowAsync())
					{
					case ContentDialogResult.Primary:
						await Task.Delay(500);
						File.Delete("C:\\Temp\\DualSenseX\\ViGEmBusSetup_x64.msi");
						if ((int)CustomMessageBox.ShowOK("ViGEm installation Requires a restart.", "DualSenseX", "Restart App", (MessageBoxImage)48) == 1)
						{
							Application.Restart();
							Environment.Exit(0);
						}
						break;
					}
					return;
				}
				WebClient webClient6 = new WebClient();
				webClient6.DownloadFileCompleted += Completed32Bit;
				webClient6.DownloadFileAsync(new Uri("https://github.com/ViGEm/ViGEmBus/releases/download/setup-v1.17.333/ViGEmBusSetup_x86.msi"), "C:\\Temp\\DualSenseX\\ViGEmBusSetup_x86.msi");
				if (((UIElement)DownloadLatesDialog).IsVisible)
				{
					return;
				}
				DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_DownloadingAndInstalling");
				((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = LocalizationManager.Instance.GetValue("GamsPage_GameListStatusTwo_PleaseWait");
				((UIElement)DownloadLatesDialog.ProgressValueLabel).Opacity = 0.0;
				((UIElement)DownloadLatesDialog.ProgressBar2).Opacity = 0.0;
				switch (await DownloadLatesDialog.ShowAsync())
				{
				case ContentDialogResult.Primary:
					await Task.Delay(500);
					File.Delete("C:\\Temp\\DualSenseX\\ViGEmBusSetup_x86.msi");
					if ((int)CustomMessageBox.ShowOK("ViGEm installation Requires a restart.", "DualSenseX", "Restart App", (MessageBoxImage)48) == 1)
					{
						Application.Restart();
						Environment.Exit(0);
					}
					break;
				}
			}
			else
			{
				_ = 2;
			}
		}
		catch (Exception)
		{
		}
	}

	public async void Start()
	{
	}

	public void DisconnectEmulationDevice()
	{
		try
		{
			dualSenseUpdated = null;
		}
		catch (Exception)
		{
		}
	}

	public static bool IsSoftwareInstalled(string softwareName)
	{
		string installedProgrammsPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
		string installedProgrammsPath2 = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
		if (Is32BitWindows())
		{
			return IsSoftwareInstalled(softwareName, (RegistryView)512, installedProgrammsPath);
		}
		bool num = IsSoftwareInstalled(softwareName, (RegistryView)256, installedProgrammsPath);
		bool flag = IsSoftwareInstalled(softwareName, (RegistryView)256, installedProgrammsPath2);
		return num || flag;
	}

	private static bool Is32BitWindows()
	{
		return !Environment.Is64BitOperatingSystem;
	}

	private static bool IsSoftwareInstalled(string softwareName, RegistryView registryView, string installedProgrammsPath)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		RegistryKey uninstallKey = RegistryKey.OpenBaseKey((RegistryHive)(-2147483646), registryView).OpenSubKey(installedProgrammsPath);
		if (uninstallKey == null)
		{
			return false;
		}
		return (from installedSoftwareString in uninstallKey.GetSubKeyNames()
			select uninstallKey.OpenSubKey(installedSoftwareString) into installedSoftwareKey
			select installedSoftwareKey.GetValue("DisplayName") as string).Any((string installedSoftwareName) => installedSoftwareName?.Contains(softwareName) ?? false);
	}

	private async void Completed64Bit(object sender, AsyncCompletedEventArgs e)
	{
		Process process = new Process();
		process.StartInfo.FileName = "C:\\Temp\\DualSenseX\\//ViGEmBusSetup_x64.msi";
		process.StartInfo.Arguments = "";
		process.Start();
		await Task.Delay(1000);
		DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_FollowViGEmBusInstructions");
		DownloadLatesDialog.BodyText.Text = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_OnceFinishedInstalling");
		((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = "";
		DownloadLatesDialog.PrimaryButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_ConnectToController");
		DownloadLatesDialog.DefaultButton = ContentDialogButton.Primary;
	}

	private async void Completed32Bit(object sender, AsyncCompletedEventArgs e)
	{
		ffmpeg.StartInfo.FileName = "C:\\Temp\\DualSenseX\\//ViGEmBusSetup_x86.msi";
		ffmpeg.StartInfo.Arguments = "/passive";
		ffmpeg.Start();
		await Task.Delay(1000);
		DownloadLatesDialog.Title = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_FollowViGEmBusInstructions");
		DownloadLatesDialog.BodyText.Text = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_OnceFinishedInstalling");
		((ContentControl)DownloadLatesDialog.PleaseWaitLabel).Content = "";
		DownloadLatesDialog.PrimaryButtonText = LocalizationManager.Instance.GetValue("AppNotificationPopUp_MissingDriver_ConnectToController");
		DownloadLatesDialog.DefaultButton = ContentDialogButton.Primary;
	}

	public void CheckConnection()
	{
		if (dualSenseUpdated != null)
		{
			dualSenseUpdated.CheckConnection();
		}
	}

	public string GetControllerID()
	{
		if (dualSenseUpdated != null)
		{
			return ControllerID;
		}
		return LocalizationManager.Instance.GetValue("ControllerPage_ConnectionType_NotConnected");
	}

	public string GetConnectionType()
	{
		if (dualSenseUpdated == null)
		{
			return LocalizationManager.Instance.GetValue("ControllerPage_ConnectionType_NotConnected");
		}
		return ConnectionType;
	}

	public int GetBatteryLevel()
	{
		if (dualSenseUpdated != null)
		{
			return dualSenseUpdated.BatteryLevel;
		}
		return 300;
	}

	public void DisconnectController()
	{
		if (dualSenseUpdated == null)
		{
			return;
		}
		try
		{
			if (GlobalVar.Savefile.SaveFile_ControllerEmulation != EmulationType.OFF)
			{
				if (GlobalVar.Savefile.SaveFile_ControllerEmulation == EmulationType.Xbox360)
				{
					dualSenseUpdated.Xbox360Controller.Disconnect();
					dualSenseUpdated.Xbox360Controller.SubmitReport();
				}
				else
				{
					dualSenseUpdated.DualShock4Controller.Disconnect();
					dualSenseUpdated.DualShock4Controller.SubmitReport();
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public void SetPlayerLedStatus(PlayerLED Status, bool checkbox1, bool checkbox2, bool checkbox3, bool checkbox4, bool checkbox5)
	{
		if (dualSenseUpdated != null)
		{
			dualSenseUpdated.SetPlayerLedStatus(Status, checkbox1, checkbox2, checkbox3, checkbox4, checkbox5);
		}
	}

	public void SetPlayerLedBrightness(PlayerLEDBrightness Status)
	{
		if (dualSenseUpdated != null)
		{
			dualSenseUpdated.SetPlayerLedBrightness(Status);
		}
	}

	public void SetMicLedStatus(MicLED Status)
	{
		if (dualSenseUpdated != null)
		{
			dualSenseUpdated.SetMicLedStatus(Status);
		}
	}

	public void SetLedColorRGB(byte R, byte G, byte B)
	{
		if (dualSenseUpdated != null)
		{
			dualSenseUpdated.SetLedColor(R, G, B);
		}
	}

	public bool IsControllerConnected()
	{
		if (dualSenseUpdated != null)
		{
			return true;
		}
		return false;
	}

	public void IsControllerConnected2()
	{
		if (dualSenseUpdated != null)
		{
			SetLedColorRGB(byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}

	public byte L2Value()
	{
		if (dualSenseUpdated != null)
		{
			return dualSenseUpdated.LeftTrigger;
		}
		return 0;
	}

	public byte R2Value()
	{
		if (dualSenseUpdated != null)
		{
			return dualSenseUpdated.RightTrigger;
		}
		return 0;
	}

	public short LeftJoystickX()
	{
		if (dualSenseUpdated != null)
		{
			return dualSenseUpdated.LeftThumbX;
		}
		return 0;
	}

	public short LeftJoystickY()
	{
		if (dualSenseUpdated != null)
		{
			return dualSenseUpdated.LeftThumbY;
		}
		return 0;
	}

	public short RightJoystickX()
	{
		if (dualSenseUpdated != null)
		{
			return dualSenseUpdated.RightThumbX;
		}
		return 0;
	}

	public short RightJoystickY()
	{
		if (dualSenseUpdated != null)
		{
			return dualSenseUpdated.RightThumbY;
		}
		return 0;
	}

	public bool XButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.X)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool AButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.A)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool BButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.B)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool YButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Y)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool UpButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Up)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool DownButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Down)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool LeftButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Left)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool RightButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Right)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool L3ButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.LeftThumb)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool R3ButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.RightThumb)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool StartButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Start)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool SelectButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Back)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool L1ButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.LeftShoulder)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool R1ButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.RightShoulder)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool R2ButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.RightTrigger > 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool L2ButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.LeftTrigger > 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool MicButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.Mic)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool PSButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.PsButton)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool TouchPadButtonPressed()
	{
		if (dualSenseUpdated != null)
		{
			if (dualSenseUpdated.TouchPadButton)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public void TouchpadLEDBrightness(double percentage)
	{
		if (dualSenseUpdated != null)
		{
			dualSenseUpdated.SetTouchpadLEDBrightness(percentage);
		}
	}

	public void NormalTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.NormalTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.NormalTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.NormalTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.NormalTrigger);
		}
	}

	public void VerySoftTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VerySoftTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VerySoftTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VerySoftTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VerySoftTrigger);
		}
	}

	public void SoftTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.SoftTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.SoftTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.SoftTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.SoftTrigger);
		}
	}

	public void HardTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.HardTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.HardTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.HardTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.HardTrigger);
		}
	}

	public void VeryHardTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VeryHardTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VeryHardTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VeryHardTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VeryHardTrigger);
		}
	}

	public void HardestTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.HardestTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.HardestTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.HardestTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.HardestTrigger);
		}
	}

	public void RigidTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.RigidTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.RigidTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.RigidTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.RigidTrigger);
		}
	}

	public void GameCubeTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 0;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 0;
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 0;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 0;
		}
	}

	public void VibrateTriggerPulseTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 1;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 1;
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 1;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 1;
		}
	}

	public void ChoppyTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 2;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 2;
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 2;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 2;
		}
	}

	public void MediumTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 3;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 3;
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 3;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 3;
		}
	}

	public void CustomTriggerValues(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 4;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 7;
				return;
			}
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 4;
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 7;
		}
	}

	public void ResistanceTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 8;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 9;
				return;
			}
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 8;
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 9;
		}
	}

	public void BowTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 10;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 11;
				return;
			}
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 10;
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 11;
		}
	}

	public void GallopingTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 12;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 13;
				return;
			}
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 12;
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 13;
		}
	}

	public void SemiAutomaticGunTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 14;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 15;
				return;
			}
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 14;
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 15;
		}
	}

	public void AutomaticGunTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 16;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 17;
				return;
			}
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 16;
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 17;
		}
	}

	public void MachineTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 18;
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 19;
				return;
			}
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 18;
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 19;
		}
	}

	public void CustomTriggerValuesTextfile(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 5;
			}
			else if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 6;
			}
		}
	}

	public void CalibrateTrigger(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.CalibrateTrigger);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.CalibrateTrigger);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.CalibrateTrigger);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.CalibrateTrigger);
		}
	}

	public void VibrateTrigger10Hz(bool leftTrigger, bool rightTrigger, bool bothTriggers)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger_10Hz);
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger_10Hz);
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger_10Hz);
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger_10Hz);
		}
	}

	public void VibrateTriggerCustom(bool leftTrigger, bool rightTrigger, bool bothTriggers, byte intensity)
	{
		if (dualSenseUpdated != null)
		{
			if (leftTrigger)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger(intensity));
				return;
			}
			if (rightTrigger)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger(intensity));
				return;
			}
			DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
			DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
			dualSenseUpdated.SetLeftAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger(intensity));
			dualSenseUpdated.SetRightAdaptiveTrigger(DuelSense_Base_Updated.VibrateTrigger(intensity));
		}
	}

	public void ControllerVibration(bool leftMotor, bool rightMotor, bool bothMotors, byte intensity)
	{
		if (dualSenseUpdated != null)
		{
			if (leftMotor)
			{
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetVibration(intensity, 0);
			}
			else if (rightMotor)
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				dualSenseUpdated.SetVibration(0, intensity);
			}
			else
			{
				DuelSense_Base_Updated.CustomRightTriggerValuesIndex = 999;
				DuelSense_Base_Updated.CustomLeftTriggerValuesIndex = 999;
				dualSenseUpdated.SetVibration(intensity, intensity);
			}
		}
	}
}
