using System;
using System.IO;

namespace NuGet;

internal class PhysicalPackageAssemblyReference : PhysicalPackageFile, IPackageAssemblyReference, IPackageFile, IFrameworkTargetable
{
	public string Name
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Path))
			{
				return System.IO.Path.GetFileName(base.Path);
			}
			return string.Empty;
		}
	}

	public PhysicalPackageAssemblyReference()
	{
	}

	public PhysicalPackageAssemblyReference(PhysicalPackageFile file)
		: base(file)
	{
	}

	public PhysicalPackageAssemblyReference(Func<Stream> streamFactory)
		: base(streamFactory)
	{
	}
}
