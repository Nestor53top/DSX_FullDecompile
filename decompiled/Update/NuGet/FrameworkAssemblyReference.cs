using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;

namespace NuGet;

internal class FrameworkAssemblyReference : IFrameworkTargetable
{
	public string AssemblyName { get; private set; }

	public IEnumerable<FrameworkName> SupportedFrameworks { get; private set; }

	public FrameworkAssemblyReference(string assemblyName)
		: this(assemblyName, Enumerable.Empty<FrameworkName>())
	{
	}

	public FrameworkAssemblyReference(string assemblyName, IEnumerable<FrameworkName> supportedFrameworks)
	{
		if (string.IsNullOrEmpty(assemblyName))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, CommonResources.Argument_Cannot_Be_Null_Or_Empty, new object[1] { "assemblyName" }));
		}
		if (supportedFrameworks == null)
		{
			throw new ArgumentNullException("supportedFrameworks");
		}
		AssemblyName = assemblyName;
		SupportedFrameworks = supportedFrameworks;
	}
}
