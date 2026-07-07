using System;

namespace Microsoft.AppCenter.Utils;

public interface IApplicationLifecycleHelper
{
	bool IsSuspended { get; }

	event EventHandler ApplicationSuspended;

	event EventHandler ApplicationResuming;

	event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
}
