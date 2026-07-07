using System;
using System.Runtime.Serialization;

namespace Medallion.Threading;

[Serializable]
public sealed class DeadlockException : InvalidOperationException
{
	public DeadlockException()
		: this("A deadlock occurred")
	{
	}

	public DeadlockException(string message)
		: base(message)
	{
	}

	public DeadlockException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	private DeadlockException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
