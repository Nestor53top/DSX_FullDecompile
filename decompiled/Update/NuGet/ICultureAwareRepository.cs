using System.Globalization;

namespace NuGet;

internal interface ICultureAwareRepository
{
	CultureInfo Culture { get; }
}
