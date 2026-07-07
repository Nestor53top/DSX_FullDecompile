using System;

namespace NuGet;

internal interface IProgressProvider
{
	event EventHandler<ProgressEventArgs> ProgressAvailable;
}
