using System.Collections.Generic;
using System.Linq;

namespace HidSharp.Reports;

public struct DataValue
{
	private int _logicalValue;

	public DataItem DataItem { get; set; }

	public int DataIndex { get; set; }

	public bool IsNull
	{
		get
		{
			if (IsValid)
			{
				if (DataItem.IsVariable)
				{
					return DataConvert.IsLogicalOutOfRange(DataItem, GetLogicalValue());
				}
				return false;
			}
			return true;
		}
	}

	public bool IsValid => DataItem != null;

	public Report Report
	{
		get
		{
			if (!IsValid)
			{
				return null;
			}
			return DataItem.Report;
		}
	}

	public IEnumerable<uint> Designators
	{
		get
		{
			if (!IsValid)
			{
				return Enumerable.Empty<uint>();
			}
			return DataItem.Designators.GetValuesFromIndex(DataIndex);
		}
	}

	public IEnumerable<uint> Strings
	{
		get
		{
			if (!IsValid)
			{
				return Enumerable.Empty<uint>();
			}
			return DataItem.Strings.GetValuesFromIndex(DataIndex);
		}
	}

	public IEnumerable<uint> Usages
	{
		get
		{
			if (!IsValid)
			{
				return Enumerable.Empty<uint>();
			}
			return DataItem.Usages.GetValuesFromIndex(DataIndex);
		}
	}

	public int GetLogicalValue()
	{
		return _logicalValue;
	}

	public void SetLogicalValue(int logicalValue)
	{
		_logicalValue = logicalValue;
	}

	public double GetFractionalValue()
	{
		return GetScaledValue(0.0, 1.0);
	}

	public double GetScaledValue(double minimum, double maximum)
	{
		return DataConvert.CustomFromLogical(DataItem, GetLogicalValue(), minimum, maximum);
	}

	public double GetPhysicalValue()
	{
		return DataConvert.PhysicalFromLogical(DataItem, GetLogicalValue());
	}
}
