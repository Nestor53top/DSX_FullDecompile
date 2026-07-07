using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf.Automation.Peers;

namespace ModernWpf.Controls;

[ContentProperty("ItemTemplate")]
public class ItemsRepeater : Panel
{
	internal static readonly Point ClearedElementsArrangePosition;

	internal static readonly Rect InvalidRect;

	public static readonly DependencyProperty ItemsSourceProperty;

	public static readonly DependencyProperty ItemTemplateProperty;

	public static readonly DependencyProperty LayoutProperty;

	public static readonly DependencyProperty AnimatorProperty;

	public static readonly DependencyProperty HorizontalCacheLengthProperty;

	public static readonly DependencyProperty VerticalCacheLengthProperty;

	internal static readonly DependencyProperty VirtualizationInfoProperty;

	private readonly ViewportManager m_viewportManager;

	private IElementFactoryShim m_itemTemplateWrapper;

	private VirtualizingLayoutContext m_layoutContext;

	private NotifyCollectionChangedEventArgs m_processingItemsSourceChange;

	private Size m_lastAvailableSize;

	private bool m_isLayoutInProgress;

	private Point m_layoutOrigin;

	private ItemsRepeaterElementPreparedEventArgs m_elementPreparedArgs;

	private ItemsRepeaterElementClearingEventArgs m_elementClearingArgs;

	private ItemsRepeaterElementIndexChangedEventArgs m_elementIndexChangedArgs;

	private int _loadedCounter;

	private int _unloadedCounter;

	private object m_itemTemplate;

	private Layout m_layout;

	private ElementAnimator m_animator;

	private bool m_isItemTemplateEmpty;

	private static readonly Size s_zeroSize;

	public object ItemsSource
	{
		get
		{
			return ((DependencyObject)this).GetValue(ItemsSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemsSourceProperty, value);
		}
	}

	public ItemsSourceView ItemsSourceView { get; private set; }

	public object ItemTemplate
	{
		get
		{
			return ((DependencyObject)this).GetValue(ItemTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemTemplateProperty, value);
		}
	}

	public Layout Layout
	{
		get
		{
			return (Layout)((DependencyObject)this).GetValue(LayoutProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LayoutProperty, (object)value);
		}
	}

	internal ElementAnimator Animator
	{
		get
		{
			return (ElementAnimator)((DependencyObject)this).GetValue(AnimatorProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(AnimatorProperty, (object)value);
		}
	}

	public double HorizontalCacheLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(HorizontalCacheLengthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HorizontalCacheLengthProperty, (object)value);
		}
	}

	public double VerticalCacheLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(VerticalCacheLengthProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(VerticalCacheLengthProperty, (object)value);
		}
	}

	internal IElementFactoryShim ItemTemplateShim => m_itemTemplateWrapper;

	internal ViewManager ViewManager { get; }

	internal AnimationManager AnimationManager { get; }

	internal object LayoutState { get; set; }

	internal Rect VisibleWindow => m_viewportManager.GetLayoutVisibleWindow();

	internal Rect RealizationWindow => m_viewportManager.GetLayoutRealizationWindow();

	internal UIElement SuggestedAnchor => m_viewportManager.SuggestedAnchor;

	internal UIElement MadeAnchor => m_viewportManager.MadeAnchor;

	internal Point LayoutOrigin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_layoutOrigin;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_layoutOrigin = value;
		}
	}

	private bool IsProcessingCollectionChange => m_processingItemsSourceChange != null;

	public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementClearingEventArgs> ElementClearing;

	public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementIndexChangedEventArgs> ElementIndexChanged;

	public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementPreparedEventArgs> ElementPrepared;

	static ItemsRepeater()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		ClearedElementsArrangePosition = new Point(-10000.0, -10000.0);
		InvalidRect = Rect.Empty;
		ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(ItemsRepeater), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
		ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(object), typeof(ItemsRepeater), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
		LayoutProperty = DependencyProperty.Register("Layout", typeof(Layout), typeof(ItemsRepeater), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
		AnimatorProperty = DependencyProperty.Register("Animator", typeof(ElementAnimator), typeof(ItemsRepeater), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
		HorizontalCacheLengthProperty = DependencyProperty.Register("HorizontalCacheLength", typeof(double), typeof(ItemsRepeater), new PropertyMetadata((object)2.0, new PropertyChangedCallback(OnPropertyChanged)));
		VerticalCacheLengthProperty = DependencyProperty.Register("VerticalCacheLength", typeof(double), typeof(ItemsRepeater), new PropertyMetadata((object)2.0, new PropertyChangedCallback(OnPropertyChanged)));
		VirtualizationInfoProperty = DependencyProperty.RegisterAttached("VirtualizationInfo", typeof(VirtualizationInfo), typeof(ItemsRepeater), (PropertyMetadata)null);
		s_zeroSize = default(Size);
		KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ItemsRepeater), (PropertyMetadata)new FrameworkPropertyMetadata((object)(KeyboardNavigationMode)1));
	}

	public ItemsRepeater()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		AnimationManager = new AnimationManager(this);
		ViewManager = new ViewManager(this);
		m_viewportManager = new ViewportManagerDownLevel(this);
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnLoaded);
		((FrameworkElement)this).Unloaded += new RoutedEventHandler(OnUnloaded);
		((DependencyObject)this).SetCurrentValue(LayoutProperty, (object)new StackLayout());
		VirtualizingLayout newValue = Layout as VirtualizingLayout;
		OnLayoutChanged(null, newValue);
	}

	~ItemsRepeater()
	{
		try
		{
			m_itemTemplate = null;
			m_animator = null;
			m_layout = null;
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new RepeaterAutomationPeer((FrameworkElement)(object)this);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (m_isLayoutInProgress)
		{
			throw new InvalidOperationException("Reentrancy detected during layout.");
		}
		if (IsProcessingCollectionChange)
		{
			throw new InvalidOperationException("Cannot run layout in the middle of a collection change.");
		}
		m_viewportManager.OnOwnerMeasuring();
		m_isLayoutInProgress = true;
		try
		{
			ViewManager.PrunePinnedElements();
			Rect layoutExtent = default(Rect);
			Size result = default(Size);
			Layout layout = Layout;
			if (layout != null)
			{
				VirtualizingLayoutContext layoutContext = GetLayoutContext();
				if (m_isItemTemplateEmpty)
				{
					((Rect)(ref layoutExtent))._002Ector(((Point)(ref m_layoutOrigin)).X, ((Point)(ref m_layoutOrigin)).Y, 0.0, 0.0);
				}
				else
				{
					result = layout.Measure(layoutContext, availableSize);
					((Rect)(ref layoutExtent))._002Ector(((Point)(ref m_layoutOrigin)).X, ((Point)(ref m_layoutOrigin)).Y, ((Size)(ref result)).Width, ((Size)(ref result)).Height);
				}
				UIElementCollection children = ((Panel)this).Children;
				for (int i = 0; i < children.Count; i++)
				{
					UIElement element = children[i];
					VirtualizationInfo virtualizationInfo = GetVirtualizationInfo(element);
					if (virtualizationInfo.Owner == ElementOwner.Layout && virtualizationInfo.AutoRecycleCandidate && !virtualizationInfo.KeepAlive)
					{
						ClearElementImpl(element);
					}
				}
			}
			m_viewportManager.SetLayoutExtent(layoutExtent);
			m_lastAvailableSize = availableSize;
			return result;
		}
		finally
		{
			m_isLayoutInProgress = false;
		}
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (m_isLayoutInProgress)
		{
			throw new InvalidOperationException("Reentrancy detected during layout.");
		}
		if (IsProcessingCollectionChange)
		{
			throw new InvalidOperationException("Cannot run layout in the middle of a collection change.");
		}
		m_isLayoutInProgress = true;
		try
		{
			Size val = default(Size);
			Layout layout = Layout;
			if (layout != null)
			{
				val = layout.Arrange(GetLayoutContext(), finalSize);
			}
			ViewManager.OnOwnerArranged();
			UIElementCollection children = ((Panel)this).Children;
			Size result;
			for (int i = 0; i < children.Count; i++)
			{
				UIElement val2 = children[i];
				VirtualizationInfo virtualizationInfo = GetVirtualizationInfo(val2);
				virtualizationInfo.KeepAlive = false;
				if (virtualizationInfo.Owner == ElementOwner.ElementFactory || virtualizationInfo.Owner == ElementOwner.PinnedPool)
				{
					Point clearedElementsArrangePosition = ClearedElementsArrangePosition;
					double x = ((Point)(ref clearedElementsArrangePosition)).X;
					result = val2.DesiredSize;
					double num = x - ((Size)(ref result)).Width;
					clearedElementsArrangePosition = ClearedElementsArrangePosition;
					double y = ((Point)(ref clearedElementsArrangePosition)).Y;
					result = val2.DesiredSize;
					val2.Arrange(new Rect(num, y - ((Size)(ref result)).Height, 0.0, 0.0));
				}
				else
				{
					Rect layoutSlot = CachedVisualTreeHelpers.GetLayoutSlot((FrameworkElement)(object)((val2 is FrameworkElement) ? val2 : null));
					if (virtualizationInfo.ArrangeBounds != InvalidRect && layoutSlot != virtualizationInfo.ArrangeBounds)
					{
						AnimationManager.OnElementBoundsChanged(val2, virtualizationInfo.ArrangeBounds, layoutSlot);
					}
					virtualizationInfo.ArrangeBounds = layoutSlot;
					virtualizationInfo.DesiredSize = val2.DesiredSize;
				}
			}
			m_viewportManager.OnOwnerArranged();
			AnimationManager.OnOwnerArranged();
			result = val;
			return result;
		}
		finally
		{
			m_isLayoutInProgress = false;
		}
	}

	public int GetElementIndex(UIElement element)
	{
		return GetElementIndexImpl(element);
	}

	public UIElement TryGetElement(int index)
	{
		return GetElementFromIndexImpl(index);
	}

	internal void PinElement(UIElement element)
	{
		ViewManager.UpdatePin(element, addPin: true);
	}

	internal void UnpinElement(UIElement element)
	{
		ViewManager.UpdatePin(element, addPin: false);
	}

	public UIElement GetOrCreateElement(int index)
	{
		return GetOrCreateElementImpl(index);
	}

	internal UIElement GetElementImpl(int index, bool forceCreate, bool suppressAutoRecycle)
	{
		return ViewManager.GetElement(index, forceCreate, suppressAutoRecycle);
	}

	internal void ClearElementImpl(UIElement element)
	{
		bool isClearedDueToCollectionChange = IsProcessingCollectionChange && (m_processingItemsSourceChange.Action == NotifyCollectionChangedAction.Remove || m_processingItemsSourceChange.Action == NotifyCollectionChangedAction.Replace || m_processingItemsSourceChange.Action == NotifyCollectionChangedAction.Reset);
		ViewManager.ClearElement(element, isClearedDueToCollectionChange);
		m_viewportManager.OnElementCleared(element);
	}

	internal int GetElementIndexImpl(UIElement element)
	{
		if ((object)VisualTreeHelper.GetParent((DependencyObject)(object)element) == this)
		{
			VirtualizationInfo virtInfo = TryGetVirtualizationInfo(element);
			return ViewManager.GetElementIndex(virtInfo);
		}
		return -1;
	}

	internal UIElement GetElementFromIndexImpl(int index)
	{
		UIElement val = null;
		UIElementCollection children = ((Panel)this).Children;
		for (int i = 0; i < children.Count; i++)
		{
			if (val != null)
			{
				break;
			}
			UIElement val2 = children[i];
			VirtualizationInfo virtualizationInfo = TryGetVirtualizationInfo(val2);
			if (virtualizationInfo != null && virtualizationInfo.IsRealized && virtualizationInfo.Index == index)
			{
				val = val2;
			}
		}
		return val;
	}

	internal UIElement GetOrCreateElementImpl(int index)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (index >= 0 && index >= ItemsSourceView.Count)
		{
			throw new ArgumentException("index", "Argument index is invalid.");
		}
		if (m_isLayoutInProgress)
		{
			throw new InvalidOperationException("GetOrCreateElement invocation is not allowed during layout.");
		}
		UIElement val = GetElementFromIndexImpl(index);
		bool flag = val == null;
		if (flag)
		{
			if (Layout == null)
			{
				throw new InvalidOperationException("Cannot make an Anchor when there is no attached layout.");
			}
			val = GetLayoutContext().GetOrCreateElementAt(index);
			val.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		}
		m_viewportManager.OnMakeAnchor(val, flag);
		((UIElement)this).InvalidateMeasure();
		return val;
	}

	internal static VirtualizationInfo TryGetVirtualizationInfo(UIElement element)
	{
		return (VirtualizationInfo)((DependencyObject)element).GetValue(VirtualizationInfoProperty);
	}

	internal static VirtualizationInfo GetVirtualizationInfo(UIElement element)
	{
		VirtualizationInfo virtualizationInfo = TryGetVirtualizationInfo(element);
		if (virtualizationInfo == null)
		{
			virtualizationInfo = CreateAndInitializeVirtualizationInfo(element);
		}
		return virtualizationInfo;
	}

	internal static VirtualizationInfo CreateAndInitializeVirtualizationInfo(UIElement element)
	{
		VirtualizationInfo virtualizationInfo = new VirtualizationInfo();
		((DependencyObject)element).SetValue(VirtualizationInfoProperty, (object)virtualizationInfo);
		return virtualizationInfo;
	}

	private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ItemsRepeater)(object)sender).PrivateOnPropertyChanged(args);
	}

	private void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		DependencyProperty property = ((DependencyPropertyChangedEventArgs)(ref args)).Property;
		if (property == ItemsSourceProperty)
		{
			if (((DependencyPropertyChangedEventArgs)(ref args)).NewValue != ((DependencyPropertyChangedEventArgs)(ref args)).OldValue)
			{
				object newValue = ((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
				ItemsSourceView itemsSourceView = newValue as ItemsSourceView;
				if (newValue != null && itemsSourceView == null)
				{
					itemsSourceView = new InspectingDataSource(newValue);
				}
				OnDataSourcePropertyChanged(ItemsSourceView, itemsSourceView);
			}
		}
		else if (property == ItemTemplateProperty)
		{
			OnItemTemplateChanged(((DependencyPropertyChangedEventArgs)(ref args)).OldValue, ((DependencyPropertyChangedEventArgs)(ref args)).NewValue);
		}
		else if (property == LayoutProperty)
		{
			OnLayoutChanged((Layout)((DependencyPropertyChangedEventArgs)(ref args)).OldValue, (Layout)((DependencyPropertyChangedEventArgs)(ref args)).NewValue);
		}
		else if (property == AnimatorProperty)
		{
			OnAnimatorChanged((ElementAnimator)((DependencyPropertyChangedEventArgs)(ref args)).OldValue, (ElementAnimator)((DependencyPropertyChangedEventArgs)(ref args)).NewValue);
		}
		else if (property == HorizontalCacheLengthProperty)
		{
			m_viewportManager.HorizontalCacheLength = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
		else if (property == VerticalCacheLengthProperty)
		{
			m_viewportManager.VerticalCacheLength = (double)((DependencyPropertyChangedEventArgs)(ref args)).NewValue;
		}
	}

	internal void OnElementPrepared(UIElement element, int index)
	{
		m_viewportManager.OnElementPrepared(element);
		if (this.ElementPrepared != null)
		{
			if (m_elementPreparedArgs == null)
			{
				m_elementPreparedArgs = new ItemsRepeaterElementPreparedEventArgs(element, index);
			}
			else
			{
				m_elementPreparedArgs.Update(element, index);
			}
			this.ElementPrepared(this, m_elementPreparedArgs);
		}
	}

	internal void OnElementClearing(UIElement element)
	{
		if (this.ElementClearing != null)
		{
			if (m_elementClearingArgs == null)
			{
				m_elementClearingArgs = new ItemsRepeaterElementClearingEventArgs(element);
			}
			else
			{
				m_elementClearingArgs.Update(element);
			}
			this.ElementClearing(this, m_elementClearingArgs);
		}
	}

	internal void OnElementIndexChanged(UIElement element, int oldIndex, int newIndex)
	{
		if (this.ElementIndexChanged != null)
		{
			if (m_elementIndexChangedArgs == null)
			{
				m_elementIndexChangedArgs = new ItemsRepeaterElementIndexChangedEventArgs(element, oldIndex, newIndex);
			}
			else
			{
				m_elementIndexChangedArgs.Update(element, oldIndex, newIndex);
			}
			this.ElementIndexChanged(this, m_elementIndexChangedArgs);
		}
	}

	private void OnLoaded(object sender, RoutedEventArgs args)
	{
		if (_loadedCounter > _unloadedCounter)
		{
			_loadedCounter = _unloadedCounter;
			((UIElement)this).InvalidateMeasure();
			m_viewportManager.ResetScrollers();
		}
		_loadedCounter++;
	}

	private void OnUnloaded(object sender, RoutedEventArgs args)
	{
		_unloadedCounter++;
		if (_unloadedCounter == _loadedCounter)
		{
			m_viewportManager.ResetScrollers();
		}
	}

	private void OnDataSourcePropertyChanged(ItemsSourceView oldValue, ItemsSourceView newValue)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		if (m_isLayoutInProgress)
		{
			throw new InvalidOperationException("Cannot set ItemsSourceView during layout.");
		}
		ItemsSourceView = newValue;
		if (oldValue != null)
		{
			oldValue.CollectionChanged -= OnItemsSourceViewChanged;
		}
		if (newValue != null)
		{
			newValue.CollectionChanged += OnItemsSourceViewChanged;
		}
		bool flag = false;
		try
		{
			Layout layout = Layout;
			if (layout == null)
			{
				return;
			}
			NotifyCollectionChangedEventArgs args = (m_processingItemsSourceChange = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			flag = true;
			if (layout is VirtualizingLayout virtualizingLayout)
			{
				((IVirtualizingLayoutOverrides)virtualizingLayout).OnItemsChangedCore(GetLayoutContext(), (object)newValue, args);
			}
			else if (layout is NonVirtualizingLayout)
			{
				foreach (UIElement child in ((Panel)this).Children)
				{
					UIElement element = child;
					if (GetVirtualizationInfo(element).IsRealized)
					{
						ClearElementImpl(element);
					}
				}
				((Panel)this).Children.Clear();
			}
			((UIElement)this).InvalidateMeasure();
		}
		finally
		{
			if (flag)
			{
				m_processingItemsSourceChange = null;
			}
		}
	}

	private void OnItemTemplateChanged(object oldValue, object newValue)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		if (m_isLayoutInProgress && oldValue != null)
		{
			throw new InvalidOperationException("ItemTemplate cannot be changed during layout.");
		}
		bool flag = false;
		try
		{
			Layout layout = Layout;
			if (layout != null)
			{
				NotifyCollectionChangedEventArgs args = (m_processingItemsSourceChange = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				flag = true;
				if (layout is VirtualizingLayout virtualizingLayout)
				{
					((IVirtualizingLayoutOverrides)virtualizingLayout).OnItemsChangedCore(GetLayoutContext(), newValue, args);
				}
				else if (layout is NonVirtualizingLayout)
				{
					foreach (UIElement child in ((Panel)this).Children)
					{
						UIElement element = child;
						if (GetVirtualizationInfo(element).IsRealized)
						{
							ClearElementImpl(element);
						}
					}
				}
			}
			m_itemTemplate = newValue;
			m_isItemTemplateEmpty = false;
			m_itemTemplateWrapper = newValue as IElementFactoryShim;
			if (m_itemTemplateWrapper == null)
			{
				DataTemplate val = (DataTemplate)((newValue is DataTemplate) ? newValue : null);
				if (val != null)
				{
					m_itemTemplateWrapper = new ItemTemplateWrapper(val);
					if (!(((FrameworkTemplate)val).LoadContent() is FrameworkElement))
					{
						m_isItemTemplateEmpty = true;
					}
				}
				else
				{
					DataTemplateSelector val2 = (DataTemplateSelector)((newValue is DataTemplateSelector) ? newValue : null);
					if (val2 == null)
					{
						throw new ArgumentException("newValue", "ItemTemplate");
					}
					m_itemTemplateWrapper = new ItemTemplateWrapper(val2);
				}
			}
			((UIElement)this).InvalidateMeasure();
		}
		finally
		{
			if (flag)
			{
				m_processingItemsSourceChange = null;
			}
		}
	}

	private void OnLayoutChanged(Layout oldValue, Layout newValue)
	{
		if (m_isLayoutInProgress)
		{
			throw new InvalidOperationException("Layout cannot be changed during layout.");
		}
		ViewManager.OnLayoutChanging();
		AnimationManager.OnLayoutChanging();
		if (oldValue != null)
		{
			oldValue.UninitializeForContext(GetLayoutContext());
			oldValue.MeasureInvalidated -= InvalidateMeasureForLayout;
			oldValue.ArrangeInvalidated -= InvalidateArrangeForLayout;
			UIElementCollection children = ((Panel)this).Children;
			for (int i = 0; i < children.Count; i++)
			{
				UIElement element = children[i];
				if (GetVirtualizationInfo(element).IsRealized)
				{
					ClearElementImpl(element);
				}
			}
			LayoutState = null;
		}
		m_layout = newValue;
		if (newValue != null)
		{
			newValue.InitializeForContext(GetLayoutContext());
			newValue.MeasureInvalidated += InvalidateMeasureForLayout;
			newValue.ArrangeInvalidated += InvalidateArrangeForLayout;
		}
		bool isVirtualizing = newValue != null && newValue is VirtualizingLayout;
		m_viewportManager.OnLayoutChanged(isVirtualizing);
		((UIElement)this).InvalidateMeasure();
	}

	private void OnAnimatorChanged(ElementAnimator oldValue, ElementAnimator newValue)
	{
		AnimationManager.OnAnimatorChanged(newValue);
		m_animator = newValue;
	}

	private void OnItemsSourceViewChanged(object sender, NotifyCollectionChangedEventArgs args)
	{
		if (m_isLayoutInProgress)
		{
			throw new InvalidOperationException("Changes in data source are not allowed during layout.");
		}
		if (IsProcessingCollectionChange)
		{
			throw new InvalidOperationException("Changes in the data source are not allowed during another change in the data source.");
		}
		m_processingItemsSourceChange = args;
		try
		{
			AnimationManager.OnItemsSourceChanged(sender, args);
			ViewManager.OnItemsSourceChanged(sender, args);
			Layout layout = Layout;
			if (layout != null)
			{
				if (layout is VirtualizingLayout virtualizingLayout)
				{
					((IVirtualizingLayoutOverrides)virtualizingLayout).OnItemsChangedCore(GetLayoutContext(), sender, args);
				}
				else
				{
					((UIElement)this).InvalidateMeasure();
				}
			}
		}
		finally
		{
			m_processingItemsSourceChange = null;
		}
	}

	private void InvalidateMeasureForLayout(object sender, object args)
	{
		((UIElement)this).InvalidateMeasure();
	}

	private void InvalidateArrangeForLayout(object sender, object args)
	{
		((UIElement)this).InvalidateArrange();
	}

	private VirtualizingLayoutContext GetLayoutContext()
	{
		if (m_layoutContext == null)
		{
			m_layoutContext = new RepeaterLayoutContext(this);
		}
		return m_layoutContext;
	}

	protected override void OnChildDesiredSizeChanged(UIElement child)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		VirtualizationInfo virtualizationInfo = TryGetVirtualizationInfo(child);
		if (virtualizationInfo == null || !virtualizationInfo.IsRealized)
		{
			return;
		}
		Size desiredSize = virtualizationInfo.DesiredSize;
		if (!((Size)(ref desiredSize)).IsEmpty)
		{
			Size desiredSize2 = child.DesiredSize;
			Size renderSize = child.RenderSize;
			if (desiredSize == s_zeroSize || desiredSize2 == s_zeroSize || (((Size)(ref desiredSize2)).Height != ((Size)(ref desiredSize)).Height && ((Size)(ref renderSize)).Height == ((Size)(ref desiredSize)).Height) || (((Size)(ref desiredSize2)).Width != ((Size)(ref desiredSize)).Width && ((Size)(ref renderSize)).Width == ((Size)(ref desiredSize)).Width))
			{
				((UIElement)this).OnChildDesiredSizeChanged(child);
			}
		}
	}

	protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
	{
		return (UIElementCollection)(object)new RepeaterUIElementCollection((UIElement)(object)this, logicalParent);
	}
}
