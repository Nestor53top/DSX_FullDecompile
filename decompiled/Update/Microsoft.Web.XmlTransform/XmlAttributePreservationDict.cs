using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlAttributePreservationDict
{
	private List<string> orderedAttributes = new List<string>();

	private Dictionary<string, string> leadingSpaces = new Dictionary<string, string>();

	private string attributeNewLineString;

	private bool computedOneAttributePerLine;

	private bool oneAttributePerLine;

	private bool OneAttributePerLine
	{
		get
		{
			if (!computedOneAttributePerLine)
			{
				computedOneAttributePerLine = true;
				oneAttributePerLine = ComputeOneAttributePerLine();
			}
			return oneAttributePerLine;
		}
	}

	internal void ReadPreservationInfo(string elementStartTag)
	{
		WhitespaceTrackingTextReader whitespaceReader = new WhitespaceTrackingTextReader(new StringReader(elementStartTag));
		int characterPosition = EnumerateAttributes(elementStartTag, delegate(int line, int linePosition, string attributeName)
		{
			orderedAttributes.Add(attributeName);
			if (whitespaceReader.ReadToPosition(line, linePosition))
			{
				leadingSpaces.Add(attributeName, whitespaceReader.PrecedingWhitespace);
			}
		});
		if (whitespaceReader.ReadToPosition(characterPosition))
		{
			leadingSpaces.Add(string.Empty, whitespaceReader.PrecedingWhitespace);
		}
	}

	private int EnumerateAttributes(string elementStartTag, Action<int, int, string> onAttributeSpotted)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		bool flag = elementStartTag.EndsWith("/>", StringComparison.Ordinal);
		string s = elementStartTag;
		if (!flag)
		{
			s = elementStartTag.Substring(0, elementStartTag.Length - 1) + "/>";
		}
		XmlTextReader val = new XmlTextReader((TextReader)new StringReader(s));
		val.Namespaces = false;
		((XmlReader)val).Read();
		bool flag2 = ((XmlReader)val).MoveToFirstAttribute();
		while (flag2)
		{
			onAttributeSpotted(val.LineNumber, val.LinePosition, ((XmlReader)val).Name);
			flag2 = ((XmlReader)val).MoveToNextAttribute();
		}
		int num = elementStartTag.Length;
		if (flag)
		{
			num--;
		}
		return num;
	}

	internal void WritePreservedAttributes(XmlAttributePreservingWriter writer, XmlAttributeCollection attributes)
	{
		string text = null;
		if (attributeNewLineString != null)
		{
			text = writer.SetAttributeNewLineString(attributeNewLineString);
		}
		try
		{
			foreach (string orderedAttribute in orderedAttributes)
			{
				XmlAttribute val = attributes[orderedAttribute];
				if (val != null)
				{
					if (leadingSpaces.ContainsKey(orderedAttribute))
					{
						writer.WriteAttributeWhitespace(leadingSpaces[orderedAttribute]);
					}
					((XmlNode)val).WriteTo((XmlWriter)(object)writer);
				}
			}
			if (leadingSpaces.ContainsKey(string.Empty))
			{
				writer.WriteAttributeTrailingWhitespace(leadingSpaces[string.Empty]);
			}
		}
		finally
		{
			if (text != null)
			{
				writer.SetAttributeNewLineString(text);
			}
		}
	}

	internal void UpdatePreservationInfo(XmlAttributeCollection updatedAttributes, XmlFormatter formatter)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		if (((XmlNamedNodeMap)updatedAttributes).Count == 0)
		{
			if (orderedAttributes.Count > 0)
			{
				leadingSpaces.Clear();
				orderedAttributes.Clear();
			}
			return;
		}
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		foreach (string orderedAttribute in orderedAttributes)
		{
			dictionary[orderedAttribute] = false;
		}
		foreach (XmlAttribute item in (XmlNamedNodeMap)updatedAttributes)
		{
			XmlAttribute val = item;
			if (!dictionary.ContainsKey(((XmlNode)val).Name))
			{
				orderedAttributes.Add(((XmlNode)val).Name);
			}
			dictionary[((XmlNode)val).Name] = true;
		}
		bool flag = true;
		string text = null;
		foreach (string orderedAttribute2 in orderedAttributes)
		{
			bool flag2 = dictionary[orderedAttribute2];
			if (!flag2)
			{
				if (leadingSpaces.ContainsKey(orderedAttribute2))
				{
					string text2 = leadingSpaces[orderedAttribute2];
					if (flag)
					{
						if (text == null)
						{
							text = text2;
						}
					}
					else if (ContainsNewLine(text2))
					{
						text = text2;
					}
					leadingSpaces.Remove(orderedAttribute2);
				}
			}
			else if (text != null)
			{
				if (flag || !leadingSpaces.ContainsKey(orderedAttribute2) || !ContainsNewLine(leadingSpaces[orderedAttribute2]))
				{
					leadingSpaces[orderedAttribute2] = text;
				}
				text = null;
			}
			else if (!leadingSpaces.ContainsKey(orderedAttribute2))
			{
				if (flag)
				{
					leadingSpaces[orderedAttribute2] = " ";
				}
				else if (OneAttributePerLine)
				{
					leadingSpaces[orderedAttribute2] = GetAttributeNewLineString(formatter);
				}
				else
				{
					EnsureAttributeNewLineString(formatter);
				}
			}
			flag = flag && !flag2;
		}
	}

	private bool ComputeOneAttributePerLine()
	{
		if (leadingSpaces.Count > 1)
		{
			bool flag = true;
			foreach (string orderedAttribute in orderedAttributes)
			{
				if (flag)
				{
					flag = false;
				}
				else if (leadingSpaces.ContainsKey(orderedAttribute) && !ContainsNewLine(leadingSpaces[orderedAttribute]))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private bool ContainsNewLine(string space)
	{
		return space.IndexOf("\n", StringComparison.Ordinal) >= 0;
	}

	public string GetAttributeNewLineString(XmlFormatter formatter)
	{
		if (attributeNewLineString == null)
		{
			attributeNewLineString = ComputeAttributeNewLineString(formatter);
		}
		return attributeNewLineString;
	}

	private string ComputeAttributeNewLineString(XmlFormatter formatter)
	{
		string text = LookAheadForNewLineString();
		if (text != null)
		{
			return text;
		}
		return formatter?.CurrentAttributeIndent;
	}

	private string LookAheadForNewLineString()
	{
		foreach (string value in leadingSpaces.Values)
		{
			if (ContainsNewLine(value))
			{
				return value;
			}
		}
		return null;
	}

	private void EnsureAttributeNewLineString(XmlFormatter formatter)
	{
		GetAttributeNewLineString(formatter);
	}
}
