using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[ContentProperty("Items")]
public class MenuFlyout : FlyoutBase
{
	public static readonly DependencyProperty MenuFlyoutPresenterStyleProperty = DependencyProperty.Register("MenuFlyoutPresenterStyle", typeof(Style), typeof(MenuFlyout), new PropertyMetadata(new PropertyChangedCallback(OnMenuFlyoutPresenterStyleChanged)));

	private MenuFlyoutPresenter m_presenter;

	private FlyoutPlacementMode? m_currentPlacement;

	public ItemCollection Items
	{
		get
		{
			EnsurePresenter();
			return ((ItemsControl)m_presenter).Items;
		}
	}

	public Style MenuFlyoutPresenterStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(MenuFlyoutPresenterStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MenuFlyoutPresenterStyleProperty, (object)value);
		}
	}

	private static void OnMenuFlyoutPresenterStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((MenuFlyout)(object)d).OnMenuFlyoutPresenterStyleChanged(e);
	}

	private void OnMenuFlyoutPresenterStyleChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if (m_presenter != null)
		{
			((FrameworkElement)m_presenter).Style = (Style)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
	}

	protected override Control CreatePresenter()
	{
		throw new InvalidOperationException();
	}

	internal override void ShowAtCore(FrameworkElement placementTarget, bool showAsContextFlyout = false)
	{
		if (showAsContextFlyout)
		{
			Show(placementTarget, (PlacementMode)8);
		}
		else
		{
			Show(placementTarget, (PlacementMode)11);
		}
	}

	internal override void HideCore()
	{
		if (m_presenter != null && ((ContextMenu)m_presenter).IsOpen)
		{
			((ContextMenu)m_presenter).IsOpen = false;
		}
	}

	internal override void OnIsOpenChanged()
	{
	}

	internal override void UpdateIsOpen()
	{
		base.IsOpen = m_presenter != null && ((ContextMenu)m_presenter).IsOpen;
	}

	internal override void OnAreOpenCloseAnimationsEnabledChanged(DependencyPropertyChangedEventArgs e)
	{
		m_presenter?.UpdatePopupAnimation();
	}

	private void Show(FrameworkElement placementTarget, PlacementMode placement = (PlacementMode)11)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (m_presenter == null || !((ContextMenu)m_presenter).IsOpen || (object)((ContextMenu)m_presenter).PlacementTarget != placementTarget || ((ContextMenu)m_presenter).Placement != placement || m_currentPlacement != base.Placement)
		{
			EnsurePresenter();
			if (((ContextMenu)m_presenter).IsOpen)
			{
				((ContextMenu)m_presenter).IsOpen = false;
			}
			((ContextMenu)m_presenter).Placement = placement;
			((ContextMenu)m_presenter).PlacementTarget = (UIElement)(object)placementTarget;
			if ((int)placement == 11)
			{
				((ContextMenu)m_presenter).PlacementRectangle = GetPlacementRectangle((UIElement)(object)placementTarget);
			}
			else
			{
				((DependencyObject)m_presenter).ClearValue(Popup.PlacementRectangleProperty);
			}
			m_currentPlacement = base.Placement;
			OnOpening();
			((ContextMenu)m_presenter).IsOpen = true;
		}
	}

	private CustomPopupPlacement[] PositionPopup(Size popupSize, Size targetSize, Point offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return PositionPopup(popupSize, targetSize, offset, (FrameworkElement)(object)m_presenter);
	}

	private void EnsurePresenter()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		if (m_presenter == null)
		{
			MenuFlyoutPresenter menuFlyoutPresenter = new MenuFlyoutPresenter();
			((FrameworkElement)menuFlyoutPresenter).Style = MenuFlyoutPresenterStyle;
			((ContextMenu)menuFlyoutPresenter).Placement = (PlacementMode)11;
			((ContextMenu)menuFlyoutPresenter).CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PositionPopup);
			((ContextMenu)menuFlyoutPresenter).StaysOpen = false;
			MenuFlyoutPresenter menuFlyoutPresenter2 = menuFlyoutPresenter;
			menuFlyoutPresenter2.SetOwningFlyout(this);
			BindPlacement((Control)(object)menuFlyoutPresenter2);
			menuFlyoutPresenter2.UpdatePopupAnimation();
			((ContextMenu)menuFlyoutPresenter2).Opened += new RoutedEventHandler(OnPresenterOpened);
			((ContextMenu)menuFlyoutPresenter2).Closed += new RoutedEventHandler(OnPresenterClosed);
			menuFlyoutPresenter2.IsOpenChanged += OnPresenterIsOpenChanged;
			m_presenter = menuFlyoutPresenter2;
		}
	}

	private void OnPresenterOpened(object sender, RoutedEventArgs e)
	{
		OnOpened();
	}

	private void OnPresenterClosed(object sender, RoutedEventArgs e)
	{
		if (!((ContextMenu)m_presenter).IsOpen)
		{
			((DependencyObject)m_presenter).ClearValue(ContextMenu.PlacementProperty);
			((DependencyObject)m_presenter).ClearValue(ContextMenu.PlacementTargetProperty);
			((DependencyObject)m_presenter).ClearValue(ContextMenu.PlacementRectangleProperty);
			m_currentPlacement = null;
		}
		OnClosed();
	}

	private void OnPresenterIsOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		UpdateIsOpen();
	}
}
