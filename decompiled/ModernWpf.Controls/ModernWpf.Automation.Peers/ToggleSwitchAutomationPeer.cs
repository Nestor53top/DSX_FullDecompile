using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class ToggleSwitchAutomationPeer : FrameworkElementAutomationPeer, IToggleProvider
{
	public ToggleState ToggleState
	{
		get
		{
			if (GetImpl().IsOn)
			{
				return (ToggleState)1;
			}
			return (ToggleState)0;
		}
	}

	public ToggleSwitchAutomationPeer(ToggleSwitch owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 15)
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override string GetClassNameCore()
	{
		return "ToggleSwitch";
	}

	protected override string GetNameCore()
	{
		string text = ((FrameworkElementAutomationPeer)this).GetNameCore();
		if (string.IsNullOrEmpty(text))
		{
			ToggleSwitch impl = GetImpl();
			string text2 = impl.Header?.ToString();
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
			string text3 = (impl.IsOn ? impl.OnContent : impl.OffContent)?.ToString();
			if (!string.IsNullOrEmpty(text3))
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += " ";
				}
				text += text3;
			}
		}
		return text ?? string.Empty;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)0;
	}

	protected override string GetLocalizedControlTypeCore()
	{
		return "toggle switch";
	}

	public void Toggle()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!((AutomationPeer)this).IsEnabled())
		{
			throw new ElementNotEnabledException();
		}
		GetImpl().Toggle();
	}

	private ToggleSwitch GetImpl()
	{
		return (ToggleSwitch)(object)((UIElementAutomationPeer)this).Owner;
	}
}
