using System.Collections.Generic;
using Microsoft.AppCenter.Crashes.Ingestion.Models;

namespace Microsoft.AppCenter.Crashes.Windows.Utils;

internal class ErrorExceptionAndBinaries
{
	public Exception Exception;

	public IList<Binary> Binaries;
}
