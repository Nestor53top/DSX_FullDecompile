using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class ProgressBarAutomationPeer : RangeBaseAutomationPeer, IRangeValueProvider
{
	private static readonly ResourceAccessor ResourceAccessor = new ResourceAccessor(typeof(ProgressBar));

	bool IRangeValueProvider.IsReadOnly => true;

	double IRangeValueProvider.SmallChange => double.NaN;

	double IRangeValueProvider.LargeChange => double.NaN;

	public ProgressBarAutomationPeer(ProgressBar owner)
		: base((RangeBase)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 3 && ((UIElementAutomationPeer)this).Owner is ProgressBar { IsIndeterminate: not false })
		{
			return null;
		}
		return ((RangeBaseAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override string GetClassNameCore()
	{
		return "ProgressBar";
	}

	protected override string GetNameCore()
	{
		string nameCore = ((FrameworkElementAutomationPeer)this).GetNameCore();
		if (((UIElementAutomationPeer)this).Owner is ProgressBar progressBar)
		{
			if (progressBar.ShowError)
			{
				return ResourceAccessor.GetLocalizedStringResource("ProgressBarErrorStatus");
			}
			if (progressBar.ShowPaused)
			{
				return ResourceAccessor.GetLocalizedStringResource("ProgressBarPausedStatus");
			}
			if (progressBar.IsIndeterminate)
			{
				return ResourceAccessor.GetLocalizedStringResource("ProgressBarIndeterminateStatus");
			}
		}
		return nameCore;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)12;
	}
}
