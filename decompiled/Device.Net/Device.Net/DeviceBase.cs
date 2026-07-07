using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Device.Net;

public abstract class DeviceBase : IDisposable
{
	private readonly SemaphoreSlim _WriteAndReadLock = new SemaphoreSlim(1, 1);

	private bool disposed;

	private string _LogRegion;

	public abstract ushort WriteBufferSize { get; }

	public abstract ushort ReadBufferSize { get; }

	public abstract bool IsInitialized { get; }

	public ConnectedDeviceDefinitionBase ConnectedDeviceDefinition { get; set; }

	public string DeviceId { get; }

	public ILogger Logger { get; }

	public ITracer Tracer { get; }

	protected DeviceBase(string deviceId, ILogger logger, ITracer tracer)
	{
		DeviceId = deviceId ?? throw new ArgumentNullException("deviceId");
		Tracer = tracer;
		Logger = logger;
	}

	private void Log(string message, string region, Exception ex, LogLevel logLevel)
	{
		Logger?.Log(message, region, ex, logLevel);
	}

	private void Log(string message, Exception ex, LogLevel logLevel, [CallerMemberName] string callMemberName = null)
	{
		if (_LogRegion == null)
		{
			_LogRegion = GetType().Name;
		}
		Log(message, _LogRegion + " - " + callMemberName, ex, logLevel);
	}

	protected void Log(string message, [CallerMemberName] string callMemberName = null)
	{
		Log(message, null, LogLevel.Information, callMemberName);
	}

	protected void Log(string message, Exception ex, [CallerMemberName] string callMemberName = null)
	{
		Log(message, ex, LogLevel.Error, callMemberName);
	}

	protected void Log(string message, LogLevel logLevel, [CallerMemberName] string callMemberName = null)
	{
		Log(message, null, logLevel, callMemberName);
	}

	public abstract Task<ReadResult> ReadAsync();

	public abstract Task WriteAsync(byte[] data);

	public virtual Task Flush()
	{
		throw new NotImplementedException("Flush has only been implemented on Serial Port devices. Please log a Github issue if you need it.");
	}

	public async Task<ReadResult> WriteAndReadAsync(byte[] writeBuffer)
	{
		await _WriteAndReadLock.WaitAsync();
		try
		{
			await WriteAsync(writeBuffer);
			ReadResult result = await ReadAsync();
			Log(Messages.SuccessMessageWriteAndReadCalled, "WriteAndReadAsync");
			return result;
		}
		catch (Exception ex)
		{
			Log("Read/Write Error", ex, "WriteAndReadAsync");
			throw;
		}
		finally
		{
			_WriteAndReadLock.Release();
		}
	}

	public static byte[] RemoveFirstByte(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		int num = bytes.Length - 1;
		byte[] array = new byte[num];
		Array.Copy(bytes, 1, array, 0, num);
		return array;
	}

	public virtual void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			_WriteAndReadLock.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	~DeviceBase()
	{
		Dispose();
	}
}
