using System;
using ModernWpf.Media.Animation;

namespace ModernWpf.Controls;

public sealed class NavigationViewItemInvokedEventArgs : EventArgs
{
	public object InvokedItem { get; internal set; }

	public bool IsSettingsInvoked { get; internal set; }

	public NavigationViewItemBase InvokedItemContainer { get; internal set; }

	public NavigationTransitionInfo RecommendedNavigationTransitionInfo { get; internal set; }
}
