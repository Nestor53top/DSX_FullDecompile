using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;

namespace NuGet;

internal class PhysicalPackageFile : IPackageFile, IFrameworkTargetable
{
	private readonly Func<Stream> _streamFactory;

	private string _targetPath;

	private FrameworkName _targetFramework;

	public string SourcePath { get; set; }

	public string TargetPath
	{
		get
		{
			return _targetPath;
		}
		set
		{
			if (string.Compare(_targetPath, value, StringComparison.OrdinalIgnoreCase) != 0)
			{
				_targetPath = value;
				_targetFramework = VersionUtility.ParseFrameworkNameFromFilePath(_targetPath, out var effectivePath);
				EffectivePath = effectivePath;
			}
		}
	}

	public string Path => TargetPath;

	public string EffectivePath { get; private set; }

	public FrameworkName TargetFramework => _targetFramework;

	public IEnumerable<FrameworkName> SupportedFrameworks
	{
		get
		{
			if (TargetFramework != null)
			{
				yield return TargetFramework;
			}
		}
	}

	public PhysicalPackageFile()
	{
	}

	public PhysicalPackageFile(PhysicalPackageFile file)
	{
		SourcePath = file.SourcePath;
		TargetPath = file.TargetPath;
	}

	internal PhysicalPackageFile(Func<Stream> streamFactory)
	{
		_streamFactory = streamFactory;
	}

	public Stream GetStream()
	{
		if (_streamFactory == null)
		{
			return File.OpenRead(SourcePath);
		}
		return _streamFactory();
	}

	public override string ToString()
	{
		return TargetPath;
	}

	public override bool Equals(object obj)
	{
		if (obj is PhysicalPackageFile physicalPackageFile && string.Equals(SourcePath, physicalPackageFile.SourcePath, StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(TargetPath, physicalPackageFile.TargetPath, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 0;
		if (SourcePath != null)
		{
			num = SourcePath.GetHashCode();
		}
		if (TargetPath != null)
		{
			num = num * 4567 + TargetPath.GetHashCode();
		}
		return num;
	}
}
