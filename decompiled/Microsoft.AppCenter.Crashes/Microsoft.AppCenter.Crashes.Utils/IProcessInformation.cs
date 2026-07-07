using System;

namespace Microsoft.AppCenter.Crashes.Utils;

public interface IProcessInformation
{
	DateTime? ProcessStartTime { get; }

	int? ProcessId { get; }

	string ProcessName { get; }

	int? ParentProcessId { get; }

	string ParentProcessName { get; }

	string ProcessArchitecture { get; }
}
