using System;
using System.Windows;

namespace Hardcodet.Wpf.TaskbarNotification;

internal static class RoutedEventHelper
{
	internal static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
	{
		UIElement val = (UIElement)(object)((target is UIElement) ? target : null);
		if (val != null)
		{
			val.RaiseEvent(args);
			return;
		}
		ContentElement val2 = (ContentElement)(object)((target is ContentElement) ? target : null);
		if (val2 != null)
		{
			val2.RaiseEvent(args);
		}
	}

	internal static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
	{
		UIElement val = (UIElement)(object)((element is UIElement) ? element : null);
		if (val != null)
		{
			val.AddHandler(routedEvent, handler);
			return;
		}
		ContentElement val2 = (ContentElement)(object)((element is ContentElement) ? element : null);
		if (val2 != null)
		{
			val2.AddHandler(routedEvent, handler);
		}
	}

	internal static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
	{
		UIElement val = (UIElement)(object)((element is UIElement) ? element : null);
		if (val != null)
		{
			val.RemoveHandler(routedEvent, handler);
			return;
		}
		ContentElement val2 = (ContentElement)(object)((element is ContentElement) ? element : null);
		if (val2 != null)
		{
			val2.RemoveHandler(routedEvent, handler);
		}
	}
}
