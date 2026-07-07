using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class SplitButtonAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IInvokeProvider
{
	public ExpandCollapseState ExpandCollapseState
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			ExpandCollapseState result = (ExpandCollapseState)0;
			SplitButton impl = GetImpl();
			if (impl != null && impl.IsFlyoutOpen)
			{
				result = (ExpandCollapseState)1;
			}
			return result;
		}
	}

	public SplitButtonAutomationPeer(SplitButton owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 6 || (int)patternInterface == 0)
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override string GetClassNameCore()
	{
		return "SplitButton";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)31;
	}

	private SplitButton GetImpl()
	{
		SplitButton result = null;
		if (((UIElementAutomationPeer)this).Owner is SplitButton splitButton)
		{
			result = splitButton;
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

	public void Invoke()
	{
		GetImpl()?.OnClickPrimary(null, null);
	}
}
