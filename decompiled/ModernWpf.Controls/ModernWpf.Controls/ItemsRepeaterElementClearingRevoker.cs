namespace ModernWpf.Controls;

internal class ItemsRepeaterElementClearingRevoker : EventRevoker<ItemsRepeater, TypedEventHandler<ItemsRepeater, ItemsRepeaterElementClearingEventArgs>>
{
	public ItemsRepeaterElementClearingRevoker(ItemsRepeater source, TypedEventHandler<ItemsRepeater, ItemsRepeaterElementClearingEventArgs> handler)
		: base(source, handler)
	{
	}

	protected override void AddHandler(ItemsRepeater source, TypedEventHandler<ItemsRepeater, ItemsRepeaterElementClearingEventArgs> handler)
	{
		source.ElementClearing += handler;
	}

	protected override void RemoveHandler(ItemsRepeater source, TypedEventHandler<ItemsRepeater, ItemsRepeaterElementClearingEventArgs> handler)
	{
		source.ElementClearing -= handler;
	}
}
