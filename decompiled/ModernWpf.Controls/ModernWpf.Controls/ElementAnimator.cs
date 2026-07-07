using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ModernWpf.Controls;

public class ElementAnimator
{
	private enum AnimationTrigger
	{
		Show,
		Hide,
		BoundsChange
	}

	private struct ElementInfo
	{
		public UIElement Element { get; }

		public AnimationTrigger Trigger { get; }

		public AnimationContext Context { get; }

		public Rect OldBounds { get; }

		public Rect NewBounds { get; }

		public ElementInfo(UIElement element, AnimationTrigger trigger, AnimationContext context)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			Element = element;
			Trigger = trigger;
			Context = context;
			OldBounds = default(Rect);
			NewBounds = default(Rect);
		}

		public ElementInfo(UIElement element, AnimationTrigger trigger, AnimationContext context, Rect oldBounds, Rect newBounds)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Element = element;
			Trigger = trigger;
			Context = context;
			OldBounds = oldBounds;
			NewBounds = newBounds;
		}
	}

	private readonly List<ElementInfo> m_animatingElements = new List<ElementInfo>();

	protected bool HasShowAnimationsPending { get; private set; }

	protected bool HasHideAnimationsPending { get; private set; }

	protected bool HasBoundsChangeAnimationsPending { get; private set; }

	protected AnimationContext SharedContext { get; private set; }

	public event ElementAnimationCompleted ShowAnimationCompleted;

	public event ElementAnimationCompleted HideAnimationCompleted;

	public event ElementAnimationCompleted BoundsChangeAnimationCompleted;

	public void OnElementShown(UIElement element, AnimationContext context)
	{
		if (HasShowAnimation(element, context))
		{
			HasShowAnimationsPending = true;
			SharedContext |= context;
			QueueElementForAnimation(new ElementInfo(element, AnimationTrigger.Show, context));
		}
	}

	public void OnElementHidden(UIElement element, AnimationContext context)
	{
		if (HasHideAnimation(element, context))
		{
			HasHideAnimationsPending = true;
			SharedContext |= context;
			QueueElementForAnimation(new ElementInfo(element, AnimationTrigger.Hide, context));
		}
	}

	public void OnElementBoundsChanged(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (HasBoundsChangeAnimation(element, context, oldBounds, newBounds))
		{
			HasBoundsChangeAnimationsPending = true;
			SharedContext |= context;
			QueueElementForAnimation(new ElementInfo(element, AnimationTrigger.BoundsChange, context, oldBounds, newBounds));
		}
	}

	public bool HasShowAnimation(UIElement element, AnimationContext context)
	{
		return HasShowAnimationCore(element, context);
	}

	public bool HasHideAnimation(UIElement element, AnimationContext context)
	{
		return HasHideAnimationCore(element, context);
	}

	public bool HasBoundsChangeAnimation(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return HasBoundsChangeAnimationCore(element, context, oldBounds, newBounds);
	}

	protected virtual bool HasShowAnimationCore(UIElement element, AnimationContext context)
	{
		throw new NotImplementedException();
	}

	protected virtual bool HasHideAnimationCore(UIElement element, AnimationContext context)
	{
		throw new NotImplementedException();
	}

	protected virtual bool HasBoundsChangeAnimationCore(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
	{
		throw new NotImplementedException();
	}

	protected virtual void StartShowAnimation(UIElement element, AnimationContext context)
	{
		throw new NotImplementedException();
	}

	protected virtual void StartHideAnimation(UIElement element, AnimationContext context)
	{
		throw new NotImplementedException();
	}

	protected virtual void StartBoundsChangeAnimation(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
	{
		throw new NotImplementedException();
	}

	protected void OnShowAnimationCompleted(UIElement element)
	{
		this.ShowAnimationCompleted?.Invoke(this, element);
	}

	protected void OnHideAnimationCompleted(UIElement element)
	{
		this.HideAnimationCompleted?.Invoke(this, element);
	}

	protected void OnBoundsChangeAnimationCompleted(UIElement element)
	{
		this.BoundsChangeAnimationCompleted?.Invoke(this, element);
	}

	private void QueueElementForAnimation(ElementInfo elementInfo)
	{
		m_animatingElements.Add(elementInfo);
		if (m_animatingElements.Count == 1)
		{
			CompositionTarget.Rendering += OnRendering;
		}
	}

	private void OnRendering(object sender, EventArgs args)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		CompositionTarget.Rendering -= OnRendering;
		try
		{
			for (int i = 0; i < m_animatingElements.Count; i++)
			{
				ElementInfo elementInfo = m_animatingElements[i];
				switch (elementInfo.Trigger)
				{
				case AnimationTrigger.Show:
					StartShowAnimation(elementInfo.Element, elementInfo.Context);
					break;
				case AnimationTrigger.Hide:
					StartHideAnimation(elementInfo.Element, elementInfo.Context);
					break;
				case AnimationTrigger.BoundsChange:
					StartBoundsChangeAnimation(elementInfo.Element, elementInfo.Context, elementInfo.OldBounds, elementInfo.NewBounds);
					break;
				}
			}
		}
		finally
		{
			ResetState();
		}
	}

	private void ResetState()
	{
		m_animatingElements.Clear();
		bool flag = (HasBoundsChangeAnimationsPending = false);
		bool hasShowAnimationsPending = (HasHideAnimationsPending = flag);
		HasShowAnimationsPending = hasShowAnimationsPending;
		SharedContext = AnimationContext.None;
	}
}
