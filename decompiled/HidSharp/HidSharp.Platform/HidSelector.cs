using System.Threading;
using HidSharp.Platform.Linux;
using HidSharp.Platform.MacOS;
using HidSharp.Platform.Unsupported;
using HidSharp.Platform.Windows;

namespace HidSharp.Platform;

internal sealed class HidSelector
{
	public static readonly HidManager Instance;

	private static readonly Thread ManagerThread;

	static HidSelector()
	{
		HidManager[] array = new HidManager[4]
		{
			new WinHidManager(),
			new LinuxHidManager(),
			new MacHidManager(),
			new UnsupportedHidManager()
		};
		foreach (HidManager hidManager in array)
		{
			if (hidManager.IsSupported)
			{
				ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
				Instance = hidManager;
				Instance.InitializeEventManager();
				ManagerThread = new Thread(Instance.RunImpl)
				{
					IsBackground = true,
					Name = "HID Manager"
				};
				ManagerThread.Start(manualResetEvent);
				manualResetEvent.WaitOne();
				break;
			}
		}
	}
}
