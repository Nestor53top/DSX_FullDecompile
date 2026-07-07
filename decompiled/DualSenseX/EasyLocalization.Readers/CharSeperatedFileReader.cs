using System.Collections.Generic;
using System.IO;
using EasyLocalization.Localization;

namespace EasyLocalization.Readers;

public class CharSeperatedFileReader : FileReader
{
	private readonly char _separator;

	public CharSeperatedFileReader(string path, char separator = ';')
		: base(path)
	{
		_separator = separator;
	}

	internal override Dictionary<string, LocalizationEntry> GetEntries()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, LocalizationEntry> dictionary = new Dictionary<string, LocalizationEntry>();
		using StreamReader streamReader = File.OpenText(Path);
		while (!streamReader.EndOfStream)
		{
			string text = streamReader.ReadLine();
			string[] array = text.Split(new char[1] { _separator });
			if (array.Length == 1)
			{
				throw new FileFormatException("Each line needs to have at least 2 values (line: " + text + ")");
			}
			switch (array.Length)
			{
			default:
				dictionary.Add(array[0], new LocalizationEntry(array[1]));
				break;
			case 3:
				dictionary.Add(array[0], new LocalizationEntry(array[1], array[2]));
				break;
			case 4:
				dictionary.Add(array[0], new LocalizationEntry(array[1], array[2], array[3]));
				break;
			}
		}
		return dictionary;
	}
}
