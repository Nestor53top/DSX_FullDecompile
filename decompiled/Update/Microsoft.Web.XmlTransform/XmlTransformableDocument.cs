using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlTransformableDocument : XmlFileInfoDocument, IXmlOriginalDocumentService
{
	private XmlDocument xmlOriginal;

	public bool IsChanged
	{
		get
		{
			if (xmlOriginal == null)
			{
				return false;
			}
			return !IsXmlEqual(xmlOriginal, (XmlDocument)(object)this);
		}
	}

	internal void OnBeforeChange()
	{
		if (xmlOriginal == null)
		{
			CloneOriginalDocument();
		}
	}

	internal void OnAfterChange()
	{
	}

	private void CloneOriginalDocument()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		xmlOriginal = (XmlDocument)((XmlNode)this).Clone();
	}

	private bool IsXmlEqual(XmlDocument xmlOriginal, XmlDocument xmlTransformed)
	{
		return false;
	}

	XmlNodeList IXmlOriginalDocumentService.SelectNodes(string xpath, XmlNamespaceManager nsmgr)
	{
		if (xmlOriginal != null)
		{
			return ((XmlNode)xmlOriginal).SelectNodes(xpath, nsmgr);
		}
		return null;
	}
}
