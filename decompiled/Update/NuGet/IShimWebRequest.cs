using System.Net;

namespace NuGet;

internal interface IShimWebRequest
{
	HttpWebRequest Request { get; }
}
