using System.Net;

namespace NuGet;

public interface IShimWebRequest
{
	HttpWebRequest Request { get; }
}
