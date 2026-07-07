using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HidSharp.Reports.Encodings;

namespace HidSharp.Platform.Windows;

internal sealed class WinHidDevice : HidDevice
{
	[Flags]
	private enum GetInfoFlags
	{
		Manufacturer = 1,
		ProductName = 2,
		SerialNumber = 4,
		ReportInfo = 8
	}

	private struct ReportDescriptorReconstructor
	{
		private sealed class ItemCaps
		{
			public bool Button;

			public int BitOffset;

			public int ReportCount;

			public int ReportSize;

			public NativeMethods.HIDP_DATA_CAPS Item;
		}

		private sealed class ReportCaps
		{
			public ItemCaps[] Items;

			public int ReportLength;
		}

		private ReportDescriptorBuilder _builder;

		private IntPtr _preparsed;

		private List<ushort> _currentNodes;

		private NativeMethods.HIDP_LINK_COLLECTION_NODE[] _nodes;

		private ReportCaps[] _types;

		private static void InitData(byte[] report, byte reportID)
		{
			Array.Clear(report, 0, report.Length);
			report[0] = reportID;
		}

		private static bool GetDataBitValue(byte[] report, int bit)
		{
			return (report[bit >> 3] & (1 << (bit & 7))) != 0;
		}

		private void GetDataStartBit(byte[] report, ItemCaps item, int maxBit)
		{
			int i;
			for (i = 0; i < maxBit && !GetDataBitValue(report, i + 8); i++)
			{
			}
			item.BitOffset = i;
		}

		private void AddButtonGlobalItems(int bitCount)
		{
			_builder.AddGlobalItemSigned(GlobalItemTag.LogicalMinimum, 0);
			_builder.AddGlobalItemSigned(GlobalItemTag.LogicalMaximum, 1);
			_builder.AddGlobalItemSigned(GlobalItemTag.PhysicalMinimum, 0);
			_builder.AddGlobalItemSigned(GlobalItemTag.PhysicalMaximum, 1);
			_builder.AddGlobalItem(GlobalItemTag.Unit, 0u);
			_builder.AddGlobalItem(GlobalItemTag.UnitExponent, 0u);
			_builder.AddGlobalItem(GlobalItemTag.ReportSize, 1u);
			_builder.AddGlobalItem(GlobalItemTag.ReportCount, (uint)bitCount);
		}

		private void PadReport(MainItemTag mainItemTag, int padToBit, ref int currentBit)
		{
			if (currentBit > padToBit)
			{
				throw new NotImplementedException();
			}
			int num = padToBit - currentBit;
			if (num > 0)
			{
				AddButtonGlobalItems(num);
				_builder.AddMainItem(mainItemTag, 3u);
				currentBit += num;
			}
		}

		private void EncodeReports(NativeMethods.HIDP_REPORT_TYPE reportType, MainItemTag mainItemTag)
		{
			ReportCaps reportCaps = _types[(int)reportType];
			byte[] array = new byte[reportCaps.ReportLength];
			IEnumerable<IGrouping<byte, ItemCaps>> enumerable = from y in reportCaps.Items
				group y by y.Item.ReportID;
			foreach (IGrouping<byte, ItemCaps> item3 in enumerable)
			{
				byte key = item3.Key;
				ItemCaps[] array2 = item3.ToArray();
				if (key != 0)
				{
					_builder.AddGlobalItem(GlobalItemTag.ReportID, key);
				}
				int maxBit = (array.Length - 1) * 8;
				foreach (ItemCaps itemCaps in array2)
				{
					bool button = itemCaps.Button;
					NativeMethods.HIDP_DATA_CAPS item = itemCaps.Item;
					if (item.IsAlias != 0)
					{
						throw new NotImplementedException();
					}
					int num2 = ((item.IsRange == 0) ? 1 : (item.DataIndexMax - item.DataIndex + 1));
					if (button)
					{
						itemCaps.ReportCount = num2;
						itemCaps.ReportSize = 1;
					}
					else
					{
						itemCaps.ReportCount = item.VALUE_ReportCount;
						itemCaps.ReportSize = item.VALUE_ReportSize;
					}
					InitData(array, key);
					if (num2 == itemCaps.ReportCount)
					{
						NativeMethods.HIDP_DATA dataList = new NativeMethods.HIDP_DATA
						{
							DataIndex = item.DataIndex,
							RawValue = uint.MaxValue
						};
						int dataCount = 1;
						int num3 = NativeMethods.HidP_SetData(reportType, ref dataList, ref dataCount, _preparsed, array, array.Length);
						if (num3 == NativeMethods.HIDP_STATUS_SUCCESS)
						{
							GetDataStartBit(array, itemCaps, maxBit);
							continue;
						}
						if (num3 != NativeMethods.HIDP_STATUS_IS_VALUE_ARRAY)
						{
							throw new NotImplementedException();
						}
						itemCaps.BitOffset = maxBit;
					}
					else if (num2 == 1)
					{
						int num4 = itemCaps.ReportCount * itemCaps.ReportSize;
						byte[] array3 = new byte[(num4 + 7) / 8];
						for (int num5 = 0; num5 < array3.Length; num5++)
						{
							array3[num5] = byte.MaxValue;
						}
						int num6 = NativeMethods.HidP_SetUsageValueArray(reportType, item.UsagePage, item.LinkCollection, item.UsageIndex, array3, (ushort)array3.Length, _preparsed, array, array.Length);
						if (num6 != NativeMethods.HIDP_STATUS_SUCCESS)
						{
							throw new NotImplementedException();
						}
						GetDataStartBit(array, itemCaps, maxBit);
					}
					else
					{
						itemCaps.BitOffset = maxBit;
					}
				}
				int currentBit = 0;
				ItemCaps[] array4 = (from x in item3
					where x.BitOffset != maxBit
					orderby x.BitOffset
					select x).ToArray();
				ItemCaps[] array5 = array4;
				foreach (ItemCaps itemCaps2 in array5)
				{
					bool button2 = itemCaps2.Button;
					NativeMethods.HIDP_DATA_CAPS item2 = itemCaps2.Item;
					int bitOffset = itemCaps2.BitOffset;
					int num8 = itemCaps2.ReportCount * itemCaps2.ReportSize;
					if (currentBit > bitOffset)
					{
						throw new NotImplementedException();
					}
					SetCollection(item2.LinkCollection);
					PadReport(mainItemTag, bitOffset, ref currentBit);
					_builder.AddGlobalItem(GlobalItemTag.UsagePage, item2.UsagePage);
					uint usageIndex = item2.UsageIndex;
					uint dataValue = ((item2.IsRange != 0) ? item2.UsageMax : usageIndex);
					if (item2.IsRange != 0)
					{
						_builder.AddLocalItem(LocalItemTag.UsageMinimum, usageIndex);
						_builder.AddLocalItem(LocalItemTag.UsageMaximum, dataValue);
					}
					else
					{
						_builder.AddLocalItem(LocalItemTag.Usage, usageIndex);
					}
					if (button2)
					{
						AddButtonGlobalItems(itemCaps2.ReportCount);
					}
					else
					{
						_builder.AddGlobalItemSigned(GlobalItemTag.LogicalMinimum, item2.VALUE_LogicalMin);
						_builder.AddGlobalItemSigned(GlobalItemTag.LogicalMaximum, item2.VALUE_LogicalMax);
						_builder.AddGlobalItemSigned(GlobalItemTag.PhysicalMinimum, item2.VALUE_PhysicalMin);
						_builder.AddGlobalItemSigned(GlobalItemTag.PhysicalMaximum, item2.VALUE_PhysicalMax);
						_builder.AddGlobalItem(GlobalItemTag.Unit, item2.VALUE_Units);
						_builder.AddGlobalItem(GlobalItemTag.UnitExponent, item2.VALUE_UnitsExp);
						_builder.AddGlobalItem(GlobalItemTag.ReportSize, (uint)itemCaps2.ReportSize);
						_builder.AddGlobalItem(GlobalItemTag.ReportCount, (uint)itemCaps2.ReportCount);
					}
					_builder.AddMainItem(mainItemTag, item2.BitField);
					currentBit += num8;
				}
				PadReport(mainItemTag, maxBit, ref currentBit);
			}
		}

		private void BeginCollection(ushort nodeIndex)
		{
			NativeMethods.HIDP_LINK_COLLECTION_NODE hIDP_LINK_COLLECTION_NODE = _nodes[nodeIndex];
			if (hIDP_LINK_COLLECTION_NODE.IsAlias != 0)
			{
				throw new NotImplementedException();
			}
			_builder.AddGlobalItem(GlobalItemTag.UsagePage, hIDP_LINK_COLLECTION_NODE.LinkUsagePage);
			_builder.AddLocalItem(LocalItemTag.Usage, hIDP_LINK_COLLECTION_NODE.LinkUsage);
			_builder.AddMainItem(MainItemTag.Collection, hIDP_LINK_COLLECTION_NODE.CollectionType);
		}

		private void EndCollection()
		{
			_builder.AddMainItem(MainItemTag.EndCollection, 0u);
		}

		private void SetCollection(List<ushort> newNodes)
		{
			int num = Math.Min(_currentNodes.Count, newNodes.Count);
			int i;
			for (i = 0; i < num && _currentNodes[i] == newNodes[i]; i++)
			{
			}
			while (_currentNodes.Count > i)
			{
				EndCollection();
				_currentNodes.RemoveAt(_currentNodes.Count - 1);
			}
			for (int j = i; j < newNodes.Count; j++)
			{
				ushort num2 = newNodes[j];
				_currentNodes.Add(num2);
				BeginCollection(num2);
			}
		}

		private void SetCollection(ushort nodeIndex)
		{
			List<ushort> currentNodes = _currentNodes;
			if (currentNodes.Count >= 1 && currentNodes[currentNodes.Count - 1] == nodeIndex)
			{
				return;
			}
			List<ushort> list = new List<ushort>();
			while (true)
			{
				list.Add(nodeIndex);
				if (nodeIndex == 0)
				{
					break;
				}
				nodeIndex = _nodes[nodeIndex].Parent;
			}
			list.Reverse();
			SetCollection(list);
		}

		private void GetReportCaps(NativeMethods.HIDP_REPORT_TYPE reportType, ushort buttonCount, ushort valueCount, ushort reportLength)
		{
			ReportCaps reportCaps = new ReportCaps();
			NativeMethods.HIDP_DATA_CAPS[] array = new NativeMethods.HIDP_DATA_CAPS[buttonCount];
			NativeMethods.HIDP_DATA_CAPS[] array2 = new NativeMethods.HIDP_DATA_CAPS[valueCount];
			ushort count = buttonCount;
			if (count > 0 && (NativeMethods.HidP_GetButtonCaps(reportType, array, ref count, _preparsed) != NativeMethods.HIDP_STATUS_SUCCESS || count != buttonCount))
			{
				throw new NotImplementedException();
			}
			count = valueCount;
			if (count > 0 && (NativeMethods.HidP_GetValueCaps(reportType, array2, ref count, _preparsed) != NativeMethods.HIDP_STATUS_SUCCESS || count != valueCount))
			{
				throw new NotImplementedException();
			}
			reportCaps.Items = array.Select((NativeMethods.HIDP_DATA_CAPS b) => new ItemCaps
			{
				Button = true,
				Item = b
			}).Concat(array2.Select((NativeMethods.HIDP_DATA_CAPS v) => new ItemCaps
			{
				Button = false,
				Item = v
			})).ToArray();
			reportCaps.ReportLength = reportLength;
			_types[(int)reportType] = reportCaps;
		}

		public byte[] Run(IntPtr preparsed, NativeMethods.HIDP_CAPS caps)
		{
			_builder = new ReportDescriptorBuilder();
			_preparsed = preparsed;
			_nodes = new NativeMethods.HIDP_LINK_COLLECTION_NODE[caps.NumberLinkCollectionNodes];
			int count = _nodes.Length;
			if (NativeMethods.HidP_GetLinkCollectionNodes(_nodes, ref count, preparsed) != NativeMethods.HIDP_STATUS_SUCCESS || count != _nodes.Length)
			{
				throw new NotImplementedException();
			}
			_types = new ReportCaps[3];
			GetReportCaps(NativeMethods.HIDP_REPORT_TYPE.Input, caps.NumberInputButtonCaps, caps.NumberInputValueCaps, caps.InputReportByteLength);
			GetReportCaps(NativeMethods.HIDP_REPORT_TYPE.Output, caps.NumberOutputButtonCaps, caps.NumberOutputValueCaps, caps.OutputReportByteLength);
			GetReportCaps(NativeMethods.HIDP_REPORT_TYPE.Feature, caps.NumberFeatureButtonCaps, caps.NumberFeatureValueCaps, caps.FeatureReportByteLength);
			_currentNodes = new List<ushort>();
			SetCollection(new List<ushort> { 0 });
			EncodeReports(NativeMethods.HIDP_REPORT_TYPE.Input, MainItemTag.Input);
			EncodeReports(NativeMethods.HIDP_REPORT_TYPE.Output, MainItemTag.Output);
			EncodeReports(NativeMethods.HIDP_REPORT_TYPE.Feature, MainItemTag.Feature);
			SetCollection(new List<ushort>());
			return _builder.GetReportDescriptor();
		}
	}

	private sealed class ReportDescriptorBuilder
	{
		private Dictionary<GlobalItemTag, uint> _globals;

		private List<EncodedItem> _items;

		public ReportDescriptorBuilder()
		{
			_globals = new Dictionary<GlobalItemTag, uint>();
			_items = new List<EncodedItem>();
		}

		public void AddGlobalItem(GlobalItemTag globalItemTag, uint dataValue)
		{
			if (!_globals.TryGetValue(globalItemTag, out var value) || value != dataValue)
			{
				_globals[globalItemTag] = dataValue;
				EncodedItem encodedItem = new EncodedItem();
				encodedItem.ItemType = ItemType.Global;
				encodedItem.TagForGlobal = globalItemTag;
				encodedItem.DataValue = dataValue;
				EncodedItem item = encodedItem;
				_items.Add(item);
			}
		}

		public void AddGlobalItemSigned(GlobalItemTag globalItemTag, int dataValue)
		{
			if (!_globals.TryGetValue(globalItemTag, out var value) || value != (uint)dataValue)
			{
				_globals[globalItemTag] = (uint)dataValue;
				EncodedItem encodedItem = new EncodedItem();
				encodedItem.ItemType = ItemType.Global;
				encodedItem.TagForGlobal = globalItemTag;
				encodedItem.DataValueSigned = dataValue;
				EncodedItem item = encodedItem;
				_items.Add(item);
			}
		}

		public void AddLocalItem(LocalItemTag localItemTag, uint dataValue)
		{
			_items.Add(new EncodedItem
			{
				ItemType = ItemType.Local,
				TagForLocal = localItemTag,
				DataValue = dataValue
			});
		}

		public void AddMainItem(MainItemTag mainItemTag, uint dataValue)
		{
			_items.Add(new EncodedItem
			{
				ItemType = ItemType.Main,
				TagForMain = mainItemTag,
				DataValue = dataValue
			});
		}

		public byte[] GetReportDescriptor()
		{
			List<byte> list = new List<byte>();
			EncodedItem.EncodeItems(_items, list);
			return list.ToArray();
		}
	}

	private GetInfoFlags _getInfoFlags;

	private object _getInfoLock = new object();

	private string _path;

	private string _id;

	private string _manufacturer;

	private string _productName;

	private string _serialNumber;

	private int _vid;

	private int _pid;

	private int _version;

	private int _maxInput;

	private int _maxOutput;

	private int _maxFeature;

	private byte[] _reportDescriptor;

	public override string DevicePath => _path;

	public override int VendorID => _vid;

	public override int ProductID => _pid;

	public override int ReleaseNumberBcd => _version;

	private WinHidDevice()
	{
	}

	internal static WinHidDevice TryCreate(string path, string id)
	{
		WinHidDevice d = new WinHidDevice
		{
			_path = path,
			_id = id
		};
		if (!d.TryOpenToGetInfo(delegate(IntPtr handle)
		{
			NativeMethods.HIDD_ATTRIBUTES attributes = default(NativeMethods.HIDD_ATTRIBUTES);
			attributes.Size = Marshal.SizeOf((object)attributes);
			if (!NativeMethods.HidD_GetAttributes(handle, ref attributes))
			{
				return false;
			}
			d._pid = attributes.ProductID;
			d._vid = attributes.VendorID;
			d._version = attributes.VersionNumber;
			return true;
		}))
		{
			return null;
		}
		return d;
	}

	private bool TryOpenToGetInfo(Func<IntPtr, bool> action)
	{
		return NativeMethods.TryOpenToGetInfo(_path, action);
	}

	protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
	{
		RequiresGetInfo(GetInfoFlags.ReportInfo);
		WinHidStream winHidStream = new WinHidStream(this);
		try
		{
			winHidStream.Init(_path);
			return winHidStream;
		}
		catch
		{
			winHidStream.Close();
			throw;
		}
	}

	private void RequiresGetInfo(GetInfoFlags flags)
	{
		lock (_getInfoLock)
		{
			flags &= ~_getInfoFlags;
			if (flags != 0)
			{
				if (!TryOpenToGetInfo(delegate(IntPtr handle)
				{
					if ((flags & GetInfoFlags.Manufacturer) != 0 && !TryGetDeviceString(handle, NativeMethods.HidD_GetManufacturerString, out _manufacturer))
					{
						return false;
					}
					if ((flags & GetInfoFlags.ProductName) != 0 && !TryGetDeviceString(handle, NativeMethods.HidD_GetProductString, out _productName))
					{
						return false;
					}
					if ((flags & GetInfoFlags.SerialNumber) != 0 && !TryGetDeviceString(handle, NativeMethods.HidD_GetSerialNumberString, out _serialNumber))
					{
						return false;
					}
					if ((flags & GetInfoFlags.ReportInfo) != 0)
					{
						if (!NativeMethods.HidD_GetPreparsedData(handle, out var preparsed))
						{
							return false;
						}
						try
						{
							NativeMethods.HIDP_CAPS caps;
							int num = NativeMethods.HidP_GetCaps(preparsed, out caps);
							if (num != NativeMethods.HIDP_STATUS_SUCCESS)
							{
								return false;
							}
							_maxInput = caps.InputReportByteLength;
							_maxOutput = caps.OutputReportByteLength;
							_maxFeature = caps.FeatureReportByteLength;
							try
							{
								_reportDescriptor = default(ReportDescriptorReconstructor).Run(preparsed, caps);
							}
							catch (NotImplementedException)
							{
								_reportDescriptor = null;
							}
							catch
							{
								return false;
							}
						}
						finally
						{
							NativeMethods.HidD_FreePreparsedData(preparsed);
						}
					}
					return true;
				}))
				{
					throw DeviceException.CreateIOException(this, "Failed to get info.");
				}
				_getInfoFlags |= flags;
			}
		}
	}

	private bool TryGetDeviceString(IntPtr handle, Func<IntPtr, char[], int, bool> callback, out string s)
	{
		char[] array = new char[128];
		if (!callback(handle, array, Marshal.SystemDefaultCharSize * array.Length))
		{
			s = null;
			return Marshal.GetLastWin32Error() == 31;
		}
		s = NativeMethods.NTString(array);
		return true;
	}

	public override string GetManufacturer()
	{
		RequiresGetInfo(GetInfoFlags.Manufacturer);
		return _manufacturer;
	}

	public override string GetProductName()
	{
		RequiresGetInfo(GetInfoFlags.ProductName);
		return _productName;
	}

	public override string GetSerialNumber()
	{
		RequiresGetInfo(GetInfoFlags.SerialNumber);
		return _serialNumber;
	}

	public override int GetMaxInputReportLength()
	{
		RequiresGetInfo(GetInfoFlags.ReportInfo);
		return _maxInput;
	}

	public override int GetMaxOutputReportLength()
	{
		RequiresGetInfo(GetInfoFlags.ReportInfo);
		return _maxOutput;
	}

	public override int GetMaxFeatureReportLength()
	{
		RequiresGetInfo(GetInfoFlags.ReportInfo);
		return _maxFeature;
	}

	public override byte[] GetRawReportDescriptor()
	{
		RequiresGetInfo(GetInfoFlags.ReportInfo);
		byte[] reportDescriptor = _reportDescriptor;
		if (reportDescriptor == null)
		{
			throw new NotSupportedException("Unable to reconstruct the report descriptor.");
		}
		return (byte[])reportDescriptor.Clone();
	}

	private bool TryGetDeviceUsbRoot(out uint devInst)
	{
		if (NativeMethods.CM_Locate_DevNode(out devInst, _id) == 0)
		{
			uint parentDevInst;
			string deviceID;
			while (NativeMethods.CM_Get_Parent(out parentDevInst, devInst) == 0 && NativeMethods.CM_Get_Device_ID(parentDevInst, out deviceID) == 0 && (deviceID.StartsWith("USB\\") || deviceID.StartsWith("HID\\")))
			{
				devInst = parentDevInst;
				if (Regex.IsMatch(deviceID, "^USB\\\\VID_[0-9A-F]{4}&PID_[0-9A-F]{4}\\\\"))
				{
					return true;
				}
			}
		}
		devInst = 0u;
		return false;
	}

	public override string[] GetSerialPorts()
	{
		List<string> ports = new List<string>();
		if (TryGetDeviceUsbRoot(out var devInst) && NativeMethods.CM_Get_Child(out devInst, devInst) == 0)
		{
			do
			{
				if (NativeMethods.CM_Get_Device_ID(devInst, out var deviceID) != 0)
				{
					continue;
				}
				NativeMethods.EnumerateDeviceInterfaces(NativeMethods.GuidForComPort, deviceID, delegate(NativeMethods.HDEVINFO deviceInfoSet, NativeMethods.SP_DEVINFO_DATA deviceInfoData, NativeMethods.SP_DEVICE_INTERFACE_DATA _, string __, string devicePath)
				{
					if (NativeMethods.TryGetSerialPortFriendlyName(deviceInfoSet, ref deviceInfoData, out var _) && NativeMethods.TryGetSerialPortName(deviceInfoSet, ref deviceInfoData, out var portName))
					{
						ports.Add("\\\\.\\" + portName);
					}
				});
			}
			while (NativeMethods.CM_Get_Sibling(out devInst, devInst) == 0);
		}
		return ports.ToArray();
	}

	public override string GetFileSystemName()
	{
		return DevicePath;
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.Windows;
		}
		return true;
	}
}
