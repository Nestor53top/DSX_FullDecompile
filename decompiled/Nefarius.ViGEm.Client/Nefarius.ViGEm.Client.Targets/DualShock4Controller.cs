using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Utilities;

namespace Nefarius.ViGEm.Client.Targets;

internal class DualShock4Controller : ViGEmTarget, IDualShock4Controller, IVirtualGamepad
{
	private static readonly List<DualShock4Button> ButtonMap = new List<DualShock4Button>
	{
		DualShock4Button.ThumbRight,
		DualShock4Button.ThumbLeft,
		DualShock4Button.Options,
		DualShock4Button.Share,
		DualShock4Button.TriggerRight,
		DualShock4Button.TriggerLeft,
		DualShock4Button.ShoulderRight,
		DualShock4Button.ShoulderLeft,
		DualShock4Button.Triangle,
		DualShock4Button.Circle,
		DualShock4Button.Cross,
		DualShock4Button.Square,
		DualShock4SpecialButton.Ps,
		DualShock4SpecialButton.Touchpad
	};

	private static readonly List<DualShock4Axis> AxisMap = new List<DualShock4Axis>
	{
		DualShock4Axis.LeftThumbX,
		DualShock4Axis.LeftThumbY,
		DualShock4Axis.RightThumbX,
		DualShock4Axis.RightThumbY
	};

	private static readonly List<DualShock4Slider> SliderMap = new List<DualShock4Slider>
	{
		DualShock4Slider.LeftTrigger,
		DualShock4Slider.RightTrigger
	};

	private ViGEmClient.DS4_REPORT _nativeReport;

	private ViGEmClient.DS4_REPORT_EX _nativeReportEx;

	private ViGEmClient.PVIGEM_DS4_NOTIFICATION _notificationCallback;

	public int ButtonCount => ButtonMap.Count;

	public int AxisCount => AxisMap.Count;

	public int SliderCount => SliderMap.Count;

	public bool AutoSubmitReport { get; set; } = true;

	public event DualShock4FeedbackReceivedEventHandler FeedbackReceived;

	public DualShock4Controller(ViGEmClient client)
		: base(client)
	{
		base.NativeHandle = ViGEmClient.vigem_target_ds4_alloc();
		ResetReport();
	}

	public DualShock4Controller(ViGEmClient client, ushort vendorId, ushort productId)
		: this(client)
	{
		base.VendorId = vendorId;
		base.ProductId = productId;
	}

	public override void Connect()
	{
		base.Connect();
		_notificationCallback = delegate(IntPtr client, IntPtr target, byte motor, byte smallMotor, ViGEmClient.DS4_LIGHTBAR_COLOR color, IntPtr userData)
		{
			this.FeedbackReceived?.Invoke(this, new DualShock4FeedbackReceivedEventArgs(motor, smallMotor, new LightbarColor(color.Red, color.Green, color.Blue)));
		};
		switch (ViGEmClient.vigem_target_ds4_register_notification(base.Client.NativeHandle, base.NativeHandle, _notificationCallback))
		{
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
			throw new VigemBusNotFoundException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
			throw new VigemInvalidTargetException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_CALLBACK_ALREADY_REGISTERED:
			throw new VigemCallbackAlreadyRegisteredException();
		}
	}

	public override void Disconnect()
	{
		ViGEmClient.vigem_target_ds4_unregister_notification(base.NativeHandle);
		base.Disconnect();
	}

	public void SetButtonState(int index, bool pressed)
	{
		SetButtonState(ButtonMap[index], pressed);
	}

	public void SetAxisValue(int index, short value)
	{
		SetAxisValue(AxisMap[index], (byte)MathUtil.ConvertRange(-32768, 32767, 0, 255, value));
	}

	public void SetSliderValue(int index, byte value)
	{
		SetSliderValue(SliderMap[index], value);
	}

	public void ResetReport()
	{
		_nativeReport = default(ViGEmClient.DS4_REPORT);
		_nativeReport.wButtons &= 65520;
		_nativeReport.wButtons |= 8;
		_nativeReport.bThumbLX = 128;
		_nativeReport.bThumbLY = 128;
		_nativeReport.bThumbRX = 128;
		_nativeReport.bThumbRY = 128;
	}

	public void SubmitReport()
	{
		SubmitNativeReport(_nativeReport);
	}

	private void SubmitNativeReport(ViGEmClient.DS4_REPORT report)
	{
		switch (ViGEmClient.vigem_target_ds4_update(base.Client.NativeHandle, base.NativeHandle, report))
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

	public void SetButtonState(DualShock4Button button, bool pressed)
	{
		if (!(button is DualShock4SpecialButton dualShock4SpecialButton))
		{
			if (button != null)
			{
				if (pressed)
				{
					_nativeReport.wButtons |= button.Value;
				}
				else
				{
					_nativeReport.wButtons &= (ushort)(~button.Value);
				}
			}
		}
		else if (pressed)
		{
			_nativeReport.bSpecial |= (byte)dualShock4SpecialButton.Value;
		}
		else
		{
			_nativeReport.bSpecial &= (byte)(~dualShock4SpecialButton.Value);
		}
		if (AutoSubmitReport)
		{
			SubmitNativeReport(_nativeReport);
		}
	}

	public void SetDPadDirection(DualShock4DPadDirection direction)
	{
		_nativeReport.wButtons &= 65520;
		_nativeReport.wButtons |= direction.Value;
		if (AutoSubmitReport)
		{
			SubmitNativeReport(_nativeReport);
		}
	}

	public void SetAxisValue(DualShock4Axis axis, byte value)
	{
		switch (axis.Name)
		{
		case "LeftThumbX":
			_nativeReport.bThumbLX = value;
			break;
		case "LeftThumbY":
			_nativeReport.bThumbLY = value;
			break;
		case "RightThumbX":
			_nativeReport.bThumbRX = value;
			break;
		case "RightThumbY":
			_nativeReport.bThumbRY = value;
			break;
		}
		if (AutoSubmitReport)
		{
			SubmitNativeReport(_nativeReport);
		}
	}

	public void SetSliderValue(DualShock4Slider slider, byte value)
	{
		string name = slider.Name;
		if (!(name == "LeftTrigger"))
		{
			if (name == "RightTrigger")
			{
				_nativeReport.bTriggerR = value;
			}
		}
		else
		{
			_nativeReport.bTriggerL = value;
		}
		if (AutoSubmitReport)
		{
			SubmitNativeReport(_nativeReport);
		}
	}

	public void SubmitRawReport(byte[] buffer)
	{
		if (buffer.Length != Marshal.SizeOf<ViGEmClient.DS4_REPORT_EX>())
		{
			throw new ArgumentOutOfRangeException("buffer", "Supplied buffer has invalid size.");
		}
		_nativeReportEx.Report = buffer;
		switch (ViGEmClient.vigem_target_ds4_update_ex(base.Client.NativeHandle, base.NativeHandle, _nativeReportEx))
		{
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_INVALID_HANDLE:
			throw new VigemBusInvalidHandleException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_INVALID_TARGET:
			throw new VigemInvalidTargetException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
			throw new VigemBusNotFoundException();
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NOT_SUPPORTED:
			throw new VigemNotSupportedException();
		default:
			throw new Win32Exception(Marshal.GetLastWin32Error());
		case ViGEmClient.VIGEM_ERROR.VIGEM_ERROR_NONE:
			break;
		}
	}
}
