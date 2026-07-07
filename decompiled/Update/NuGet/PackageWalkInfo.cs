namespace NuGet;

internal class PackageWalkInfo
{
	public PackageTargets InitialTarget { get; private set; }

	public PackageTargets Target { get; set; }

	public IPackage Parent { get; set; }

	public PackageWalkInfo(PackageTargets initialTarget)
	{
		InitialTarget = initialTarget;
		Target = initialTarget;
	}

	public override string ToString()
	{
		return "Initial Target:" + InitialTarget.ToString() + ", Current Target: " + Target.ToString() + ", Parent: " + Parent;
	}
}
