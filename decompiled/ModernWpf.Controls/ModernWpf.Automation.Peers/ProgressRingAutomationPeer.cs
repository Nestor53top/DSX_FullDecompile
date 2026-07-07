using System.Windows;
using System.Windows.Automation.Peers;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class ProgressRingAutomationPeer : FrameworkElementAutomationPeer
{
	private static readonly ResourceAccessor ResourceAccessor = new ResourceAccessor(typeof(ProgressRing));

	public ProgressRingAutomationPeer(ProgressRing owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	protected override string GetClassNameCore()
	{
		return "ProgressRing";
	}

	protected override string GetNameCore()
	{
		string nameCore = ((FrameworkElementAutomationPeer)this).GetNameCore();
		if (((UIElementAutomationPeer)this).Owner is ProgressRing { IsActive: not false })
		{
			return ResourceAccessor.GetLocalizedStringResource("ProgressRingIndeterminateStatus") + nameCore;
		}
		return nameCore;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)12;
	}

	protected override string GetLocalizedControlTypeCore()
	{
		return ResourceAccessor.GetLocalizedStringResource("ProgressRingName");
	}
}
