using System;

namespace NuGet;

internal interface IPackageOperationEventListener
{
	void OnBeforeAddPackageReference(IProjectManager projectManager);

	void OnAfterAddPackageReference(IProjectManager projectManager);

	void OnAddPackageReferenceError(IProjectManager projectManager, Exception exception);
}
