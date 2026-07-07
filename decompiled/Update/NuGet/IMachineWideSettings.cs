using System.Collections.Generic;

namespace NuGet;

internal interface IMachineWideSettings
{
	IEnumerable<Settings> Settings { get; }
}
