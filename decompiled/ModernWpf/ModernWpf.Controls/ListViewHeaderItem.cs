using System.Windows;

namespace ModernWpf.Controls;

public class ListViewHeaderItem : ListViewBaseHeaderItem
{
	static ListViewHeaderItem()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ListViewHeaderItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ListViewHeaderItem)));
	}
}
