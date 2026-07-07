using System;

namespace HidSharp.Experimental;

[Flags]
public enum BleCharacteristicProperties : byte
{
	None = 0,
	Broadcast = 1,
	Read = 2,
	WriteWithoutResponse = 4,
	Write = 8,
	Notify = 0x10,
	Indicate = 0x20,
	SignedWrite = 0x40,
	ExtendedProperties = 0x80
}
