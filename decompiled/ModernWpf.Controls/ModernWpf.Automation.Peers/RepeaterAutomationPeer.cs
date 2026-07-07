using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class RepeaterAutomationPeer : FrameworkElementAutomationPeer
{
	public RepeaterAutomationPeer(FrameworkElement owner)
		: base(owner)
	{
	}

	protected override List<AutomationPeer> GetChildrenCore()
	{
		ItemsRepeater repeater = (ItemsRepeater)(object)((UIElementAutomationPeer)this).Owner;
		List<AutomationPeer> childrenCore = ((UIElementAutomationPeer)this).GetChildrenCore();
		if (childrenCore == null)
		{
			return null;
		}
		int count = childrenCore.Count;
		List<Tuple<int, AutomationPeer>> list = new List<Tuple<int, AutomationPeer>>();
		list.Capacity = count;
		for (int i = 0; i < count; i++)
		{
			AutomationPeer val = childrenCore[i];
			UIElement element = GetElement(val, repeater);
			if (element != null)
			{
				VirtualizationInfo virtualizationInfo = ItemsRepeater.GetVirtualizationInfo(element);
				if (virtualizationInfo.IsRealized)
				{
					list.Add(Tuple.Create<int, AutomationPeer>(virtualizationInfo.Index, val));
				}
			}
		}
		List<AutomationPeer> list2 = new List<AutomationPeer>(list.Count);
		foreach (Tuple<int, AutomationPeer> item in list.OrderBy((Tuple<int, AutomationPeer> x) => x.Item1))
		{
			list2.Add(item.Item2);
		}
		return list2;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)26;
	}

	private UIElement GetElement(AutomationPeer childPeer, ItemsRepeater repeater)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		DependencyObject val = (DependencyObject)(object)((UIElementAutomationPeer)(FrameworkElementAutomationPeer)childPeer).Owner;
		DependencyObject parent = CachedVisualTreeHelpers.GetParent(val);
		while (parent != null && parent as ItemsRepeater != repeater)
		{
			val = parent;
			parent = CachedVisualTreeHelpers.GetParent(val);
		}
		return (UIElement)val;
	}
}
