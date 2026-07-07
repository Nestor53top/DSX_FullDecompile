using System;
using System.Globalization;

namespace HidSharp.Experimental;

public struct BleUuid : IEquatable<BleUuid>
{
	private Guid _guid;

	public bool IsShortUuid
	{
		get
		{
			byte[] array = _guid.ToByteArray();
			if (array[4] == 0 && array[5] == 0 && array[6] == 0 && array[7] == 16 && array[8] == 128 && array[9] == 0 && array[10] == 0 && array[11] == 128 && array[12] == 95 && array[13] == 155 && array[14] == 52)
			{
				return array[15] == 251;
			}
			return false;
		}
	}

	public BleUuid(int uuid)
	{
		this = default(BleUuid);
		Initialize(uuid);
	}

	public BleUuid(Guid guid)
	{
		this = default(BleUuid);
		Initialize(guid);
	}

	public BleUuid(string uuid)
	{
		this = default(BleUuid);
		Initialize(uuid);
	}

	private void Initialize(int uuid)
	{
		_guid = new Guid(uuid, 0, 4096, 128, 0, 0, 128, 95, 155, 52, 251);
	}

	private void Initialize(Guid guid)
	{
		_guid = guid;
	}

	private void Initialize(string guid)
	{
		if (uint.TryParse(guid, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
		{
			Initialize((int)result);
		}
		else
		{
			Initialize(new Guid(guid));
		}
	}

	public override bool Equals(object other)
	{
		if (other is BleUuid)
		{
			return Equals((BleUuid)other);
		}
		return false;
	}

	public bool Equals(BleUuid other)
	{
		return _guid.Equals(other._guid);
	}

	public override int GetHashCode()
	{
		return _guid.GetHashCode();
	}

	public static implicit operator BleUuid(Guid guid)
	{
		return new BleUuid(guid);
	}

	public static implicit operator Guid(BleUuid uuid)
	{
		return uuid.ToGuid();
	}

	public int ToShortUuid()
	{
		if (!IsShortUuid)
		{
			throw new InvalidOperationException();
		}
		byte[] array = _guid.ToByteArray();
		return (ushort)(array[0] | (array[1] << 8) | (array[2] << 16) | (array[3] << 24));
	}

	private static void SwapNetworkOrder(byte[] guid)
	{
		byte b = guid[0];
		guid[0] = guid[3];
		guid[3] = b;
		b = guid[1];
		guid[1] = guid[2];
		guid[2] = b;
		b = guid[4];
		guid[4] = guid[5];
		guid[5] = b;
		b = guid[6];
		guid[6] = guid[7];
		guid[7] = b;
	}

	public byte[] ToByteArray()
	{
		byte[] array = _guid.ToByteArray();
		SwapNetworkOrder(array);
		return array;
	}

	public Guid ToGuid()
	{
		return _guid;
	}

	public override string ToString()
	{
		if (!IsShortUuid)
		{
			return ToGuid().ToString("D", CultureInfo.InvariantCulture);
		}
		return ToShortUuid().ToString("X", CultureInfo.InvariantCulture);
	}

	public static bool operator ==(BleUuid lhs, BleUuid rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(BleUuid lhs, BleUuid rhs)
	{
		return !lhs.Equals(rhs);
	}
}
