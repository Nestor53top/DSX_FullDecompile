using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Device.Net;
using Device.Net.Exceptions;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;

namespace Hid.Net.Windows;

public sealed class WindowsHidDevice : WindowsDeviceBase, IHidDevice, IDevice, IDisposable
{
	private Stream _ReadFileStream;

	private Stream _WriteFileStream;

	private SafeFileHandle _ReadSafeFileHandle;

	private SafeFileHandle _WriteSafeFileHandle;

	private bool _IsClosing;

	private bool disposed;

	private readonly ushort? _WriteBufferSize;

	private readonly ushort? _ReadBufferSize;

	private bool ReadBufferHasReportId => ReadBufferSize == 65;

	protected override string LogSection => "WindowsHidDevice";

	public override bool IsInitialized
	{
		get
		{
			if (_ReadSafeFileHandle != null)
			{
				return !_ReadSafeFileHandle.IsInvalid;
			}
			return false;
		}
	}

	public override ushort WriteBufferSize
	{
		get
		{
			ushort? writeBufferSize = _WriteBufferSize;
			if (!writeBufferSize.HasValue)
			{
				if (base.ConnectedDeviceDefinition != null)
				{
					return (ushort)base.ConnectedDeviceDefinition.WriteBufferSize.Value;
				}
				return 0;
			}
			return writeBufferSize.GetValueOrDefault();
		}
	}

	public override ushort ReadBufferSize
	{
		get
		{
			ushort? readBufferSize = _ReadBufferSize;
			if (!readBufferSize.HasValue)
			{
				if (base.ConnectedDeviceDefinition != null)
				{
					return (ushort)base.ConnectedDeviceDefinition.ReadBufferSize.Value;
				}
				return 0;
			}
			return readBufferSize.GetValueOrDefault();
		}
	}

	public bool? IsReadOnly { get; private set; }

	public byte DefaultReportId { get; set; }

	public IHidApiService HidService { get; }

	public WindowsHidDevice(string deviceId)
		: this(deviceId, null, null, null, null)
	{
	}

	public WindowsHidDevice(string deviceId, ILogger logger, ITracer tracer)
		: this(deviceId, null, null, logger, tracer)
	{
	}

	public WindowsHidDevice(string deviceId, ushort? writeBufferSize, ushort? readBufferSize, ILogger logger, ITracer tracer)
		: this(deviceId, writeBufferSize, readBufferSize, logger, tracer, null)
	{
	}

	public WindowsHidDevice(string deviceId, ushort? writeBufferSize, ushort? readBufferSize, ILogger logger, ITracer tracer, IHidApiService hidService)
		: base(deviceId, logger, tracer)
	{
		_WriteBufferSize = writeBufferSize;
		_ReadBufferSize = readBufferSize;
		HidService = hidService ?? new WindowsHidApiService(logger);
	}

	private bool Initialize()
	{
		try
		{
			Close();
			if (string.IsNullOrEmpty(base.DeviceId))
			{
				throw new ValidationException("DeviceId must be specified before Initialize can be called.");
			}
			_ReadSafeFileHandle = HidService.CreateReadConnection(base.DeviceId, FileAccessRights.GenericRead);
			_WriteSafeFileHandle = HidService.CreateWriteConnection(base.DeviceId);
			if (_ReadSafeFileHandle.IsInvalid)
			{
				throw new ApiException("Could not open connection for reading");
			}
			IsReadOnly = _WriteSafeFileHandle.IsInvalid;
			if (IsReadOnly.Value)
			{
				base.Logger?.Log(Messages.WarningMessageOpeningInReadonlyMode(base.DeviceId), "WindowsHidDevice", null, LogLevel.Warning);
			}
			base.ConnectedDeviceDefinition = HidService.GetDeviceDefinition(base.DeviceId, _ReadSafeFileHandle);
			ushort readBufferSize = ReadBufferSize;
			ushort writeBufferSize = WriteBufferSize;
			if (readBufferSize == 0)
			{
				throw new ValidationException("ReadBufferSize must be specified. HidD_GetAttributes may have failed or returned an InputReportByteLength of 0. Please specify this argument in the constructor");
			}
			_ReadFileStream = HidService.OpenRead(_ReadSafeFileHandle, readBufferSize);
			if (_ReadFileStream.CanRead)
			{
				base.Logger?.Log("Read file stream opened successfully", "WindowsHidDevice", null, LogLevel.Information);
			}
			else
			{
				base.Logger?.Log("Read file stream cannot be read from", "WindowsHidDevice", null, LogLevel.Warning);
			}
			if (!IsReadOnly.Value)
			{
				if (writeBufferSize == 0)
				{
					throw new ValidationException("WriteBufferSize must be specified. HidD_GetAttributes may have failed or returned an OutputReportByteLength of 0. Please specify this argument in the constructor");
				}
				_WriteFileStream = HidService.OpenWrite(_WriteSafeFileHandle, writeBufferSize);
				if (_WriteFileStream.CanWrite)
				{
					base.Logger?.Log("Write file stream opened successfully", "WindowsHidDevice", null, LogLevel.Information);
				}
				else
				{
					base.Logger?.Log("Write file stream cannot be written to", "WindowsHidDevice", null, LogLevel.Warning);
				}
			}
		}
		catch (Exception ex)
		{
			base.Logger?.Log("Initialize error.", "WindowsHidDevice", ex, LogLevel.Error);
			throw;
		}
		return true;
	}

	public void Close()
	{
		if (_IsClosing)
		{
			return;
		}
		_IsClosing = true;
		try
		{
			_ReadFileStream?.Dispose();
			_WriteFileStream?.Dispose();
			_ReadFileStream = null;
			_WriteFileStream = null;
			if (_ReadSafeFileHandle != null)
			{
				_ReadSafeFileHandle.Dispose();
				_ReadSafeFileHandle = null;
			}
			if (_WriteSafeFileHandle != null)
			{
				_WriteSafeFileHandle.Dispose();
				_WriteSafeFileHandle = null;
			}
		}
		catch (Exception)
		{
		}
		_IsClosing = false;
	}

	public override void Dispose()
	{
		if (!disposed)
		{
			GC.SuppressFinalize(this);
			disposed = true;
			Close();
			base.Dispose();
		}
	}

	public override async Task InitializeAsync()
	{
		if (disposed)
		{
			throw new ValidationException("This device has already been disposed");
		}
		await Task.Run(() => Initialize());
	}

	public override async Task<ReadResult> ReadAsync()
	{
		byte[] data = (await ReadReportAsync()).Data;
		base.Tracer?.Trace(isWrite: false, data);
		return data;
	}

	public async Task<ReadReport> ReadReportAsync()
	{
		byte? reportId = null;
		if (_ReadFileStream == null)
		{
			throw new NotInitializedException("The device has not been initialized.");
		}
		byte[] bytes = new byte[ReadBufferSize];
		try
		{
			await _ReadFileStream.ReadAsync(bytes, 0, bytes.Length);
		}
		catch (Exception ex)
		{
			Log("An error occurred while attempting to read from the device", ex, "ReadReportAsync");
			throw new IOException("An error occurred while attempting to read from the device", ex);
		}
		if (ReadBufferHasReportId)
		{
			reportId = bytes.First();
		}
		byte[] data = (ReadBufferHasReportId ? DeviceBase.RemoveFirstByte(bytes) : bytes);
		return new ReadReport(reportId, data);
	}

	public override Task WriteAsync(byte[] data)
	{
		return WriteReportAsync(data, 0);
	}

	public async Task WriteReportAsync(byte[] data, byte? reportId)
	{
		if (IsReadOnly.HasValue && IsReadOnly.Value)
		{
			throw new ValidationException("This device was opened in Read Only mode.");
		}
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (_WriteFileStream == null)
		{
			throw new NotInitializedException("The device has not been initialized");
		}
		if (data.Length > WriteBufferSize)
		{
			throw new ValidationException($"Data is longer than {WriteBufferSize - 1} bytes which is the device's OutputReportByteLength.");
		}
		byte[] bytes;
		if (WriteBufferSize == 65)
		{
			if (WriteBufferSize == data.Length)
			{
				throw new DeviceException("The data sent to the device was a the same length as the HidCollectionCapabilities.OutputReportByteLength. This probably indicates that DataHasExtraByte should be set to false.");
			}
			bytes = new byte[WriteBufferSize];
			Array.Copy(data, 0, bytes, 1, data.Length);
			bytes[0] = reportId ?? DefaultReportId;
		}
		else
		{
			bytes = data;
		}
		if (_WriteFileStream.CanWrite)
		{
			try
			{
				await _WriteFileStream.WriteAsync(bytes, 0, bytes.Length);
				base.Tracer?.Trace(isWrite: true, bytes);
				return;
			}
			catch (Exception ex)
			{
				Log("An error occurred while attempting to write to the device", ex, "WriteReportAsync");
				throw new IOException("An error occurred while attempting to write to the device", ex);
			}
		}
		throw new IOException("The file stream cannot be written to");
	}

	~WindowsHidDevice()
	{
		Dispose();
	}

	Task<ReadResult> IDevice.WriteAndReadAsync(byte[] writeBuffer)
	{
		return WriteAndReadAsync(writeBuffer);
	}
}
