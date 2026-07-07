using System;
using System.Runtime.InteropServices;
using System.Security;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;

namespace Nefarius.ViGEm.Client;

[SuppressUnmanagedCodeSecurity]
public class ViGEmClient : IDisposable
{
	internal enum VIGEM_ERROR : uint
	{
		VIGEM_ERROR_NONE = 536870912u,
		VIGEM_ERROR_BUS_NOT_FOUND = 3758096385u,
		VIGEM_ERROR_NO_FREE_SLOT = 3758096386u,
		VIGEM_ERROR_INVALID_TARGET = 3758096387u,
		VIGEM_ERROR_REMOVAL_FAILED = 3758096388u,
		VIGEM_ERROR_ALREADY_CONNECTED = 3758096389u,
		VIGEM_ERROR_TARGET_UNINITIALIZED = 3758096390u,
		VIGEM_ERROR_TARGET_NOT_PLUGGED_IN = 3758096391u,
		VIGEM_ERROR_BUS_VERSION_MISMATCH = 3758096392u,
		VIGEM_ERROR_BUS_ACCESS_FAILED = 3758096393u,
		VIGEM_ERROR_CALLBACK_ALREADY_REGISTERED = 3758096400u,
		VIGEM_ERROR_CALLBACK_NOT_FOUND = 3758096401u,
		VIGEM_ERROR_BUS_ALREADY_CONNECTED = 3758096402u,
		VIGEM_ERROR_BUS_INVALID_HANDLE = 3758096403u,
		VIGEM_ERROR_XUSB_USERINDEX_OUT_OF_RANGE = 3758096404u,
		VIGEM_ERROR_INVALID_PARAMETER = 3758096405u,
		VIGEM_ERROR_NOT_SUPPORTED = 3758096406u
	}

	internal struct XUSB_REPORT
	{
		public ushort wButtons;

		public byte bLeftTrigger;

		public byte bRightTrigger;

		public short sThumbLX;

		public short sThumbLY;

		public short sThumbRX;

		public short sThumbRY;
	}

	internal struct DS4_REPORT
	{
		public byte bThumbLX;

		public byte bThumbLY;

		public byte bThumbRX;

		public byte bThumbRY;

		public ushort wButtons;

		public byte bSpecial;

		public byte bTriggerL;

		public byte bTriggerR;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct DS4_REPORT_EX
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 63)]
		public byte[] Report;
	}

	internal enum VIGEM_TARGET_TYPE : uint
	{
		Xbox360Wired,
		XboxOneWired,
		DualShock4Wired
	}

	internal struct DS4_LIGHTBAR_COLOR
	{
		public byte Red;

		public byte Green;

		public byte Blue;
	}

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void PVIGEM_X360_NOTIFICATION(IntPtr Client, IntPtr Target, byte LargeMotor, byte SmallMotor, byte LedNumber, IntPtr UserData);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void PVIGEM_DS4_NOTIFICATION(IntPtr Client, IntPtr Target, byte LargeMotor, byte SmallMotor, DS4_LIGHTBAR_COLOR LightbarColor, IntPtr UserData);

	private bool disposedValue;

	internal IntPtr NativeHandle { get; }

	public ViGEmClient()
	{
		NativeHandle = vigem_alloc();
		if (NativeHandle == IntPtr.Zero)
		{
			throw new VigemAllocFailedException();
		}
		switch (vigem_connect(NativeHandle))
		{
		case VIGEM_ERROR.VIGEM_ERROR_ALREADY_CONNECTED:
			throw new VigemAlreadyConnectedException();
		case VIGEM_ERROR.VIGEM_ERROR_BUS_NOT_FOUND:
			throw new VigemBusNotFoundException();
		case VIGEM_ERROR.VIGEM_ERROR_BUS_ACCESS_FAILED:
			throw new VigemBusAccessFailedException();
		case VIGEM_ERROR.VIGEM_ERROR_BUS_VERSION_MISMATCH:
			throw new VigemBusVersionMismatchException();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			vigem_disconnect(NativeHandle);
			vigem_free(NativeHandle);
			disposedValue = true;
		}
	}

	~ViGEmClient()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public IXbox360Controller CreateXbox360Controller()
	{
		return new Xbox360Controller(this);
	}

	public IXbox360Controller CreateXbox360Controller(ushort vendorId, ushort productId)
	{
		return new Xbox360Controller(this, vendorId, productId);
	}

	public IDualShock4Controller CreateDualShock4Controller()
	{
		return new DualShock4Controller(this);
	}

	public IDualShock4Controller CreateDualShock4Controller(ushort vendorId, ushort productId)
	{
		return new DualShock4Controller(this, vendorId, productId);
	}

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern IntPtr vigem_alloc();

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern void vigem_free(IntPtr vigem);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern VIGEM_ERROR vigem_connect(IntPtr vigem);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern void vigem_disconnect(IntPtr vigem);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern IntPtr vigem_target_x360_alloc();

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern IntPtr vigem_target_ds4_alloc();

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern void vigem_target_free(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern VIGEM_ERROR vigem_target_add(IntPtr vigem, IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern VIGEM_ERROR vigem_target_add_async(IntPtr vigem, IntPtr target, IntPtr result);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern VIGEM_ERROR vigem_target_remove(IntPtr vigem, IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern VIGEM_ERROR vigem_target_x360_register_notification(IntPtr vigem, IntPtr target, PVIGEM_X360_NOTIFICATION notification);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern VIGEM_ERROR vigem_target_ds4_register_notification(IntPtr vigem, IntPtr target, PVIGEM_DS4_NOTIFICATION notification);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern void vigem_target_x360_unregister_notification(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern void vigem_target_ds4_unregister_notification(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern void vigem_target_set_vid(IntPtr target, ushort vid);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern void vigem_target_set_pid(IntPtr target, ushort pid);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern ushort vigem_target_get_vid(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern ushort vigem_target_get_pid(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern VIGEM_ERROR vigem_target_x360_update(IntPtr vigem, IntPtr target, XUSB_REPORT report);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern VIGEM_ERROR vigem_target_ds4_update(IntPtr vigem, IntPtr target, DS4_REPORT report);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	internal static extern VIGEM_ERROR vigem_target_ds4_update_ex(IntPtr vigem, IntPtr target, DS4_REPORT_EX report);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern uint vigem_target_get_index(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern VIGEM_TARGET_TYPE vigem_target_get_type(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern bool vigem_target_is_attached(IntPtr target);

	[DllImport("vigemclient.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
	private static extern VIGEM_ERROR vigem_target_x360_get_user_index(IntPtr vigem, IntPtr target, out uint index);
}
