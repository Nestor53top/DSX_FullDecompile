namespace Microsoft.Web.XmlTransform;

internal sealed class Condition : Locator
{
	protected override string ConstructPredicate()
	{
		EnsureArguments(1, 1);
		return base.Arguments[0];
	}
}
