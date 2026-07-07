using System;
using System.Windows;

namespace ModernWpf.Controls;

internal class VirtualizationInfo
{
	private uint m_pinCounter;

	public ElementOwner Owner { get; private set; }

	public int Index { get; private set; } = -1;

	public bool IsPinned => m_pinCounter != 0;

	public bool IsHeldByLayout => Owner == ElementOwner.Layout;

	public bool IsRealized
	{
		get
		{
			if (!IsHeldByLayout)
			{
				return Owner == ElementOwner.PinnedPool;
			}
			return true;
		}
	}

	public bool IsInUniqueIdResetPool => Owner == ElementOwner.UniqueIdResetPool;

	public bool MustClearDataContext { get; set; }

	public Rect ArrangeBounds { get; set; }

	public string UniqueId { get; private set; } = string.Empty;

	public bool KeepAlive { get; set; }

	public bool AutoRecycleCandidate { get; set; }

	public Size DesiredSize { get; set; } = Size.Empty;

	public VirtualizationInfo()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		ArrangeBounds = ItemsRepeater.InvalidRect;
	}

	public void MoveOwnershipToLayoutFromElementFactory(int index, string uniqueId)
	{
		Owner = ElementOwner.Layout;
		Index = index;
		UniqueId = uniqueId;
	}

	public void MoveOwnershipToLayoutFromUniqueIdResetPool()
	{
		Owner = ElementOwner.Layout;
	}

	public void MoveOwnershipToLayoutFromPinnedPool()
	{
		Owner = ElementOwner.Layout;
	}

	public void MoveOwnershipToElementFactory()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Owner = ElementOwner.ElementFactory;
		m_pinCounter = 0u;
		Index = -1;
		UniqueId = string.Empty;
		ArrangeBounds = ItemsRepeater.InvalidRect;
	}

	public void MoveOwnershipToUniqueIdResetPoolFromLayout()
	{
		Owner = ElementOwner.UniqueIdResetPool;
	}

	public void MoveOwnershipToAnimator()
	{
		Owner = ElementOwner.Animator;
		Index = -1;
		m_pinCounter = 0u;
	}

	public void MoveOwnershipToPinnedPool()
	{
		Owner = ElementOwner.PinnedPool;
	}

	public uint AddPin()
	{
		if (!IsRealized)
		{
			throw new InvalidOperationException("You can't pin an unrealized element.");
		}
		return ++m_pinCounter;
	}

	public uint RemovePin()
	{
		if (!IsRealized)
		{
			throw new InvalidOperationException("You can't unpin an unrealized element.");
		}
		if (!IsPinned)
		{
			throw new InvalidOperationException("UnpinElement was called more often than PinElement.");
		}
		return --m_pinCounter;
	}

	public void UpdateIndex(int newIndex)
	{
		Index = newIndex;
	}
}
