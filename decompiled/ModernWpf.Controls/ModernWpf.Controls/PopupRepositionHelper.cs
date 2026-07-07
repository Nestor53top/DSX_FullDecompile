using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ModernWpf.Controls;

internal class PopupRepositionHelper : IDisposable
{
	private readonly Popup m_popup;

	private readonly UIElement m_target;

	private bool m_isPopupOpen;

	private Point m_popupPosition;

	private Window m_hostWindow;

	public PopupRepositionHelper(Popup popup, UIElement target)
	{
		m_popup = popup;
		m_target = target;
		m_popup.Opened += OnPopupOpened;
		m_popup.Closed += OnPopupClosed;
	}

	public void Dispose()
	{
		m_popup.Opened -= OnPopupOpened;
		m_popup.Closed -= OnPopupClosed;
		m_target.LayoutUpdated -= OnTargetLayoutUpdated;
		OnPopupClosed(null, null);
	}

	private void OnPopupOpened(object sender, EventArgs e)
	{
		if (!m_isPopupOpen)
		{
			m_isPopupOpen = true;
			m_target.LayoutUpdated += OnTargetLayoutUpdated;
			m_hostWindow = Window.GetWindow((DependencyObject)(object)m_target);
			if (m_hostWindow != null)
			{
				m_hostWindow.LocationChanged += OnHostWindowLocationChanged;
			}
		}
	}

	private void OnPopupClosed(object sender, EventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		m_target.LayoutUpdated -= OnTargetLayoutUpdated;
		if (m_hostWindow != null)
		{
			m_hostWindow.LocationChanged -= OnHostWindowLocationChanged;
			m_hostWindow = null;
		}
		m_isPopupOpen = false;
		m_popupPosition = default(Point);
	}

	private void OnTargetLayoutUpdated(object sender, EventArgs e)
	{
		RepositionPopup();
	}

	private void OnHostWindowLocationChanged(object sender, EventArgs e)
	{
		RepositionPopup();
	}

	private void RepositionPopup()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (m_popup == null)
		{
			return;
		}
		UIElement child = m_popup.Child;
		if (child != null && child.IsVisible && m_target.IsVisible)
		{
			Point val = child.TranslatePoint(default(Point), m_target);
			if (m_popupPosition != val)
			{
				m_popupPosition = val;
				m_popup.Reposition();
			}
		}
	}
}
