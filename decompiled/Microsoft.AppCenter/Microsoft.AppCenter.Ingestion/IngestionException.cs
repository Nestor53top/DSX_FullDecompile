using System;

namespace Microsoft.AppCenter.Ingestion;

public class IngestionException : AppCenterException
{
	private const string DefaultMessage = "The ingestion operation failed";

	public virtual bool IsRecoverable => false;

	public IngestionException(string message)
		: base(message)
	{
	}

	public IngestionException(Exception innerException)
		: base("The ingestion operation failed", innerException)
	{
	}

	public IngestionException()
		: base("The ingestion operation failed")
	{
	}
}
