using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class NavigationViewItemSeparator : NavigationViewItemBase
{
	private const string c_rootGrid = "NavigationViewItemSeparatorRootGrid";

	private bool m_appliedTemplate;

	private bool m_isClosedCompact;

	private Grid m_rootGrid;

	static NavigationViewItemSeparator()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemSeparator), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(NavigationViewItemSeparator)));
	}

	private void UpdateVisualState(bool useTransitions)
	{
		if (m_appliedTemplate)
		{
			string stateName = ((base.Position == NavigationViewRepeaterPosition.TopPrimary || base.Position == NavigationViewRepeaterPosition.TopFooter) ? "VerticalLine" : (m_isClosedCompact ? "HorizontalLineCompact" : "HorizontalLine"));
			VisualStateUtil.GoToStateIfGroupExists((Control)(object)this, "NavigationSeparatorLineStates", stateName, useTransitions: false);
		}
	}

	public override void OnApplyTemplate()
	{
		m_appliedTemplate = false;
		((FrameworkElement)this).OnApplyTemplate();
		Grid templateChildT = CppWinRTHelpers.GetTemplateChildT<Grid>("NavigationViewItemSeparatorRootGrid", (IControlProtected)this);
		if (templateChildT != null)
		{
			m_rootGrid = templateChildT;
		}
		SplitView splitView = GetSplitView();
		if (splitView != null)
		{
			splitView.IsPaneOpenChanged += OnSplitViewPropertyChanged;
			splitView.DisplayModeChanged += OnSplitViewPropertyChanged;
			UpdateIsClosedCompact(updateVisualState: false);
		}
		m_appliedTemplate = true;
		UpdateVisualState(useTransitions: false);
		UpdateItemIndentation();
	}

	private protected override void OnNavigationViewItemBaseDepthChanged()
	{
		UpdateVisualState(useTransitions: false);
	}

	private protected override void OnNavigationViewItemBasePositionChanged()
	{
		UpdateVisualState(useTransitions: false);
	}

	private void OnSplitViewPropertyChanged(DependencyObject sender, DependencyProperty args)
	{
		UpdateIsClosedCompact(updateVisualState: true);
	}

	private void UpdateItemIndentation()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Grid rootGrid = m_rootGrid;
		if (rootGrid != null)
		{
			Thickness margin = ((FrameworkElement)rootGrid).Margin;
			int num = base.Depth * 25;
			((FrameworkElement)rootGrid).Margin = new Thickness((double)num, ((Thickness)(ref margin)).Top, ((Thickness)(ref margin)).Right, ((Thickness)(ref margin)).Bottom);
		}
	}

	private void UpdateIsClosedCompact(bool updateVisualState)
	{
		SplitView splitView = GetSplitView();
		if (splitView != null)
		{
			m_isClosedCompact = !splitView.IsPaneOpen && (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);
			if (updateVisualState)
			{
				UpdateVisualState(useTransitions: false);
			}
		}
	}
}
