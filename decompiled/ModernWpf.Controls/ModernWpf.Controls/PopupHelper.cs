using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

internal static class PopupHelper
{
	private static readonly Lazy<Action<Popup>> s_reposition = new Lazy<Action<Popup>>(CreateRepositionDelegate);

	public static void Reposition(this Popup popup)
	{
		if (popup == null)
		{
			throw new ArgumentNullException("popup");
		}
		PopupPositioner positioner = PopupPositioner.GetPositioner(popup);
		if (positioner != null)
		{
			positioner.Reposition();
			return;
		}
		Action<Popup> value = s_reposition.Value;
		if (value != null)
		{
			value(popup);
			return;
		}
		double horizontalOffset = popup.HorizontalOffset;
		((DependencyObject)popup).SetCurrentValue(Popup.HorizontalOffsetProperty, (object)(horizontalOffset + 0.1));
		((DependencyObject)popup).InvalidateProperty(Popup.HorizontalOffsetProperty);
	}

	private static Action<Popup> CreateRepositionDelegate()
	{
		try
		{
			return DelegateHelper.CreateDelegate<Action<Popup>>(typeof(Popup), "Reposition", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		catch
		{
			return null;
		}
	}
}
