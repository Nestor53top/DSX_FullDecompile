using System.IO.Pipes;
using NamedPipeWrapper.IO;

namespace NamedPipeWrapper;

internal static class PipeClientFactory
{
	public static PipeStreamWrapper<TRead, TWrite> Connect<TRead, TWrite>(string pipeName) where TRead : class where TWrite : class
	{
		return new PipeStreamWrapper<TRead, TWrite>(CreateAndConnectPipe(pipeName));
	}

	public static NamedPipeClientStream CreateAndConnectPipe(string pipeName)
	{
		NamedPipeClientStream namedPipeClientStream = CreatePipe(pipeName);
		namedPipeClientStream.Connect();
		return namedPipeClientStream;
	}

	private static NamedPipeClientStream CreatePipe(string pipeName)
	{
		return new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous);
	}
}
