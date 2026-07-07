using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlNodeContext
{
	private XmlNode node;

	public XmlNode Node => node;

	public bool HasLineInfo => node is IXmlLineInfo;

	public int LineNumber
	{
		get
		{
			XmlNode obj = node;
			IXmlLineInfo val = (IXmlLineInfo)(object)((obj is IXmlLineInfo) ? obj : null);
			if (val != null)
			{
				return val.LineNumber;
			}
			return 0;
		}
	}

	public int LinePosition
	{
		get
		{
			XmlNode obj = node;
			IXmlLineInfo val = (IXmlLineInfo)(object)((obj is IXmlLineInfo) ? obj : null);
			if (val != null)
			{
				return val.LinePosition;
			}
			return 0;
		}
	}

	public XmlNodeContext(XmlNode node)
	{
		this.node = node;
	}
}
