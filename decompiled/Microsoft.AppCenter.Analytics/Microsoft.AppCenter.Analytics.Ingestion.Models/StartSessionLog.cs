using System;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Analytics.Ingestion.Models;

[JsonObject("startSession")]
public class StartSessionLog : Log
{
	internal static StartSessionLog Empty = new StartSessionLog();

	internal const string JsonIdentifier = "startSession";

	public StartSessionLog()
	{
	}

	public StartSessionLog(Microsoft.AppCenter.Ingestion.Models.Device device, DateTime? timestamp = null, Guid? sid = null)
		: base(device, timestamp, sid)
	{
	}

	public override void Validate()
	{
		base.Validate();
	}
}
