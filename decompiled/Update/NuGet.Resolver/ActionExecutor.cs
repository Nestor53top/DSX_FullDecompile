using System;
using System.Collections.Generic;

namespace NuGet.Resolver;

internal class ActionExecutor
{
	public ILogger Logger { get; set; }

	public IPackageOperationEventListener PackageOperationEventListener { get; set; }

	public bool CatchProjectOperationException { get; set; }

	public ActionExecutor()
	{
		Logger = NullLogger.Instance;
	}

	public void Execute(IEnumerable<PackageAction> actions)
	{
		List<PackageAction> list = new List<PackageAction>();
		try
		{
			foreach (PackageAction action in actions)
			{
				list.Add(action);
				if (action is PackageProjectAction packageProjectAction)
				{
					ExecuteProjectOperation(packageProjectAction);
					if (packageProjectAction.ActionType == PackageActionType.Install && packageProjectAction.ProjectManager.PackageManager != null && packageProjectAction.ProjectManager.PackageManager.BindingRedirectEnabled && packageProjectAction.ProjectManager.Project.IsBindingRedirectSupported)
					{
						packageProjectAction.ProjectManager.PackageManager.AddBindingRedirects(packageProjectAction.ProjectManager);
					}
					continue;
				}
				PackageSolutionAction packageSolutionAction = (PackageSolutionAction)action;
				packageSolutionAction.PackageManager.Logger = Logger;
				if (packageSolutionAction.ActionType == PackageActionType.AddToPackagesFolder)
				{
					packageSolutionAction.PackageManager.Execute(new PackageOperation(action.Package, NuGet.PackageAction.Install));
				}
				else if (packageSolutionAction.ActionType == PackageActionType.DeleteFromPackagesFolder)
				{
					packageSolutionAction.PackageManager.Execute(new PackageOperation(action.Package, NuGet.PackageAction.Uninstall));
				}
			}
		}
		catch
		{
			Rollback(list);
			throw;
		}
	}

	private void ExecuteProjectOperation(PackageProjectAction action)
	{
		try
		{
			if (PackageOperationEventListener != null)
			{
				PackageOperationEventListener.OnBeforeAddPackageReference(action.ProjectManager);
			}
			action.ProjectManager.Execute(new PackageOperation(action.Package, (action.ActionType != PackageActionType.Install) ? NuGet.PackageAction.Uninstall : NuGet.PackageAction.Install));
		}
		catch (Exception exception)
		{
			if (CatchProjectOperationException)
			{
				Logger.Log(MessageLevel.Error, ExceptionUtility.Unwrap(exception).Message);
				if (PackageOperationEventListener != null)
				{
					PackageOperationEventListener.OnAddPackageReferenceError(action.ProjectManager, exception);
				}
				return;
			}
			throw;
		}
		finally
		{
			if (PackageOperationEventListener != null)
			{
				PackageOperationEventListener.OnAfterAddPackageReference(action.ProjectManager);
			}
		}
	}

	private static PackageActionType GetReverseActionType(PackageActionType actionType)
	{
		return actionType switch
		{
			PackageActionType.AddToPackagesFolder => PackageActionType.DeleteFromPackagesFolder, 
			PackageActionType.DeleteFromPackagesFolder => PackageActionType.AddToPackagesFolder, 
			PackageActionType.Install => PackageActionType.Uninstall, 
			PackageActionType.Uninstall => PackageActionType.Install, 
			_ => throw new InvalidOperationException(), 
		};
	}

	private static PackageAction CreateReverseAction(PackageAction action)
	{
		if (action is PackageProjectAction packageProjectAction)
		{
			return new PackageProjectAction(GetReverseActionType(packageProjectAction.ActionType), packageProjectAction.Package, packageProjectAction.ProjectManager);
		}
		PackageSolutionAction packageSolutionAction = (PackageSolutionAction)action;
		return new PackageSolutionAction(GetReverseActionType(packageSolutionAction.ActionType), packageSolutionAction.Package, packageSolutionAction.PackageManager);
	}

	private void Rollback(List<PackageAction> executedOperations)
	{
		if (executedOperations.Count > 0)
		{
			Logger.Log(MessageLevel.Warning, "Rolling back");
		}
		executedOperations.Reverse();
		foreach (PackageAction executedOperation in executedOperations)
		{
			PackageAction packageAction = CreateReverseAction(executedOperation);
			if (packageAction is PackageProjectAction packageProjectAction)
			{
				packageProjectAction.ProjectManager.Logger = NullLogger.Instance;
				packageProjectAction.ProjectManager.Execute(new PackageOperation(packageProjectAction.Package, (packageProjectAction.ActionType != PackageActionType.Install) ? NuGet.PackageAction.Uninstall : NuGet.PackageAction.Install));
				continue;
			}
			PackageSolutionAction packageSolutionAction = (PackageSolutionAction)packageAction;
			packageSolutionAction.PackageManager.Logger = NullLogger.Instance;
			if (packageSolutionAction.ActionType == PackageActionType.AddToPackagesFolder)
			{
				packageSolutionAction.PackageManager.Execute(new PackageOperation(packageSolutionAction.Package, NuGet.PackageAction.Install));
			}
			else if (packageSolutionAction.ActionType == PackageActionType.DeleteFromPackagesFolder)
			{
				packageSolutionAction.PackageManager.Execute(new PackageOperation(packageSolutionAction.Package, NuGet.PackageAction.Uninstall));
			}
		}
	}
}
