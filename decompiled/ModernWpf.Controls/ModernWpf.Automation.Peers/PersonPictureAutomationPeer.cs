using System.Windows;
using System.Windows.Automation.Peers;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class PersonPictureAutomationPeer : FrameworkElementAutomationPeer
{
	public PersonPictureAutomationPeer(PersonPicture owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)20;
	}

	protected override string GetClassNameCore()
	{
		return "PersonPicture";
	}
}
