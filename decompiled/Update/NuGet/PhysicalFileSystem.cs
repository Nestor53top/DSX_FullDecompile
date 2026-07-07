using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NuGet.Resources;

namespace NuGet;

internal class PhysicalFileSystem : IFileSystem
{
	private readonly string _root;

	private ILogger _logger;

	public string Root => _root;

	public ILogger Logger
	{
		get
		{
			return _logger ?? NullLogger.Instance;
		}
		set
		{
			_logger = value;
		}
	}

	public PhysicalFileSystem(string root)
	{
		if (string.IsNullOrEmpty(root))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "root");
		}
		_root = root;
	}

	public virtual string GetFullPath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return Root;
		}
		return Path.Combine(Root, path);
	}

	public virtual void AddFile(string path, Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		AddFileCore(path, delegate(Stream targetStream)
		{
			stream.CopyTo(targetStream);
		});
	}

	public virtual void AddFile(string path, Action<Stream> writeToStream)
	{
		if (writeToStream == null)
		{
			throw new ArgumentNullException("writeToStream");
		}
		AddFileCore(path, writeToStream);
	}

	public virtual void AddFiles(IEnumerable<IPackageFile> files, string rootDir)
	{
		FileSystemExtensions.AddFiles(this, files, rootDir);
	}

	private void AddFileCore(string path, Action<Stream> writeToStream)
	{
		EnsureDirectory(Path.GetDirectoryName(path));
		using (Stream obj = File.Create(GetFullPath(path)))
		{
			writeToStream(obj);
		}
		WriteAddedFileAndDirectory(path);
	}

	private void WriteAddedFileAndDirectory(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directoryName))
		{
			Logger.Log(MessageLevel.Debug, NuGetResources.Debug_AddedFileToFolder, Path.GetFileName(path), directoryName);
		}
		else
		{
			Logger.Log(MessageLevel.Debug, NuGetResources.Debug_AddedFile, Path.GetFileName(path));
		}
	}

	public virtual void DeleteFile(string path)
	{
		if (!FileExists(path))
		{
			return;
		}
		try
		{
			MakeFileWritable(path);
			path = GetFullPath(path);
			File.Delete(path);
			string directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directoryName))
			{
				Logger.Log(MessageLevel.Debug, NuGetResources.Debug_RemovedFileFromFolder, Path.GetFileName(path), directoryName);
			}
			else
			{
				Logger.Log(MessageLevel.Debug, NuGetResources.Debug_RemovedFile, Path.GetFileName(path));
			}
		}
		catch (FileNotFoundException)
		{
		}
	}

	public virtual void DeleteFiles(IEnumerable<IPackageFile> files, string rootDir)
	{
		FileSystemExtensions.DeleteFiles(this, files, rootDir);
	}

	public virtual void DeleteDirectory(string path)
	{
		DeleteDirectory(path, recursive: false);
	}

	public virtual void DeleteDirectory(string path, bool recursive)
	{
		if (!DirectoryExists(path))
		{
			return;
		}
		try
		{
			path = GetFullPath(path);
			Directory.Delete(path, recursive);
			int num = 0;
			while (Directory.Exists(path) && num < 5)
			{
				Thread.Sleep(100);
				num++;
			}
			Logger.Log(MessageLevel.Debug, NuGetResources.Debug_RemovedFolder, path);
		}
		catch (DirectoryNotFoundException)
		{
		}
	}

	public virtual IEnumerable<string> GetFiles(string path, bool recursive)
	{
		return GetFiles(path, null, recursive);
	}

	public virtual IEnumerable<string> GetFiles(string path, string filter, bool recursive)
	{
		path = PathUtility.EnsureTrailingSlash(GetFullPath(path));
		if (string.IsNullOrEmpty(filter))
		{
			filter = "*.*";
		}
		try
		{
			if (!Directory.Exists(path))
			{
				return Enumerable.Empty<string>();
			}
			return Directory.EnumerateFiles(path, filter, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(MakeRelativePath);
		}
		catch (UnauthorizedAccessException)
		{
		}
		catch (DirectoryNotFoundException)
		{
		}
		return Enumerable.Empty<string>();
	}

	public virtual IEnumerable<string> GetDirectories(string path)
	{
		try
		{
			path = PathUtility.EnsureTrailingSlash(GetFullPath(path));
			if (!Directory.Exists(path))
			{
				return Enumerable.Empty<string>();
			}
			return Directory.EnumerateDirectories(path).Select(MakeRelativePath);
		}
		catch (UnauthorizedAccessException)
		{
		}
		catch (DirectoryNotFoundException)
		{
		}
		return Enumerable.Empty<string>();
	}

	public virtual DateTimeOffset GetLastModified(string path)
	{
		path = GetFullPath(path);
		if (File.Exists(path))
		{
			return File.GetLastWriteTimeUtc(path);
		}
		return Directory.GetLastWriteTimeUtc(path);
	}

	public DateTimeOffset GetCreated(string path)
	{
		path = GetFullPath(path);
		if (File.Exists(path))
		{
			return File.GetCreationTimeUtc(path);
		}
		return Directory.GetCreationTimeUtc(path);
	}

	public DateTimeOffset GetLastAccessed(string path)
	{
		path = GetFullPath(path);
		if (File.Exists(path))
		{
			return File.GetLastAccessTimeUtc(path);
		}
		return Directory.GetLastAccessTimeUtc(path);
	}

	public virtual bool FileExists(string path)
	{
		path = GetFullPath(path);
		return File.Exists(path);
	}

	public virtual bool DirectoryExists(string path)
	{
		path = GetFullPath(path);
		return Directory.Exists(path);
	}

	public virtual Stream OpenFile(string path)
	{
		path = GetFullPath(path);
		return File.OpenRead(path);
	}

	public virtual Stream CreateFile(string path)
	{
		string fullPath = GetFullPath(path);
		string directoryName = Path.GetDirectoryName(fullPath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		WriteAddedFileAndDirectory(path);
		return File.Create(fullPath);
	}

	protected string MakeRelativePath(string fullPath)
	{
		return fullPath.Substring(Root.Length).TrimStart(new char[1] { Path.DirectorySeparatorChar });
	}

	protected virtual void EnsureDirectory(string path)
	{
		path = GetFullPath(path);
		Directory.CreateDirectory(path);
	}

	public void MakeFileWritable(string path)
	{
		path = GetFullPath(path);
		FileAttributes attributes = File.GetAttributes(path);
		if (attributes.HasFlag(FileAttributes.ReadOnly))
		{
			File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
		}
	}

	public virtual void MoveFile(string source, string destination)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (destination == null)
		{
			throw new ArgumentNullException("destination");
		}
		string fullPath = GetFullPath(source);
		string fullPath2 = GetFullPath(destination);
		if (string.Equals(fullPath, fullPath2, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		try
		{
			File.Move(fullPath, fullPath2);
		}
		catch (IOException)
		{
			File.Delete(fullPath);
		}
	}
}
