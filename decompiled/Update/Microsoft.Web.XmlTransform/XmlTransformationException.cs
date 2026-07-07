using System;

namespace Microsoft.Web.XmlTransform;

[Serializable]
internal class XmlTransformationException : Exception
{
	public XmlTransformationException(string message)
		: base(message)
	{
	}

	public XmlTransformationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
