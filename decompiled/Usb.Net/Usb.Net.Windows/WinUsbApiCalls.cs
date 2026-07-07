using System;
using System.Runtime.InteropServices;
using System.Text;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Usb.Net.Windows;

public static class WinUsbApiCalls
{
	public enum USBD_PIPE_TYPE
	{
		UsbdPipeTypeControl,
		UsbdPipeTypeIsochronous,
		UsbdPipeTypeBulk,
		UsbdPipeTypeInterrupt
	}

	public const int EnglishLanguageID = 1033;

	public const uint DEVICE_SPEED = 1u;

	public const byte USB_ENDPOINT_DIRECTION_MASK = 128;

	public const byte WritePipeId = 128;

	public const int DEFAULT_DESCRIPTOR_TYPE = 1;

	public const int USB_STRING_DESCRIPTOR_TYPE = 3;

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, byte[] Buffer, uint BufferLength, ref uint LengthTransferred, IntPtr Overlapped);

	[DllImport("winusb.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool WinUsb_GetAssociatedInterface(SafeFileHandle InterfaceHandle, byte AssociatedInterfaceIndex, out SafeFileHandle AssociatedInterfaceHandle);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_GetDescriptor(SafeFileHandle InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, out USB_DEVICE_DESCRIPTOR deviceDesc, uint BufferLength, out uint LengthTransfered);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_GetDescriptor(SafeFileHandle InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, byte[] Buffer, uint BufferLength, out uint LengthTransfered);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_Free(SafeFileHandle InterfaceHandle);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_Initialize(SafeFileHandle DeviceHandle, out SafeFileHandle InterfaceHandle);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_QueryDeviceInformation(IntPtr InterfaceHandle, uint InformationType, ref uint BufferLength, ref byte Buffer);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_QueryInterfaceSettings(SafeFileHandle InterfaceHandle, byte AlternateInterfaceNumber, out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_QueryPipe(SafeFileHandle InterfaceHandle, byte AlternateInterfaceNumber, byte PipeIndex, out WINUSB_PIPE_INFORMATION PipeInformation);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_ReadPipe(SafeFileHandle InterfaceHandle, byte PipeID, byte[] Buffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_SetPipePolicy(SafeFileHandle InterfaceHandle, byte PipeID, uint PolicyType, uint ValueLength, ref uint Value);

	[DllImport("winusb.dll", SetLastError = true)]
	public static extern bool WinUsb_WritePipe(SafeFileHandle InterfaceHandle, byte PipeID, byte[] Buffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);

	public static string GetDescriptor(SafeFileHandle defaultInterfaceHandle, byte index, string errorMessage)
	{
		byte[] array = new byte[256];
		WindowsDeviceBase.HandleError(WinUsb_GetDescriptor(defaultInterfaceHandle, 3, index, 1033, array, (uint)array.Length, out var LengthTransfered), errorMessage);
		string text = new string(Encoding.Unicode.GetChars(array, 2, (int)LengthTransfered));
		return text.Substring(0, text.Length - 1);
	}
}
