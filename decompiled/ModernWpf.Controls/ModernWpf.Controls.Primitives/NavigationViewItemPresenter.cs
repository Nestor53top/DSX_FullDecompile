using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ModernWpf.Input;

namespace ModernWpf.Controls.Primitives;

public class NavigationViewItemPresenter : ContentControl, IControlProtected
{
	private const string c_contentGrid = "PresenterContentRootGrid";

	private const string c_expandCollapseChevron = "ExpandCollapseChevron";

	private const string c_expandCollapseRotateExpandedStoryboard = "ExpandCollapseRotateExpandedStoryboard";

	private const string c_expandCollapseRotateCollapsedStoryboard = "ExpandCollapseRotateCollapsedStoryboard";

	private const string c_expandCollapseRotateTransform = "ExpandCollapseChevronRotateTransform";

	private const string c_iconBoxColumnDefinitionName = "IconColumn";

	public static readonly DependencyProperty IconProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	private double m_compactPaneLengthValue = 40.0;

	private NavigationViewItemHelper<NavigationViewItemPresenter> m_helper = new NavigationViewItemHelper<NavigationViewItemPresenter>();

	private Grid m_contentGrid;

	private Grid m_expandCollapseChevron;

	private double m_leftIndentation;

	private Storyboard m_chevronExpandedStoryboard;

	private Storyboard m_chevronCollapsedStoryboard;

	private RotateTransform m_expandCollapseRotateTransform;

	public IconElement Icon
	{
		get
		{
			return (IconElement)((DependencyObject)this).GetValue(IconProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IconProperty, (object)value);
		}
	}

	public bool UseSystemFocusVisuals
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(UseSystemFocusVisualsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UseSystemFocusVisualsProperty, (object)value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (CornerRadius)((DependencyObject)this).GetValue(CornerRadiusProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CornerRadiusProperty, (object)value);
		}
	}

	static NavigationViewItemPresenter()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(NavigationViewItemPresenter), (PropertyMetadata)null);
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(NavigationViewItemPresenter));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(NavigationViewItemPresenter));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(NavigationViewItemPresenter)));
		Control.HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NavigationViewItemPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)(HorizontalAlignment)1));
		Control.VerticalContentAlignmentProperty.OverrideMetadata(typeof(NavigationViewItemPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)(VerticalAlignment)1));
	}

	public NavigationViewItemPresenter()
	{
		InputHelper.SetIsTapEnabled((UIElement)(object)this, value: true);
	}

	public override void OnApplyTemplate()
	{
		m_helper.Init(this);
		Grid templateChildT = CppWinRTHelpers.GetTemplateChildT<Grid>("PresenterContentRootGrid", (IControlProtected)this);
		if (templateChildT != null)
		{
			m_contentGrid = templateChildT;
		}
		NavigationViewItem navigationViewItem = GetNavigationViewItem();
		if (navigationViewItem != null)
		{
			Grid templateChildT2 = CppWinRTHelpers.GetTemplateChildT<Grid>("ExpandCollapseChevron", (IControlProtected)this);
			if (templateChildT2 != null)
			{
				m_expandCollapseChevron = templateChildT2;
				InputHelper.SetIsTapEnabled((UIElement)(object)templateChildT2, value: true);
				InputHelper.AddTappedHandler((UIElement)(object)templateChildT2, navigationViewItem.OnExpandCollapseChevronTapped);
			}
			navigationViewItem.UpdateVisualStateNoTransition();
			navigationViewItem.UpdateIsClosedCompact();
			NavigationView navigationView = navigationViewItem.GetNavigationView();
			if (navigationView != null && navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Top)
			{
				UpdateCompactPaneLength(m_compactPaneLengthValue, shouldUpdate: true);
			}
		}
		FrameworkElement templateRoot = ((Control)(object)this).GetTemplateRoot();
		if (templateRoot != null)
		{
			object obj = templateRoot.Resources[(object)"ExpandCollapseRotateExpandedStoryboard"];
			m_chevronExpandedStoryboard = (Storyboard)((obj is Storyboard) ? obj : null);
			object obj2 = templateRoot.Resources[(object)"ExpandCollapseRotateCollapsedStoryboard"];
			m_chevronCollapsedStoryboard = (Storyboard)((obj2 is Storyboard) ? obj2 : null);
		}
		m_expandCollapseRotateTransform = CppWinRTHelpers.GetTemplateChildT<RotateTransform>("ExpandCollapseChevronRotateTransform", (IControlProtected)this);
		UpdateMargin();
	}

	internal void RotateExpandCollapseChevron(bool isExpanded)
	{
		if (isExpanded)
		{
			Storyboard chevronExpandedStoryboard = m_chevronExpandedStoryboard;
			if (chevronExpandedStoryboard != null)
			{
				chevronExpandedStoryboard.Begin();
			}
			if (m_expandCollapseRotateTransform != null)
			{
				m_expandCollapseRotateTransform.Angle = 180.0;
			}
		}
		else
		{
			Storyboard chevronCollapsedStoryboard = m_chevronCollapsedStoryboard;
			if (chevronCollapsedStoryboard != null)
			{
				chevronCollapsedStoryboard.Begin();
			}
			if (m_expandCollapseRotateTransform != null)
			{
				m_expandCollapseRotateTransform.Angle = 0.0;
			}
		}
	}

	internal UIElement GetSelectionIndicator()
	{
		return m_helper.GetSelectionIndicator();
	}

	private NavigationViewItem GetNavigationViewItem()
	{
		NavigationViewItem result = null;
		NavigationViewItem ancestorOfType = SharedHelpers.GetAncestorOfType<NavigationViewItem>(VisualTreeHelper.GetParent((DependencyObject)(object)this));
		if (ancestorOfType != null)
		{
			result = ancestorOfType;
		}
		return result;
	}

	internal void UpdateContentLeftIndentation(double leftIndentation)
	{
		m_leftIndentation = leftIndentation;
		UpdateMargin();
	}

	private void UpdateMargin()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Grid contentGrid = m_contentGrid;
		if (contentGrid != null)
		{
			Thickness margin = ((FrameworkElement)contentGrid).Margin;
			((FrameworkElement)contentGrid).Margin = new Thickness(m_leftIndentation, ((Thickness)(ref margin)).Top, ((Thickness)(ref margin)).Right, ((Thickness)(ref margin)).Bottom);
		}
	}

	internal void UpdateCompactPaneLength(double compactPaneLength, bool shouldUpdate)
	{
		m_compactPaneLengthValue = compactPaneLength;
		if (shouldUpdate)
		{
			ColumnDefinition templateChildT = CppWinRTHelpers.GetTemplateChildT<ColumnDefinition>("IconColumn", (IControlProtected)this);
			if (templateChildT != null)
			{
				ColumnDefinitionHelper.SetPixelWidth(templateChildT, compactPaneLength);
			}
		}
	}

	internal void UpdateClosedCompactVisualState(bool isTopLevelItem, bool isClosedCompact)
	{
		string text = ((isClosedCompact && isTopLevelItem) ? "ClosedCompactAndTopLevelItem" : "NotClosedCompactAndTopLevelItem");
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, false);
	}

	DependencyObject IControlProtected.GetTemplateChild(string childName)
	{
		return ((FrameworkElement)this).GetTemplateChild(childName);
	}
}
