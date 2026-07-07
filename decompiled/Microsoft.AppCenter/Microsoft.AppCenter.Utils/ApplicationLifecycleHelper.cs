using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.AppCenter.Utils;

public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
{
	private delegate void WinEventDelegate(IntPtr winEventHookHandle, uint eventType, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds);

	private static IApplicationLifecycleHelper _instance;

	private const uint EVENT_SYSTEM_MINIMIZESTART = 22u;

	private const uint EVENT_SYSTEM_MINIMIZEEND = 23u;

	private const uint WINEVENT_OUTOFCONTEXT = 0u;

	private static WinEventDelegate hookDelegate;

	private static bool suspended;

	private static bool started;

	private static Action Minimize;

	private static Action Restore;

	private static Action Start;

	private static readonly dynamic WpfApplication;

	private static readonly int WpfMinimizedState;

	private bool enabled;

	public static IApplicationLifecycleHelper Instance
	{
		get
		{
			return _instance ?? (_instance = new ApplicationLifecycleHelper());
		}
		internal set
		{
			_instance = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			if (value != enabled)
			{
				if (value)
				{
					Start = InvokeStarted;
					Restore = InvokeResuming;
					Minimize = InvokeSuspended;
				}
				else
				{
					Start = null;
					Restore = null;
					Minimize = null;
				}
				enabled = value;
			}
		}
	}

	public bool HasShownWindow => started;

	public bool IsSuspended => suspended;

	public event EventHandler ApplicationSuspended;

	public event EventHandler ApplicationResuming;

	public event EventHandler ApplicationStarted;

	public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;

	[DllImport("user32.dll")]
	private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr eventHookAssemblyHandle, WinEventDelegate eventHookHandle, uint processId, uint threadId, uint dwFlags);

	[DllImport("user32.dll")]
	private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

	private static void WinEventHook(IntPtr winEventHookHandle, uint eventType, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds)
	{
		if (objectId == 0 && childId == 0)
		{
			bool flag = IsAnyWindowNotMinimized();
			if (!started && flag)
			{
				started = true;
				Start?.Invoke();
			}
			if (suspended && flag)
			{
				suspended = false;
				Restore?.Invoke();
			}
			else if (!suspended && !flag)
			{
				suspended = true;
				Minimize?.Invoke();
			}
		}
	}

	static ApplicationLifecycleHelper()
	{
		hookDelegate = WinEventHook;
		suspended = false;
		started = false;
		if (WpfHelper.IsRunningOnWpf)
		{
			Type type = WpfHelper.PresentationFramework.GetType("System.Windows.Application");
			WpfApplication = type.GetRuntimeProperty("Current")?.GetValue(type);
			WpfMinimizedState = (int)WpfHelper.PresentationFramework.GetType("System.Windows.WindowState").GetField("Minimized").GetRawConstantValue();
		}
		IntPtr hook = SetWinEventHook(22u, 23u, IntPtr.Zero, hookDelegate, (uint)Process.GetCurrentProcess().Id, 0u, 0u);
		Application.ApplicationExit += delegate
		{
			UnhookWinEvent(hook);
		};
	}

	private static bool IsAnyWindowNotMinimized()
	{
		if (WpfApplication == null)
		{
			return ((IEnumerable)Application.OpenForms).Cast<Form>().Any((Form form) => (int)form.WindowState != 1);
		}
		foreach (dynamic item in WpfApplication.Windows)
		{
			if ((int)item.WindowState != WpfMinimizedState && ApplicationLifecycleHelper.WindowIntersectsWithAnyScreen(item))
			{
				return true;
			}
		}
		return false;
	}

	public ApplicationLifecycleHelper()
	{
		Enabled = true;
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs eventArgs)
		{
			this.UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs((Exception)eventArgs.ExceptionObject));
		};
	}

	private void InvokeResuming()
	{
		this.ApplicationResuming?.Invoke(null, EventArgs.Empty);
	}

	private void InvokeStarted()
	{
		this.ApplicationStarted?.Invoke(null, EventArgs.Empty);
	}

	private void InvokeSuspended()
	{
		this.ApplicationSuspended?.Invoke(null, EventArgs.Empty);
	}

	private static Rectangle WindowsRectToRectangle(dynamic windowsRect)
	{
		return new Rectangle
		{
			X = (int)windowsRect.X,
			Y = (int)windowsRect.Y,
			Width = (int)windowsRect.Width,
			Height = (int)windowsRect.Height
		};
	}

	private static bool WindowIntersectsWithAnyScreen(dynamic window)
	{
		dynamic windowBounds = ApplicationLifecycleHelper.WindowsRectToRectangle(window.RestoreBounds);
		return Screen.AllScreens.Any((Screen screen) => screen.Bounds.IntersectsWith(windowBounds));
	}
}
