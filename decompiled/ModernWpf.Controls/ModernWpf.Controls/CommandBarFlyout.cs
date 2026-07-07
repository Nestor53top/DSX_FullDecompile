using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[ContentProperty("PrimaryCommands")]
public class CommandBarFlyout : FlyoutBase
{
	private CommandBarFlyoutCommandBar m_commandBar;

	private Dictionary<ICommandBarElement, RoutedEventHandlerRevoker> m_secondaryButtonClickRevokerByElementMap = new Dictionary<ICommandBarElement, RoutedEventHandlerRevoker>();

	private Dictionary<ICommandBarElement, RoutedEventHandlerRevoker> m_secondaryToggleButtonCheckedRevokerByElementMap = new Dictionary<ICommandBarElement, RoutedEventHandlerRevoker>();

	private Dictionary<ICommandBarElement, RoutedEventHandlerRevoker> m_secondaryToggleButtonUncheckedRevokerByElementMap = new Dictionary<ICommandBarElement, RoutedEventHandlerRevoker>();

	private bool m_isClosingAfterCloseAnimation;

	public ObservableCollection<ICommandBarElement> PrimaryCommands { get; }

	public ObservableCollection<ICommandBarElement> SecondaryCommands { get; }

	internal override PopupAnimation DesiredPopupAnimation => (PopupAnimation)1;

	public CommandBarFlyout()
	{
		PrimaryCommands = new ObservableCollection<ICommandBarElement>();
		SecondaryCommands = new ObservableCollection<ICommandBarElement>();
		PrimaryCommands.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (m_commandBar != null)
			{
				SharedHelpers.ForwardCollectionChange((ObservableCollection<ICommandBarElement>)sender, m_commandBar.PrimaryCommands, args);
			}
		};
		SecondaryCommands.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs args)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			CommandBarFlyoutCommandBar commandBar = m_commandBar;
			if (commandBar != null)
			{
				SharedHelpers.ForwardCollectionChange((ObservableCollection<ICommandBarElement>)sender, commandBar.SecondaryCommands, args);
				RoutedEventHandler handler = (RoutedEventHandler)delegate
				{
					Hide();
				};
				switch (args.Action)
				{
				case NotifyCollectionChangedAction.Replace:
				{
					ICommandBarElement commandBarElement2 = (ICommandBarElement)args.NewItems[0];
					ICommandBarElement element2 = (ICommandBarElement)args.OldItems[0];
					AppBarButton appBarButton2 = commandBarElement2 as AppBarButton;
					AppBarToggleButton appBarToggleButton2 = commandBarElement2 as AppBarToggleButton;
					RevokeAndRemove(m_secondaryButtonClickRevokerByElementMap, element2);
					RevokeAndRemove(m_secondaryToggleButtonCheckedRevokerByElementMap, element2);
					RevokeAndRemove(m_secondaryToggleButtonUncheckedRevokerByElementMap, element2);
					if (appBarButton2 != null && appBarButton2.Flyout == null)
					{
						m_secondaryButtonClickRevokerByElementMap[commandBarElement2] = new RoutedEventHandlerRevoker((UIElement)(object)appBarButton2, ButtonBase.ClickEvent, (Delegate)(object)handler);
						RevokeAndRemove(m_secondaryToggleButtonCheckedRevokerByElementMap, commandBarElement2);
						RevokeAndRemove(m_secondaryToggleButtonUncheckedRevokerByElementMap, commandBarElement2);
					}
					else if (appBarToggleButton2 != null)
					{
						RevokeAndRemove(m_secondaryButtonClickRevokerByElementMap, commandBarElement2);
						m_secondaryToggleButtonCheckedRevokerByElementMap[commandBarElement2] = new RoutedEventHandlerRevoker((UIElement)(object)appBarToggleButton2, ToggleButton.CheckedEvent, (Delegate)(object)handler);
						m_secondaryToggleButtonUncheckedRevokerByElementMap[commandBarElement2] = new RoutedEventHandlerRevoker((UIElement)(object)appBarToggleButton2, ToggleButton.UncheckedEvent, (Delegate)(object)handler);
					}
					else
					{
						RevokeAndRemove(m_secondaryButtonClickRevokerByElementMap, commandBarElement2);
						RevokeAndRemove(m_secondaryToggleButtonCheckedRevokerByElementMap, commandBarElement2);
						RevokeAndRemove(m_secondaryToggleButtonUncheckedRevokerByElementMap, commandBarElement2);
					}
					break;
				}
				case NotifyCollectionChangedAction.Add:
				{
					ICommandBarElement commandBarElement = (ICommandBarElement)args.NewItems[0];
					AppBarButton appBarButton = commandBarElement as AppBarButton;
					AppBarToggleButton appBarToggleButton = commandBarElement as AppBarToggleButton;
					if (appBarButton != null && appBarButton.Flyout == null)
					{
						m_secondaryButtonClickRevokerByElementMap[commandBarElement] = new RoutedEventHandlerRevoker((UIElement)(object)appBarButton, ButtonBase.ClickEvent, (Delegate)(object)handler);
					}
					else if (appBarToggleButton != null)
					{
						m_secondaryToggleButtonCheckedRevokerByElementMap[commandBarElement] = new RoutedEventHandlerRevoker((UIElement)(object)appBarToggleButton, ToggleButton.CheckedEvent, (Delegate)(object)handler);
						m_secondaryToggleButtonUncheckedRevokerByElementMap[commandBarElement] = new RoutedEventHandlerRevoker((UIElement)(object)appBarToggleButton, ToggleButton.UncheckedEvent, (Delegate)(object)handler);
					}
					break;
				}
				case NotifyCollectionChangedAction.Remove:
				{
					ICommandBarElement element = (ICommandBarElement)args.OldItems[0];
					RevokeAndRemove(m_secondaryButtonClickRevokerByElementMap, element);
					RevokeAndRemove(m_secondaryToggleButtonCheckedRevokerByElementMap, element);
					RevokeAndRemove(m_secondaryToggleButtonUncheckedRevokerByElementMap, element);
					break;
				}
				case NotifyCollectionChangedAction.Reset:
					SetSecondaryCommandsToCloseWhenExecuted();
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				}
			}
		};
		base.Opening += delegate
		{
			base.InternalPopup.SuppressFadeAnimation = true;
			if (base.ShowMode == FlyoutShowMode.Standard)
			{
				m_commandBar.IsOpen = true;
			}
		};
		base.Opened += delegate
		{
			if (m_commandBar != null && m_commandBar.HasOpenAnimation())
			{
				m_commandBar.PlayOpenAnimation();
			}
		};
		base.Closing += delegate
		{
			CommandBarFlyoutCommandBar commandBar = m_commandBar;
			if (commandBar != null)
			{
				if (!m_isClosingAfterCloseAnimation && commandBar.HasCloseAnimation())
				{
					commandBar.PlayCloseAnimation(delegate
					{
						m_isClosingAfterCloseAnimation = true;
						Hide();
						m_isClosingAfterCloseAnimation = false;
					});
				}
				commandBar.IsOpen = false;
				commandBar.ClearShadow();
			}
		};
		base.Closed += delegate
		{
			if (m_commandBar != null && m_commandBar.IsOpen)
			{
				m_commandBar.IsOpen = false;
			}
		};
	}

	protected override Control CreatePresenter()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		CommandBarFlyoutCommandBar commandBarFlyoutCommandBar = new CommandBarFlyoutCommandBar();
		commandBarFlyoutCommandBar.Opened += delegate
		{
			((DependencyObject)this).SetCurrentValue(FlyoutBase.ShowModeProperty, (object)FlyoutShowMode.Standard);
		};
		SharedHelpers.CopyList(PrimaryCommands, commandBarFlyoutCommandBar.PrimaryCommands);
		SharedHelpers.CopyList(SecondaryCommands, commandBarFlyoutCommandBar.SecondaryCommands);
		SetSecondaryCommandsToCloseWhenExecuted();
		FlyoutPresenter flyoutPresenter = new FlyoutPresenter();
		((Control)flyoutPresenter).Background = null;
		((Control)flyoutPresenter).Foreground = null;
		((Control)flyoutPresenter).BorderBrush = null;
		((FrameworkElement)flyoutPresenter).MinWidth = 0.0;
		((FrameworkElement)flyoutPresenter).MaxWidth = double.PositiveInfinity;
		((FrameworkElement)flyoutPresenter).MinHeight = 0.0;
		((FrameworkElement)flyoutPresenter).MaxHeight = double.PositiveInfinity;
		((Control)flyoutPresenter).BorderThickness = new Thickness(0.0);
		((Control)flyoutPresenter).Padding = new Thickness(0.0);
		((ContentControl)flyoutPresenter).Content = commandBarFlyoutCommandBar;
		flyoutPresenter.CornerRadius = new CornerRadius(0.0);
		flyoutPresenter.IsDefaultShadowEnabled = false;
		commandBarFlyoutCommandBar.SetOwningFlyout(this);
		m_commandBar = commandBarFlyoutCommandBar;
		return (Control)(object)flyoutPresenter;
	}

	private void SetSecondaryCommandsToCloseWhenExecuted()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		RevokeAndClear(m_secondaryButtonClickRevokerByElementMap);
		RevokeAndClear(m_secondaryToggleButtonCheckedRevokerByElementMap);
		RevokeAndClear(m_secondaryToggleButtonUncheckedRevokerByElementMap);
		RoutedEventHandler handler = (RoutedEventHandler)delegate
		{
			Hide();
		};
		for (int num = 0; num < SecondaryCommands.Count; num++)
		{
			ICommandBarElement commandBarElement = SecondaryCommands[num];
			AppBarButton appBarButton = commandBarElement as AppBarButton;
			AppBarToggleButton appBarToggleButton = commandBarElement as AppBarToggleButton;
			if (appBarButton != null && appBarButton.Flyout == null)
			{
				m_secondaryButtonClickRevokerByElementMap[commandBarElement] = new RoutedEventHandlerRevoker((UIElement)(object)appBarButton, ButtonBase.ClickEvent, (Delegate)(object)handler);
			}
			else if (appBarToggleButton != null)
			{
				m_secondaryToggleButtonCheckedRevokerByElementMap[commandBarElement] = new RoutedEventHandlerRevoker((UIElement)(object)appBarToggleButton, ToggleButton.CheckedEvent, (Delegate)(object)handler);
				m_secondaryToggleButtonUncheckedRevokerByElementMap[commandBarElement] = new RoutedEventHandlerRevoker((UIElement)(object)appBarToggleButton, ToggleButton.UncheckedEvent, (Delegate)(object)handler);
			}
		}
	}

	private static void RevokeAndRemove(IDictionary<ICommandBarElement, RoutedEventHandlerRevoker> map, ICommandBarElement element)
	{
		if (map.TryGetValue(element, out var value))
		{
			value.Revoke();
			map.Remove(element);
		}
	}

	private static void RevokeAndClear(IDictionary<ICommandBarElement, RoutedEventHandlerRevoker> map)
	{
		foreach (RoutedEventHandlerRevoker value in map.Values)
		{
			value.Revoke();
		}
		map.Clear();
	}
}
