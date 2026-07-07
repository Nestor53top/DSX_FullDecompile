using System.Collections.Generic;

namespace NuGet;

internal interface IBatchProcessor<in T>
{
	void BeginProcessing(IEnumerable<T> batch, PackageAction action);

	void EndProcessing();
}
