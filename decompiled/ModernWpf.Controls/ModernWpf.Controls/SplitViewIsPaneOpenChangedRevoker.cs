namespace ModernWpf.Controls;

internal class SplitViewIsPaneOpenChangedRevoker : EventRevoker<SplitView, DependencyPropertyChangedCallback>
{
	public SplitViewIsPaneOpenChangedRevoker(SplitView source, DependencyPropertyChangedCallback handler)
		: base(source, handler)
	{
	}

	protected override void AddHandler(SplitView source, DependencyPropertyChangedCallback handler)
	{
		source.IsPaneOpenChanged += handler;
	}

	protected override void RemoveHandler(SplitView source, DependencyPropertyChangedCallback handler)
	{
		source.IsPaneOpenChanged -= handler;
	}
}
