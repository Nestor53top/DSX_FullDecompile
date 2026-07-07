using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGet;

internal class NullFileSystem : IFileSystem
{
	private static readonly NullFileSystem _instance = new NullFileSystem();

	public static NullFileSystem Instance => _instance;

	public ILogger Logger { get; set; }

	public string Root => string.Empty;

	private NullFileSystem()
	{
	}

	public void DeleteDirectory(string path, bool recursive)
	{
	}

	public IEnumerable<string> GetFiles(string path, string filter, bool recursive)
	{
		return Enumerable.Empty<string>();
	}

	public IEnumerable<string> GetDirectories(string path)
	{
		return Enumerable.Empty<string>();
	}

	public string GetFullPath(string path)
	{
		return path;
	}

	public void DeleteFile(string path)
	{
	}

	public void DeleteFiles(IEnumerable<IPackageFile> files, string rootDir)
	{
	}

	public bool FileExists(string path)
	{
		return false;
	}

	public bool DirectoryExists(string path)
	{
		return false;
	}

	public void AddFile(string path, Stream stream)
	{
	}

	public void AddFile(string path, Action<Stream> writeToStream)
	{
	}

	public void AddFiles(IEnumerable<IPackageFile> files, string rootDir)
	{
	}

	public Stream CreateFile(string path)
	{
		return Stream.Null;
	}

	public Stream OpenFile(string path)
	{
		return Stream.Null;
	}

	public DateTimeOffset GetLastModified(string path)
	{
		return DateTimeOffset.MinValue;
	}

	public DateTimeOffset GetCreated(string path)
	{
		return DateTimeOffset.MinValue;
	}

	public DateTimeOffset GetLastAccessed(string path)
	{
		return DateTimeOffset.MinValue;
	}

	public void MakeFileWritable(string path)
	{
	}

	public void MoveFile(string source, string destination)
	{
	}
}
