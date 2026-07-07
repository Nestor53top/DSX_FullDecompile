namespace ModernWpf.Controls;

internal class SplitViewCompactPaneLengthChangedRevoker : EventRevoker<SplitView, DependencyPropertyChangedCallback>
{
	public SplitViewCompactPaneLengthChangedRevoker(SplitView source, DependencyPropertyChangedCallback handler)
		: base(source, handler)
	{
	}

	protected override void AddHandler(SplitView source, DependencyPropertyChangedCallback handler)
	{
		source.CompactPaneLengthChanged += handler;
	}

	protected override void RemoveHandler(SplitView source, DependencyPropertyChangedCallback handler)
	{
		source.CompactPaneLengthChanged -= handler;
	}
}
