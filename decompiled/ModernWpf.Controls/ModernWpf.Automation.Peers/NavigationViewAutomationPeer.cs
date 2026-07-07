using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

internal class NavigationViewAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
{
	public bool CanSelectMultiple => false;

	public bool IsSelectionRequired => false;

	public NavigationViewAutomationPeer(NavigationView owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 1)
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(patternInterface);
	}

	public IRawElementProviderSimple[] GetSelection()
	{
		if (((UIElementAutomationPeer)this).Owner is NavigationView navigationView)
		{
			NavigationViewItem selectedContainer = navigationView.GetSelectedContainer();
			if (selectedContainer != null)
			{
				AutomationPeer val = UIElementAutomationPeer.CreatePeerForElement((UIElement)(object)selectedContainer);
				if (val != null)
				{
					return (IRawElementProviderSimple[])(object)new IRawElementProviderSimple[1] { ((AutomationPeer)this).ProviderFromPeer(val) };
				}
			}
		}
		return (IRawElementProviderSimple[])(object)new IRawElementProviderSimple[0];
	}

	internal void RaiseSelectionChangedEvent(object oldSelection, object newSelecttion)
	{
		if (!AutomationPeer.ListenerExists((AutomationEvents)9) || !(((UIElementAutomationPeer)this).Owner is NavigationView navigationView))
		{
			return;
		}
		NavigationViewItem selectedContainer = navigationView.GetSelectedContainer();
		if (selectedContainer != null)
		{
			AutomationPeer val = UIElementAutomationPeer.CreatePeerForElement((UIElement)(object)selectedContainer);
			if (val != null)
			{
				val.RaiseAutomationEvent((AutomationEvents)8);
			}
		}
	}
}
