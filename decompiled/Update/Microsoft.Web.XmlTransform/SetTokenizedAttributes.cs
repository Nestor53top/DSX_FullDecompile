using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class SetTokenizedAttributes : AttributeTransform
{
	protected delegate string GetValueCallback(string key);

	private SetTokenizedAttributeStorage storageDictionary;

	private bool fInitStorageDictionary;

	public static readonly string Token = "Token";

	public static readonly string TokenNumber = "TokenNumber";

	public static readonly string XPathWithIndex = "XPathWithIndex";

	public static readonly string ParameterAttribute = "Parameter";

	public static readonly string XpathLocator = "XpathLocator";

	public static readonly string XPathWithLocator = "XPathWithLocator";

	private XmlAttribute tokenizeValueCurrentXmlAttribute;

	private static Regex s_dirRegex = null;

	private static Regex s_parentAttribRegex = null;

	private static Regex s_tokenFormatRegex = null;

	protected SetTokenizedAttributeStorage TransformStorage
	{
		get
		{
			if (storageDictionary == null && !fInitStorageDictionary)
			{
				storageDictionary = GetService<SetTokenizedAttributeStorage>();
				fInitStorageDictionary = true;
			}
			return storageDictionary;
		}
	}

	internal static Regex DirRegex
	{
		get
		{
			if (s_dirRegex == null)
			{
				s_dirRegex = new Regex("\\G\\{%(\\s*(?<attrname>\\w+(?=\\W))(\\s*(?<equal>=)\\s*'(?<attrval>[^']*)'|\\s*(?<equal>=)\\s*(?<attrval>[^\\s%>]*)|(?<equal>)(?<attrval>\\s*?)))*\\s*?%\\}");
			}
			return s_dirRegex;
		}
	}

	internal static Regex ParentAttributeRegex
	{
		get
		{
			if (s_parentAttribRegex == null)
			{
				s_parentAttribRegex = new Regex("\\G\\$\\((?<tagname>[\\w:\\.]+)\\)");
			}
			return s_parentAttribRegex;
		}
	}

	internal static Regex TokenFormatRegex
	{
		get
		{
			if (s_tokenFormatRegex == null)
			{
				s_tokenFormatRegex = new Regex("\\G\\#\\((?<tagname>[\\w:\\.]+)\\)");
			}
			return s_tokenFormatRegex;
		}
	}

	protected override void Apply()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		bool flag = false;
		SetTokenizedAttributeStorage transformStorage = TransformStorage;
		List<Dictionary<string, string>> parameters = null;
		if (transformStorage != null)
		{
			flag = transformStorage.EnableTokenizeParameters;
			if (flag)
			{
				parameters = transformStorage.DictionaryList;
			}
		}
		foreach (XmlAttribute transformAttribute in base.TransformAttributes)
		{
			XmlAttribute val = transformAttribute;
			XmlNode namedItem = ((XmlNamedNodeMap)base.TargetNode.Attributes).GetNamedItem(((XmlNode)val).Name);
			XmlAttribute val2 = (XmlAttribute)(object)((namedItem is XmlAttribute) ? namedItem : null);
			string value = TokenizeValue(val2, val, flag, parameters);
			if (val2 != null)
			{
				((XmlNode)val2).Value = value;
			}
			else
			{
				XmlAttribute val3 = (XmlAttribute)((XmlNode)val).Clone();
				((XmlNode)val3).Value = value;
				base.TargetNode.Attributes.Append(val3);
			}
			base.Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformMessageSetAttribute, ((XmlNode)val).Name);
		}
		if (base.TransformAttributes.Count > 0)
		{
			base.Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformMessageSetAttributes, base.TransformAttributes.Count);
		}
		else
		{
			base.Log.LogWarning(SR.XMLTRANSFORMATION_TransformMessageNoSetAttributes);
		}
	}

	protected string GetAttributeValue(string attributeName)
	{
		string result = null;
		XmlNode namedItem = ((XmlNamedNodeMap)base.TargetNode.Attributes).GetNamedItem(attributeName);
		XmlAttribute val = (XmlAttribute)(object)((namedItem is XmlAttribute) ? namedItem : null);
		if (val == null && string.Compare(attributeName, ((XmlNode)tokenizeValueCurrentXmlAttribute).Name, StringComparison.OrdinalIgnoreCase) != 0)
		{
			XmlNode namedItem2 = ((XmlNamedNodeMap)base.TransformNode.Attributes).GetNamedItem(attributeName);
			val = (XmlAttribute)(object)((namedItem2 is XmlAttribute) ? namedItem2 : null);
		}
		if (val != null)
		{
			result = ((XmlNode)val).Value;
		}
		return result;
	}

	protected string EscapeDirRegexSpecialCharacter(string value, bool escape)
	{
		if (escape)
		{
			return value.Replace("'", "&apos;");
		}
		return value.Replace("&apos;", "'");
	}

	protected static string SubstituteKownValue(string transformValue, Regex patternRegex, string patternPrefix, GetValueCallback getValueDelegate)
	{
		int num = 0;
		List<System.Text.RegularExpressions.Match> list = new List<System.Text.RegularExpressions.Match>();
		do
		{
			num = transformValue.IndexOf(patternPrefix, num, StringComparison.OrdinalIgnoreCase);
			if (num > -1)
			{
				System.Text.RegularExpressions.Match match = patternRegex.Match(transformValue, num);
				if (match.Success)
				{
					list.Add(match);
					num = match.Index + match.Length;
				}
				else
				{
					num++;
				}
			}
		}
		while (num > -1);
		StringBuilder stringBuilder = new StringBuilder(transformValue.Length);
		if (list.Count > 0)
		{
			stringBuilder.Remove(0, stringBuilder.Length);
			num = 0;
			int num2 = 0;
			foreach (System.Text.RegularExpressions.Match item in list)
			{
				stringBuilder.Append(transformValue.Substring(num, item.Index - num));
				Capture capture = item.Groups["tagname"];
				string value = capture.Value;
				string text = getValueDelegate(value);
				if (text != null)
				{
					stringBuilder.Append(text);
				}
				else
				{
					stringBuilder.Append(item.Value);
				}
				num = item.Index + item.Length;
				num2++;
			}
			stringBuilder.Append(transformValue.Substring(num));
			transformValue = stringBuilder.ToString();
		}
		return transformValue;
	}

	private string GetXPathToAttribute(XmlAttribute xmlAttribute)
	{
		return GetXPathToAttribute(xmlAttribute, null);
	}

	private string GetXPathToAttribute(XmlAttribute xmlAttribute, IList<string> locators)
	{
		string result = string.Empty;
		if (xmlAttribute != null)
		{
			string text = GetXPathToNode((XmlNode)(object)xmlAttribute.OwnerElement);
			if (!string.IsNullOrEmpty(text))
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				if (locators != null && locators.Count != 0)
				{
					foreach (string locator in locators)
					{
						string attributeValue = GetAttributeValue(locator);
						if (!string.IsNullOrEmpty(attributeValue))
						{
							if (stringBuilder.Length != 0)
							{
								stringBuilder.Append(" and ");
							}
							stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "@{0}='{1}'", new object[2] { locator, attributeValue }));
							continue;
						}
						throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_MatchAttributeDoesNotExist, new object[1] { locator }));
					}
				}
				if (stringBuilder.Length == 0)
				{
					for (int i = 0; i < base.TargetNodes.Count; i++)
					{
						if ((object)base.TargetNodes[i] == xmlAttribute.OwnerElement)
						{
							stringBuilder.Append((i + 1).ToString(CultureInfo.InvariantCulture));
							break;
						}
					}
				}
				text = text + "[" + stringBuilder.ToString() + "]";
			}
			result = text + "/@" + ((XmlNode)xmlAttribute).Name;
		}
		return result;
	}

	private string GetXPathToNode(XmlNode xmlNode)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if (xmlNode == null || (int)xmlNode.NodeType == 9)
		{
			return null;
		}
		string xPathToNode = GetXPathToNode(xmlNode.ParentNode);
		return xPathToNode + "/" + xmlNode.Name;
	}

	private string TokenizeValue(XmlAttribute targetAttribute, XmlAttribute transformAttribute, bool fTokenizeParameter, List<Dictionary<string, string>> parameters)
	{
		tokenizeValueCurrentXmlAttribute = transformAttribute;
		string value = ((XmlNode)transformAttribute).Value;
		string xPathToAttribute = GetXPathToAttribute(targetAttribute);
		value = SubstituteKownValue(value, ParentAttributeRegex, "$(", (string attributeName) => EscapeDirRegexSpecialCharacter(GetAttributeValue(attributeName), escape: true));
		if (fTokenizeParameter && parameters != null)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder(value.Length);
			num = 0;
			List<System.Text.RegularExpressions.Match> list = new List<System.Text.RegularExpressions.Match>();
			do
			{
				num = value.IndexOf("{%", num, StringComparison.OrdinalIgnoreCase);
				if (num > -1)
				{
					System.Text.RegularExpressions.Match match = DirRegex.Match(value, num);
					if (match.Success)
					{
						list.Add(match);
						num = match.Index + match.Length;
					}
					else
					{
						num++;
					}
				}
			}
			while (num > -1);
			if (list.Count > 0)
			{
				stringBuilder.Remove(0, stringBuilder.Length);
				num = 0;
				int num2 = 0;
				foreach (System.Text.RegularExpressions.Match item in list)
				{
					stringBuilder.Append(value.Substring(num, item.Index - num));
					CaptureCollection captures = item.Groups["attrname"].Captures;
					if (captures != null && captures.Count > 0)
					{
						CaptureCollection captures2 = item.Groups["attrval"].Captures;
						Dictionary<string, string> paramDictionary = new Dictionary<string, string>(4, StringComparer.OrdinalIgnoreCase);
						paramDictionary[XPathWithIndex] = xPathToAttribute;
						paramDictionary[TokenNumber] = num2.ToString(CultureInfo.InvariantCulture);
						for (int num3 = 0; num3 < captures.Count; num3++)
						{
							string value2 = captures[num3].Value;
							string value3 = null;
							if (captures2 != null && num3 < captures2.Count)
							{
								value3 = EscapeDirRegexSpecialCharacter(captures2[num3].Value, escape: false);
							}
							paramDictionary[value2] = value3;
						}
						string value4 = null;
						if (!paramDictionary.TryGetValue(Token, out value4))
						{
							value4 = storageDictionary.TokenFormat;
						}
						if (!string.IsNullOrEmpty(value4))
						{
							paramDictionary[Token] = value4;
						}
						int count = paramDictionary.Count;
						string[] array = new string[count];
						paramDictionary.Keys.CopyTo(array, 0);
						for (int num4 = 0; num4 < count; num4++)
						{
							string key = array[num4];
							string transformValue = paramDictionary[key];
							string value5 = SubstituteKownValue(transformValue, TokenFormatRegex, "#(", (string key2) => (!paramDictionary.ContainsKey(key2)) ? null : paramDictionary[key2]);
							paramDictionary[key] = value5;
						}
						if (paramDictionary.TryGetValue(Token, out value4))
						{
							stringBuilder.Append(value4);
						}
						if (paramDictionary.TryGetValue(XpathLocator, out var value6) && !string.IsNullOrEmpty(value6))
						{
							IList<string> locators = XmlArgumentUtility.SplitArguments(value6);
							string xPathToAttribute2 = GetXPathToAttribute(targetAttribute, locators);
							if (!string.IsNullOrEmpty(xPathToAttribute2))
							{
								paramDictionary[XPathWithLocator] = xPathToAttribute2;
							}
						}
						parameters.Add(paramDictionary);
					}
					num = item.Index + item.Length;
					num2++;
				}
				stringBuilder.Append(value.Substring(num));
				value = stringBuilder.ToString();
			}
		}
		return value;
	}
}
