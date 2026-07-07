using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class RadioMenuItem : MenuItem
{
	public static readonly DependencyProperty GroupNameProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	private bool m_isSafeUncheck;

	private bool m_surpressOnChecked;

	public string GroupName
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(GroupNameProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(GroupNameProperty, (object)value);
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

	static RadioMenuItem()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(RadioMenuItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnGroupNameChanged)));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(RadioMenuItem));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioMenuItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(RadioMenuItem)));
		MenuItem.IsCheckableProperty.OverrideMetadata(typeof(RadioMenuItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)true, (PropertyChangedCallback)null, new CoerceValueCallback(CoerceIsCheckable)));
	}

	private static object CoerceIsCheckable(DependencyObject d, object baseValue)
	{
		return true;
	}

	private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((RadioMenuItem)(object)d).UpdateSiblings();
	}

	protected override void OnChecked(RoutedEventArgs e)
	{
		if (m_surpressOnChecked)
		{
			e.Handled = true;
			return;
		}
		UpdateSiblings();
		((MenuItem)this).OnChecked(e);
	}

	protected override void OnUnchecked(RoutedEventArgs e)
	{
		if (!m_isSafeUncheck)
		{
			m_surpressOnChecked = true;
			((DependencyObject)this).SetCurrentValue(MenuItem.IsCheckedProperty, (object)true);
			m_surpressOnChecked = false;
			e.Handled = true;
		}
		else
		{
			((MenuItem)this).OnUnchecked(e);
		}
	}

	private void UpdateSiblings()
	{
		if (!((MenuItem)this).IsChecked)
		{
			return;
		}
		ItemsControl val = ItemsControl.ItemsControlFromItemContainer((DependencyObject)(object)this);
		if (val == null)
		{
			return;
		}
		int count = ((CollectionView)val.Items).Count;
		for (int i = 0; i < count; i++)
		{
			if (val.Items[i] is RadioMenuItem radioMenuItem && radioMenuItem != this && radioMenuItem.GroupName == GroupName)
			{
				radioMenuItem.m_isSafeUncheck = true;
				((DependencyObject)radioMenuItem).SetCurrentValue(MenuItem.IsCheckedProperty, (object)false);
				radioMenuItem.m_isSafeUncheck = false;
			}
		}
	}
}
