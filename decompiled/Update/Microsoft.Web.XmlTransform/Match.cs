using System.Globalization;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal sealed class Match : Locator
{
	protected override string ConstructPredicate()
	{
		EnsureArguments(1);
		string text = null;
		foreach (string argument in base.Arguments)
		{
			XmlNode namedItem = ((XmlNamedNodeMap)base.CurrentElement.Attributes).GetNamedItem(argument);
			XmlAttribute val = (XmlAttribute)(object)((namedItem is XmlAttribute) ? namedItem : null);
			if (val != null)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "@{0}='{1}'", new object[2]
				{
					((XmlNode)val).Name,
					((XmlNode)val).Value
				});
				text = ((text != null) ? (text + " and " + text2) : text2);
				continue;
			}
			throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_MatchAttributeDoesNotExist, new object[1] { argument }));
		}
		return text;
	}
}
