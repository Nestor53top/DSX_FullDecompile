using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class HyperlinkButtonAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider
{
	public HyperlinkButtonAutomationPeer(HyperlinkButton owner)
		: base((ButtonBase)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 0)
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)5;
	}

	protected override string GetClassNameCore()
	{
		return "Hyperlink";
	}

	protected override bool IsControlElementCore()
	{
		return true;
	}

	void IInvokeProvider.Invoke()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!((AutomationPeer)this).IsEnabled())
		{
			throw new ElementNotEnabledException();
		}
		((HyperlinkButton)(object)((UIElementAutomationPeer)this).Owner).AutomationButtonBaseClick();
	}
}
