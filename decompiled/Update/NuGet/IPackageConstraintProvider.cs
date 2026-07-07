namespace NuGet;

internal interface IPackageConstraintProvider
{
	string Source { get; }

	IVersionSpec GetConstraint(string packageId);
}
