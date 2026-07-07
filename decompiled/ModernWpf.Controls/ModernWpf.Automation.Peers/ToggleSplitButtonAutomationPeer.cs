using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class ToggleSplitButtonAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IToggleProvider
{
	public ExpandCollapseState ExpandCollapseState
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			ExpandCollapseState result = (ExpandCollapseState)0;
			ToggleSplitButton impl = GetImpl();
			if (impl != null && impl.IsFlyoutOpen)
			{
				result = (ExpandCollapseState)1;
			}
			return result;
		}
	}

	public ToggleState ToggleState
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			ToggleState result = (ToggleState)0;
			ToggleSplitButton impl = GetImpl();
			if (impl != null && impl.IsChecked)
			{
				result = (ToggleState)1;
			}
			return result;
		}
	}

	public ToggleSplitButtonAutomationPeer(ToggleSplitButton owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 6 || (int)patternInterface == 15)
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override string GetClassNameCore()
	{
		return "ToggleSplitButton";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)31;
	}

	private ToggleSplitButton GetImpl()
	{
		ToggleSplitButton result = null;
		if (((UIElementAutomationPeer)this).Owner is ToggleSplitButton toggleSplitButton)
		{
			result = toggleSplitButton;
		}
		return result;
	}

	public void Expand()
	{
		GetImpl()?.OpenFlyout();
	}

	public void Collapse()
	{
		GetImpl()?.CloseFlyout();
	}

	public void Toggle()
	{
		GetImpl()?.Toggle();
	}
}
