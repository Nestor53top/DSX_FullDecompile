using System;
using System.Windows;
using System.Windows.Threading;

namespace DualSenseX;

public static class DispatcherHelper
{
	public static void RunOnMainThread(Action action)
	{
		((DispatcherObject)(object)Application.Current).RunOnUIThread(action);
	}

	public static void RunOnUIThread(this DispatcherObject d, Action action)
	{
		Dispatcher dispatcher = d.Dispatcher;
		if (dispatcher.CheckAccess())
		{
			action();
		}
		else
		{
			dispatcher.BeginInvoke((Delegate)action, Array.Empty<object>());
		}
	}
}
