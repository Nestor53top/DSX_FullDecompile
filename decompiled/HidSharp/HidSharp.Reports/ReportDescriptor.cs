using System;
using System.Collections.Generic;
using System.Linq;
using HidSharp.Reports.Encodings;
using HidSharp.Reports.Input;
using HidSharp.Reports.Units;

namespace HidSharp.Reports;

public class ReportDescriptor
{
	public int MaxInputReportLength { get; private set; }

	public int MaxOutputReportLength { get; private set; }

	public int MaxFeatureReportLength { get; private set; }

	public IEnumerable<Report> InputReports => Reports.Where((Report report) => report.ReportType == ReportType.Input);

	public IEnumerable<Report> OutputReports => Reports.Where((Report report) => report.ReportType == ReportType.Output);

	public IEnumerable<Report> FeatureReports => Reports.Where((Report report) => report.ReportType == ReportType.Feature);

	public IList<Report> Reports { get; private set; }

	public bool ReportsUseID { get; private set; }

	public IList<DeviceItem> DeviceItems { get; private set; }

	private DescriptorCollectionItem RootItem => State.RootItem;

	private ReportDescriptorParseState State { get; set; }

	private ReportDescriptor()
	{
		State = new ReportDescriptorParseState();
		StartParsing();
	}

	public ReportDescriptor(byte[] buffer)
		: this()
	{
		StartParsing();
		ParseRawReportDescriptor(buffer);
		FinishParsing();
	}

	public HidDeviceInputReceiver CreateHidDeviceInputReceiver()
	{
		return new HidDeviceInputReceiver(this);
	}

	private void StartParsing()
	{
		Reports = new List<Report>();
		ReportsUseID = false;
		State.Reset();
	}

	private void FinishParsing()
	{
		Reports = Array.AsReadOnly(Reports.ToArray());
		MaxInputReportLength = GetMaxLengthOfReports(InputReports);
		MaxOutputReportLength = GetMaxLengthOfReports(OutputReports);
		MaxFeatureReportLength = GetMaxLengthOfReports(FeatureReports);
		DeviceItems = Array.AsReadOnly(RootItem.ChildItems.OfType<DeviceItem>().ToArray());
	}

	public Report GetReport(ReportType type, byte id)
	{
		if (!TryGetReport(type, id, out var report))
		{
			throw new ArgumentException("Report not found.");
		}
		return report;
	}

	public bool TryGetReport(ReportType type, byte id, out Report report)
	{
		for (int i = 0; i < Reports.Count; i++)
		{
			report = Reports[i];
			if (report.ReportType == type && report.ReportID == id)
			{
				return true;
			}
		}
		report = null;
		return false;
	}

	private static int GetMaxLengthOfReports(IEnumerable<Report> reports)
	{
		int num = 0;
		foreach (Report report in reports)
		{
			num = Math.Max(num, report.Length);
		}
		return num;
	}

	private void ParseRawReportDescriptor(byte[] buffer)
	{
		Throw.If.Null(buffer, "buffer");
		IEnumerable<EncodedItem> items = EncodedItem.DecodeItems(buffer, 0, buffer.Length);
		ParseEncodedItems(items);
	}

	private void ParseEncodedItems(IEnumerable<EncodedItem> items)
	{
		Throw.If.Null(items, "items");
		foreach (EncodedItem item in items)
		{
			ParseEncodedItem(item);
		}
	}

	private void ParseEncodedItem(EncodedItem item)
	{
		Throw.If.Null(item, "item");
		uint num = item.DataValue;
		switch (item.ItemType)
		{
		case ItemType.Main:
			ParseMain(item.TagForMain, num);
			State.LocalItemState.Clear();
			break;
		case ItemType.Local:
			switch (item.TagForLocal)
			{
			case LocalItemTag.Usage:
			case LocalItemTag.UsageMinimum:
			case LocalItemTag.UsageMaximum:
				if (num <= 65535)
				{
					num |= State.GetGlobalItemValue(GlobalItemTag.UsagePage) << 16;
				}
				break;
			}
			State.LocalItemState.Add(new KeyValuePair<LocalItemTag, uint>(item.TagForLocal, num));
			break;
		case ItemType.Global:
			switch (item.TagForGlobal)
			{
			case GlobalItemTag.Push:
				State.GlobalItemStateStack.Add(new Dictionary<GlobalItemTag, EncodedItem>(State.GlobalItemState));
				break;
			case GlobalItemTag.Pop:
				State.GlobalItemStateStack.RemoveAt(State.GlobalItemState.Count - 1);
				break;
			default:
			{
				GlobalItemTag tagForGlobal = item.TagForGlobal;
				if (tagForGlobal == GlobalItemTag.ReportID)
				{
					ReportsUseID = true;
				}
				State.GlobalItemState[item.TagForGlobal] = item;
				break;
			}
			}
			break;
		}
	}

	private void ParseMain(MainItemTag tag, uint value)
	{
		switch (tag)
		{
		case MainItemTag.Collection:
			ParseMainCollection(value);
			break;
		case MainItemTag.EndCollection:
			ParseMainCollectionEnd();
			break;
		case MainItemTag.Input:
		case MainItemTag.Output:
		case MainItemTag.Feature:
			ParseMainData(tag, value);
			break;
		}
	}

	private void ParseMainCollection(uint value)
	{
		DescriptorCollectionItem descriptorCollectionItem = ((State.CurrentCollectionItem != State.RootItem) ? new DescriptorCollectionItem() : new DeviceItem());
		descriptorCollectionItem.CollectionType = (CollectionType)value;
		State.CurrentCollectionItem.ChildItems.Add(descriptorCollectionItem);
		State.CurrentCollectionItem = descriptorCollectionItem;
		ParseMainIndexes(descriptorCollectionItem);
	}

	private void ParseMainCollectionEnd()
	{
		State.CurrentCollectionItem = State.CurrentCollectionItem.ParentItem;
	}

	private static void AddIndex(List<KeyValuePair<int, uint>> list, int action, uint value)
	{
		list.Add(new KeyValuePair<int, uint>(action, value));
	}

	private static void UpdateIndexMinimum(ref Indexes index, uint value)
	{
		if (!(index is IndexRange))
		{
			index = new IndexRange();
		}
		((IndexRange)index).Minimum = value;
	}

	private static void UpdateIndexMaximum(ref Indexes index, uint value)
	{
		if (!(index is IndexRange))
		{
			index = new IndexRange();
		}
		((IndexRange)index).Maximum = value;
	}

	private static void UpdateIndexList(List<uint> values, int delimiter, ref Indexes index, uint value)
	{
		values.Add(value);
		UpdateIndexListCommit(values, delimiter, ref index);
	}

	private static void UpdateIndexListCommit(List<uint> values, int delimiter, ref Indexes index)
	{
		if (delimiter == 0 && values.Count != 0)
		{
			if (!(index is IndexList))
			{
				index = new IndexList();
			}
			((IndexList)index).Indices.Add(new List<uint>(values));
			values.Clear();
		}
	}

	private void ParseMainIndexes(DescriptorItem item)
	{
		int num = 0;
		List<uint> list = new List<uint>();
		Indexes index = Indexes.Unset;
		List<uint> list2 = new List<uint>();
		Indexes index2 = Indexes.Unset;
		List<uint> list3 = new List<uint>();
		Indexes index3 = Indexes.Unset;
		foreach (KeyValuePair<LocalItemTag, uint> item2 in State.LocalItemState)
		{
			switch (item2.Key)
			{
			case LocalItemTag.DesignatorMinimum:
				UpdateIndexMinimum(ref index, item2.Value);
				break;
			case LocalItemTag.StringMinimum:
				UpdateIndexMinimum(ref index2, item2.Value);
				break;
			case LocalItemTag.UsageMinimum:
				UpdateIndexMinimum(ref index3, item2.Value);
				break;
			case LocalItemTag.DesignatorMaximum:
				UpdateIndexMaximum(ref index, item2.Value);
				break;
			case LocalItemTag.StringMaximum:
				UpdateIndexMaximum(ref index2, item2.Value);
				break;
			case LocalItemTag.UsageMaximum:
				UpdateIndexMaximum(ref index3, item2.Value);
				break;
			case LocalItemTag.DesignatorIndex:
				UpdateIndexList(list, num, ref index, item2.Value);
				break;
			case LocalItemTag.StringIndex:
				UpdateIndexList(list2, num, ref index2, item2.Value);
				break;
			case LocalItemTag.Usage:
				UpdateIndexList(list3, num, ref index3, item2.Value);
				break;
			case LocalItemTag.Delimiter:
				if (item2.Value == 1)
				{
					if (num++ == 0)
					{
						list.Clear();
						list2.Clear();
						list3.Clear();
					}
				}
				else if (item2.Value == 0)
				{
					num--;
					UpdateIndexListCommit(list, num, ref index);
					UpdateIndexListCommit(list2, num, ref index2);
					UpdateIndexListCommit(list3, num, ref index3);
				}
				break;
			}
		}
		item.Designators = index;
		item.Strings = index2;
		item.Usages = index3;
	}

	private void ParseMainData(MainItemTag tag, uint value)
	{
		DataItem dataItem = new DataItem();
		dataItem.Flags = (DataItemFlags)value;
		dataItem.ParentItem = State.CurrentCollectionItem;
		dataItem.ElementCount = (int)State.GetGlobalItemValue(GlobalItemTag.ReportCount);
		dataItem.ElementBits = (int)State.GetGlobalItemValue(GlobalItemTag.ReportSize);
		dataItem.Unit = new Unit(State.GetGlobalItemValue(GlobalItemTag.Unit));
		dataItem.UnitExponent = Unit.DecodeExponent(State.GetGlobalItemValue(GlobalItemTag.UnitExponent));
		EncodedItem globalItem = State.GetGlobalItem(GlobalItemTag.LogicalMinimum);
		EncodedItem globalItem2 = State.GetGlobalItem(GlobalItemTag.LogicalMaximum);
		dataItem.IsLogicalSigned = !dataItem.IsArray && (globalItem?.DataValue ?? 0) > (globalItem2?.DataValue ?? 0);
		int num = ((globalItem != null) ? (dataItem.IsLogicalSigned ? globalItem.DataValueSigned : ((int)globalItem.DataValue)) : 0);
		int num2 = ((globalItem2 != null) ? (dataItem.IsLogicalSigned ? globalItem2.DataValueSigned : ((int)globalItem2.DataValue)) : 0);
		EncodedItem globalItem3 = State.GetGlobalItem(GlobalItemTag.PhysicalMinimum);
		EncodedItem globalItem4 = State.GetGlobalItem(GlobalItemTag.PhysicalMaximum);
		bool flag = !dataItem.IsArray && (globalItem3?.DataValue ?? 0) > (globalItem4?.DataValue ?? 0);
		int num3 = ((globalItem3 != null) ? (flag ? globalItem3.DataValueSigned : ((int)globalItem3.DataValue)) : 0);
		int num4 = ((globalItem4 != null) ? (flag ? globalItem4.DataValueSigned : ((int)globalItem4.DataValue)) : 0);
		if (num3 == 0 && num4 == 0)
		{
			num3 = num;
			num4 = num2;
		}
		dataItem.LogicalMinimum = num;
		dataItem.LogicalMaximum = num2;
		dataItem.RawPhysicalMinimum = num3;
		dataItem.RawPhysicalMaximum = num4;
		ReportType reportType = tag switch
		{
			MainItemTag.Feature => ReportType.Feature, 
			MainItemTag.Output => ReportType.Output, 
			_ => ReportType.Input, 
		};
		uint globalItemValue = State.GetGlobalItemValue(GlobalItemTag.ReportID);
		if (!TryGetReport(reportType, (byte)globalItemValue, out var report))
		{
			Report report2 = new Report();
			report2.ReportID = (byte)globalItemValue;
			report2.ReportType = reportType;
			report = report2;
			Reports.Add(report);
			DescriptorCollectionItem descriptorCollectionItem = State.CurrentCollectionItem;
			while (descriptorCollectionItem != null && !(descriptorCollectionItem is DeviceItem))
			{
				descriptorCollectionItem = descriptorCollectionItem.ParentItem;
			}
			if (descriptorCollectionItem is DeviceItem)
			{
				((DeviceItem)descriptorCollectionItem).Reports.Add(report);
			}
		}
		report.DataItems.Add(dataItem);
		ParseMainIndexes(dataItem);
	}
}
