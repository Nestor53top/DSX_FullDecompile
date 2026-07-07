using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel;

public interface IChannelUnit : IChannel, IDisposable
{
	Task EnqueueAsync(Log log);
}
