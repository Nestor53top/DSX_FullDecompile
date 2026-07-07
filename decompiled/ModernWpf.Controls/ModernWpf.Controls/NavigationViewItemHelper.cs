using System.Windows;

namespace ModernWpf.Controls;

internal class NavigationViewItemHelper
{
	internal const string c_OnLeftNavigationReveal = "OnLeftNavigationReveal";

	internal const string c_OnLeftNavigation = "OnLeftNavigation";

	internal const string c_OnTopNavigationPrimary = "OnTopNavigationPrimary";

	internal const string c_OnTopNavigationPrimaryReveal = "OnTopNavigationPrimaryReveal";

	internal const string c_OnTopNavigationOverflow = "OnTopNavigationOverflow";
}
internal class NavigationViewItemHelper<T> : NavigationViewItemHelper
{
	private UIElement m_selectionIndicator;

	private const string c_selectionIndicatorName = "SelectionIndicator";

	public UIElement GetSelectionIndicator()
	{
		return m_selectionIndicator;
	}

	public void Init(IControlProtected controlProtected)
	{
		DependencyObject templateChild = controlProtected.GetTemplateChild("SelectionIndicator");
		m_selectionIndicator = (UIElement)(object)((templateChild is UIElement) ? templateChild : null);
	}
}
