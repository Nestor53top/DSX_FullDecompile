using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[ContentProperty("PrimaryCommands")]
[TemplatePart(Name = "PART_ToolBar", Type = typeof(CommandBarToolBar))]
public class CommandBar : Control
{
	public static readonly DependencyProperty ContentProperty;

	public static readonly DependencyProperty ContentTemplateProperty;

	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty IsOpenProperty;

	public static readonly DependencyProperty CommandBarOverflowPresenterStyleProperty;

	public static readonly DependencyProperty DefaultLabelPositionProperty;

	public static readonly DependencyProperty IsDynamicOverflowEnabledProperty;

	public static readonly DependencyProperty OverflowButtonVisibilityProperty;

	private CommandBarToolBar m_toolBar;

	internal const string ToolBarName = "PART_ToolBar";

	public object Content
	{
		get
		{
			return ((DependencyObject)this).GetValue(ContentProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentProperty, value);
		}
	}

	public DataTemplate ContentTemplate
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (DataTemplate)((DependencyObject)this).GetValue(ContentTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentTemplateProperty, (object)value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (CornerRadius)((DependencyObject)this).GetValue(CornerRadiusProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CornerRadiusProperty, (object)value);
		}
	}

	public bool IsOpen
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsOpenProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsOpenProperty, (object)value);
		}
	}

	public ObservableCollection<ICommandBarElement> PrimaryCommands { get; }

	private bool HasPrimaryCommands
	{
		get
		{
			if (m_toolBar != null)
			{
				return m_toolBar.HasPrimaryCommands;
			}
			return false;
		}
	}

	public ObservableCollection<ICommandBarElement> SecondaryCommands { get; }

	private bool HasSecondaryCommands
	{
		get
		{
			if (m_toolBar != null)
			{
				return ((ToolBar)m_toolBar).HasOverflowItems;
			}
			return false;
		}
	}

	public Style CommandBarOverflowPresenterStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(CommandBarOverflowPresenterStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CommandBarOverflowPresenterStyleProperty, (object)value);
		}
	}

	public CommandBarDefaultLabelPosition DefaultLabelPosition
	{
		get
		{
			return (CommandBarDefaultLabelPosition)((DependencyObject)this).GetValue(DefaultLabelPositionProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DefaultLabelPositionProperty, (object)value);
		}
	}

	public bool IsDynamicOverflowEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsDynamicOverflowEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsDynamicOverflowEnabledProperty, (object)value);
		}
	}

	public CommandBarOverflowButtonVisibility OverflowButtonVisibility
	{
		get
		{
			return (CommandBarOverflowButtonVisibility)((DependencyObject)this).GetValue(OverflowButtonVisibilityProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OverflowButtonVisibilityProperty, (object)value);
		}
	}

	public event EventHandler<object> Opened;

	public event EventHandler<object> Closed;

	static CommandBar()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(CommandBar));
		ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(CommandBar));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(CommandBar));
		IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(CommandBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (FrameworkPropertyMetadataOptions)256));
		CommandBarOverflowPresenterStyleProperty = DependencyProperty.Register("CommandBarOverflowPresenterStyle", typeof(Style), typeof(CommandBar), (PropertyMetadata)null);
		DefaultLabelPositionProperty = CommandBarToolBar.DefaultLabelPositionProperty.AddOwner(typeof(CommandBar));
		IsDynamicOverflowEnabledProperty = CommandBarToolBar.IsDynamicOverflowEnabledProperty.AddOwner(typeof(CommandBar), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsDynamicOverflowEnabledChanged)));
		OverflowButtonVisibilityProperty = CommandBarToolBar.OverflowButtonVisibilityProperty.AddOwner(typeof(CommandBar));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(CommandBar)));
	}

	public CommandBar()
	{
		PrimaryCommands = new ObservableCollection<ICommandBarElement>();
		PrimaryCommands.CollectionChanged += PrimaryCommands_CollectionChanged;
		SecondaryCommands = new ObservableCollection<ICommandBarElement>();
		SecondaryCommands.CollectionChanged += SecondaryCommands_CollectionChanged;
	}

	private void PrimaryCommands_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems != null)
		{
			UpdateOverflowModeForPrimaryCommands(e.NewItems.OfType<DependencyObject>());
		}
	}

	private void SecondaryCommands_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems != null)
		{
			UpdateOverflowModeForSecondaryCommands(e.NewItems.OfType<DependencyObject>());
		}
	}

	private static void OnIsDynamicOverflowEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((CommandBar)(object)d).OnIsDynamicOverflowEnabledChanged();
	}

	private void OnIsDynamicOverflowEnabledChanged()
	{
		UpdateOverflowModeForPrimaryCommands(PrimaryCommands.OfType<DependencyObject>());
		UpdateOverflowModeForSecondaryCommands(SecondaryCommands.OfType<DependencyObject>());
	}

	private void UpdateOverflowModeForPrimaryCommands(IEnumerable<DependencyObject> items)
	{
		bool isDynamicOverflowEnabled = IsDynamicOverflowEnabled;
		foreach (DependencyObject item in items)
		{
			ToolBar.SetOverflowMode(item, (OverflowMode)((!isDynamicOverflowEnabled) ? 2 : 0));
		}
	}

	private void UpdateOverflowModeForSecondaryCommands(IEnumerable<DependencyObject> items)
	{
		foreach (DependencyObject item in items)
		{
			ToolBar.SetOverflowMode(item, (OverflowMode)1);
		}
	}

	public override void OnApplyTemplate()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00db: Expected O, but got Unknown
		if (m_toolBar != null)
		{
			((DependencyObject)m_toolBar).ClearValue(ItemsControl.ItemsSourceProperty);
			m_toolBar.OverflowOpened -= OnOverflowOpened;
			m_toolBar.OverflowClosed -= OnOverflowClosed;
			m_toolBar.HasPrimaryCommandsChanged -= OnHasPrimaryCommandsChanged;
			m_toolBar.HasOverflowItemsChanged -= OnHasOverflowItemsChanged;
		}
		((FrameworkElement)this).OnApplyTemplate();
		m_toolBar = ((FrameworkElement)this).GetTemplateChild("PART_ToolBar") as CommandBarToolBar;
		if (m_toolBar != null)
		{
			CommandBarToolBar toolBar = m_toolBar;
			CompositeCollection val = new CompositeCollection();
			val.Add((object)new CollectionContainer
			{
				Collection = PrimaryCommands
			});
			val.Add((object)new CollectionContainer
			{
				Collection = SecondaryCommands
			});
			((ItemsControl)toolBar).ItemsSource = (IEnumerable)val;
			m_toolBar.OverflowOpened += OnOverflowOpened;
			m_toolBar.OverflowClosed += OnOverflowClosed;
			m_toolBar.HasPrimaryCommandsChanged += OnHasPrimaryCommandsChanged;
			m_toolBar.HasOverflowItemsChanged += OnHasOverflowItemsChanged;
		}
		UpdateVisualState(useTransitions: false);
	}

	private void OnOverflowOpened(object sender, EventArgs e)
	{
		this.Opened?.Invoke(this, null);
	}

	private void OnOverflowClosed(object sender, EventArgs e)
	{
		this.Closed?.Invoke(this, null);
	}

	private void OnHasPrimaryCommandsChanged(object sender, EventArgs e)
	{
		UpdateVisualState();
	}

	private void OnHasOverflowItemsChanged(object sender, EventArgs e)
	{
		UpdateVisualState();
	}

	internal void UpdateVisualState(bool useTransitions = true)
	{
		if (m_toolBar != null)
		{
			string text = ((HasPrimaryCommands && HasSecondaryCommands) ? "BothCommands" : ((!HasSecondaryCommands) ? "PrimaryCommandsOnly" : "SecondaryCommandsOnly"));
			VisualStateManager.GoToState((FrameworkElement)(object)m_toolBar, text, useTransitions);
		}
	}
}
