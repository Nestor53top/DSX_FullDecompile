using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

[Serializable]
internal sealed class XmlNodeException : XmlTransformationException
{
	private XmlFileInfoDocument document;

	private IXmlLineInfo lineInfo;

	public bool HasErrorInfo => lineInfo != null;

	public string FileName
	{
		get
		{
			if (document == null)
			{
				return null;
			}
			return document.FileName;
		}
	}

	public int LineNumber
	{
		get
		{
			if (lineInfo == null)
			{
				return 0;
			}
			return lineInfo.LineNumber;
		}
	}

	public int LinePosition
	{
		get
		{
			if (lineInfo == null)
			{
				return 0;
			}
			return lineInfo.LinePosition;
		}
	}

	public static Exception Wrap(Exception ex, XmlNode node)
	{
		if (ex is XmlNodeException)
		{
			return ex;
		}
		return new XmlNodeException(ex, node);
	}

	public XmlNodeException(Exception innerException, XmlNode node)
		: base(innerException.Message, innerException)
	{
		lineInfo = (IXmlLineInfo)(object)((node is IXmlLineInfo) ? node : null);
		document = node.OwnerDocument as XmlFileInfoDocument;
	}

	public XmlNodeException(string message, XmlNode node)
		: base(message)
	{
		lineInfo = (IXmlLineInfo)(object)((node is IXmlLineInfo) ? node : null);
		document = node.OwnerDocument as XmlFileInfoDocument;
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("document", document);
		info.AddValue("lineInfo", lineInfo);
	}
}
