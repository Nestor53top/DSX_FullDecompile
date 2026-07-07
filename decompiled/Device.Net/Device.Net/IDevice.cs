using System;
using System.Threading.Tasks;

namespace Device.Net;

public interface IDevice : IDisposable
{
	bool IsInitialized { get; }

	string DeviceId { get; }

	ConnectedDeviceDefinitionBase ConnectedDeviceDefinition { get; }

	Task<ReadResult> ReadAsync();

	Task WriteAsync(byte[] data);

	Task InitializeAsync();

	Task<ReadResult> WriteAndReadAsync(byte[] writeBuffer);

	void Close();

	Task Flush();
}
