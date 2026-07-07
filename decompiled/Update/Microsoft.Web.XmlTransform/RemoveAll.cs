namespace Microsoft.Web.XmlTransform;

internal class RemoveAll : Remove
{
	public RemoveAll()
	{
		base.ApplyTransformToAllTargetNodes = true;
	}

	protected override void Apply()
	{
		RemoveNode();
	}
}
