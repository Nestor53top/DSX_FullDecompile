using System.Windows;

namespace ModernWpf.Controls;

public class GridViewItem : ListViewBaseItem
{
	static GridViewItem()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(GridViewItem)));
	}
}
