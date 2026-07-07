using System;
using System.Globalization;
using System.IO;
using System.Net;
using NuGet.Resources;

namespace NuGet;

internal class PackageServer
{
	private const string ServiceEndpoint = "/api/v2/package";

	private const string ApiKeyHeader = "X-NuGet-ApiKey";

	private const int MaxRediretionCount = 20;

	private Lazy<Uri> _baseUri;

	private readonly string _source;

	private readonly string _userAgent;

	public string Source => _source;

	public event EventHandler<WebRequestEventArgs> SendingRequest = delegate
	{
	};

	public PackageServer(string source, string userAgent)
	{
		if (string.IsNullOrEmpty(source))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "source");
		}
		_source = source;
		_userAgent = userAgent;
		_baseUri = new Lazy<Uri>(ResolveBaseUrl);
	}

	public void PushPackage(string apiKey, IPackage package, long packageSize, int timeout, bool disableBuffering)
	{
		Uri uri = new Uri(Source);
		if (uri.IsFile)
		{
			PushPackageToFileSystem(new PhysicalFileSystem(uri.LocalPath), package);
		}
		else
		{
			PushPackageToServer(apiKey, package.GetStream, packageSize, timeout, disableBuffering);
		}
	}

	private void PushPackageToServer(string apiKey, Func<Stream> packageStreamFactory, long packageSize, int timeout, bool disableBuffering)
	{
		int num = 0;
		do
		{
			HttpClient client = GetClient("", "PUT", "application/octet-stream");
			client.DisableBuffering = disableBuffering;
			client.SendingRequest += delegate(object sender, WebRequestEventArgs e)
			{
				this.SendingRequest(this, e);
				HttpWebRequest httpWebRequest = (HttpWebRequest)e.Request;
				if (timeout <= 0)
				{
					timeout = httpWebRequest.ReadWriteTimeout;
				}
				httpWebRequest.Timeout = timeout;
				httpWebRequest.ReadWriteTimeout = timeout;
				if (!string.IsNullOrEmpty(apiKey))
				{
					httpWebRequest.Headers.Add("X-NuGet-ApiKey", apiKey);
				}
				MultipartWebRequest multipartWebRequest = new MultipartWebRequest();
				multipartWebRequest.AddFile(packageStreamFactory, "package", packageSize);
				multipartWebRequest.CreateMultipartRequest(httpWebRequest);
			};
			if (EnsureSuccessfulResponse(client))
			{
				return;
			}
			num++;
		}
		while (num <= 20);
		throw new WebException(NuGetResources.Error_TooManyRedirections);
	}

	private static void PushPackageToFileSystem(IFileSystem fileSystem, IPackage package)
	{
		string packageFileName = new DefaultPackagePathResolver(fileSystem).GetPackageFileName(package);
		using Stream stream = package.GetStream();
		fileSystem.AddFile(packageFileName, stream);
	}

	public void DeletePackage(string apiKey, string packageId, string packageVersion)
	{
		Uri uri = new Uri(Source);
		if (uri.IsFile)
		{
			DeletePackageFromFileSystem(new PhysicalFileSystem(uri.LocalPath), packageId, packageVersion);
		}
		else
		{
			DeletePackageFromServer(apiKey, packageId, packageVersion);
		}
	}

	private void DeletePackageFromServer(string apiKey, string packageId, string packageVersion)
	{
		string path = string.Join("/", new string[2] { packageId, packageVersion });
		HttpClient client = GetClient(path, "DELETE", "text/html");
		client.SendingRequest += delegate(object sender, WebRequestEventArgs e)
		{
			this.SendingRequest(this, e);
			((HttpWebRequest)e.Request).Headers.Add("X-NuGet-ApiKey", apiKey);
		};
		EnsureSuccessfulResponse(client);
	}

	private static void DeletePackageFromFileSystem(IFileSystem fileSystem, string packageId, string packageVersion)
	{
		string packageFileName = new DefaultPackagePathResolver(fileSystem).GetPackageFileName(packageId, new SemanticVersion(packageVersion));
		fileSystem.DeleteFile(packageFileName);
	}

	private HttpClient GetClient(string path, string method, string contentType)
	{
		HttpClient httpClient = new HttpClient(GetServiceEndpointUrl(_baseUri.Value, path))
		{
			ContentType = contentType,
			Method = method
		};
		if (!string.IsNullOrEmpty(_userAgent))
		{
			httpClient.UserAgent = HttpUtility.CreateUserAgentString(_userAgent);
		}
		return httpClient;
	}

	internal static Uri GetServiceEndpointUrl(Uri baseUrl, string path)
	{
		if (string.IsNullOrEmpty(baseUrl.AbsolutePath.TrimStart(new char[1] { '/' })))
		{
			return new Uri(baseUrl, "/api/v2/package/" + path);
		}
		return new Uri(baseUrl, path);
	}

	private bool EnsureSuccessfulResponse(HttpClient client, HttpStatusCode? expectedStatusCode = null)
	{
		HttpWebResponse httpWebResponse = null;
		try
		{
			httpWebResponse = (HttpWebResponse)client.GetResponse();
			if (httpWebResponse != null && ((expectedStatusCode.HasValue && expectedStatusCode.Value != httpWebResponse.StatusCode) || (!expectedStatusCode.HasValue && httpWebResponse.StatusCode >= HttpStatusCode.BadRequest)))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.PackageServerError, new object[2]
				{
					httpWebResponse.StatusDescription,
					string.Empty
				}));
			}
			return true;
		}
		catch (WebException ex)
		{
			if (ex.Response == null)
			{
				throw;
			}
			httpWebResponse = (HttpWebResponse)ex.Response;
			if (httpWebResponse.StatusCode == HttpStatusCode.MultipleChoices || httpWebResponse.StatusCode == HttpStatusCode.MovedPermanently || httpWebResponse.StatusCode == HttpStatusCode.Found || httpWebResponse.StatusCode == HttpStatusCode.SeeOther || httpWebResponse.StatusCode == HttpStatusCode.TemporaryRedirect)
			{
				string relativeUri = httpWebResponse.Headers["Location"];
				if (!Uri.TryCreate(client.Uri, relativeUri, out Uri newUri))
				{
					throw;
				}
				_baseUri = new Lazy<Uri>(() => newUri);
				return false;
			}
			if (expectedStatusCode != httpWebResponse.StatusCode)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.PackageServerError, new object[2] { httpWebResponse.StatusDescription, ex.Message }), ex);
			}
			return true;
		}
		finally
		{
			if (httpWebResponse != null)
			{
				httpWebResponse.Close();
				httpWebResponse = null;
			}
		}
	}

	private Uri ResolveBaseUrl()
	{
		Uri uri;
		try
		{
			uri = new RedirectedHttpClient(new Uri(Source)).Uri;
		}
		catch (WebException ex)
		{
			HttpWebResponse obj = (HttpWebResponse)ex.Response;
			if (obj == null)
			{
				throw;
			}
			uri = obj.ResponseUri;
		}
		return EnsureTrailingSlash(uri);
	}

	private static Uri EnsureTrailingSlash(Uri uri)
	{
		string text = uri.OriginalString;
		if (!text.EndsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			text += "/";
		}
		return new Uri(text);
	}
}
