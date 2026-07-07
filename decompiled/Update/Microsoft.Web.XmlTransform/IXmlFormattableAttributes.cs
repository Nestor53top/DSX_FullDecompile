namespace Microsoft.Web.XmlTransform;

internal interface IXmlFormattableAttributes
{
	string AttributeIndent { get; }

	void FormatAttributes(XmlFormatter formatter);
}
