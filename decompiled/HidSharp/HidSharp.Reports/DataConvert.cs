using System;

namespace HidSharp.Reports;

public static class DataConvert
{
	public static int LogicalFromPhysical(DataItem item, double physicalValue)
	{
		Throw.If.Null(item);
		if (item.IsArray)
		{
			if (!(physicalValue > 0.0))
			{
				return 0;
			}
			return 1;
		}
		return LogicalFromCustom(item, physicalValue, item.PhysicalMinimum, item.PhysicalMaximum);
	}

	public static int LogicalFromCustom(DataItem item, double physicalValue, double minimum, double maximum)
	{
		return Math.Max(item.LogicalMinimum, Math.Min(item.LogicalMaximum, (int)Math.Round((physicalValue - minimum) * (double)item.LogicalRange / (maximum - minimum))));
	}

	public static int LogicalFromRaw(DataItem item, uint value)
	{
		Throw.If.Null(item);
		uint num = (uint)(1 << item.ElementBits - 1);
		uint num2 = num - 1;
		if ((value & num) == 0)
		{
			return (int)value;
		}
		return (int)(value | ~num2);
	}

	public static int DataIndexFromLogical(DataItem item, int logicalValue)
	{
		Throw.If.Null(item);
		if (!item.IsArray)
		{
			throw new ArgumentException("Data item is not an array.", "item");
		}
		if (!IsLogicalOutOfRange(item, logicalValue))
		{
			return logicalValue - item.LogicalMinimum;
		}
		return -1;
	}

	public static double PhysicalFromLogical(DataItem item, int logicalValue)
	{
		Throw.If.Null(item);
		if (item.IsArray)
		{
			return (logicalValue > 0) ? 1 : 0;
		}
		return CustomFromLogical(item, logicalValue, item.PhysicalMinimum, item.PhysicalRange);
	}

	public static double CustomFromLogical(DataItem item, int logicalValue, double minimum, double maximum)
	{
		if (IsLogicalOutOfRange(item, logicalValue))
		{
			return double.NaN;
		}
		return minimum + (double)(logicalValue - item.LogicalMinimum) * (maximum - minimum) / (double)item.LogicalRange;
	}

	public static uint RawFromLogical(DataItem item, int value)
	{
		Throw.If.Null(item);
		uint num = (uint)(1 << item.ElementBits - 1);
		uint num2 = num - 1;
		return ((uint)value & num2) | ((value < 0) ? num : 0);
	}

	public static bool IsLogicalOutOfRange(DataItem item, int logicalValue)
	{
		Throw.If.Null(item);
		if (!item.IsLogicalSigned)
		{
			if ((uint)logicalValue >= (uint)item.LogicalMinimum)
			{
				return (uint)logicalValue > (uint)item.LogicalMaximum;
			}
			return true;
		}
		if (logicalValue >= item.LogicalMinimum)
		{
			return logicalValue > item.LogicalMaximum;
		}
		return true;
	}
}
