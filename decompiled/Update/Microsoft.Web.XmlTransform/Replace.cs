using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class Replace : Transform
{
	protected override void Apply()
	{
		CommonErrors.ExpectNoArguments(base.Log, base.TransformNameShort, base.ArgumentString);
		CommonErrors.WarnIfMultipleTargets(base.Log, base.TransformNameShort, base.TargetNodes, base.ApplyTransformToAllTargetNodes);
		XmlNode parentNode = base.TargetNode.ParentNode;
		parentNode.ReplaceChild(base.TransformNode, base.TargetNode);
		base.Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformMessageReplace, base.TargetNode.Name);
	}
}
