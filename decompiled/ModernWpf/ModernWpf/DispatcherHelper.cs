using System;
using System.Windows.Threading;

namespace ModernWpf;

internal static class DispatcherHelper
{
	public static void DoEvents(DispatcherPriority priority = (DispatcherPriority)4)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		DispatcherFrame val = new DispatcherFrame();
		Dispatcher.CurrentDispatcher.BeginInvoke(priority, (Delegate)new DispatcherOperationCallback(ExitFrame), (object)val);
		Dispatcher.PushFrame(val);
	}

	private static object ExitFrame(object f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((DispatcherFrame)f).Continue = false;
		return null;
	}
}
