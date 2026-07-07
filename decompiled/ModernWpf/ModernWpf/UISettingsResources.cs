using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

namespace ModernWpf;

internal class UISettingsResources : ResourceDictionary
{
	private const string UniversalApiContractName = "Windows.Foundation.UniversalApiContract";

	private const string AutoHideScrollBarsKey = "AutoHideScrollBars";

	private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

	private UISettings _uiSettings;

	public UISettingsResources()
	{
		if (!DesignMode.DesignModeEnabled && OSVersionHelper.IsWindows10OrGreater)
		{
			Initialize();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Initialize()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_uiSettings = new UISettings();
		if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", (ushort)4))
		{
			InitializeForContract4();
		}
		if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", (ushort)8))
		{
			InitializeForContract8();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void InitializeForContract4()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		UISettings uiSettings = _uiSettings;
		WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<UISettings, object>>((Func<TypedEventHandler<UISettings, object>, EventRegistrationToken>)uiSettings.add_AdvancedEffectsEnabledChanged, (Action<EventRegistrationToken>)uiSettings.remove_AdvancedEffectsEnabledChanged, (TypedEventHandler<UISettings, object>)delegate
		{
			_dispatcher.BeginInvoke(ApplyAdvancedEffectsEnabled);
		});
		if (PackagedAppHelper.IsPackagedApp)
		{
			SystemEvents.UserPreferenceChanged += (UserPreferenceChangedEventHandler)delegate(object sender, UserPreferenceChangedEventArgs args)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				if ((int)args.Category == 4)
				{
					ApplyAdvancedEffectsEnabled();
				}
			};
		}
		ApplyAdvancedEffectsEnabled();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void InitializeForContract8()
	{
		UISettings uiSettings = _uiSettings;
		WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<UISettings, UISettingsAutoHideScrollBarsChangedEventArgs>>((Func<TypedEventHandler<UISettings, UISettingsAutoHideScrollBarsChangedEventArgs>, EventRegistrationToken>)uiSettings.add_AutoHideScrollBarsChanged, (Action<EventRegistrationToken>)uiSettings.remove_AutoHideScrollBarsChanged, (TypedEventHandler<UISettings, UISettingsAutoHideScrollBarsChangedEventArgs>)delegate
		{
			_dispatcher.BeginInvoke(ApplyAutoHideScrollBars);
		});
		ApplyAutoHideScrollBars();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ApplyAdvancedEffectsEnabled()
	{
		ResourceKey dropShadowKey = SystemParameters.DropShadowKey;
		if (_uiSettings.AdvancedEffectsEnabled)
		{
			((ResourceDictionary)this).Remove((object)dropShadowKey);
		}
		else
		{
			((ResourceDictionary)this)[(object)dropShadowKey] = false;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ApplyAutoHideScrollBars()
	{
		((ResourceDictionary)this)[(object)"AutoHideScrollBars"] = _uiSettings.AutoHideScrollBars;
	}
}
