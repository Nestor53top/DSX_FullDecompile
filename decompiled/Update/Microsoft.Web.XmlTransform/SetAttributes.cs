using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class SetAttributes : AttributeTransform
{
	protected override void Apply()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		foreach (XmlAttribute transformAttribute in base.TransformAttributes)
		{
			XmlAttribute val = transformAttribute;
			XmlNode namedItem = ((XmlNamedNodeMap)base.TargetNode.Attributes).GetNamedItem(((XmlNode)val).Name);
			XmlAttribute val2 = (XmlAttribute)(object)((namedItem is XmlAttribute) ? namedItem : null);
			if (val2 != null)
			{
				((XmlNode)val2).Value = ((XmlNode)val).Value;
			}
			else
			{
				base.TargetNode.Attributes.Append((XmlAttribute)((XmlNode)val).Clone());
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
}
