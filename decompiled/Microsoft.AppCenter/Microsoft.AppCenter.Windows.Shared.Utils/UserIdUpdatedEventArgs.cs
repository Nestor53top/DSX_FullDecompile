using System;

namespace Microsoft.AppCenter.Windows.Shared.Utils;

public class UserIdUpdatedEventArgs : EventArgs
{
	public string UserId { get; internal set; }
}
