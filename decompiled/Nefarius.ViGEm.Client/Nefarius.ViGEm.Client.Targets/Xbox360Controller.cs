using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using Nefarius.ViGEm.Client.Targets.Xbox360.Exceptions;

namespace Nefarius.ViGEm.Client.Targets;

internal class Xbox360Controller : ViGEmTarget, IXbox360Controller, IVirtualGamepad
{
	private static readonly List<Xbox360Button> ButtonMap = new List<Xbox360Button>
	{
		Xbox360Button.Up,
		Xbox360Button.Down,
		Xbox360Button.Left,
		Xbox360Button.Right,
		Xbox360Button.Start,
		Xbox360Button.Back,
		Xbox360Button.LeftThumb,
		Xbox360Button.RightThumb,
		Xbox360Button.LeftShoulder,
		Xbox360Button.RightShoulder,
		Xbox360Button.Guide,
		Xbox360Button.A,
		Xbox360Button.B,
		Xbox360Button.X,
		Xbox360Button.Y
	};

	private static readonly List<Xbox360Axis> AxisMap = new List<Xbox360Axis>
	{
		Xbox360Axis.LeftThumbX,
		Xbox360Axis.LeftThumbY,
		Xbox360Axis.RightThumbX,
		Xbox360Axis.RightThumbY
	};

	private static readonly List<Xbox360Slider> SliderMap = new List<Xbox360Slider>
	{
		Xbox360Slider.LeftTrigger,
		Xbox360Slider.RightTrigger
	};

	private ViGEmClient.XUSB_REPORT _nativeReport;

	private ViGEmClient.PVIGEM_X360_NOTIFICATION _notificationCallback;

	private int _userIndex = -1;

	public int ButtonCount => ButtonMap.Count;

	public int AxisCount => AxisMap.Count;

	public int SliderCount => SliderMap.Count;

	public bool AutoSubmitReport { get; set; } = true;

	public int UserIndex
	{
		get
		{
			if (_userIndex == -1)
			{
				throw new Xbox360UserIndexNotReportedException();
			}
			return _userIndex;
		}
		private set
		{
			_userIndex = value;
		}
	}

	public event Xbox360FeedbackReceivedEventHandler FeedbackReceived;

	public Xbox360Controller(ViGEmClient client)
		: base(client)
	{
		base.NativeHandle = ViGEmClient.vigem_target_x360_alloc();
	}

	public Xbox360Controller(ViGEmClient client, ushort vendorId, ushort productId)
		: this(client)
	{
		base.VendorId = vendorId;
		base.ProductId = productId;
	}

	public override void Connect()
	{
		base.Connect();
		_notificationCallback = delegate(IntPtr client, IntPtr target, byte largeMotor, byte smallMotor, byte number, IntPtr userData)
		{
			UserIndex = number;
			this.FeedbackReceived?.Invoke(this, new Xbox360FeedbackReceivedEventArgs(largeMotor, smallMotor, number));
		};
		switch (ViGEmClient.vigem_target_x360_register_notification(base.Client.NativeHandle, base.NativeHandle, _notificationCallback))
		{
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
			throw new VigemBusNotFoundException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
			throw new VigemInvalidTargetException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_CALLBACK_ALREADY_REGISTERED:
			throw new VigemCallbackAlreadyRegisteredException();
		default:
			throw new Win32Exception(Marshal.GetLastWin32Error());
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NONE:
			break;
		}
	}

	public override void Disconnect()
	{
		ViGEmClient.vigem_target_x360_unregister_notification(base.NativeHandle);
		base.Disconnect();
	}

	public void SetButtonState(int index, bool pressed)
	{
		SetButtonState(ButtonMap[index], pressed);
	}

	public void SetAxisValue(int index, short value)
	{
		SetAxisValue(AxisMap[index], value);
	}

	public void SetSliderValue(int index, byte value)
	{
		SetSliderValue(SliderMap[index], value);
	}

	public void ResetReport()
	{
		_nativeReport = default(ViGEmClient.XUSB_REPORT);
	}

	public void SubmitReport()
	{
		SubmitNativeReport(_nativeReport);
	}

	private void SubmitNativeReport(ViGEmClient.XUSB_REPORT report)
	{
		switch (ViGEmClient.vigem_target_x360_update(base.Client.NativeHandle, base.NativeHandle, report))
		{
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_INVALID_HANDLE:
			throw new VigemBusInvalidHandleException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
			throw new VigemInvalidTargetException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
			throw new VigemBusNotFoundException();
		default:
			throw new Win32Exception(Marshal.GetLastWin32Error());
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NONE:
			break;
		}
	}

	public void SetButtonState(Xbox360Button button, bool pressed)
	{
		if (pressed)
		{
			_nativeReport.wButtons |= button.Value;
		}
		else
		{
			_nativeReport.wButtons &= (ushort)(~button.Value);
		}
		if (AutoSubmitReport)
		{
			SubmitNativeReport(_nativeReport);
		}
	}

	public void SetAxisValue(Xbox360Axis axis, short value)
	{
		switch (axis.Name)
		{
		case "LeftThumbX":
			_nativeReport.sThumbLX = value;
			break;
		case "LeftThumbY":
			_nativeReport.sThumbLY = value;
			break;
		case "RightThumbX":
			_nativeReport.sThumbRX = value;
			break;
		case "RightThumbY":
			_nativeReport.sThumbRY = value;
			break;
		}
		if (AutoSubmitReport)
		{
			SubmitNativeReport(_nativeReport);
		}
	}

	public void SetSliderValue(Xbox360Slider slider, byte value)
	{
		string name = slider.Name;
		if (!(name == "LeftTrigger"))
		{
			if (name == "RightTrigger")
			{
				_nativeReport.bRightTrigger = value;
			}
		}
		else
		{
			_nativeReport.bLeftTrigger = value;
		}
		if (AutoSubmitReport)
		{
			SubmitNativeReport(_nativeReport);
		}
	}
}
