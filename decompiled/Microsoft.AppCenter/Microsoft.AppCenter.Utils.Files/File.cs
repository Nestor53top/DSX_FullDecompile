using System.IO;

namespace Microsoft.AppCenter.Utils.Files;

public class File
{
	private readonly FileInfo _underlyingFileInfo;

	public virtual string Name => _underlyingFileInfo.Name;

	public virtual string FullName => _underlyingFileInfo.FullName;

	public File()
	{
	}

	public File(string filePath)
		: this(new FileInfo(filePath))
	{
	}

	internal File(FileInfo fileInfo)
	{
		_underlyingFileInfo = fileInfo;
	}

	public virtual void Delete()
	{
		_underlyingFileInfo.Delete();
	}

	public virtual string ReadAllText()
	{
		return System.IO.File.ReadAllText(FullName);
	}
}
