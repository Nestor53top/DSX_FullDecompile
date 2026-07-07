using System.Collections.Specialized;
using System.Windows;

namespace ModernWpf.Controls;

internal class AnimationManager
{
	private readonly ItemsRepeater m_owner;

	private ElementAnimator m_animator;

	private bool m_hasRecordedAdds;

	private bool m_hasRecordedRemoves;

	private bool m_hasRecordedResets;

	private bool m_hasRecordedLayoutTransitions;

	public AnimationManager(ItemsRepeater owner)
	{
		m_owner = owner;
	}

	public void OnAnimatorChanged(ElementAnimator newAnimator)
	{
		if (m_animator != null)
		{
			m_animator.HideAnimationCompleted -= OnHideAnimationCompleted;
		}
		m_animator = newAnimator;
		if (newAnimator != null)
		{
			newAnimator.HideAnimationCompleted += OnHideAnimationCompleted;
		}
	}

	public void OnLayoutChanging()
	{
		m_hasRecordedLayoutTransitions = true;
	}

	public void OnItemsSourceChanged(object source, NotifyCollectionChangedEventArgs args)
	{
		switch (args.Action)
		{
		case NotifyCollectionChangedAction.Add:
			m_hasRecordedAdds = true;
			break;
		case NotifyCollectionChangedAction.Remove:
			m_hasRecordedRemoves = true;
			break;
		case NotifyCollectionChangedAction.Replace:
			m_hasRecordedAdds = true;
			m_hasRecordedRemoves = true;
			break;
		case NotifyCollectionChangedAction.Reset:
			m_hasRecordedResets = true;
			break;
		case NotifyCollectionChangedAction.Move:
			break;
		}
	}

	public void OnElementPrepared(UIElement element)
	{
		if (m_animator != null)
		{
			AnimationContext animationContext = AnimationContext.None;
			if (m_hasRecordedAdds)
			{
				animationContext |= AnimationContext.CollectionChangeAdd;
			}
			if (m_hasRecordedResets)
			{
				animationContext |= AnimationContext.CollectionChangeReset;
			}
			if (m_hasRecordedLayoutTransitions)
			{
				animationContext |= AnimationContext.LayoutTransition;
			}
			if (animationContext != AnimationContext.None)
			{
				m_animator.OnElementShown(element, animationContext);
			}
		}
	}

	public bool ClearElement(UIElement element)
	{
		bool flag = false;
		if (m_animator != null)
		{
			AnimationContext animationContext = AnimationContext.None;
			if (m_hasRecordedRemoves)
			{
				animationContext |= AnimationContext.CollectionChangeRemove;
			}
			if (m_hasRecordedResets)
			{
				animationContext |= AnimationContext.CollectionChangeReset;
			}
			flag = animationContext != AnimationContext.None && m_animator.HasHideAnimation(element, animationContext);
			if (flag)
			{
				m_animator.OnElementHidden(element, animationContext);
			}
		}
		return flag;
	}

	public void OnElementBoundsChanged(UIElement element, Rect oldBounds, Rect newBounds)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (m_animator != null)
		{
			AnimationContext animationContext = AnimationContext.None;
			if (m_hasRecordedAdds)
			{
				animationContext |= AnimationContext.CollectionChangeAdd;
			}
			if (m_hasRecordedRemoves)
			{
				animationContext |= AnimationContext.CollectionChangeRemove;
			}
			if (m_hasRecordedResets)
			{
				animationContext |= AnimationContext.CollectionChangeReset;
			}
			if (m_hasRecordedLayoutTransitions)
			{
				animationContext |= AnimationContext.LayoutTransition;
			}
			m_animator.OnElementBoundsChanged(element, animationContext, oldBounds, newBounds);
		}
	}

	public void OnOwnerArranged()
	{
		m_hasRecordedAdds = false;
		m_hasRecordedRemoves = false;
		m_hasRecordedResets = false;
		m_hasRecordedLayoutTransitions = false;
	}

	private void OnHideAnimationCompleted(ElementAnimator sender, UIElement element)
	{
		if ((object)CachedVisualTreeHelpers.GetParent((DependencyObject)(object)element) == m_owner)
		{
			m_owner.ViewManager.ClearElementToElementFactory(element);
			((UIElement)m_owner).InvalidateArrange();
		}
	}
}
