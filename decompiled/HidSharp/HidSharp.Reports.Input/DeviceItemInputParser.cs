using System;
using System.Collections.Generic;

namespace HidSharp.Reports.Input;

public class DeviceItemInputParser
{
	private DeviceItem _deviceItem;

	private DataValue[] _oldDataValues;

	private DataValue[] _newDataValues;

	private Queue<int> _changedElements;

	private int[][] _rangesOfElementsByIndexOfDataItem;

	private int[] _rangeOfElementsByIndexOfReport;

	private Dictionary<byte, int> _reportIDtoIndex;

	private object _syncRoot;

	public DeviceItem DeviceItem => _deviceItem;

	public int ValueCount => _newDataValues.Length;

	public bool HasChanged => _changedElements.Count > 0;

	public DeviceItemInputParser(DeviceItem deviceItem)
	{
		Throw.If.Null(deviceItem);
		_deviceItem = deviceItem;
		_changedElements = new Queue<int>();
		_reportIDtoIndex = new Dictionary<byte, int>();
		int num = 0;
		List<int[]> list = new List<int[]>();
		List<int> list2 = new List<int> { 0 };
		int num2 = 0;
		List<DataValue> list3 = new List<DataValue>();
		foreach (Report inputReport in deviceItem.InputReports)
		{
			_reportIDtoIndex[inputReport.ReportID] = num2;
			List<int> list4 = new List<int> { 0 };
			int num3 = 0;
			foreach (DataItem dataItem in inputReport.DataItems)
			{
				int elementCount = dataItem.ElementCount;
				Indexes usages = dataItem.Usages;
				int count = usages.Count;
				if ((dataItem.IsArray && dataItem.LogicalMaximum - dataItem.LogicalMinimum + 1 == count) || (!dataItem.IsArray && elementCount == count))
				{
					for (int i = 0; i < count; i++)
					{
						DataValue dataValue = new DataValue
						{
							DataItem = dataItem,
							DataIndex = i
						};
						SetDefaultValue(ref dataValue);
						list3.Add(dataValue);
						num++;
					}
				}
				num3++;
				list4.Add(num);
			}
			num2++;
			list.Add(list4.ToArray());
			list2.Add(num);
		}
		_oldDataValues = list3.ToArray();
		_newDataValues = list3.ToArray();
		_rangesOfElementsByIndexOfDataItem = list.ToArray();
		_rangeOfElementsByIndexOfReport = list2.ToArray();
		_syncRoot = new object();
	}

	private void SetDefaultValue(ref DataValue dataValue)
	{
		dataValue.SetLogicalValue((!dataValue.DataItem.IsArray) ? (dataValue.DataItem.LogicalMaximum + 1) : 0);
	}

	public bool TryParseReport(byte[] buffer, int offset, Report report)
	{
		Throw.If.Null(buffer).Null(report).OutOfRange(buffer, offset, report.Length);
		lock (_syncRoot)
		{
			if (!_reportIDtoIndex.TryGetValue(report.ReportID, out var value))
			{
				return false;
			}
			int num = _rangeOfElementsByIndexOfReport[value];
			int num2 = _rangeOfElementsByIndexOfReport[value + 1];
			int length = num2 - num;
			Array.Copy(_newDataValues, num, _oldDataValues, num, length);
			for (int i = num; i < num2; i++)
			{
				SetDefaultValue(ref _newDataValues[i]);
			}
			int[] rangeOfElementsByIndexOfDataItem = _rangesOfElementsByIndexOfDataItem[value];
			report.Read(buffer, offset, delegate(byte[] reportBuffer, int bitOffset, DataItem dataItem, int indexOfDataItem)
			{
				int num4 = rangeOfElementsByIndexOfDataItem[indexOfDataItem];
				int num5 = rangeOfElementsByIndexOfDataItem[indexOfDataItem + 1];
				if (num4 != num5)
				{
					int elementCount = dataItem.ElementCount;
					for (int j = 0; j < elementCount; j++)
					{
						if (dataItem.TryReadValue(reportBuffer, bitOffset, j, out var value2))
						{
							int num6 = num4 + value2.DataIndex;
							_newDataValues[num6] = value2;
						}
					}
				}
			});
			_changedElements.Clear();
			for (int num3 = num; num3 < num2; num3++)
			{
				DataValue dataValue = _oldDataValues[num3];
				DataValue dataValue2 = _newDataValues[num3];
				if (dataValue.GetLogicalValue() != dataValue2.GetLogicalValue())
				{
					_changedElements.Enqueue(num3);
				}
			}
			return true;
		}
	}

	public DataValue GetPreviousValue(int index)
	{
		return _oldDataValues[index];
	}

	public DataValue GetValue(int index)
	{
		return _newDataValues[index];
	}

	public int GetNextChangedIndex()
	{
		lock (_syncRoot)
		{
			return HasChanged ? _changedElements.Dequeue() : (-1);
		}
	}
}
