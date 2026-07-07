using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ModernWpf.Controls;

internal class ViewportManagerDownLevel : ViewportManager
{
	private struct ScrollerInfo(IRepeaterScrollingSurface scroller)
	{
		public IRepeaterScrollingSurface Scroller { get; } = scroller;
	}

	private const double CacheBufferPerSideInflationPixelDelta = 40.0;

	private readonly ItemsRepeater m_owner;

	private bool m_ensuredScrollers;

	private readonly List<ScrollerInfo> m_parentScrollers = new List<ScrollerInfo>();

	private IRepeaterScrollingSurface m_horizontalScroller;

	private IRepeaterScrollingSurface m_verticalScroller;

	private IRepeaterScrollingSurface m_innerScrollableScroller;

	private UIElement m_makeAnchorElement;

	private bool m_isAnchorOutsideRealizedRange;

	private DispatcherOperation m_cacheBuildAction;

	private Rect m_visibleWindow;

	private Rect m_layoutExtent;

	private Point m_expectedViewportShift;

	private double m_maximumHorizontalCacheLength = 2.0;

	private double m_maximumVerticalCacheLength = 2.0;

	private double m_horizontalCacheBufferPerSide;

	private double m_verticalCacheBufferPerSide;

	private bool m_managingViewportDisabled;

	public override UIElement SuggestedAnchor
	{
		get
		{
			UIElement val = m_makeAnchorElement;
			UIElement owner = (UIElement)(object)m_owner;
			if (val == null)
			{
				UIElement val2 = m_innerScrollableScroller?.AnchorElement;
				if (val2 != null)
				{
					UIElement val3 = val2;
					DependencyObject parent = CachedVisualTreeHelpers.GetParent((DependencyObject)(object)val3);
					UIElement val4 = (UIElement)(object)((parent is UIElement) ? parent : null);
					while (val4 != null)
					{
						if (val4 == owner)
						{
							val = val3;
							break;
						}
						val3 = val4;
						DependencyObject parent2 = CachedVisualTreeHelpers.GetParent((DependencyObject)(object)val3);
						val4 = (UIElement)(object)((parent2 is UIElement) ? parent2 : null);
					}
				}
			}
			return val;
		}
	}

	public override double HorizontalCacheLength
	{
		get
		{
			return m_maximumHorizontalCacheLength;
		}
		set
		{
			if (m_maximumHorizontalCacheLength != value)
			{
				ValidateCacheLength(value);
				m_maximumHorizontalCacheLength = value;
				ResetCacheBuffer();
			}
		}
	}

	public override double VerticalCacheLength
	{
		get
		{
			return m_maximumVerticalCacheLength;
		}
		set
		{
			if (m_maximumVerticalCacheLength != value)
			{
				ValidateCacheLength(value);
				m_maximumVerticalCacheLength = value;
				ResetCacheBuffer();
			}
		}
	}

	public override UIElement MadeAnchor => m_makeAnchorElement;

	private bool HasScrollers
	{
		get
		{
			if (m_horizontalScroller == null)
			{
				return m_verticalScroller != null;
			}
			return true;
		}
	}

	public ViewportManagerDownLevel(ItemsRepeater owner)
	{
		m_owner = owner;
	}

	public override Rect GetLayoutVisibleWindow()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		Rect visibleWindow = m_visibleWindow;
		if (m_makeAnchorElement != null && m_isAnchorOutsideRealizedRange)
		{
			((Rect)(ref visibleWindow)).X = 0.0;
			((Rect)(ref visibleWindow)).Y = 0.0;
		}
		else if (HasScrollers)
		{
			((Rect)(ref visibleWindow)).X = ((Rect)(ref visibleWindow)).X + (((Rect)(ref m_layoutExtent)).X + ((Point)(ref m_expectedViewportShift)).X);
			((Rect)(ref visibleWindow)).Y = ((Rect)(ref visibleWindow)).Y + (((Rect)(ref m_layoutExtent)).Y + ((Point)(ref m_expectedViewportShift)).Y);
		}
		return visibleWindow;
	}

	public override Rect GetLayoutRealizationWindow()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Rect layoutVisibleWindow = GetLayoutVisibleWindow();
		if (HasScrollers)
		{
			((Rect)(ref layoutVisibleWindow)).X = ((Rect)(ref layoutVisibleWindow)).X - m_horizontalCacheBufferPerSide;
			((Rect)(ref layoutVisibleWindow)).Y = ((Rect)(ref layoutVisibleWindow)).Y - m_verticalCacheBufferPerSide;
			((Rect)(ref layoutVisibleWindow)).Width = ((Rect)(ref layoutVisibleWindow)).Width + m_horizontalCacheBufferPerSide * 2.0;
			((Rect)(ref layoutVisibleWindow)).Height = ((Rect)(ref layoutVisibleWindow)).Height + m_verticalCacheBufferPerSide * 2.0;
		}
		return layoutVisibleWindow;
	}

	public override void SetLayoutExtent(Rect extent)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		ref Point expectedViewportShift = ref m_expectedViewportShift;
		((Point)(ref expectedViewportShift)).X = ((Point)(ref expectedViewportShift)).X + (((Rect)(ref m_layoutExtent)).X - ((Rect)(ref extent)).X);
		ref Point expectedViewportShift2 = ref m_expectedViewportShift;
		((Point)(ref expectedViewportShift2)).Y = ((Point)(ref expectedViewportShift2)).Y + (((Rect)(ref m_layoutExtent)).Y - ((Rect)(ref extent)).Y);
		m_layoutExtent = extent;
		IRepeaterScrollingSurface outerScroller = GetOuterScroller();
		if (outerScroller != null)
		{
			((UIElement)outerScroller).InvalidateArrange();
		}
		if (m_horizontalScroller != null && m_horizontalScroller != outerScroller)
		{
			((UIElement)m_horizontalScroller).InvalidateArrange();
		}
		if (m_verticalScroller != null && m_verticalScroller != outerScroller)
		{
			((UIElement)m_verticalScroller).InvalidateArrange();
		}
	}

	public override Point GetOrigin()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Point(((Rect)(ref m_layoutExtent)).X, ((Rect)(ref m_layoutExtent)).Y);
	}

	public override void OnLayoutChanged(bool isVirtualizing)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		m_managingViewportDisabled = !isVirtualizing;
		m_layoutExtent = default(Rect);
		m_expectedViewportShift = default(Point);
		ResetCacheBuffer();
	}

	public override void OnElementPrepared(UIElement element)
	{
	}

	public override void OnElementCleared(UIElement element)
	{
		if (m_horizontalScroller != null)
		{
			m_horizontalScroller.UnregisterAnchorCandidate(element);
		}
		if (m_verticalScroller != null && m_verticalScroller != m_horizontalScroller)
		{
			m_verticalScroller.UnregisterAnchorCandidate(element);
		}
	}

	public override void OnOwnerMeasuring()
	{
		if (!m_managingViewportDisabled)
		{
			EnsureScrollers();
		}
	}

	public override void OnOwnerArranged()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (m_managingViewportDisabled)
		{
			return;
		}
		m_expectedViewportShift = default(Point);
		if (HasScrollers)
		{
			double num = m_maximumHorizontalCacheLength * ((Rect)(ref m_visibleWindow)).Width / 2.0;
			double num2 = m_maximumVerticalCacheLength * ((Rect)(ref m_visibleWindow)).Height / 2.0;
			if (m_horizontalCacheBufferPerSide < num || m_verticalCacheBufferPerSide < num2)
			{
				m_horizontalCacheBufferPerSide += 40.0;
				m_verticalCacheBufferPerSide += 40.0;
				m_horizontalCacheBufferPerSide = Math.Min(m_horizontalCacheBufferPerSide, num);
				m_verticalCacheBufferPerSide = Math.Min(m_verticalCacheBufferPerSide, num2);
				RegisterCacheBuildWork();
			}
		}
	}

	public override void OnMakeAnchor(UIElement anchor, bool isAnchorOutsideRealizedRange)
	{
		m_makeAnchorElement = anchor;
		m_isAnchorOutsideRealizedRange = isAnchorOutsideRealizedRange;
	}

	public override void ResetScrollers()
	{
		m_parentScrollers.Clear();
		m_horizontalScroller = null;
		m_verticalScroller = null;
		m_innerScrollableScroller = null;
		m_ensuredScrollers = false;
	}

	private void OnCacheBuildActionCompleted()
	{
		m_cacheBuildAction = null;
		((UIElement)m_owner).InvalidateMeasure();
	}

	private void OnViewportChanged(IRepeaterScrollingSurface sender, bool isFinal)
	{
		if (!m_managingViewportDisabled)
		{
			if (isFinal)
			{
				m_makeAnchorElement = null;
				m_isAnchorOutsideRealizedRange = false;
			}
			TryInvalidateMeasure();
		}
	}

	private void OnPostArrange(IRepeaterScrollingSurface sender)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (m_managingViewportDisabled)
		{
			return;
		}
		UpdateViewport();
		if (m_visibleWindow == default(Rect))
		{
			m_layoutExtent = default(Rect);
		}
		else
		{
			if (m_horizontalScroller == null && m_verticalScroller == null)
			{
				return;
			}
			UIElementCollection children = ((Panel)m_owner).Children;
			for (int i = 0; i < children.Count; i++)
			{
				UIElement element = children[i];
				if (ItemsRepeater.GetVirtualizationInfo(element).IsHeldByLayout)
				{
					if (m_horizontalScroller != null)
					{
						m_horizontalScroller.RegisterAnchorCandidate(element);
					}
					if (m_verticalScroller != null && m_verticalScroller != m_horizontalScroller)
					{
						m_verticalScroller.RegisterAnchorCandidate(element);
					}
				}
			}
		}
	}

	private void OnConfigurationChanged(IRepeaterScrollingSurface sender)
	{
		m_ensuredScrollers = false;
		TryInvalidateMeasure();
	}

	private void EnsureScrollers()
	{
		if (!m_ensuredScrollers)
		{
			ResetScrollers();
			DependencyObject parent = CachedVisualTreeHelpers.GetParent((DependencyObject)(object)m_owner);
			while (parent != null && (!(parent is IRepeaterScrollingSurface scroller) || !AddScroller(scroller)))
			{
				parent = CachedVisualTreeHelpers.GetParent(parent);
			}
			if (m_parentScrollers.Empty())
			{
				UpdateViewport();
			}
			else
			{
				m_parentScrollers.Last().Scroller.PostArrange += OnPostArrange;
			}
			m_ensuredScrollers = true;
		}
	}

	private bool AddScroller(IRepeaterScrollingSurface scroller)
	{
		bool isHorizontallyScrollable = scroller.IsHorizontallyScrollable;
		bool isVerticallyScrollable = scroller.IsVerticallyScrollable;
		bool result = (m_horizontalScroller != null || isHorizontallyScrollable) && (m_verticalScroller != null || isVerticallyScrollable);
		bool flag = m_horizontalScroller == null && isHorizontallyScrollable;
		bool flag2 = m_verticalScroller == null && isVerticallyScrollable;
		bool flag3 = m_innerScrollableScroller == null && (flag || flag2);
		if (flag)
		{
			m_horizontalScroller = scroller;
		}
		if (flag2)
		{
			m_verticalScroller = scroller;
		}
		if (flag3)
		{
			m_innerScrollableScroller = scroller;
		}
		ScrollerInfo item = new ScrollerInfo(scroller);
		scroller.ConfigurationChanged += OnConfigurationChanged;
		if (flag || flag2)
		{
			scroller.ViewportChanged += OnViewportChanged;
		}
		m_parentScrollers.Add(item);
		return result;
	}

	private void UpdateViewport()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		Rect visibleWindow = m_visibleWindow;
		Rect val = (Rect)((m_horizontalScroller != null) ? m_horizontalScroller.GetRelativeViewport((UIElement)(object)m_owner) : default(Rect));
		Rect val2 = (Rect)((m_verticalScroller == null) ? default(Rect) : ((m_verticalScroller == m_horizontalScroller) ? val : m_verticalScroller.GetRelativeViewport((UIElement)(object)m_owner)));
		Rect visibleWindow2 = (HasScrollers ? new Rect((m_horizontalScroller != null) ? ((Rect)(ref val)).X : ((Rect)(ref val2)).X, (m_verticalScroller != null) ? ((Rect)(ref val2)).Y : ((Rect)(ref val)).Y, (m_horizontalScroller != null) ? ((Rect)(ref val)).Width : ((Rect)(ref val2)).Width, (m_verticalScroller != null) ? ((Rect)(ref val2)).Height : ((Rect)(ref val)).Height) : new Rect(0.0, 0.0, double.MaxValue, double.MaxValue));
		double num = 0.0 - ((Rect)(ref visibleWindow2)).X;
		Point clearedElementsArrangePosition = ItemsRepeater.ClearedElementsArrangePosition;
		if (num <= ((Point)(ref clearedElementsArrangePosition)).X)
		{
			double num2 = 0.0 - ((Rect)(ref visibleWindow2)).Y;
			clearedElementsArrangePosition = ItemsRepeater.ClearedElementsArrangePosition;
			if (num2 <= ((Point)(ref clearedElementsArrangePosition)).Y)
			{
				m_visibleWindow = default(Rect);
				goto IL_0142;
			}
		}
		m_visibleWindow = visibleWindow2;
		goto IL_0142;
		IL_0142:
		if (Math.Abs(((Rect)(ref m_visibleWindow)).X - ((Rect)(ref visibleWindow)).X) > 1.0 || Math.Abs(((Rect)(ref m_visibleWindow)).Y - ((Rect)(ref visibleWindow)).Y) > 1.0 || ((Rect)(ref m_visibleWindow)).Width != ((Rect)(ref visibleWindow)).Width || ((Rect)(ref m_visibleWindow)).Height != ((Rect)(ref visibleWindow)).Height)
		{
			TryInvalidateMeasure();
		}
	}

	private void ResetCacheBuffer()
	{
		m_horizontalCacheBufferPerSide = 0.0;
		m_verticalCacheBufferPerSide = 0.0;
		if (!m_managingViewportDisabled)
		{
			RegisterCacheBuildWork();
		}
	}

	private void ValidateCacheLength(double cacheLength)
	{
		if (cacheLength < 0.0 || double.IsInfinity(cacheLength) || double.IsNaN(cacheLength))
		{
			throw new ArgumentOutOfRangeException("The maximum cache length must be equal or superior to zero.");
		}
	}

	private void RegisterCacheBuildWork()
	{
		if (m_owner.Layout != null && m_cacheBuildAction == null)
		{
			_ = m_owner;
			m_cacheBuildAction = ((DispatcherObject)m_owner).Dispatcher.BeginInvoke(delegate
			{
				OnCacheBuildActionCompleted();
			}, (DispatcherPriority)3);
		}
	}

	private void TryInvalidateMeasure()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (m_visibleWindow != default(Rect))
		{
			((UIElement)m_owner).InvalidateMeasure();
		}
	}

	private IRepeaterScrollingSurface GetOuterScroller()
	{
		IRepeaterScrollingSurface result = null;
		if (!m_parentScrollers.Empty())
		{
			result = m_parentScrollers.Last().Scroller;
		}
		return result;
	}

	private string GetLayoutId()
	{
		string result = null;
		Layout layout = m_owner.Layout;
		if (layout != null)
		{
			result = layout.LayoutId;
		}
		return result;
	}
}
