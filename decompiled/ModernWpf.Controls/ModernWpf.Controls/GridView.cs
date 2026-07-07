using System.Windows;

namespace ModernWpf.Controls;

public class GridView : ListViewBase
{
	static GridView()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(GridView), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(GridView)));
	}

	protected override bool IsItemItsOwnContainerOverride(object item)
	{
		return item is GridViewItem;
	}

	protected override DependencyObject GetContainerForItemOverride()
	{
		return (DependencyObject)(object)new GridViewItem();
	}
}
