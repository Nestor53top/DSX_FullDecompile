using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models;

public class Exception
{
	[JsonProperty(PropertyName = "type")]
	public string Type { get; set; }

	[JsonProperty(PropertyName = "message")]
	public string Message { get; set; }

	[JsonProperty(PropertyName = "stackTrace")]
	public string StackTrace { get; set; }

	[JsonProperty(PropertyName = "frames")]
	public IList<StackFrame> Frames { get; set; }

	[JsonProperty(PropertyName = "innerExceptions")]
	public IList<Exception> InnerExceptions { get; set; }

	[JsonProperty(PropertyName = "wrapperSdkName")]
	public string WrapperSdkName { get; set; }

	public Exception()
	{
	}

	public Exception(string type, string message = null, string stackTrace = null, IList<StackFrame> frames = null, IList<Exception> innerExceptions = null, string wrapperSdkName = null)
	{
		Type = type;
		Message = message;
		StackTrace = stackTrace;
		Frames = frames;
		InnerExceptions = innerExceptions;
		WrapperSdkName = wrapperSdkName;
	}

	public virtual void Validate()
	{
		if (Type == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Type");
		}
		if (InnerExceptions == null)
		{
			return;
		}
		foreach (Exception innerException in InnerExceptions)
		{
			innerException?.Validate();
		}
	}
}
