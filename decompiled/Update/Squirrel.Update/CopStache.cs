using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Squirrel.Update;

public static class CopStache
{
	public static string Render(string template, Dictionary<string, string> identifiers)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = template.Split(new char[1] { '\n' });
		foreach (string value in array)
		{
			identifiers["RandomGuid"] = Guid.NewGuid().ToString();
			foreach (string key in identifiers.Keys)
			{
				stringBuilder.Replace("{{" + key + "}}", SecurityElement.Escape(identifiers[key]));
			}
			stringBuilder.AppendLine(value);
		}
		return stringBuilder.ToString();
	}
}
