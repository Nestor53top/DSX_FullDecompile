using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AppCenter.Utils;

public static class WpfHelper
{
	public static bool IsRunningOnWpf { get; }

	public static Assembly PresentationFramework { get; }

	static WpfHelper()
	{
		try
		{
			PresentationFramework = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly assembly) => assembly.GetName().Name == "PresentationFramework");
			IsRunningOnWpf = PresentationFramework != null;
		}
		catch (AppDomainUnloadedException)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Unabled to determine whether this application is WPF or Windows Forms; proceeding as though it is Windows Forms.");
		}
	}
}
