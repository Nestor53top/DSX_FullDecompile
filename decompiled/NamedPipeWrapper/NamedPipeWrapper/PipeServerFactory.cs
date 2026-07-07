using System.IO.Pipes;

namespace NamedPipeWrapper;

internal static class PipeServerFactory
{
	public static NamedPipeServerStream CreateAndConnectPipe(string pipeName, PipeSecurity pipeSecurity)
	{
		NamedPipeServerStream namedPipeServerStream = CreatePipe(pipeName, pipeSecurity);
		namedPipeServerStream.WaitForConnection();
		return namedPipeServerStream;
	}

	public static NamedPipeServerStream CreatePipe(string pipeName, PipeSecurity pipeSecurity)
	{
		return new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.WriteThrough | PipeOptions.Asynchronous, 0, 0, pipeSecurity);
	}
}
