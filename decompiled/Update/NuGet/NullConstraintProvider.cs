namespace NuGet;

internal class NullConstraintProvider : IPackageConstraintProvider
{
	private static readonly NullConstraintProvider _instance = new NullConstraintProvider();

	public static NullConstraintProvider Instance => _instance;

	public string Source => string.Empty;

	private NullConstraintProvider()
	{
	}

	public IVersionSpec GetConstraint(string packageId)
	{
		return null;
	}
}
