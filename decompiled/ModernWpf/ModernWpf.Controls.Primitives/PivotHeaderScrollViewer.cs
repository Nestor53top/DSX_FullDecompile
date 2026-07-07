using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

public class PivotHeaderScrollViewer : ScrollViewer
{
	private TabControl _tabControl;

	private static readonly DependencyPropertyKey CanScrollLeftPropertyKey;

	public static readonly DependencyProperty CanScrollLeftProperty;

	private static readonly DependencyPropertyKey CanScrollRightPropertyKey;

	public static readonly DependencyProperty CanScrollRightProperty;

	public bool CanScrollLeft
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(CanScrollLeftProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(CanScrollLeftPropertyKey, (object)value);
		}
	}

	public bool CanScrollRight
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(CanScrollRightProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(CanScrollRightPropertyKey, (object)value);
		}
	}

	static PivotHeaderScrollViewer()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		CanScrollLeftPropertyKey = DependencyProperty.RegisterReadOnly("CanScrollLeft", typeof(bool), typeof(PivotHeaderScrollViewer), new PropertyMetadata((object)false));
		CanScrollLeftProperty = CanScrollLeftPropertyKey.DependencyProperty;
		CanScrollRightPropertyKey = DependencyProperty.RegisterReadOnly("CanScrollRight", typeof(bool), typeof(PivotHeaderScrollViewer), new PropertyMetadata((object)false));
		CanScrollRightProperty = CanScrollRightPropertyKey.DependencyProperty;
		FrameworkElement.FlowDirectionProperty.OverrideMetadata(typeof(PivotHeaderScrollViewer), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlowDirectionChanged)));
		ScrollViewer.HorizontalScrollBarVisibilityProperty.OverrideMetadata(typeof(PivotHeaderScrollViewer), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHorizontalScrollBarVisibilityChanged)));
	}

	public PivotHeaderScrollViewer()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnLoaded);
	}

	protected override void OnVisualParentChanged(DependencyObject oldParent)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (_tabControl != null)
		{
			((Selector)_tabControl).SelectionChanged -= new SelectionChangedEventHandler(OnTabControlSelectionChanged);
		}
		((FrameworkElement)this).OnVisualParentChanged(oldParent);
		DependencyObject templatedParent = ((FrameworkElement)this).TemplatedParent;
		_tabControl = (TabControl)(object)((templatedParent is TabControl) ? templatedParent : null);
		if (_tabControl != null)
		{
			((Selector)_tabControl).SelectionChanged += new SelectionChangedEventHandler(OnTabControlSelectionChanged);
		}
	}

	protected override void OnScrollChanged(ScrollChangedEventArgs e)
	{
		((ScrollViewer)this).OnScrollChanged(e);
		if (e.HorizontalChange != 0.0 || e.ExtentWidthChange != 0.0 || e.ViewportWidthChange != 0.0)
		{
			UpdateCanScrollHorizontally();
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
	}

	private static void OnFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((PivotHeaderScrollViewer)(object)d).UpdateCanScrollHorizontally();
	}

	private static void OnHorizontalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((PivotHeaderScrollViewer)(object)d).UpdateCanScrollHorizontally();
	}

	private void UpdateCanScrollHorizontally()
	{
		bool flag = CanScrollHorizontallyInDirection(inPositiveDirection: false);
		if (CanScrollLeft != flag)
		{
			CanScrollLeft = flag;
		}
		bool flag2 = CanScrollHorizontallyInDirection(inPositiveDirection: true);
		if (CanScrollRight != flag2)
		{
			CanScrollRight = flag2;
		}
	}

	private bool CanScrollHorizontallyInDirection(bool inPositiveDirection)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if ((int)((FrameworkElement)this).FlowDirection == 1)
		{
			inPositiveDirection = !inPositiveDirection;
		}
		if ((int)((ScrollViewer)this).HorizontalScrollBarVisibility != 0)
		{
			double extentWidth = ((ScrollViewer)this).ExtentWidth;
			double viewportWidth = ((ScrollViewer)this).ViewportWidth;
			if (extentWidth > viewportWidth)
			{
				if (inPositiveDirection)
				{
					double num = extentWidth - viewportWidth;
					if (((ScrollViewer)this).HorizontalOffset < num)
					{
						result = true;
					}
				}
				else if (((ScrollViewer)this).HorizontalOffset > 0.0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		BringSelectedTabItemIntoView();
	}

	private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		BringSelectedTabItemIntoView();
	}

	private void BringSelectedTabItemIntoView()
	{
		if (_tabControl != null)
		{
			TabItem selectedTabItem = GetSelectedTabItem(_tabControl);
			if (selectedTabItem != null)
			{
				((FrameworkElement)selectedTabItem).BringIntoView();
			}
		}
	}

	private static TabItem GetSelectedTabItem(TabControl tabControl)
	{
		object selectedItem = ((Selector)tabControl).SelectedItem;
		if (selectedItem != null)
		{
			TabItem val = (TabItem)((selectedItem is TabItem) ? selectedItem : null);
			if (val == null)
			{
				DependencyObject obj = ((ItemsControl)tabControl).ItemContainerGenerator.ContainerFromIndex(((Selector)tabControl).SelectedIndex);
				val = (TabItem)(object)((obj is TabItem) ? obj : null);
				if (val == null || !EqualsEx(selectedItem, ((ItemsControl)tabControl).ItemContainerGenerator.ItemFromContainer((DependencyObject)(object)val)))
				{
					DependencyObject obj2 = ((ItemsControl)tabControl).ItemContainerGenerator.ContainerFromItem(selectedItem);
					val = (TabItem)(object)((obj2 is TabItem) ? obj2 : null);
				}
			}
			return val;
		}
		return null;
	}

	private static bool EqualsEx(object o1, object o2)
	{
		try
		{
			return object.Equals(o1, o2);
		}
		catch (InvalidCastException)
		{
			return false;
		}
	}
}
