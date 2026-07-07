namespace ModernWpf.Controls;

internal class SplitViewDisplayModeChangedRevoker : EventRevoker<SplitView, DependencyPropertyChangedCallback>
{
	public SplitViewDisplayModeChangedRevoker(SplitView source, DependencyPropertyChangedCallback handler)
		: base(source, handler)
	{
	}

	protected override void AddHandler(SplitView source, DependencyPropertyChangedCallback handler)
	{
		source.DisplayModeChanged += handler;
	}

	protected override void RemoveHandler(SplitView source, DependencyPropertyChangedCallback handler)
	{
		source.DisplayModeChanged -= handler;
	}
}
