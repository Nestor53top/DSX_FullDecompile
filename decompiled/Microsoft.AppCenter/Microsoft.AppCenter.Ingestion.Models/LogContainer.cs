using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

public class LogContainer
{
	[JsonProperty(PropertyName = "logs")]
	public IList<Log> Logs { get; set; }

	public LogContainer()
	{
	}

	public LogContainer(IList<Log> logs)
	{
		Logs = logs;
	}

	public virtual void Validate()
	{
		if (Logs == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Logs");
		}
		if (Logs == null)
		{
			return;
		}
		if (Logs.Count < 1)
		{
			throw new ValidationException(ValidationException.Rule.MinItems, "Logs", 1);
		}
		foreach (Log log in Logs)
		{
			log?.Validate();
		}
	}
}
