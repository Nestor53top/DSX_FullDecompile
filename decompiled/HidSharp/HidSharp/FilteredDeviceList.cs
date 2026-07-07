using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HidSharp;

public class FilteredDeviceList : DeviceList
{
	private int _dirty;

	private List<Func<bool>> _areDriversBeingInstalled;

	private Dictionary<Device, int> _refCounts;

	public override bool AreDriversBeingInstalled => _areDriversBeingInstalled.Any((Func<bool> callback) => callback());

	public FilteredDeviceList()
	{
		_areDriversBeingInstalled = new List<Func<bool>>();
		_refCounts = new Dictionary<Device, int>();
	}

	public void Add(Device device)
	{
		Throw.If.Null(device, "device");
		lock (_refCounts)
		{
			IncrementRefCount(device);
		}
		RaiseChangedIfDirty();
	}

	public void Add(DeviceList deviceList)
	{
		Add(deviceList, (Device device) => true);
	}

	public void Add(DeviceList deviceList, DeviceFilter filter)
	{
		Throw.If.Null(deviceList, "deviceList").Null(filter, "filter");
		Device[] oldDevices = new Device[0];
		Action updateDeviceList = delegate
		{
			Device[] array = deviceList.GetAllDevices(filter).ToArray();
			lock (_refCounts)
			{
				Device[] array2 = array;
				foreach (Device device in array2)
				{
					IncrementRefCount(device);
				}
				Device[] array3 = oldDevices;
				foreach (Device device2 in array3)
				{
					DecrementRefCount(device2);
				}
			}
			oldDevices = array;
			RaiseChangedIfDirty();
		};
		_areDriversBeingInstalled.Add(() => deviceList.AreDriversBeingInstalled);
		deviceList.Changed += delegate
		{
			updateDeviceList();
		};
		updateDeviceList();
	}

	public override IEnumerable<Device> GetAllDevices()
	{
		lock (_refCounts)
		{
			return _refCounts.Keys.ToList();
		}
	}

	private void IncrementRefCount(Device device)
	{
		if (_refCounts.ContainsKey(device))
		{
			_refCounts[device]++;
			return;
		}
		_refCounts[device] = 1;
		_dirty = 1;
	}

	private void DecrementRefCount(Device device)
	{
		if (--_refCounts[device] == 0)
		{
			_refCounts.Remove(device);
			_dirty = 1;
		}
	}

	private void RaiseChangedIfDirty()
	{
		if (1 == Interlocked.CompareExchange(ref _dirty, 0, 1))
		{
			RaiseChanged();
		}
	}
}
