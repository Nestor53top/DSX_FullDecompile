using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class RemoveAttributes : AttributeTransform
{
	protected override void Apply()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		foreach (XmlAttribute targetAttribute in base.TargetAttributes)
		{
			XmlAttribute val = targetAttribute;
			base.TargetNode.Attributes.Remove(val);
			base.Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformMessageRemoveAttribute, ((XmlNode)val).Name);
		}
		if (base.TargetAttributes.Count > 0)
		{
			base.Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformMessageRemoveAttributes, base.TargetAttributes.Count);
		}
		else
		{
			base.Log.LogWarning(base.TargetNode, SR.XMLTRANSFORMATION_TransformMessageNoRemoveAttributes);
		}
	}
}
