using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class Remove : Transform
{
	protected override void Apply()
	{
		CommonErrors.WarnIfMultipleTargets(base.Log, base.TransformNameShort, base.TargetNodes, base.ApplyTransformToAllTargetNodes);
		RemoveNode();
	}

	protected void RemoveNode()
	{
		CommonErrors.ExpectNoArguments(base.Log, base.TransformNameShort, base.ArgumentString);
		XmlNode parentNode = base.TargetNode.ParentNode;
		parentNode.RemoveChild(base.TargetNode);
		base.Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformMessageRemove, base.TargetNode.Name);
	}
}
