using System;
using System.Windows;
using System.Windows.Input;

namespace ModernWpf.Controls.Primitives;

[TemplatePart(Name = "PART_ToolBar", Type = typeof(CommandBarFlyoutToolBar))]
public class CommandBarFlyoutCommandBar : CommandBar
{
	private CommandBarFlyoutToolBar m_toolBar;

	private WeakReference<CommandBarFlyout> m_owningFlyout;

	internal WeakReference<CommandBarFlyout> OwningFlyout => m_owningFlyout;

	static CommandBarFlyoutCommandBar()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandBarFlyoutCommandBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(CommandBarFlyoutCommandBar)));
	}

	public CommandBarFlyoutCommandBar()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
	}

	internal void SetOwningFlyout(CommandBarFlyout owningFlyout)
	{
		m_owningFlyout = new WeakReference<CommandBarFlyout>(owningFlyout);
	}

	internal bool HasOpenAnimation()
	{
		if (m_toolBar == null)
		{
			return false;
		}
		return m_toolBar.HasOpenAnimation();
	}

	internal void PlayOpenAnimation()
	{
		m_toolBar?.PlayOpenAnimation();
	}

	internal bool HasCloseAnimation()
	{
		if (m_toolBar == null)
		{
			return false;
		}
		return m_toolBar.HasCloseAnimation();
	}

	internal void PlayCloseAnimation(Action onCompleteFunc)
	{
		m_toolBar?.PlayCloseAnimation(onCompleteFunc);
	}

	internal void ClearShadow()
	{
		m_toolBar?.ClearShadow();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		m_toolBar = ((FrameworkElement)this).GetTemplateChild("PART_ToolBar") as CommandBarFlyoutToolBar;
	}

	private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			InputManager.Current.PostProcessInput += new ProcessInputEventHandler(OnPostProcessInput);
		}
		else
		{
			InputManager.Current.PostProcessInput -= new ProcessInputEventHandler(OnPostProcessInput);
		}
	}

	private void OnPostProcessInput(object sender, ProcessInputEventArgs e)
	{
		if (((RoutedEventArgs)((NotifyInputEventArgs)e).StagingItem.Input).RoutedEvent == Mouse.MouseUpEvent)
		{
			((RoutedEventArgs)((NotifyInputEventArgs)e).StagingItem.Input).Handled = true;
		}
	}
}
