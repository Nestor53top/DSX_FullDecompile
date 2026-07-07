using System;
using System.Linq;

namespace NuGet;

internal abstract class PackageRepositoryBase : IPackageRepository
{
	private PackageSaveModes _packageSave;

	public abstract string Source { get; }

	public PackageSaveModes PackageSaveMode
	{
		get
		{
			return _packageSave;
		}
		set
		{
			if (value == PackageSaveModes.None)
			{
				throw new ArgumentException("PackageSave cannot be set to None");
			}
			_packageSave = value;
		}
	}

	public abstract bool SupportsPrereleasePackages { get; }

	protected PackageRepositoryBase()
	{
		_packageSave = PackageSaveModes.Nupkg;
	}

	public abstract IQueryable<IPackage> GetPackages();

	public virtual void AddPackage(IPackage package)
	{
		throw new NotSupportedException();
	}

	public virtual void RemovePackage(IPackage package)
	{
		throw new NotSupportedException();
	}
}
