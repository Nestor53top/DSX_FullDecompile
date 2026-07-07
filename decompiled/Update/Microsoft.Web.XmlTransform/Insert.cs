namespace Microsoft.Web.XmlTransform;

internal class Insert : Transform
{
	public Insert()
		: base(TransformFlags.UseParentAsTargetNode, MissingTargetMessage.Error)
	{
	}

	protected override void Apply()
	{
		CommonErrors.ExpectNoArguments(base.Log, base.TransformNameShort, base.ArgumentString);
		base.TargetNode.AppendChild(base.TransformNode);
		base.Log.LogMessage(MessageType.Verbose, SR.XMLTRANSFORMATION_TransformMessageInsert, base.TransformNode.Name);
	}
}
