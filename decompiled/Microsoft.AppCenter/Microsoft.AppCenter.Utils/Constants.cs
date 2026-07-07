using System;
using System.IO;

namespace Microsoft.AppCenter.Utils;

public static class Constants
{
	private static string AppCenterFilesDirectoryPathBacking;

	public static string AppCenterDatabasePath = Path.Combine(AppCenterFilesDirectoryPath, "Logs.db");

	public const string KeyPrefix = "AppCenter";

	public const int DefaultTriggerCount = 50;

	public static readonly TimeSpan DefaultTriggerInterval = TimeSpan.FromSeconds(3.0);

	public const int DefaultTriggerMaxParallelRequests = 3;

	public static string AppCenterFilesDirectoryPath
	{
		get
		{
			if (AppCenterFilesDirectoryPathBacking == null)
			{
				string path = AppCenter.GetInstallIdAsync().Result.ToString();
				AppCenterFilesDirectoryPathBacking = Path.Combine(LocalAppData, "Microsoft", "AppCenter", path);
			}
			return AppCenterFilesDirectoryPathBacking;
		}
	}

	public static string LocalAppData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

	public static string UserName { get; internal set; } = Environment.UserName;
}
