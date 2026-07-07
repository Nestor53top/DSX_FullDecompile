using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModernWpf.Controls.Primitives;

public class CommandBarOverflowPanel : ToolBarOverflowPanel
{
	internal bool HasToggleButton { get; private set; }

	internal bool HasMenuIcon { get; private set; }

	public CommandBarOverflowPanel()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnLoaded);
	}

	protected override Size MeasureOverride(Size constraint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		((ToolBarOverflowPanel)this).MeasureOverride(constraint);
		Size result = default(Size);
		UIElementCollection internalChildren = ((Panel)this).InternalChildren;
		Size val = constraint;
		((Size)(ref val)).Height = double.PositiveInfinity;
		int i = 0;
		for (int count = internalChildren.Count; i < count; i++)
		{
			UIElement val2 = internalChildren[i];
			if (val2 != null)
			{
				if (val2 is AppBarSeparator appBarSeparator && IsPrimaryCommand((DependencyObject)(object)appBarSeparator))
				{
					UpdateSeparatorVisibility(i, appBarSeparator);
				}
				val2.Measure(val);
				Size desiredSize = val2.DesiredSize;
				((Size)(ref result)).Width = Math.Max(((Size)(ref result)).Width, ((Size)(ref desiredSize)).Width);
				((Size)(ref result)).Height = ((Size)(ref result)).Height + ((Size)(ref desiredSize)).Height;
			}
		}
		return result;
	}

	protected override Size ArrangeOverride(Size arrangeBounds)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		UIElementCollection internalChildren = ((Panel)this).InternalChildren;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(arrangeBounds);
		double num = 0.0;
		int i = 0;
		for (int count = internalChildren.Count; i < count; i++)
		{
			UIElement val2 = internalChildren[i];
			if (val2 != null)
			{
				((Rect)(ref val)).Y = ((Rect)(ref val)).Y + num;
				Size desiredSize = val2.DesiredSize;
				num = (((Rect)(ref val)).Height = ((Size)(ref desiredSize)).Height);
				double width = ((Size)(ref arrangeBounds)).Width;
				desiredSize = val2.DesiredSize;
				((Rect)(ref val)).Width = Math.Max(width, ((Size)(ref desiredSize)).Width);
				val2.Arrange(val);
			}
		}
		return arrangeBounds;
	}

	protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
	{
		((Panel)this).OnVisualChildrenChanged(visualAdded, visualRemoved);
		if (visualAdded != null)
		{
			UpdateChildrenApplicationViewState();
		}
		if (visualRemoved is AppBarSeparator appBarSeparator && IsPrimaryCommand((DependencyObject)(object)appBarSeparator))
		{
			RestoreSeparatorVisibility(appBarSeparator);
		}
	}

	private void UpdateChildrenApplicationViewState()
	{
		bool flag = false;
		bool flag2 = false;
		UIElementCollection internalChildren = ((Panel)this).InternalChildren;
		int i = 0;
		for (int count = internalChildren.Count; i < count; i++)
		{
			UIElement val = internalChildren[i];
			if (!val.IsVisible)
			{
				continue;
			}
			if (val is AppBarButton appBarButton)
			{
				if (!flag2 && appBarButton.Icon != null)
				{
					flag2 = true;
				}
			}
			else if (val is AppBarToggleButton appBarToggleButton)
			{
				if (!flag2 && appBarToggleButton.Icon != null)
				{
					flag2 = true;
				}
				if (!flag)
				{
					flag = true;
				}
			}
			if (flag2 && flag)
			{
				break;
			}
		}
		HasToggleButton = flag;
		HasMenuIcon = flag2;
		int j = 0;
		for (int count2 = internalChildren.Count; j < count2; j++)
		{
			if (internalChildren[j] is IAppBarElement appBarElement)
			{
				appBarElement.UpdateApplicationViewState();
			}
		}
	}

	private bool IsPrimaryCommand(DependencyObject element)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		return (int)ToolBar.GetOverflowMode(element) != 1;
	}

	private void UpdateSeparatorVisibility(int index, AppBarSeparator separator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Visibility visibility = ((UIElement)separator).Visibility;
		if (index == 0)
		{
			if ((int)visibility == 0)
			{
				((DependencyObject)separator).SetCurrentValue(UIElement.VisibilityProperty, (object)(Visibility)2);
			}
		}
		else
		{
			RestoreSeparatorVisibility(separator);
		}
	}

	private void RestoreSeparatorVisibility(AppBarSeparator separator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)separator).Visibility == 2)
		{
			ValueSource valueSource = DependencyPropertyHelper.GetValueSource((DependencyObject)(object)separator, UIElement.VisibilityProperty);
			if (((ValueSource)(ref valueSource)).IsCurrent)
			{
				((DependencyObject)separator).InvalidateProperty(UIElement.VisibilityProperty);
			}
		}
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		UpdateChildrenApplicationViewState();
	}
}
