using System.IO.Pipes;

namespace NamedPipeWrapper;

public class NamedPipeServer<TReadWrite> : Server<TReadWrite, TReadWrite> where TReadWrite : class
{
	public NamedPipeServer(string pipeName)
		: base(pipeName, (PipeSecurity)null)
	{
	}

	public NamedPipeServer(string pipeName, PipeSecurity pipeSecurity)
		: base(pipeName, pipeSecurity)
	{
	}
}
