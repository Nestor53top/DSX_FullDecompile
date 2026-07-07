using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal abstract class AttributeTransform : Transform
{
	private XmlNode transformAttributeSource;

	private XmlNodeList transformAttributes;

	private XmlNode targetAttributeSource;

	private XmlNodeList targetAttributes;

	protected XmlNodeList TransformAttributes
	{
		get
		{
			if (transformAttributes == null || transformAttributeSource != base.TransformNode)
			{
				transformAttributeSource = base.TransformNode;
				transformAttributes = GetAttributesFrom(base.TransformNode);
			}
			return transformAttributes;
		}
	}

	protected XmlNodeList TargetAttributes
	{
		get
		{
			if (targetAttributes == null || targetAttributeSource != base.TargetNode)
			{
				targetAttributeSource = base.TargetNode;
				targetAttributes = GetAttributesFrom(base.TargetNode);
			}
			return targetAttributes;
		}
	}

	protected AttributeTransform()
		: base(TransformFlags.ApplyTransformToAllTargetNodes)
	{
	}

	private XmlNodeList GetAttributesFrom(XmlNode node)
	{
		if (base.Arguments == null || base.Arguments.Count == 0)
		{
			return GetAttributesFrom(node, "*", warnIfEmpty: false);
		}
		if (base.Arguments.Count == 1)
		{
			return GetAttributesFrom(node, base.Arguments[0], warnIfEmpty: true);
		}
		foreach (string argument in base.Arguments)
		{
			GetAttributesFrom(node, argument, warnIfEmpty: true);
		}
		return GetAttributesFrom(node, base.Arguments, warnIfEmpty: false);
	}

	private XmlNodeList GetAttributesFrom(XmlNode node, string argument, bool warnIfEmpty)
	{
		return GetAttributesFrom(node, new string[1] { argument }, warnIfEmpty);
	}

	private XmlNodeList GetAttributesFrom(XmlNode node, IList<string> arguments, bool warnIfEmpty)
	{
		string[] array = new string[arguments.Count];
		arguments.CopyTo(array, 0);
		string text = "@" + string.Join("|@", array);
		XmlNodeList val = node.SelectNodes(text);
		if (val.Count == 0 && warnIfEmpty && arguments.Count == 1)
		{
			base.Log.LogWarning(SR.XMLTRANSFORMATION_TransformArgumentFoundNoAttributes, new object[1] { arguments[0] });
		}
		return val;
	}
}
