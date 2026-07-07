using System;
using System.Windows;

namespace ModernWpf.Controls;

internal class RepeaterLayoutContext : VirtualizingLayoutContext
{
	private readonly WeakReference<ItemsRepeater> m_owner;

	protected override object LayoutStateCore
	{
		get
		{
			return GetOwner().LayoutState;
		}
		set
		{
			GetOwner().LayoutState = value;
		}
	}

	protected override int RecommendedAnchorIndexCore
	{
		get
		{
			int result = -1;
			ItemsRepeater owner = GetOwner();
			UIElement suggestedAnchor = owner.SuggestedAnchor;
			if (suggestedAnchor != null)
			{
				result = owner.GetElementIndex(suggestedAnchor);
			}
			return result;
		}
	}

	protected override Point LayoutOriginCore
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return GetOwner().LayoutOrigin;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			GetOwner().LayoutOrigin = value;
		}
	}

	public RepeaterLayoutContext(ItemsRepeater owner)
	{
		m_owner = new WeakReference<ItemsRepeater>(owner);
	}

	protected override int ItemCountCore()
	{
		return GetOwner().ItemsSourceView?.Count ?? 0;
	}

	protected override UIElement GetOrCreateElementAtCore(int index, ElementRealizationOptions options)
	{
		return GetOwner().GetElementImpl(index, (options & ElementRealizationOptions.ForceCreate) == ElementRealizationOptions.ForceCreate, (options & ElementRealizationOptions.SuppressAutoRecycle) == ElementRealizationOptions.SuppressAutoRecycle);
	}

	protected override object GetItemAtCore(int index)
	{
		return GetOwner().ItemsSourceView.GetAt(index);
	}

	protected override void RecycleElementCore(UIElement element)
	{
		GetOwner().ClearElementImpl(element);
	}

	protected override Rect RealizationRectCore()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetOwner().RealizationWindow;
	}

	private ItemsRepeater GetOwner()
	{
		m_owner.TryGetTarget(out var target);
		return target;
	}
}
