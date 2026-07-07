using System;
using System.ComponentModel;

namespace Hardcodet.Wpf.TaskbarNotification.Interop;

public class WindowMessageSink : IDisposable
{
	public const int CallbackMessageId = 1024;

	private uint taskbarRestartMessageId;

	private bool isDoubleClick;

	private WindowProcedureHandler messageHandler;

	internal string WindowId { get; private set; }

	internal IntPtr MessageWindowHandle { get; private set; }

	public NotifyIconVersion Version { get; set; }

	public bool IsDisposed { get; private set; }

	public event Action<bool> ChangeToolTipStateRequest;

	public event Action<MouseEvent> MouseEventReceived;

	public event Action<bool> BalloonToolTipChanged;

	public event Action TaskbarCreated;

	public WindowMessageSink(NotifyIconVersion version)
	{
		Version = version;
		CreateMessageWindow();
	}

	private WindowMessageSink()
	{
	}

	internal static WindowMessageSink CreateEmpty()
	{
		return new WindowMessageSink
		{
			MessageWindowHandle = IntPtr.Zero,
			Version = NotifyIconVersion.Vista
		};
	}

	private void CreateMessageWindow()
	{
		WindowId = "WPFTaskbarIcon_" + Guid.NewGuid();
		messageHandler = OnWindowMessageReceived;
		WindowClass lpWndClass = default(WindowClass);
		lpWndClass.style = 0u;
		lpWndClass.lpfnWndProc = messageHandler;
		lpWndClass.cbClsExtra = 0;
		lpWndClass.cbWndExtra = 0;
		lpWndClass.hInstance = IntPtr.Zero;
		lpWndClass.hIcon = IntPtr.Zero;
		lpWndClass.hCursor = IntPtr.Zero;
		lpWndClass.hbrBackground = IntPtr.Zero;
		lpWndClass.lpszMenuName = string.Empty;
		lpWndClass.lpszClassName = WindowId;
		WinApi.RegisterClass(ref lpWndClass);
		taskbarRestartMessageId = WinApi.RegisterWindowMessage("TaskbarCreated");
		MessageWindowHandle = WinApi.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		if (MessageWindowHandle == IntPtr.Zero)
		{
			throw new Win32Exception("Message window handle was not a valid pointer");
		}
	}

	private IntPtr OnWindowMessageReceived(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
	{
		if (messageId == taskbarRestartMessageId)
		{
			this.TaskbarCreated?.Invoke();
		}
		ProcessWindowMessage(messageId, wParam, lParam);
		return WinApi.DefWindowProc(hWnd, messageId, wParam, lParam);
	}

	private void ProcessWindowMessage(uint msg, IntPtr wParam, IntPtr lParam)
	{
		if (msg != 1024)
		{
			if (msg == 736)
			{
				SystemInfo.UpdateDpiFactors();
			}
			return;
		}
		switch ((WindowsMessages)lParam.ToInt32())
		{
		case WindowsMessages.WM_MOUSEMOVE:
			this.MouseEventReceived?.Invoke(MouseEvent.MouseMove);
			break;
		case WindowsMessages.WM_LBUTTONDOWN:
			this.MouseEventReceived?.Invoke(MouseEvent.IconLeftMouseDown);
			break;
		case WindowsMessages.WM_LBUTTONUP:
			if (!isDoubleClick)
			{
				this.MouseEventReceived?.Invoke(MouseEvent.IconLeftMouseUp);
			}
			isDoubleClick = false;
			break;
		case WindowsMessages.WM_LBUTTONDBLCLK:
			isDoubleClick = true;
			this.MouseEventReceived?.Invoke(MouseEvent.IconDoubleClick);
			break;
		case WindowsMessages.WM_RBUTTONDOWN:
			this.MouseEventReceived?.Invoke(MouseEvent.IconRightMouseDown);
			break;
		case WindowsMessages.WM_RBUTTONUP:
			this.MouseEventReceived?.Invoke(MouseEvent.IconRightMouseUp);
			break;
		case WindowsMessages.WM_MBUTTONDOWN:
			this.MouseEventReceived?.Invoke(MouseEvent.IconMiddleMouseDown);
			break;
		case WindowsMessages.WM_MBUTTONUP:
			this.MouseEventReceived?.Invoke(MouseEvent.IconMiddleMouseUp);
			break;
		case WindowsMessages.NIN_BALLOONSHOW:
			this.BalloonToolTipChanged?.Invoke(obj: true);
			break;
		case WindowsMessages.NIN_BALLOONHIDE:
		case WindowsMessages.NIN_BALLOONTIMEOUT:
			this.BalloonToolTipChanged?.Invoke(obj: false);
			break;
		case WindowsMessages.NIN_BALLOONUSERCLICK:
			this.MouseEventReceived?.Invoke(MouseEvent.BalloonToolTipClicked);
			break;
		case WindowsMessages.NIN_POPUPOPEN:
			this.ChangeToolTipStateRequest?.Invoke(obj: true);
			break;
		case WindowsMessages.NIN_POPUPCLOSE:
			this.ChangeToolTipStateRequest?.Invoke(obj: false);
			break;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~WindowMessageSink()
	{
		Dispose(disposing: false);
	}

	private void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			IsDisposed = true;
			WinApi.DestroyWindow(MessageWindowHandle);
			messageHandler = null;
		}
	}
}
