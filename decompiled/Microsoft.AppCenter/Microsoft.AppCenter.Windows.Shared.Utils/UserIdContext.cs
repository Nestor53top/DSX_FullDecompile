using System;

namespace Microsoft.AppCenter.Windows.Shared.Utils;

public class UserIdContext
{
	private static readonly object UserIdLock = new object();

	private static UserIdContext _instanceField;

	private string _userId;

	private readonly object UserIdContextLock = new object();

	public static int UserIdMaxLength = 256;

	public static UserIdContext Instance
	{
		get
		{
			lock (UserIdLock)
			{
				return _instanceField ?? (_instanceField = new UserIdContext());
			}
		}
		set
		{
			lock (UserIdLock)
			{
				_instanceField = value;
			}
		}
	}

	public string UserId
	{
		get
		{
			lock (UserIdContextLock)
			{
				return _userId;
			}
		}
		set
		{
			EventHandler<UserIdUpdatedEventArgs> eventHandler = null;
			lock (UserIdContextLock)
			{
				if (_userId != value)
				{
					_userId = value;
					eventHandler = UserIdContext.UserIdUpdated;
				}
			}
			eventHandler?.Invoke(this, new UserIdUpdatedEventArgs
			{
				UserId = value
			});
		}
	}

	public static event EventHandler<UserIdUpdatedEventArgs> UserIdUpdated;

	internal UserIdContext()
	{
	}

	public static bool CheckUserIdValidForAppCenter(string userId)
	{
		if (userId != null && userId.Length > UserIdMaxLength)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "userId is limited to " + UserIdMaxLength + " characters.");
			return false;
		}
		return true;
	}
}
