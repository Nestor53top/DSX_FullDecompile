using System.Runtime.Versioning;

namespace NuGet;

internal static class ProjectManagerExtensions
{
	public static FrameworkName GetTargetFrameworkForPackage(this IProjectManager projectManager, string packageId)
	{
		if (projectManager == null)
		{
			return null;
		}
		FrameworkName frameworkName = null;
		if (projectManager.LocalRepository is IPackageReferenceRepository packageReferenceRepository)
		{
			frameworkName = packageReferenceRepository.GetPackageTargetFramework(packageId);
		}
		if (frameworkName == null && projectManager.Project != null)
		{
			frameworkName = projectManager.Project.TargetFramework;
		}
		return frameworkName;
	}
}
