using System;
using System.Windows;

namespace ModernWpf.Controls;

internal class RoutedEventHandlerRevoker
{
	private readonly WeakReference<UIElement> m_source;

	private readonly RoutedEvent m_event;

	private readonly WeakReference<Delegate> m_handler;

	public RoutedEventHandlerRevoker(UIElement source, RoutedEvent routedEvent, Delegate handler)
	{
		m_source = new WeakReference<UIElement>(source);
		m_event = routedEvent;
		m_handler = new WeakReference<Delegate>(handler);
		source.AddHandler(routedEvent, handler);
	}

	public void Revoke()
	{
		if (m_source.TryGetTarget(out var target) && m_handler.TryGetTarget(out var target2))
		{
			target.RemoveHandler(m_event, target2);
		}
	}
}
