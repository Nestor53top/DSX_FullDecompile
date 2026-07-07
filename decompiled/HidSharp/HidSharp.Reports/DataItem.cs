using System;
using HidSharp.Reports.Units;

namespace HidSharp.Reports;

public class DataItem : DescriptorItem
{
	private int _elementBits;

	private int _elementSize;

	private int _rawPhysicalMinimum;

	private int _rawPhysicalMaximum;

	private int _unitExponent;

	private int _totalBits;

	private double _unitMultiplier;

	private int _rawPhysicalRange;

	private double _physicalMinimum;

	private double _physicalMaximum;

	private double _physicalRange;

	public int TotalBits => _totalBits;

	public int ElementCount
	{
		get
		{
			return _elementBits;
		}
		set
		{
			Throw.If.Negative(value, "value");
			_elementBits = value;
			InvalidateBitCount();
		}
	}

	public int ElementBits
	{
		get
		{
			return _elementSize;
		}
		set
		{
			Throw.If.Negative(value, "value");
			_elementSize = value;
			InvalidateBitCount();
		}
	}

	public DataItemFlags Flags { get; set; }

	public bool HasNullState => 0 != (Flags & DataItemFlags.NullState);

	public bool HasPreferredState => 0 == (Flags & DataItemFlags.NoPreferred);

	public bool IsArray => !IsVariable;

	public bool IsVariable => 0 != (Flags & DataItemFlags.Variable);

	public bool IsBoolean
	{
		get
		{
			if (ElementBits != 1)
			{
				return IsArray;
			}
			return true;
		}
	}

	public bool IsConstant => 0 != (Flags & DataItemFlags.Constant);

	public bool IsAbsolute => !IsRelative;

	public bool IsRelative => 0 != (Flags & DataItemFlags.Relative);

	public ExpectedUsageType ExpectedUsageType
	{
		get
		{
			if (!IsConstant)
			{
				if (IsBoolean)
				{
					if (IsAbsolute && HasPreferredState)
					{
						return ExpectedUsageType.PushButton;
					}
					if (IsAbsolute && !HasPreferredState)
					{
						return ExpectedUsageType.ToggleButton;
					}
					if (IsRelative && HasPreferredState)
					{
						return ExpectedUsageType.OneShot;
					}
				}
				else if (IsVariable && IsRelative && HasPreferredState && ElementBits >= 2 && -LogicalMinimum == LogicalMaximum && LogicalMaximum >= 1 && LogicalMaximum == 1)
				{
					return ExpectedUsageType.UpDown;
				}
			}
			return (ExpectedUsageType)0;
		}
	}

	public bool IsLogicalSigned { get; set; }

	public int LogicalMinimum { get; set; }

	public int LogicalMaximum { get; set; }

	public int LogicalRange => LogicalMaximum - LogicalMinimum;

	public double PhysicalMinimum => _physicalMinimum;

	public double PhysicalMaximum => _physicalMaximum;

	public double PhysicalRange => _physicalRange;

	public int RawPhysicalMinimum
	{
		get
		{
			return _rawPhysicalMinimum;
		}
		set
		{
			_rawPhysicalMinimum = value;
			InvalidatePhysicalRange();
		}
	}

	public int RawPhysicalMaximum
	{
		get
		{
			return _rawPhysicalMaximum;
		}
		set
		{
			_rawPhysicalMaximum = value;
			InvalidatePhysicalRange();
		}
	}

	public int RawPhysicalRange => _rawPhysicalRange;

	public Report Report { get; internal set; }

	public Unit Unit { get; set; }

	public int UnitExponent
	{
		get
		{
			return _unitExponent;
		}
		set
		{
			_unitExponent = value;
			InvalidatePhysicalRange();
		}
	}

	private double UnitMultiplier => _unitMultiplier;

	public DataItem()
	{
		UnitExponent = 0;
	}

	public int ReadLogical(byte[] buffer, int bitOffset, int elementIndex)
	{
		uint num = ReadRaw(buffer, bitOffset, elementIndex);
		if (!IsLogicalSigned)
		{
			return (int)num;
		}
		return DataConvert.LogicalFromRaw(this, num);
	}

	public uint ReadRaw(byte[] buffer, int bitOffset, int elementIndex)
	{
		Throw.If.Null(buffer).OutOfRange(ElementCount, elementIndex, 1);
		uint num = 0u;
		int num2 = Math.Min(ElementBits, 32);
		bitOffset += elementIndex * ElementBits;
		int num3 = 0;
		while (num3 < num2)
		{
			int num4 = bitOffset >> 3;
			byte b = (byte)(1 << (bitOffset & 7));
			num |= (uint)(((buffer[num4] & b) != 0) ? (1 << num3) : 0);
			num3++;
			bitOffset++;
		}
		return num;
	}

	public bool TryReadValue(byte[] buffer, int bitOffset, int elementIndex, out DataValue value)
	{
		value = default(DataValue);
		int num = ReadLogical(buffer, bitOffset, elementIndex);
		if (IsArray)
		{
			int num2 = DataConvert.DataIndexFromLogical(this, num);
			if (num2 < 0)
			{
				return false;
			}
			value.DataItem = this;
			value.DataIndex = num;
			value.SetLogicalValue(1);
		}
		else
		{
			value.DataItem = this;
			value.DataIndex = elementIndex;
			value.SetLogicalValue(num);
		}
		return true;
	}

	public void WriteLogical(byte[] buffer, int bitOffset, int elementIndex, int logicalValue)
	{
		WriteRaw(buffer, bitOffset, elementIndex, IsLogicalSigned ? DataConvert.RawFromLogical(this, logicalValue) : ((uint)logicalValue));
	}

	public void WriteRaw(byte[] buffer, int bitOffset, int elementIndex, uint rawValue)
	{
		Throw.If.Null(buffer).OutOfRange(ElementCount, elementIndex, 1);
		int num = Math.Min(ElementBits, 32);
		bitOffset += elementIndex * ElementBits;
		int num2 = 0;
		while (num2 < num)
		{
			int num3 = bitOffset >> 3;
			uint num4 = (uint)(1 << (bitOffset & 7));
			if ((rawValue & (1 << num2)) != 0)
			{
				buffer[num3] |= (byte)num4;
			}
			else
			{
				buffer[num3] &= (byte)(~num4);
			}
			num2++;
			bitOffset++;
		}
	}

	private void InvalidateBitCount()
	{
		_totalBits = ElementCount * ElementBits;
		if (Report != null)
		{
			Report.InvalidateBitCount();
		}
	}

	private void InvalidatePhysicalRange()
	{
		_unitMultiplier = Math.Pow(10.0, UnitExponent);
		_rawPhysicalRange = RawPhysicalMaximum - RawPhysicalMinimum;
		_physicalMinimum = (double)RawPhysicalMinimum * UnitMultiplier;
		_physicalMaximum = (double)RawPhysicalMaximum * UnitMultiplier;
		_physicalRange = PhysicalMaximum - PhysicalMinimum;
	}
}
