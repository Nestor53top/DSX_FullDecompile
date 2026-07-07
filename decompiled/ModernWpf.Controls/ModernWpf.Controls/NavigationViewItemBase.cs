using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class NavigationViewItemBase : ContentControl, IControlProtected
{
	public static readonly DependencyProperty IsSelectedProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	internal const int c_itemIndentation = 25;

	private protected WeakReference<NavigationView> m_navigationView;

	private NavigationViewRepeaterPosition m_position;

	private int m_depth;

	private bool m_isTopLevelItem;

	private bool m_createdByNavigationViewItemsFactory;

	public bool IsSelected
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsSelectedProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsSelectedProperty, (object)value);
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

	internal NavigationViewRepeaterPosition Position
	{
		get
		{
			return m_position;
		}
		set
		{
			if (m_position != value)
			{
				m_position = value;
				OnNavigationViewItemBasePositionChanged();
			}
		}
	}

	internal int Depth
	{
		get
		{
			return m_depth;
		}
		set
		{
			if (m_depth != value)
			{
				m_depth = value;
				OnNavigationViewItemBaseDepthChanged();
			}
		}
	}

	internal bool IsTopLevelItem
	{
		get
		{
			return m_isTopLevelItem;
		}
		set
		{
			m_isTopLevelItem = value;
		}
	}

	internal bool CreatedByNavigationViewItemsFactory
	{
		get
		{
			return m_createdByNavigationViewItemsFactory;
		}
		set
		{
			m_createdByNavigationViewItemsFactory = value;
		}
	}

	internal event DependencyPropertyChangedCallback IsSelectedChanged;

	static NavigationViewItemBase()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationViewItemBase), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(NavigationViewItemBase));
		Control.HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NavigationViewItemBase), (PropertyMetadata)new FrameworkPropertyMetadata((object)(HorizontalAlignment)1));
		Control.VerticalContentAlignmentProperty.OverrideMetadata(typeof(NavigationViewItemBase), (PropertyMetadata)new FrameworkPropertyMetadata((object)(VerticalAlignment)1));
		KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(NavigationViewItemBase), (PropertyMetadata)new FrameworkPropertyMetadata((object)(KeyboardNavigationMode)3));
	}

	internal NavigationViewItemBase()
	{
	}

	private static void OnIsSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((NavigationViewItemBase)(object)sender).OnIsSelectedPropertyChanged(args);
	}

	private void OnIsSelectedPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnPropertyChangedPrivate(args);
		this.IsSelectedChanged?.Invoke((DependencyObject)(object)this, ((DependencyPropertyChangedEventArgs)(ref args)).Property);
	}

	private protected virtual void OnNavigationViewItemBasePositionChanged()
	{
	}

	internal NavigationView GetNavigationView()
	{
		if (m_navigationView != null && m_navigationView.TryGetTarget(out var target))
		{
			return target;
		}
		return null;
	}

	private protected virtual void OnNavigationViewItemBaseDepthChanged()
	{
	}

	private protected virtual void OnNavigationViewItemBaseIsSelectedChanged()
	{
	}

	internal SplitView GetSplitView()
	{
		SplitView result = null;
		NavigationView navigationView = GetNavigationView();
		if (navigationView != null)
		{
			result = navigationView.GetSplitView();
		}
		return result;
	}

	internal void SetNavigationViewParent(NavigationView navigationView)
	{
		m_navigationView = new WeakReference<NavigationView>(navigationView);
	}

	private void OnPropertyChangedPrivate(DependencyPropertyChangedEventArgs args)
	{
		if (((DependencyPropertyChangedEventArgs)(ref args)).Property == IsSelectedProperty)
		{
			OnNavigationViewItemBaseIsSelectedChanged();
		}
	}

	DependencyObject IControlProtected.GetTemplateChild(string childName)
	{
		return ((FrameworkElement)this).GetTemplateChild(childName);
	}
}
