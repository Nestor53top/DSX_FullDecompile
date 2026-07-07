using System.Windows;

namespace ModernWpf.Controls;

public class ListView : ListViewBase
{
	static ListView()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ListView), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ListView)));
	}

	protected override bool IsItemItsOwnContainerOverride(object item)
	{
		return item is ListViewItem;
	}

	protected override DependencyObject GetContainerForItemOverride()
	{
		return (DependencyObject)(object)new ListViewItem();
	}
}
