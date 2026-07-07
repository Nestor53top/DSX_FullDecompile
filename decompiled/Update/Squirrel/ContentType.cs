using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Squirrel;

internal static class ContentType
{
	public static void Clean(XmlDocument doc)
	{
		XmlNode nextSibling = ((XmlNode)doc).FirstChild.NextSibling;
		if (nextSibling.Name.ToLowerInvariant() != "types")
		{
			throw new Exception("Invalid ContentTypes file, expected root node should be 'Types'");
		}
		foreach (XmlElement item in ((IEnumerable)nextSibling.ChildNodes).OfType<XmlElement>())
		{
			if (item.GetAttribute("Extension") == "")
			{
				nextSibling.RemoveChild((XmlNode)(object)item);
			}
		}
	}

	public static void Merge(XmlDocument doc)
	{
		Tuple<string, string, string>[] source = new Tuple<string, string, string>[5]
		{
			Tuple.Create("Default", "diff", "application/octet"),
			Tuple.Create("Default", "bsdiff", "application/octet"),
			Tuple.Create("Default", "exe", "application/octet"),
			Tuple.Create("Default", "dll", "application/octet"),
			Tuple.Create("Default", "shasum", "text/plain")
		};
		XmlNode typesElement = ((XmlNode)doc).FirstChild.NextSibling;
		if (typesElement.Name.ToLowerInvariant() != "types")
		{
			throw new Exception("Invalid ContentTypes file, expected root node should be 'Types'");
		}
		IEnumerable<Tuple<string, string, string>> existingTypes = from k in ((IEnumerable)typesElement.ChildNodes).OfType<XmlElement>()
			select Tuple.Create(((XmlNode)k).Name, k.GetAttribute("Extension").ToLowerInvariant(), k.GetAttribute("ContentType").ToLowerInvariant());
		foreach (XmlElement item in source.Where((Tuple<string, string, string> x) => existingTypes.All((Tuple<string, string, string> t) => t.Item2 != x.Item2.ToLowerInvariant())).Select(delegate(Tuple<string, string, string> element)
		{
			XmlElement obj = doc.CreateElement(element.Item1, typesElement.NamespaceURI);
			XmlAttribute val = doc.CreateAttribute("Extension");
			((XmlNode)val).Value = element.Item2;
			XmlAttribute val2 = doc.CreateAttribute("ContentType");
			((XmlNode)val2).Value = element.Item3;
			((XmlNode)obj).Attributes.Append(val);
			((XmlNode)obj).Attributes.Append(val2);
			return obj;
		}))
		{
			typesElement.AppendChild((XmlNode)(object)item);
		}
	}
}
