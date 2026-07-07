using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mono.Options;

public abstract class ArgumentSource
{
	public abstract string Description { get; }

	public abstract string[] GetNames();

	public abstract bool GetArguments(string value, out IEnumerable<string> replacement);

	public static IEnumerable<string> GetArgumentsFromFile(string file)
	{
		return GetArguments(File.OpenText(file), close: true);
	}

	public static IEnumerable<string> GetArguments(TextReader reader)
	{
		return GetArguments(reader, close: false);
	}

	private static IEnumerable<string> GetArguments(TextReader reader, bool close)
	{
		try
		{
			StringBuilder arg = new StringBuilder();
			while (true)
			{
				string text;
				string line = (text = reader.ReadLine());
				if (text == null)
				{
					break;
				}
				int t = line.Length;
				for (int i = 0; i < t; i++)
				{
					char c = line[i];
					switch (c)
					{
					case '"':
					case '\'':
					{
						char c2 = c;
						for (i++; i < t; i++)
						{
							c = line[i];
							if (c == c2)
							{
								break;
							}
							arg.Append(c);
						}
						break;
					}
					case ' ':
						if (arg.Length > 0)
						{
							yield return arg.ToString();
							arg.Length = 0;
						}
						break;
					default:
						arg.Append(c);
						break;
					}
				}
				if (arg.Length > 0)
				{
					yield return arg.ToString();
					arg.Length = 0;
				}
			}
		}
		finally
		{
			if (close)
			{
				reader.Close();
			}
		}
	}
}
