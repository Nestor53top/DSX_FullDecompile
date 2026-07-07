using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class MenuFlyoutPresenter : ContextMenu
{
	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty IsDefaultShadowEnabledProperty;

	private Popup _parentPopup;

	private WeakReference<MenuFlyout> m_owningFlyout;

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

	public bool IsDefaultShadowEnabled
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsDefaultShadowEnabledProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsDefaultShadowEnabledProperty, (object)value);
		}
	}

	internal event EventHandler<DependencyPropertyChangedEventArgs> IsOpenChanged;

	static MenuFlyoutPresenter()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(MenuFlyoutPresenter));
		IsDefaultShadowEnabledProperty = DependencyProperty.Register("IsDefaultShadowEnabled", typeof(bool), typeof(MenuFlyoutPresenter), new PropertyMetadata((object)true));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuFlyoutPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(MenuFlyoutPresenter)));
		ContextMenu.IsOpenProperty.OverrideMetadata(typeof(MenuFlyoutPresenter), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsOpenChanged)));
	}

	protected override void OnVisualParentChanged(DependencyObject oldParent)
	{
		((ContextMenu)this).OnVisualParentChanged(oldParent);
		if (_parentPopup == null)
		{
			HookupParentPopup();
		}
	}

	internal void SetOwningFlyout(MenuFlyout owningFlyout)
	{
		m_owningFlyout = new WeakReference<MenuFlyout>(owningFlyout);
	}

	internal void UpdatePopupAnimation()
	{
		if (_parentPopup != null && m_owningFlyout.TryGetTarget(out var target))
		{
			if (target.AreOpenCloseAnimationsEnabled)
			{
				((FrameworkElement)_parentPopup).Resources.Remove((object)SystemParameters.MenuPopupAnimationKey);
			}
			else
			{
				((FrameworkElement)_parentPopup).Resources[(object)SystemParameters.MenuPopupAnimationKey] = (object)(PopupAnimation)0;
			}
		}
	}

	private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((MenuFlyoutPresenter)(object)d).OnIsOpenChanged(e);
	}

	private void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		this.IsOpenChanged?.Invoke(this, e);
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue && _parentPopup == null)
		{
			HookupParentPopup();
		}
	}

	private void HookupParentPopup()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		DependencyObject parent = ((FrameworkElement)this).Parent;
		_parentPopup = (Popup)(object)((parent is Popup) ? parent : null);
		if (_parentPopup != null)
		{
			((UIElement)_parentPopup).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(HandlePopupMouseButtonEvent);
			((UIElement)_parentPopup).PreviewMouseRightButtonDown += new MouseButtonEventHandler(HandlePopupMouseButtonEvent);
			((UIElement)_parentPopup).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(HandlePopupMouseButtonEvent);
			((UIElement)_parentPopup).PreviewMouseRightButtonUp += new MouseButtonEventHandler(HandlePopupMouseButtonEvent);
			UpdatePopupAnimation();
		}
	}

	private void HandlePopupMouseButtonEvent(object sender, MouseButtonEventArgs e)
	{
		if (!_parentPopup.IsOpen)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}
}
