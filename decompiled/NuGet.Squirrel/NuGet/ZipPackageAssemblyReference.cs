using System.IO;
using System.Text;

namespace NuGet;

internal class ZipPackageAssemblyReference : ZipPackageFile, IPackageAssemblyReference, IPackageFile, IFrameworkTargetable
{
	public string Name => System.IO.Path.GetFileName(base.Path);

	public ZipPackageAssemblyReference(IPackageFile file)
		: base(file)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (base.TargetFramework != null)
		{
			stringBuilder.Append(base.TargetFramework).Append(" ");
		}
		stringBuilder.Append(Name).AppendFormat(" ({0})", base.Path);
		return stringBuilder.ToString();
	}
}
