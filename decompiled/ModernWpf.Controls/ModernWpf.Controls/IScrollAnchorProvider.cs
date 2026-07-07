using System.Windows;

namespace ModernWpf.Controls;

public interface IScrollAnchorProvider
{
	UIElement CurrentAnchor { get; }

	void RegisterAnchorCandidate(UIElement element);

	void UnregisterAnchorCandidate(UIElement element);
}
