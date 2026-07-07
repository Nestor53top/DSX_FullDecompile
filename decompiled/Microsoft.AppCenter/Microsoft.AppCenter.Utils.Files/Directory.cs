using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.AppCenter.Utils.Files;

public class Directory
{
	private readonly DirectoryInfo _underlyingDirectoryInfo;

	public virtual string FullName => _underlyingDirectoryInfo.FullName;

	public Directory()
	{
	}

	public Directory(string directoryPath)
	{
		_underlyingDirectoryInfo = new DirectoryInfo(directoryPath);
	}

	public virtual IEnumerable<File> EnumerateFiles(string searchPattern)
	{
		return from fileInfo in _underlyingDirectoryInfo.EnumerateFiles(searchPattern)
			select new File(fileInfo);
	}

	public virtual void CreateFile(string name, string contents)
	{
		System.IO.File.WriteAllText(Path.Combine(FullName, name), contents);
	}

	public virtual void Create()
	{
		_underlyingDirectoryInfo.Create();
	}

	public virtual void Delete(bool recursive)
	{
		_underlyingDirectoryInfo.Delete(recursive);
	}

	public virtual bool Exists()
	{
		return _underlyingDirectoryInfo.Exists;
	}

	public virtual void Refresh()
	{
		_underlyingDirectoryInfo.Refresh();
	}
}
