using System.Collections.Generic;
using System.Runtime.Versioning;

namespace NuGet;

internal interface IFrameworkTargetable
{
	IEnumerable<FrameworkName> SupportedFrameworks { get; }
}
