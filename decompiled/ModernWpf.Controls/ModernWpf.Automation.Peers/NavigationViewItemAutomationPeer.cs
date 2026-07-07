using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class NavigationViewItemAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider, ISelectionItemProvider, IExpandCollapseProvider
{
	private enum AutomationOutput
	{
		Position,
		Size
	}

	private static readonly ResourceAccessor ResourceAccessor = new ResourceAccessor(typeof(NavigationView));

	ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			ExpandCollapseState result = (ExpandCollapseState)3;
			if (((UIElementAutomationPeer)this).Owner is NavigationViewItem navigationViewItem)
			{
				result = (ExpandCollapseState)(navigationViewItem.IsExpanded ? 1 : 0);
			}
			return result;
		}
	}

	bool ISelectionItemProvider.IsSelected
	{
		get
		{
			if (((UIElementAutomationPeer)this).Owner is NavigationViewItem navigationViewItem)
			{
				return navigationViewItem.IsSelected;
			}
			return false;
		}
	}

	IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
	{
		get
		{
			NavigationView parentNavigationView = GetParentNavigationView();
			if (parentNavigationView != null)
			{
				AutomationPeer val = UIElementAutomationPeer.CreatePeerForElement((UIElement)(object)parentNavigationView);
				if (val != null)
				{
					return ((AutomationPeer)this).ProviderFromPeer(val);
				}
			}
			return null;
		}
	}

	public NavigationViewItemAutomationPeer(NavigationViewItem owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	protected override string GetNameCore()
	{
		string text = ((FrameworkElementAutomationPeer)this).GetNameCore();
		if (string.IsNullOrEmpty(text) && ((UIElementAutomationPeer)this).Owner is NavigationViewItem navigationViewItem)
		{
			text = SharedHelpers.TryGetStringRepresentationFromObject(((ContentControl)navigationViewItem).Content);
		}
		if (string.IsNullOrEmpty(text))
		{
			text = ResourceAccessor.GetLocalizedStringResource("NavigationViewItemDefaultControlName");
		}
		return text;
	}

	public override object GetPattern(PatternInterface pattern)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if ((int)pattern == 11 || ((int)pattern == 6 && HasChildren()))
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(pattern);
	}

	protected override string GetClassNameCore()
	{
		return "NavigationViewItem";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		if (!IsOnTopNavigation())
		{
			return (AutomationControlType)7;
		}
		return (AutomationControlType)19;
	}

	void IInvokeProvider.Invoke()
	{
		NavigationView parentNavigationView = GetParentNavigationView();
		if (parentNavigationView != null && ((UIElementAutomationPeer)this).Owner is NavigationViewItem navigationViewItem)
		{
			if (navigationViewItem == parentNavigationView.SettingsItem)
			{
				parentNavigationView.OnSettingsInvoked();
			}
			else
			{
				parentNavigationView.OnNavigationViewItemInvoked(navigationViewItem);
			}
		}
	}

	void IExpandCollapseProvider.Collapse()
	{
		NavigationView parentNavigationView = GetParentNavigationView();
		if (parentNavigationView != null && ((UIElementAutomationPeer)this).Owner is NavigationViewItem item)
		{
			parentNavigationView.Collapse(item);
			RaiseExpandCollapseAutomationEvent((ExpandCollapseState)0);
		}
	}

	void IExpandCollapseProvider.Expand()
	{
		NavigationView parentNavigationView = GetParentNavigationView();
		if (parentNavigationView != null && ((UIElementAutomationPeer)this).Owner is NavigationViewItem item)
		{
			parentNavigationView.Expand(item);
			RaiseExpandCollapseAutomationEvent((ExpandCollapseState)1);
		}
	}

	internal void RaiseExpandCollapseAutomationEvent(ExpandCollapseState newState)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (AutomationPeer.ListenerExists((AutomationEvents)13))
		{
			ExpandCollapseState val = (ExpandCollapseState)((int)newState != 1);
			((AutomationPeer)this).RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, (object)val, (object)newState);
		}
	}

	private NavigationView GetParentNavigationView()
	{
		NavigationView result = null;
		if (((UIElementAutomationPeer)this).Owner is NavigationViewItemBase navigationViewItemBase)
		{
			result = navigationViewItemBase.GetNavigationView();
		}
		return result;
	}

	private int GetNavigationViewItemCountInPrimaryList()
	{
		int result = 0;
		NavigationView parentNavigationView = GetParentNavigationView();
		if (parentNavigationView != null)
		{
			result = parentNavigationView.GetNavigationViewItemCountInPrimaryList();
		}
		return result;
	}

	private int GetNavigationViewItemCountInTopNav()
	{
		int result = 0;
		NavigationView parentNavigationView = GetParentNavigationView();
		if (parentNavigationView != null)
		{
			result = parentNavigationView.GetNavigationViewItemCountInTopNav();
		}
		return result;
	}

	private bool IsSettingsItem()
	{
		NavigationView parentNavigationView = GetParentNavigationView();
		if (parentNavigationView != null)
		{
			NavigationViewItem navigationViewItem = ((UIElementAutomationPeer)this).Owner as NavigationViewItem;
			object settingsItem = parentNavigationView.SettingsItem;
			if (navigationViewItem != null && settingsItem != null && (navigationViewItem == settingsItem || ((ContentControl)navigationViewItem).Content == settingsItem))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsOnTopNavigation()
	{
		NavigationViewRepeaterPosition navigationViewRepeaterPosition = GetNavigationViewRepeaterPosition();
		if (navigationViewRepeaterPosition != NavigationViewRepeaterPosition.LeftNav)
		{
			return navigationViewRepeaterPosition != NavigationViewRepeaterPosition.LeftFooter;
		}
		return false;
	}

	internal bool IsOnTopNavigationOverflow()
	{
		return GetNavigationViewRepeaterPosition() == NavigationViewRepeaterPosition.TopOverflow;
	}

	private bool IsOnFooterNavigation()
	{
		NavigationViewRepeaterPosition navigationViewRepeaterPosition = GetNavigationViewRepeaterPosition();
		if (navigationViewRepeaterPosition != NavigationViewRepeaterPosition.LeftFooter)
		{
			return navigationViewRepeaterPosition == NavigationViewRepeaterPosition.TopFooter;
		}
		return true;
	}

	private NavigationViewRepeaterPosition GetNavigationViewRepeaterPosition()
	{
		if (((UIElementAutomationPeer)this).Owner is NavigationViewItemBase navigationViewItemBase)
		{
			return navigationViewItemBase.Position;
		}
		return NavigationViewRepeaterPosition.LeftNav;
	}

	private ItemsRepeater GetParentItemsRepeater()
	{
		NavigationView parentNavigationView = GetParentNavigationView();
		if (parentNavigationView != null && ((UIElementAutomationPeer)this).Owner is NavigationViewItemBase nvib)
		{
			return parentNavigationView.GetParentItemsRepeaterForContainer(nvib);
		}
		return null;
	}

	private int GetPositionOrSetCountInLeftNavHelper(AutomationOutput automationOutput)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		ItemsRepeater parentItemsRepeater = GetParentItemsRepeater();
		if (parentItemsRepeater != null)
		{
			AutomationPeer val = UIElementAutomationPeer.CreatePeerForElement((UIElement)(object)parentItemsRepeater);
			if (val != null)
			{
				List<AutomationPeer> children = val.GetChildren();
				if (children != null)
				{
					int num2 = 0;
					bool flag = false;
					foreach (AutomationPeer item in children)
					{
						_ = item;
						UIElement val2 = parentItemsRepeater.TryGetElement(num2);
						if (val2 != null)
						{
							if (val2 is NavigationViewItemHeader)
							{
								if (automationOutput == AutomationOutput.Size && flag)
								{
									break;
								}
								num = 0;
							}
							else if (val2 is NavigationViewItem navigationViewItem && (int)((UIElement)navigationViewItem).Visibility == 0)
							{
								num++;
								if ((object)UIElementAutomationPeer.FromElement((UIElement)(object)navigationViewItem) == this)
								{
									if (automationOutput == AutomationOutput.Position)
									{
										break;
									}
									flag = true;
								}
							}
						}
						num2++;
					}
				}
			}
		}
		return num;
	}

	private int GetPositionOrSetCountInTopNavHelper(AutomationOutput automationOutput)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		bool flag = false;
		ItemsRepeater parentItemsRepeater = GetParentItemsRepeater();
		if (parentItemsRepeater != null)
		{
			ItemsSourceView itemsSourceView = parentItemsRepeater.ItemsSourceView;
			if (itemsSourceView != null)
			{
				int count = itemsSourceView.Count;
				for (int i = 0; i < count; i++)
				{
					UIElement val = parentItemsRepeater.TryGetElement(i);
					if (val == null)
					{
						continue;
					}
					if (val is NavigationViewItemHeader)
					{
						if (automationOutput == AutomationOutput.Size && flag)
						{
							break;
						}
						num = 0;
					}
					else
					{
						if (!(val is NavigationViewItem navigationViewItem) || (int)((UIElement)navigationViewItem).Visibility != 0)
						{
							continue;
						}
						num++;
						if ((object)UIElementAutomationPeer.FromElement((UIElement)(object)navigationViewItem) == this)
						{
							if (automationOutput == AutomationOutput.Position)
							{
								break;
							}
							flag = true;
						}
					}
				}
			}
		}
		return num;
	}

	void ISelectionItemProvider.AddToSelection()
	{
		ChangeSelection(isSelected: true);
	}

	void ISelectionItemProvider.Select()
	{
		ChangeSelection(isSelected: true);
	}

	void ISelectionItemProvider.RemoveFromSelection()
	{
		ChangeSelection(isSelected: false);
	}

	private void ChangeSelection(bool isSelected)
	{
		if (((UIElementAutomationPeer)this).Owner is NavigationViewItem navigationViewItem)
		{
			navigationViewItem.IsSelected = isSelected;
		}
	}

	private bool HasChildren()
	{
		if (((UIElementAutomationPeer)this).Owner is NavigationViewItem navigationViewItem)
		{
			return navigationViewItem.HasChildren();
		}
		return false;
	}
}
