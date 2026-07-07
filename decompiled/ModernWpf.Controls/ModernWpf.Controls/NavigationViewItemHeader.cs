using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class NavigationViewItemHeader : NavigationViewItemBase
{
	private const string c_rootGrid = "NavigationViewItemHeaderRootGrid";

	private bool m_isClosedCompact;

	private Grid m_rootGrid;

	static NavigationViewItemHeader()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemHeader), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(NavigationViewItemHeader)));
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		SplitView splitView = GetSplitView();
		if (splitView != null)
		{
			splitView.IsPaneOpenChanged += OnSplitViewPropertyChanged;
			splitView.DisplayModeChanged += OnSplitViewPropertyChanged;
			UpdateIsClosedCompact();
		}
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("NavigationViewItemHeaderRootGrid");
		Grid val = (Grid)(object)((templateChild is Grid) ? templateChild : null);
		if (val != null)
		{
			m_rootGrid = val;
		}
		UpdateVisualState(useTransitions: false);
		UpdateItemIndentation();
	}

	private void OnSplitViewPropertyChanged(DependencyObject sender, DependencyProperty args)
	{
		if (args == SplitView.IsPaneOpenProperty || args == SplitView.DisplayModeProperty)
		{
			UpdateIsClosedCompact();
		}
	}

	private void UpdateIsClosedCompact()
	{
		SplitView splitView = GetSplitView();
		if (splitView != null)
		{
			m_isClosedCompact = !splitView.IsPaneOpen && (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);
			UpdateVisualState(useTransitions: true);
		}
	}

	private void UpdateVisualState(bool useTransitions)
	{
		VisualStateManager.GoToState((FrameworkElement)(object)this, (m_isClosedCompact && base.IsTopLevelItem) ? "HeaderTextCollapsed" : "HeaderTextVisible", useTransitions);
	}

	private protected override void OnNavigationViewItemBaseDepthChanged()
	{
		UpdateItemIndentation();
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
}
