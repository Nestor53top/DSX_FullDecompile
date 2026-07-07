using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion;

public interface IIngestion : IDisposable
{
	bool IsEnabled { get; }

	void SetLogUrl(string logUrl);

	IServiceCall Call(string appSecret, Guid installId, IList<Log> logs);

	void Close();
}
