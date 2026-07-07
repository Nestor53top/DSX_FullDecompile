using System.Collections.Generic;
using System.IO;
using EasyLocalization.Localization;

namespace EasyLocalization.Readers;

public abstract class FileReader
{
	protected string Path;

	protected FileReader(string path)
	{
		if (!File.Exists(path))
		{
			throw new FileNotFoundException("File '" + path + "' not found.", path);
		}
		Path = path;
	}

	internal abstract Dictionary<string, LocalizationEntry> GetEntries();
}
