using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DualSenseX.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)(object)SettingsBase.Synchronized((SettingsBase)(object)new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool MinimizeWindowToTray
	{
		get
		{
			return (bool)((SettingsBase)this)["MinimizeWindowToTray"];
		}
		set
		{
			((SettingsBase)this)["MinimizeWindowToTray"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool AutoConnectOnStart
	{
		get
		{
			return (bool)((SettingsBase)this)["AutoConnectOnStart"];
		}
		set
		{
			((SettingsBase)this)["AutoConnectOnStart"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool KillSteamOnControllerConnect
	{
		get
		{
			return (bool)((SettingsBase)this)["KillSteamOnControllerConnect"];
		}
		set
		{
			((SettingsBase)this)["KillSteamOnControllerConnect"] = value;
		}
	}

	private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
	{
	}

	private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
	{
	}
}
