using System.ComponentModel;

namespace System.Windows.Threading;

public static class DispatcherExtensions
{
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action)
	{
		return dispatcher.BeginInvoke((Delegate)action, Array.Empty<object>());
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return dispatcher.BeginInvoke((Delegate)action, priority, Array.Empty<object>());
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Invoke(this Dispatcher dispatcher, Action action)
	{
		dispatcher.Invoke(action);
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Invoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		dispatcher.Invoke(action, priority);
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Invoke(this Dispatcher dispatcher, Action action, TimeSpan timeout)
	{
		dispatcher.Invoke((Delegate)action, timeout, Array.Empty<object>());
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Invoke(this Dispatcher dispatcher, Action action, TimeSpan timeout, DispatcherPriority priority)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		dispatcher.Invoke((Delegate)action, timeout, priority, Array.Empty<object>());
	}
}
