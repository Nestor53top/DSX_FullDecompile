using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class DropDownButtonAutomationPeer : ButtonAutomationPeer, IExpandCollapseProvider
{
	public ExpandCollapseState ExpandCollapseState
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			ExpandCollapseState result = (ExpandCollapseState)0;
			DropDownButton impl = GetImpl();
			if (impl != null && impl.IsFlyoutOpen)
			{
				result = (ExpandCollapseState)1;
			}
			return result;
		}
	}

	public DropDownButtonAutomationPeer(DropDownButton owner)
		: base((Button)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 6)
		{
			return this;
		}
		return ((ButtonAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override string GetClassNameCore()
	{
		return "DropDownButton";
	}

	private DropDownButton GetImpl()
	{
		DropDownButton result = null;
		if (((UIElementAutomationPeer)this).Owner is DropDownButton dropDownButton)
		{
			result = dropDownButton;
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
}
