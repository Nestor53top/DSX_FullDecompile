using System;
using System.Collections.Generic;

namespace HidSharp.Reports.Encodings;

public class EncodedItem
{
	public IList<byte> Data { get; private set; }

	public int DataSize
	{
		get
		{
			if (!IsShortTag)
			{
				return 0;
			}
			return Data.Count;
		}
	}

	public uint DataValue
	{
		get
		{
			if (!IsShortTag)
			{
				return 0u;
			}
			return (uint)(DataAt(0) | (DataAt(1) << 8) | (DataAt(2) << 16) | (DataAt(3) << 24));
		}
		set
		{
			Data.Clear();
			Data.Add((byte)value);
			if (value > 255)
			{
				Data.Add((byte)(value >> 8));
			}
			if (value > 65535)
			{
				Data.Add((byte)(value >> 16));
				Data.Add((byte)(value >> 24));
			}
		}
	}

	public int DataValueSigned
	{
		get
		{
			if (!IsShortTag)
			{
				return 0;
			}
			if (Data.Count != 4)
			{
				if (Data.Count != 2)
				{
					if (Data.Count != 1)
					{
						return 0;
					}
					return (sbyte)DataValue;
				}
				return (short)DataValue;
			}
			return (int)DataValue;
		}
		set
		{
			if (value == 0)
			{
				DataValue = (uint)value;
			}
			else if (value >= -128 && value <= 127)
			{
				DataValue = (uint)(sbyte)value;
				if (value < 0)
				{
					Data.Add(0);
				}
			}
			else if (value >= -32768 && value <= 32767)
			{
				DataValue = (uint)(short)value;
				if (value < 0)
				{
					Data.Add(0);
					Data.Add(0);
				}
			}
			else
			{
				DataValue = (uint)value;
			}
		}
	}

	public byte Tag { get; set; }

	public GlobalItemTag TagForGlobal
	{
		get
		{
			return (GlobalItemTag)Tag;
		}
		set
		{
			Tag = (byte)value;
		}
	}

	public LocalItemTag TagForLocal
	{
		get
		{
			return (LocalItemTag)Tag;
		}
		set
		{
			Tag = (byte)value;
		}
	}

	public MainItemTag TagForMain
	{
		get
		{
			return (MainItemTag)Tag;
		}
		set
		{
			Tag = (byte)value;
		}
	}

	public ItemType ItemType { get; set; }

	public bool IsShortTag
	{
		get
		{
			if (!IsLongTag)
			{
				if (Data.Count != 0 && Data.Count != 1 && Data.Count != 2)
				{
					return Data.Count == 4;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsLongTag
	{
		get
		{
			if (Tag == 15 && ItemType == ItemType.Reserved)
			{
				return Data.Count >= 2;
			}
			return false;
		}
	}

	public EncodedItem()
	{
		Data = new List<byte>();
	}

	public override string ToString()
	{
		return ItemType switch
		{
			ItemType.Global => $"{TagForGlobal} {DataValue}", 
			ItemType.Local => $"{TagForLocal} {DataValue}", 
			ItemType.Main => $"{TagForMain} {DataValue}", 
			_ => Tag.ToString(), 
		};
	}

	public void Reset()
	{
		Data.Clear();
		Tag = 0;
		ItemType = ItemType.Main;
	}

	private byte DataAt(int index)
	{
		if (index < 0 || index >= Data.Count)
		{
			return 0;
		}
		return Data[index];
	}

	private static byte GetByte(IList<byte> buffer, ref int offset, ref int count)
	{
		if (count <= 0)
		{
			return 0;
		}
		count--;
		if (offset < 0 || offset >= buffer.Count)
		{
			return 0;
		}
		return buffer[offset++];
	}

	public int Decode(IList<byte> buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		Reset();
		int num = count;
		byte b = GetByte(buffer, ref offset, ref count);
		int num2 = b & 3;
		if (num2 == 3)
		{
			num2 = 4;
		}
		ItemType = (ItemType)((b >> 2) & 3);
		Tag = (byte)(b >> 4);
		for (int i = 0; i < num2; i++)
		{
			Data.Add(GetByte(buffer, ref offset, ref count));
		}
		return num - count;
	}

	public static IEnumerable<EncodedItem> DecodeItems(IList<byte> buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		while (count > 0)
		{
			EncodedItem item = new EncodedItem();
			int bytes = item.Decode(buffer, offset, count);
			offset += bytes;
			count -= bytes;
			yield return item;
		}
	}

	public void Encode(IList<byte> buffer)
	{
		Throw.If.Null(buffer, "buffer");
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (!IsShortTag)
		{
			return;
		}
		int dataSize = DataSize;
		buffer.Add((byte)((uint)((dataSize == 4) ? 3 : dataSize) | ((uint)ItemType << 2) | (uint)(Tag << 4)));
		foreach (byte datum in Data)
		{
			buffer.Add(datum);
		}
	}

	public static void EncodeItems(IEnumerable<EncodedItem> items, IList<byte> buffer)
	{
		Throw.If.Null(buffer, "buffer").Null(items, "items");
		foreach (EncodedItem item in items)
		{
			item.Encode(buffer);
		}
	}
}
