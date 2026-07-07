using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Channel;

public interface IChannelGroup : IChannel, IDisposable
{
	void AddChannel(IChannelUnit channel);

	IChannelUnit AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches);

	void SetLogUrl(string logUrl);

	Task WaitStorageOperationsAsync();

	Task<bool> SetMaxStorageSizeAsync(long sizeInBytes);
}
