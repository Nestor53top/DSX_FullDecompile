using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace ModernWpf.Controls;

[ContentProperty("ScrollViewer")]
public class ItemsRepeaterScrollHost : Panel, IRepeaterScrollingSurface
{
	private class CandidateInfo
	{
		public static Rect InvalidBounds = Rect.Empty;

		public UIElement Element { get; }

		public Rect RelativeBounds { get; set; }

		public bool IsRelativeBoundsSet => RelativeBounds != InvalidBounds;

		public CandidateInfo(UIElement element)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RelativeBounds = InvalidBounds;
			Element = element;
		}
	}

	private class BringIntoViewState
	{
		public UIElement TargetElement { get; private set; }

		public double AlignmentX { get; private set; }

		public double AlignmentY { get; private set; }

		public double OffsetX { get; private set; }

		public double OffsetY { get; private set; }

		public bool Animate { get; }

		public bool ChangeViewCalled { get; set; }

		public Point ChangeViewOffset { get; set; }

		public BringIntoViewState(UIElement owner)
		{
			TargetElement = owner;
		}

		public BringIntoViewState(UIElement targetElement, double alignmentX, double alignmentY, double offsetX, double offsetY, bool animate)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			TargetElement = targetElement;
			AlignmentX = alignmentX;
			AlignmentY = alignmentY;
			OffsetX = offsetX;
			OffsetY = offsetY;
			Animate = animate;
			ChangeViewCalled = false;
			ChangeViewOffset = default(Point);
		}

		public void Reset()
		{
			TargetElement = null;
			double num = (OffsetY = 0.0);
			double num3 = (OffsetX = num);
			double alignmentX = (AlignmentY = num3);
			AlignmentX = alignmentX;
		}
	}

	private readonly List<CandidateInfo> m_candidates = new List<CandidateInfo>();

	private UIElement m_anchorElement;

	private Rect m_anchorElementRelativeBounds;

	private bool m_isAnchorElementDirty = true;

	private double m_horizontalEdge;

	private double m_verticalEdge;

	private BringIntoViewState m_pendingBringIntoView;

	private double m_pendingViewportShift;

	public double HorizontalAnchorRatio
	{
		get
		{
			return m_horizontalEdge;
		}
		set
		{
			m_horizontalEdge = value;
		}
	}

	public double VerticalAnchorRatio
	{
		get
		{
			return m_verticalEdge;
		}
		set
		{
			m_verticalEdge = value;
		}
	}

	public UIElement CurrentAnchor
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Rect relativeBounds = Rect.Empty;
			return GetAnchorElement(ref relativeBounds);
		}
	}

	public ScrollViewer ScrollViewer
	{
		get
		{
			ScrollViewer result = null;
			UIElementCollection children = ((Panel)this).Children;
			if (children.Count > 0)
			{
				UIElement obj = children[0];
				result = (ScrollViewer)(object)((obj is ScrollViewer) ? obj : null);
			}
			return result;
		}
		set
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			ScrollViewer scrollViewer = ScrollViewer;
			if (scrollViewer != null)
			{
				scrollViewer.ScrollChanged -= new ScrollChangedEventHandler(OnScrollViewerScrollChanged);
				((FrameworkElement)scrollViewer).SizeChanged -= new SizeChangedEventHandler(OnScrollViewerSizeChanged);
			}
			UIElementCollection children = ((Panel)this).Children;
			children.Clear();
			children.Add((UIElement)(object)value);
			value.ScrollChanged += new ScrollChangedEventHandler(OnScrollViewerScrollChanged);
			((FrameworkElement)value).SizeChanged += new SizeChangedEventHandler(OnScrollViewerSizeChanged);
		}
	}

	bool IRepeaterScrollingSurface.IsHorizontallyScrollable => true;

	bool IRepeaterScrollingSurface.IsVerticallyScrollable => true;

	UIElement IRepeaterScrollingSurface.AnchorElement
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Rect relativeBounds = Rect.Empty;
			return GetAnchorElement(ref relativeBounds);
		}
	}

	private bool HasPendingBringIntoView => m_pendingBringIntoView.TargetElement != null;

	event ViewportChangedEventHandler IRepeaterScrollingSurface.ViewportChanged
	{
		add
		{
			ViewportChanged += value;
		}
		remove
		{
			ViewportChanged -= value;
		}
	}

	event PostArrangeEventHandler IRepeaterScrollingSurface.PostArrange
	{
		add
		{
			PostArrange += value;
		}
		remove
		{
			PostArrange -= value;
		}
	}

	event ConfigurationChangedEventHandler IRepeaterScrollingSurface.ConfigurationChanged
	{
		add
		{
			ConfigurationChanged += value;
		}
		remove
		{
			ConfigurationChanged -= value;
		}
	}

	private event ViewportChangedEventHandler ViewportChanged;

	private event PostArrangeEventHandler PostArrange;

	private event ConfigurationChangedEventHandler ConfigurationChanged;

	public ItemsRepeaterScrollHost()
	{
		m_pendingBringIntoView = new BringIntoViewState((UIElement)(object)this);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Size result = default(Size);
		ScrollViewer scrollViewer = ScrollViewer;
		if (scrollViewer != null)
		{
			((UIElement)scrollViewer).Measure(availableSize);
			return ((UIElement)scrollViewer).DesiredSize;
		}
		return result;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Size result = finalSize;
		ScrollViewer scrollViewer = ScrollViewer;
		if (scrollViewer != null)
		{
			bool num = scrollViewer != null && HasPendingBringIntoView && !m_pendingBringIntoView.ChangeViewCalled;
			Rect relativeBounds = default(Rect);
			UIElement val = (num ? null : GetAnchorElement(ref relativeBounds));
			((UIElement)scrollViewer).Arrange(new Rect(0.0, 0.0, ((Size)(ref finalSize)).Width, ((Size)(ref finalSize)).Height));
			m_pendingViewportShift = 0.0;
			if (num)
			{
				ApplyPendingChangeView(scrollViewer);
			}
			else if (val != null)
			{
				m_pendingViewportShift = TrackElement(val, relativeBounds, scrollViewer);
			}
			else if (scrollViewer == null)
			{
				m_pendingBringIntoView.Reset();
			}
			m_candidates.Clear();
			m_isAnchorElementDirty = true;
			PostArrangeEventHandler postArrangeEventHandler = this.PostArrange;
			if (postArrangeEventHandler == null)
			{
				return result;
			}
			postArrangeEventHandler(this);
		}
		return result;
	}

	void IRepeaterScrollingSurface.RegisterAnchorCandidate(UIElement element)
	{
		if ((!double.IsNaN(HorizontalAnchorRatio) || !double.IsNaN(VerticalAnchorRatio)) && ScrollViewer != null)
		{
			m_candidates.Add(new CandidateInfo(element));
			m_isAnchorElementDirty = true;
		}
	}

	void IRepeaterScrollingSurface.UnregisterAnchorCandidate(UIElement element)
	{
		int num = m_candidates.FindIndex((CandidateInfo c) => c.Element == element);
		if (num >= 0)
		{
			m_candidates.RemoveAt(num);
			m_isAnchorElementDirty = true;
		}
	}

	Rect IRepeaterScrollingSurface.GetRelativeViewport(UIElement element)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		ScrollViewer scrollViewer = ScrollViewer;
		if (scrollViewer != null)
		{
			bool hasPendingBringIntoView = HasPendingBringIntoView;
			GeneralTransform obj = ((Visual)(object)element).SafeTransformToVisual((Visual)(hasPendingBringIntoView ? ((object)scrollViewer.GetContentTemplateRoot()) : ((object)scrollViewer)));
			double num = 1.0;
			double num2 = scrollViewer.ViewportWidth / num;
			double num3 = scrollViewer.ViewportHeight / num;
			Point val = default(Point);
			Point val2 = obj.Transform(val);
			((Point)(ref val2)).X = 0.0 - ((Point)(ref val2)).X;
			((Point)(ref val2)).Y = 0.0 - ((Point)(ref val2)).Y + m_pendingViewportShift;
			if (hasPendingBringIntoView)
			{
				double x = ((Point)(ref val2)).X;
				val = m_pendingBringIntoView.ChangeViewOffset;
				((Point)(ref val2)).X = x + ((Point)(ref val)).X;
				double y = ((Point)(ref val2)).Y;
				val = m_pendingBringIntoView.ChangeViewOffset;
				((Point)(ref val2)).Y = y + ((Point)(ref val)).Y;
			}
			return new Rect(((Point)(ref val2)).X, ((Point)(ref val2)).Y, num2, num3);
		}
		return default(Rect);
	}

	internal void StartBringIntoView(UIElement element, double alignmentX, double alignmentY, double offsetX, double offsetY, bool animate)
	{
		m_pendingBringIntoView = new BringIntoViewState(element, alignmentX, alignmentY, offsetX, offsetY, animate);
	}

	private void ApplyPendingChangeView(ScrollViewer scrollViewer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		BringIntoViewState pendingBringIntoView = m_pendingBringIntoView;
		pendingBringIntoView.ChangeViewCalled = true;
		Rect layoutSlot = CachedVisualTreeHelpers.GetLayoutSlot((FrameworkElement)pendingBringIntoView.TargetElement);
		Rect val = ((Visual)(object)pendingBringIntoView.TargetElement).SafeTransformToVisual((Visual)(object)scrollViewer.GetContentTemplateRoot()).TransformBounds(new Rect(0.0, 0.0, ((Rect)(ref layoutSlot)).Width, ((Rect)(ref layoutSlot)).Height));
		Point val2 = default(Point);
		((Point)(ref val2))._002Ector(scrollViewer.ViewportWidth - ((Rect)(ref val)).Width, scrollViewer.ViewportHeight - ((Rect)(ref val)).Height);
		Point changeViewOffset = default(Point);
		((Point)(ref changeViewOffset))._002Ector(Math.Max(0.0, Math.Min(((Rect)(ref val)).X + pendingBringIntoView.OffsetX - ((Point)(ref val2)).X * pendingBringIntoView.AlignmentX, scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)), Math.Max(0.0, Math.Min(((Rect)(ref val)).Y + pendingBringIntoView.OffsetY - ((Point)(ref val2)).Y * pendingBringIntoView.AlignmentY, scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)));
		pendingBringIntoView.ChangeViewOffset = changeViewOffset;
		scrollViewer.ChangeView(((Point)(ref changeViewOffset)).X, ((Point)(ref changeViewOffset)).Y, null, !pendingBringIntoView.Animate);
	}

	private double TrackElement(UIElement element, Rect previousBounds, ScrollViewer scrollViewer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Rect layoutSlot = LayoutInformation.GetLayoutSlot((FrameworkElement)element);
		Rect val = ((Visual)(object)element).SafeTransformToVisual((Visual)(object)scrollViewer.GetContentTemplateRoot()).TransformBounds(new Rect(0.0, 0.0, ((Rect)(ref layoutSlot)).Width, ((Rect)(ref layoutSlot)).Height));
		double num = ((Rect)(ref previousBounds)).Y + HorizontalAnchorRatio * ((Rect)(ref previousBounds)).Height;
		double num2 = ((Rect)(ref val)).Y + HorizontalAnchorRatio * ((Rect)(ref val)).Height - num;
		double num3 = num2;
		double num4;
		if (!HasPendingBringIntoView || m_pendingBringIntoView.Animate)
		{
			num4 = scrollViewer.VerticalOffset;
		}
		else
		{
			Point changeViewOffset = m_pendingBringIntoView.ChangeViewOffset;
			num4 = ((Point)(ref changeViewOffset)).Y;
		}
		double num5 = num4;
		if (num5 + num3 < 0.0)
		{
			num3 = 0.0 - num5;
		}
		else if (num5 + scrollViewer.ViewportHeight + num3 > scrollViewer.ExtentHeight)
		{
			num3 = scrollViewer.ExtentHeight - scrollViewer.ViewportHeight - num5;
		}
		if (scrollViewer.ScrollableHeight == 0.0)
		{
			num3 = 0.0;
		}
		if (Math.Abs(num3) > 1.0)
		{
			scrollViewer.ChangeView(null, num5 + num3, null, disableAnimation: true);
		}
		else
		{
			num3 = 0.0;
			if (Math.Abs(num2) > 1.0)
			{
				this.ViewportChanged?.Invoke(this, isFinal: true);
			}
		}
		return num3;
	}

	private UIElement GetAnchorElement(ref Rect relativeBounds)
	{
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		if (m_isAnchorElementDirty)
		{
			ScrollViewer scrollViewer = ScrollViewer;
			if (scrollViewer != null)
			{
				double num;
				if (!HasPendingBringIntoView || m_pendingBringIntoView.Animate)
				{
					num = scrollViewer.VerticalOffset;
				}
				else
				{
					Point changeViewOffset = m_pendingBringIntoView.ChangeViewOffset;
					num = ((Point)(ref changeViewOffset)).Y;
				}
				double num2 = num + HorizontalAnchorRatio * scrollViewer.ViewportHeight + m_pendingViewportShift;
				CandidateInfo candidateInfo = null;
				double num3 = 3.4028234663852886E+38;
				foreach (CandidateInfo candidate in m_candidates)
				{
					UIElement element = candidate.Element;
					if (!candidate.IsRelativeBoundsSet)
					{
						Rect layoutSlot = LayoutInformation.GetLayoutSlot((FrameworkElement)element);
						GeneralTransform val = ((Visual)(object)element).SafeTransformToVisual((Visual)(object)scrollViewer.GetContentTemplateRoot());
						candidate.RelativeBounds = val.TransformBounds(new Rect(0.0, 0.0, ((Rect)(ref layoutSlot)).Width, ((Rect)(ref layoutSlot)).Height));
					}
					Rect relativeBounds2 = candidate.RelativeBounds;
					double y = ((Rect)(ref relativeBounds2)).Y;
					double horizontalAnchorRatio = HorizontalAnchorRatio;
					relativeBounds2 = candidate.RelativeBounds;
					double num4 = Math.Abs(y + horizontalAnchorRatio * ((Rect)(ref relativeBounds2)).Height - num2);
					if (num4 < num3)
					{
						candidateInfo = candidate;
						num3 = num4;
					}
				}
				if (candidateInfo != null)
				{
					m_anchorElement = candidateInfo.Element;
					m_anchorElementRelativeBounds = candidateInfo.RelativeBounds;
				}
				else
				{
					m_anchorElement = null;
					m_anchorElementRelativeBounds = CandidateInfo.InvalidBounds;
				}
			}
			m_isAnchorElementDirty = false;
		}
		if (relativeBounds != Rect.Empty)
		{
			relativeBounds = m_anchorElementRelativeBounds;
		}
		return m_anchorElement;
	}

	private void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		if (e.ViewportWidthChange != 0.0 || e.ViewportHeightChange != 0.0)
		{
			((UIElement)this).InvalidateArrange();
		}
		if (e.HorizontalChange != 0.0 || e.VerticalChange != 0.0)
		{
			m_pendingViewportShift = 0.0;
			if (HasPendingBringIntoView && m_pendingBringIntoView.ChangeViewCalled)
			{
				m_pendingBringIntoView.Reset();
			}
			this.ViewportChanged?.Invoke(this, isFinal: true);
		}
	}

	private void OnScrollViewerSizeChanged(object sender, SizeChangedEventArgs args)
	{
		this.ViewportChanged?.Invoke(this, isFinal: true);
	}
}
