using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Device.Net.Exceptions;

namespace Device.Net;

public sealed class DeviceListener : IDisposable
{
	private bool _IsDisposed;

	private readonly System.Timers.Timer _PollTimer;

	private readonly SemaphoreSlim _ListenSemaphoreSlim = new SemaphoreSlim(1, 1);

	private readonly Dictionary<FilterDeviceDefinition, IDevice> _CreatedDevicesByDefinition = new Dictionary<FilterDeviceDefinition, IDevice>();

	public List<FilterDeviceDefinition> FilterDeviceDefinitions { get; } = new List<FilterDeviceDefinition>();

	public ILogger Logger { get; set; }

	public event EventHandler<DeviceEventArgs> DeviceInitialized;

	public event EventHandler<DeviceEventArgs> DeviceDisconnected;

	public DeviceListener(IEnumerable<FilterDeviceDefinition> filterDeviceDefinitions, int? pollMilliseconds)
	{
		FilterDeviceDefinitions.AddRange(filterDeviceDefinitions);
		if (pollMilliseconds.HasValue)
		{
			_PollTimer = new System.Timers.Timer(pollMilliseconds.Value);
			_PollTimer.Elapsed += _PollTimer_Elapsed;
		}
	}

	private async void _PollTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		await CheckForDevicesAsync();
	}

	private void Log(string message, Exception ex, [CallerMemberName] string callerMemberName = null)
	{
		Logger?.Log(message, "DeviceListener - " + callerMemberName, ex, (ex == null) ? LogLevel.Information : LogLevel.Error);
	}

	public void Start()
	{
		if (_PollTimer == null)
		{
			throw new ValidationException("Polling is not enabled. Please specify pollMilliseconds in the constructor");
		}
		if (DeviceManager.Current.DeviceFactories.Count == 0)
		{
			throw new DeviceFactoriesNotRegisteredException();
		}
		_PollTimer.Start();
	}

	public async Task CheckForDevicesAsync()
	{
		_ = 2;
		try
		{
			await _ListenSemaphoreSlim.WaitAsync();
			List<ConnectedDeviceDefinition> connectedDeviceDefinitions = new List<ConnectedDeviceDefinition>();
			foreach (FilterDeviceDefinition filterDeviceDefinition2 in FilterDeviceDefinitions)
			{
				List<ConnectedDeviceDefinition> list = connectedDeviceDefinitions;
				list.AddRange(await DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(filterDeviceDefinition2));
			}
			foreach (ConnectedDeviceDefinition connectedDeviceDefinition in connectedDeviceDefinitions)
			{
				FilterDeviceDefinition filterDeviceDefinition = FilterDeviceDefinitions.FirstOrDefault((FilterDeviceDefinition d) => DeviceManager.IsDefinitionMatch(d, connectedDeviceDefinition));
				if (filterDeviceDefinition != null)
				{
					IDevice device = null;
					if (_CreatedDevicesByDefinition.ContainsKey(filterDeviceDefinition))
					{
						device = _CreatedDevicesByDefinition[filterDeviceDefinition];
					}
					if (device == null)
					{
						device = DeviceManager.Current.GetDevice(connectedDeviceDefinition);
						_CreatedDevicesByDefinition.Add(filterDeviceDefinition, device);
					}
					if (!device.IsInitialized)
					{
						Log("Attempting to initialize with DeviceId of " + device.DeviceId, null, "CheckForDevicesAsync");
						await device.InitializeAsync();
						this.DeviceInitialized?.Invoke(this, new DeviceEventArgs(device));
						Log("Device connected", null, "CheckForDevicesAsync");
					}
				}
			}
			List<FilterDeviceDefinition> list2 = new List<FilterDeviceDefinition>();
			foreach (FilterDeviceDefinition filteredDeviceDefinitionKey in _CreatedDevicesByDefinition.Keys)
			{
				IDevice device2 = _CreatedDevicesByDefinition[filteredDeviceDefinitionKey];
				if (!connectedDeviceDefinitions.Any((ConnectedDeviceDefinition cdd) => DeviceManager.IsDefinitionMatch(filteredDeviceDefinitionKey, cdd)) && device2.IsInitialized)
				{
					this.DeviceDisconnected?.Invoke(this, new DeviceEventArgs(device2));
					device2.Close();
					list2.Add(filteredDeviceDefinitionKey);
					Log("Disconnected", null, "CheckForDevicesAsync");
				}
			}
			foreach (FilterDeviceDefinition item in list2)
			{
				_CreatedDevicesByDefinition.Remove(item);
			}
			Log("Poll complete", null, "CheckForDevicesAsync");
		}
		catch (Exception ex)
		{
			Log("Hid polling error", ex, "CheckForDevicesAsync");
		}
		finally
		{
			_ListenSemaphoreSlim.Release();
		}
	}

	public void Stop()
	{
		_PollTimer.Stop();
	}

	public void Dispose()
	{
		if (_IsDisposed)
		{
			return;
		}
		_IsDisposed = true;
		Stop();
		_PollTimer?.Dispose();
		foreach (FilterDeviceDefinition key in _CreatedDevicesByDefinition.Keys)
		{
			_CreatedDevicesByDefinition[key].Dispose();
		}
		_CreatedDevicesByDefinition.Clear();
		_ListenSemaphoreSlim.Dispose();
		this.DeviceInitialized = null;
		this.DeviceDisconnected = null;
		GC.SuppressFinalize(this);
	}

	~DeviceListener()
	{
		Dispose();
	}
}
