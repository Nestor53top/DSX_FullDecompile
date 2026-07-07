using System.Collections.Generic;
using System.IO;
using System.Xml;
using EasyLocalization.Localization;

namespace EasyLocalization.Readers;

public class XmlFileReader : FileReader
{
	public XmlFileReader(string path)
		: base(path)
	{
	}

	internal override Dictionary<string, LocalizationEntry> GetEntries()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, LocalizationEntry> dictionary = new Dictionary<string, LocalizationEntry>();
		XmlDocument val = new XmlDocument();
		val.Load(Path);
		foreach (XmlNode item in ((XmlNode)val).SelectNodes("//Entry") ?? throw new FileFormatException("Invalid xml file."))
		{
			XmlNode val2 = item;
			XmlAttributeCollection attributes = val2.Attributes;
			string value = ((XmlNode)(((attributes != null) ? attributes["key"] : null) ?? throw new FileFormatException("All entries must have a 'key' attribute."))).Value;
			XmlNode val3 = val2.SelectSingleNode("Value");
			if (val3 == null)
			{
				dictionary.Add(value, new LocalizationEntry(val2.InnerText.Trim()));
				continue;
			}
			XmlNode val4 = val2.SelectSingleNode("ZeroValue");
			XmlNode val5 = val2.SelectSingleNode("PluralValue");
			dictionary.Add(value, new LocalizationEntry(val3.InnerText, (val4 != null) ? val4.InnerText : null, (val5 != null) ? val5.InnerText : null));
		}
		return dictionary;
	}
}
