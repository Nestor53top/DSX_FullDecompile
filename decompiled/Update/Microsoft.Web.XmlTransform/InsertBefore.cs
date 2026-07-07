using System.Globalization;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class InsertBefore : InsertBase
{
	protected override void Apply()
	{
		((XmlNode)base.SiblingElement).ParentNode.InsertBefore(base.TransformNode, (XmlNode)(object)base.SiblingElement);
		base.Log.LogMessage(MessageType.Verbose, string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_TransformMessageInsert, new object[1] { base.TransformNode.Name }));
	}
}
