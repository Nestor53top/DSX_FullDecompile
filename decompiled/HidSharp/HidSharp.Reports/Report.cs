using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HidSharp.Reports;

public class Report
{
	private sealed class ReportDataItems : Collection<DataItem>
	{
		private Report _report;

		public ReportDataItems(Report report)
		{
			_report = report;
		}

		protected override void ClearItems()
		{
			using (IEnumerator<DataItem> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DataItem current = enumerator.Current;
					current.Report = null;
				}
			}
			base.ClearItems();
			_report.InvalidateBitCount();
		}

		protected override void InsertItem(int index, DataItem item)
		{
			Throw.If.Null(item).False(item.Report == null);
			item.Report = _report;
			base.InsertItem(index, item);
			_report.InvalidateBitCount();
		}

		protected override void RemoveItem(int index)
		{
			base[index].Report = null;
			base.RemoveItem(index);
			_report.InvalidateBitCount();
		}

		protected override void SetItem(int index, DataItem item)
		{
			throw new NotImplementedException();
		}
	}

	private bool _computed;

	private int _computedLength;

	public DeviceItem DeviceItem { get; internal set; }

	public IList<DataItem> DataItems { get; private set; }

	public int Length
	{
		get
		{
			ComputeLength();
			return _computedLength;
		}
	}

	public byte ReportID { get; set; }

	public ReportType ReportType { get; set; }

	public Report()
	{
		DataItems = new ReportDataItems(this);
	}

	public IEnumerable<uint> GetAllUsages()
	{
		return DataItems.SelectMany((DataItem item) => item.Usages.GetAllValues());
	}

	public void Read(byte[] buffer, int offset, ReportScanCallback callback)
	{
		Throw.If.Null(buffer).OutOfRange(buffer, offset, Length).Null(callback);
		if (buffer[offset] != ReportID)
		{
			throw new ArgumentException("Report ID not correctly set.", "buffer");
		}
		int num = (offset + 1) * 8;
		IList<DataItem> dataItems = DataItems;
		int count = dataItems.Count;
		for (int i = 0; i < count; i++)
		{
			DataItem dataItem = dataItems[i];
			callback(buffer, num, dataItem, i);
			num += dataItem.TotalBits;
		}
	}

	public void Read(byte[] buffer, int offset, ReportValueCallback callback)
	{
		Read(buffer, offset, delegate(byte[] readBuffer, int bitOffset, DataItem dataItem, int indexOfDataItem)
		{
			int elementCount = dataItem.ElementCount;
			for (int i = 0; i < elementCount; i++)
			{
				if (dataItem.TryReadValue(readBuffer, bitOffset, i, out var value))
				{
					callback(value);
				}
			}
		});
	}

	public byte[] Write(ReportScanCallback callback)
	{
		byte[] array = new byte[Length];
		Write(array, 0, callback);
		return array;
	}

	public void Write(byte[] buffer, int offset, ReportScanCallback callback)
	{
		Throw.If.OutOfRange(buffer, offset, Length);
		buffer[offset] = ReportID;
		Array.Clear(buffer, offset + 1, Length - 1);
		Read(buffer, offset, callback);
	}

	internal void InvalidateBitCount()
	{
		_computed = false;
	}

	private void ComputeLength()
	{
		if (_computed)
		{
			return;
		}
		int num = 0;
		foreach (DataItem dataItem in DataItems)
		{
			num += dataItem.TotalBits;
		}
		_computedLength = (num + 7) / 8 + 1;
		_computed = true;
	}
}
