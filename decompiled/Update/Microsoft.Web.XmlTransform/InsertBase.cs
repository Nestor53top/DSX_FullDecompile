using System.Globalization;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal abstract class InsertBase : Transform
{
	private XmlElement siblingElement;

	protected XmlElement SiblingElement
	{
		get
		{
			if (siblingElement == null)
			{
				if (base.Arguments == null || base.Arguments.Count == 0)
				{
					throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_InsertMissingArgument, new object[1] { GetType().Name }));
				}
				if (base.Arguments.Count > 1)
				{
					throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_InsertTooManyArguments, new object[1] { GetType().Name }));
				}
				string text = base.Arguments[0];
				XmlNodeList val = base.TargetNode.SelectNodes(text);
				if (val.Count == 0)
				{
					throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_InsertBadXPath, new object[1] { text }));
				}
				XmlNode obj = val[0];
				siblingElement = (XmlElement)(object)((obj is XmlElement) ? obj : null);
				if (siblingElement == null)
				{
					throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_InsertBadXPathResult, new object[1] { text }));
				}
			}
			return siblingElement;
		}
	}

	internal InsertBase()
		: base(TransformFlags.UseParentAsTargetNode, MissingTargetMessage.Error)
	{
	}
}
