using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Windows.Utils;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using Microsoft.AppCenter.Windows.Shared.Utils;

namespace Microsoft.AppCenter.Crashes.Utils;

public class ErrorLogHelper
{
	public const string ErrorLogFileExtension = ".json";

	public const string ExceptionFileExtension = ".exception";

	public const string ErrorStorageDirectoryName = "Errors";

	internal IDeviceInformationHelper _deviceInformationHelper;

	internal IProcessInformation _processInformation;

	internal Microsoft.AppCenter.Utils.Files.Directory _crashesDirectory;

	private static readonly object LockObject = new object();

	private static ErrorLogHelper _instanceField;

	internal static ErrorLogHelper Instance
	{
		get
		{
			lock (LockObject)
			{
				return _instanceField ?? (_instanceField = new ErrorLogHelper());
			}
		}
		set
		{
			lock (LockObject)
			{
				_instanceField = value;
			}
		}
	}

	internal static ErrorExceptionAndBinaries CreateModelExceptionAndBinaries(System.Exception exception)
	{
		Microsoft.AppCenter.Crashes.Ingestion.Models.Exception ex = new Microsoft.AppCenter.Crashes.Ingestion.Models.Exception
		{
			Type = exception.GetType().ToString(),
			Message = exception.Message,
			StackTrace = exception.StackTrace
		};
		if (exception is AggregateException ex2 && ex2.InnerExceptions.Count != 0)
		{
			ex.InnerExceptions = new List<Microsoft.AppCenter.Crashes.Ingestion.Models.Exception>();
			foreach (System.Exception innerException in ex2.InnerExceptions)
			{
				ex.InnerExceptions.Add(CreateModelExceptionAndBinaries(innerException).Exception);
			}
		}
		if (exception.InnerException != null)
		{
			ex.InnerExceptions = ex.InnerExceptions ?? new List<Microsoft.AppCenter.Crashes.Ingestion.Models.Exception>();
			ex.InnerExceptions.Add(CreateModelExceptionAndBinaries(exception.InnerException).Exception);
		}
		return new ErrorExceptionAndBinaries
		{
			Exception = ex,
			Binaries = null
		};
	}

	public ErrorLogHelper()
	{
		_deviceInformationHelper = new DeviceInformationHelper();
		_processInformation = new ProcessInformation();
		string directoryPath = Path.Combine(Constants.AppCenterFilesDirectoryPath, "Errors");
		_crashesDirectory = new Microsoft.AppCenter.Utils.Files.Directory(directoryPath);
	}

	public static ManagedErrorLog CreateErrorLog(System.Exception exception)
	{
		return Instance.InstanceCreateErrorLog(exception);
	}

	public static IEnumerable<Microsoft.AppCenter.Utils.Files.File> GetErrorLogFiles()
	{
		return Instance.InstanceGetErrorLogFiles();
	}

	public static Microsoft.AppCenter.Utils.Files.Directory GetErrorStorageDirectory()
	{
		return Instance.InstanceGetErrorStorageDirectory();
	}

	public static Microsoft.AppCenter.Utils.Files.File GetStoredErrorLogFile(Guid errorId)
	{
		return Instance.InstanceGetStoredErrorLogFile(errorId);
	}

	public static Microsoft.AppCenter.Utils.Files.File GetStoredExceptionFile(Guid errorId)
	{
		return Instance.InstanceGetStoredExceptionFile(errorId);
	}

	public static ManagedErrorLog ReadErrorLogFile(Microsoft.AppCenter.Utils.Files.File file)
	{
		return Instance.InstanceReadErrorLogFile(file);
	}

	public static string ReadExceptionFile(Microsoft.AppCenter.Utils.Files.File file)
	{
		return Instance.InstanceReadExceptionFile(file);
	}

	public static void SaveErrorLogFiles(System.Exception exception, ManagedErrorLog errorLog)
	{
		Instance.InstanceSaveErrorLogFiles(exception, errorLog);
	}

	public static void RemoveStoredErrorLogFile(Guid errorId)
	{
		Instance.InstanceRemoveStoredErrorLogFile(errorId);
	}

	public static void RemoveStoredExceptionFile(Guid errorId)
	{
		Instance.InstanceRemoveStoredExceptionFile(errorId);
	}

	public static void RemoveAllStoredErrorLogFiles()
	{
		Instance.InstanceRemoveAllStoredErrorLogFiles();
	}

	private ManagedErrorLog InstanceCreateErrorLog(System.Exception exception)
	{
		ErrorExceptionAndBinaries errorExceptionAndBinaries = CreateModelExceptionAndBinaries(exception);
		return new ManagedErrorLog
		{
			Id = Guid.NewGuid(),
			Timestamp = DateTime.UtcNow,
			Device = _deviceInformationHelper.GetDeviceInformation(),
			ProcessId = _processInformation.ProcessId.GetValueOrDefault(),
			ProcessName = _processInformation.ProcessName,
			ParentProcessId = _processInformation.ParentProcessId,
			ParentProcessName = _processInformation.ParentProcessName,
			AppLaunchTimestamp = _processInformation.ProcessStartTime?.ToUniversalTime(),
			Architecture = _processInformation.ProcessArchitecture,
			Fatal = true,
			Exception = errorExceptionAndBinaries.Exception,
			Sid = SessionContext.SessionId,
			UserId = UserIdContext.Instance.UserId,
			Binaries = errorExceptionAndBinaries.Binaries
		};
	}

	public virtual IEnumerable<Microsoft.AppCenter.Utils.Files.File> InstanceGetErrorLogFiles()
	{
		lock (LockObject)
		{
			try
			{
				return InstanceGetErrorStorageDirectory().EnumerateFiles("*.json").ToList();
			}
			catch (System.Exception exception)
			{
				AppCenterLog.Error("AppCenterCrashes", "Failed to retrieve error log files.", exception);
			}
			return new List<Microsoft.AppCenter.Utils.Files.File>();
		}
	}

	private Microsoft.AppCenter.Utils.Files.File InstanceGetStoredErrorLogFile(Guid errorId)
	{
		return GetStoredFile(errorId, ".json");
	}

	public virtual Microsoft.AppCenter.Utils.Files.File InstanceGetStoredExceptionFile(Guid errorId)
	{
		return GetStoredFile(errorId, ".exception");
	}

	public virtual ManagedErrorLog InstanceReadErrorLogFile(Microsoft.AppCenter.Utils.Files.File file)
	{
		try
		{
			return (ManagedErrorLog)LogSerializer.DeserializeLog(file.ReadAllText());
		}
		catch (System.Exception exception)
		{
			AppCenterLog.Error("AppCenterCrashes", "Encountered an unexpected error while reading an error log file: " + file.Name, exception);
		}
		return null;
	}

	public virtual string InstanceReadExceptionFile(Microsoft.AppCenter.Utils.Files.File file)
	{
		try
		{
			return file.ReadAllText();
		}
		catch (System.Exception exception)
		{
			AppCenterLog.Error("AppCenterCrashes", "Encountered an unexpected error while reading stack trace file: " + file.Name, exception);
		}
		return null;
	}

	public virtual Microsoft.AppCenter.Utils.Files.Directory InstanceGetErrorStorageDirectory()
	{
		_crashesDirectory.Create();
		return _crashesDirectory;
	}

	public virtual void InstanceSaveErrorLogFiles(System.Exception exception, ManagedErrorLog errorLog)
	{
		try
		{
			string contents = ObfuscateUserName(LogSerializer.Serialize(errorLog));
			string text = errorLog.Id.ToString() + ".json";
			AppCenterLog.Debug("AppCenterCrashes", "Saving uncaught exception.");
			Microsoft.AppCenter.Utils.Files.Directory directory = InstanceGetErrorStorageDirectory();
			directory.CreateFile(text, contents);
			AppCenterLog.Debug("AppCenterCrashes", "Saved error log in directory Errors with name " + text + ".");
			try
			{
				string text2 = errorLog.Id.ToString() + ".exception";
				directory.CreateFile(text2, ObfuscateUserName(exception.ToString()));
				AppCenterLog.Debug("AppCenterCrashes", "Saved exception in directory Errors with name " + text2 + ".");
			}
			catch (System.Exception exception2)
			{
				AppCenterLog.Warn("AppCenterCrashes", "Failed to serialize exception for client side inspection.", exception2);
			}
		}
		catch (System.Exception exception3)
		{
			AppCenterLog.Error("AppCenterCrashes", "Failed to save error log.", exception3);
		}
	}

	public virtual void InstanceRemoveStoredErrorLogFile(Guid errorId)
	{
		lock (LockObject)
		{
			Microsoft.AppCenter.Utils.Files.File storedErrorLogFile = GetStoredErrorLogFile(errorId);
			if (storedErrorLogFile != null)
			{
				AppCenterLog.Info("AppCenterCrashes", "Deleting error log file " + storedErrorLogFile.Name + ".");
				try
				{
					storedErrorLogFile.Delete();
					return;
				}
				catch (System.Exception exception)
				{
					AppCenterLog.Warn("AppCenterCrashes", "Failed to delete error log file " + storedErrorLogFile.Name + ".", exception);
					return;
				}
			}
		}
	}

	public virtual void InstanceRemoveStoredExceptionFile(Guid errorId)
	{
		lock (LockObject)
		{
			Microsoft.AppCenter.Utils.Files.File storedExceptionFile = GetStoredExceptionFile(errorId);
			if (storedExceptionFile != null)
			{
				AppCenterLog.Info("AppCenterCrashes", "Deleting exception file " + storedExceptionFile.Name + ".");
				try
				{
					storedExceptionFile.Delete();
					return;
				}
				catch (System.Exception exception)
				{
					AppCenterLog.Warn("AppCenterCrashes", "Failed to delete exception file " + storedExceptionFile.Name + ".", exception);
					return;
				}
			}
		}
	}

	public virtual void InstanceRemoveAllStoredErrorLogFiles()
	{
		lock (LockObject)
		{
			_crashesDirectory.Refresh();
			if (_crashesDirectory.Exists())
			{
				AppCenterLog.Debug("AppCenterCrashes", "Deleting error log directory.");
				try
				{
					_crashesDirectory.Delete(recursive: true);
				}
				catch (System.Exception exception)
				{
					AppCenterLog.Warn("AppCenterCrashes", "Failed to delete error log directory.", exception);
				}
				AppCenterLog.Debug("AppCenterCrashes", "Deleted crashes local files.");
			}
		}
	}

	internal static string ObfuscateUserName(string errorString)
	{
		if (string.IsNullOrEmpty(errorString) || string.IsNullOrEmpty(Constants.UserName))
		{
			return errorString;
		}
		return errorString.Replace("\\" + Constants.UserName + "\\", "\\USER\\");
	}

	private Microsoft.AppCenter.Utils.Files.File GetStoredFile(Guid errorId, string extension)
	{
		string text = $"{errorId}{extension}";
		try
		{
			lock (LockObject)
			{
				return InstanceGetErrorStorageDirectory().EnumerateFiles(text).SingleOrDefault();
			}
		}
		catch (System.Exception exception)
		{
			AppCenterLog.Error("AppCenterCrashes", "Failed to retrieve error log file " + text + ".", exception);
		}
		return null;
	}
}
