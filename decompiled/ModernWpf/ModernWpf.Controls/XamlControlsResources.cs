using System.Windows;

namespace ModernWpf.Controls;

public class XamlControlsResources : ResourceDictionary
{
	private static ResourceDictionary _controlsResources;

	private static ResourceDictionary _compactResources;

	private static ResourceDictionary _uiSettingsResources;

	private bool _useCompactResources;

	public bool UseCompactResources
	{
		get
		{
			return _useCompactResources;
		}
		set
		{
			if (_useCompactResources != value)
			{
				_useCompactResources = value;
				if (UseCompactResources)
				{
					((ResourceDictionary)this).MergedDictionaries.Add(CompactResources);
				}
				else
				{
					((ResourceDictionary)this).MergedDictionaries.Remove(CompactResources);
				}
			}
		}
	}

	internal static ResourceDictionary ControlsResources
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			if (_controlsResources == null)
			{
				_controlsResources = new ResourceDictionary
				{
					Source = PackUriHelper.GetAbsoluteUri("ControlsResources.xaml")
				};
			}
			return _controlsResources;
		}
	}

	internal static ResourceDictionary CompactResources
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			if (_compactResources == null)
			{
				_compactResources = new ResourceDictionary
				{
					Source = PackUriHelper.GetAbsoluteUri("DensityStyles/Compact.xaml")
				};
			}
			return _compactResources;
		}
	}

	internal static ResourceDictionary UISettingsResources => _uiSettingsResources ?? (_uiSettingsResources = (ResourceDictionary)(object)new UISettingsResources());

	public XamlControlsResources()
	{
		((ResourceDictionary)this).MergedDictionaries.Add(ControlsResources);
		((ResourceDictionary)this).MergedDictionaries.Add(UISettingsResources);
		if (DesignMode.DesignModeEnabled)
		{
			_ = CompactResources;
		}
	}
}
