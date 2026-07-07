using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Runtime.Versioning;

namespace NuGet;

internal class ZipPackageFile : IPackageFile, IFrameworkTargetable
{
	private readonly Func<Stream> _streamFactory;

	private readonly FrameworkName _targetFramework;

	public string Path { get; private set; }

	public string EffectivePath { get; private set; }

	public FrameworkName TargetFramework => _targetFramework;

	IEnumerable<FrameworkName> IFrameworkTargetable.SupportedFrameworks
	{
		get
		{
			if (TargetFramework != null)
			{
				yield return TargetFramework;
			}
		}
	}

	public ZipPackageFile(PackagePart part)
		: this(UriUtility.GetPath(part.Uri), part.GetStream().ToStreamFactory())
	{
	}

	public ZipPackageFile(IPackageFile file)
		: this(file.Path, file.GetStream().ToStreamFactory())
	{
	}

	protected ZipPackageFile(string path, Func<Stream> streamFactory)
	{
		Path = path;
		_streamFactory = streamFactory;
		_targetFramework = VersionUtility.ParseFrameworkNameFromFilePath(path, out var effectivePath);
		EffectivePath = effectivePath;
	}

	public Stream GetStream()
	{
		return _streamFactory();
	}

	public override string ToString()
	{
		return Path;
	}
}
