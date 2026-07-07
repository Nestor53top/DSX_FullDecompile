using System;
using ModernWpf.Media.Animation;

namespace ModernWpf.Controls;

public sealed class NavigationViewSelectionChangedEventArgs : EventArgs
{
	public object SelectedItem { get; internal set; }

	public bool IsSettingsSelected { get; internal set; }

	public NavigationViewItemBase SelectedItemContainer { get; internal set; }

	public NavigationTransitionInfo RecommendedNavigationTransitionInfo { get; internal set; }

	internal NavigationViewSelectionChangedEventArgs()
	{
	}
}
