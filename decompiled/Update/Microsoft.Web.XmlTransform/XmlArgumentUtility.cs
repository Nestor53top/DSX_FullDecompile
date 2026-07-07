using System.Collections.Generic;

namespace Microsoft.Web.XmlTransform;

internal static class XmlArgumentUtility
{
	internal static IList<string> SplitArguments(string argumentString)
	{
		if (argumentString.IndexOf(',') == -1)
		{
			return new string[1] { argumentString };
		}
		List<string> list = new List<string>();
		list.AddRange(argumentString.Split(new char[1] { ',' }));
		IList<string> list2 = RecombineArguments(list, ',');
		TrimStrings(list2);
		return list2;
	}

	private static IList<string> RecombineArguments(IList<string> arguments, char separator)
	{
		List<string> list = new List<string>();
		string text = null;
		int num = 0;
		foreach (string argument in arguments)
		{
			text = ((text != null) ? (text + separator + argument) : argument);
			num += CountParens(argument);
			if (num == 0)
			{
				list.Add(text);
				text = null;
			}
		}
		if (text != null)
		{
			list.Add(text);
		}
		if (arguments.Count != list.Count)
		{
			arguments = list;
		}
		return arguments;
	}

	private static void TrimStrings(IList<string> arguments)
	{
		for (int i = 0; i < arguments.Count; i++)
		{
			arguments[i] = arguments[i].Trim();
		}
	}

	private static int CountParens(string str)
	{
		int num = 0;
		for (int i = 0; i < str.Length; i++)
		{
			switch (str[i])
			{
			case '(':
				num++;
				break;
			case ')':
				num--;
				break;
			}
		}
		return num;
	}
}
