using System;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using NuGet.Resources;

namespace NuGet;

internal static class STSAuthHelper
{
	private const string STSEndPointHeader = "X-NuGet-STS-EndPoint";

	private const string STSRealmHeader = "X-NuGet-STS-Realm";

	private const string STSTokenHeader = "X-NuGet-STS-Token";

	public static void PrepareSTSRequest(WebRequest request)
	{
		string cacheKey = GetCacheKey(request.RequestUri);
		if (MemoryCache.Instance.TryGetValue<string>(cacheKey, out var value))
		{
			request.Headers["X-NuGet-STS-Token"] = EncodeHeader(value);
		}
	}

	public static bool TryRetrieveSTSToken(Uri requestUri, IHttpWebResponse response)
	{
		if (response.StatusCode != HttpStatusCode.Unauthorized)
		{
			return false;
		}
		string endPoint = GetSTSEndPoint(response);
		string realm = response.Headers["X-NuGet-STS-Realm"];
		if (string.IsNullOrEmpty(endPoint) || string.IsNullOrEmpty(realm))
		{
			return false;
		}
		string cacheKey = GetCacheKey(requestUri);
		MemoryCache.Instance.GetOrAdd(cacheKey, () => GetSTSToken(requestUri, endPoint, realm), TimeSpan.FromMinutes(30.0), absoluteExpiration: true);
		return true;
	}

	private static string GetSTSToken(Uri requestUri, string endPoint, string appliesTo)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		WIFTypeProvider wIFTypes = WIFTypeProvider.GetWIFTypes();
		if (wIFTypes == null)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnableToLocateWIF, new object[1] { requestUri }));
		}
		WS2007HttpBinding val = new WS2007HttpBinding((SecurityMode)1);
		dynamic val2 = Activator.CreateInstance(wIFTypes.ChannelFactory, val, endPoint);
		val2.TrustVersion = TrustVersion.WSTrust13;
		dynamic val3 = Activator.CreateInstance(wIFTypes.RequestSecurityToken);
		val3.RequestType = GetFieldValue<string>(wIFTypes.RequestTypes, "Issue");
		val3.KeyType = GetFieldValue<string>(wIFTypes.KeyTypes, "Bearer");
		object obj = Activator.CreateInstance(wIFTypes.EndPoint, appliesTo);
		STSAuthHelper.SetProperty(val3, "AppliesTo", obj);
		dynamic val4 = val2.CreateChannel();
		dynamic val5 = val4.Issue(val3);
		return val5.TokenXml.OuterXml;
	}

	private static void SetProperty(object instance, string propertyName, object value)
	{
		instance.GetType().GetProperty(propertyName).GetSetMethod()
			.Invoke(instance, new object[1] { value });
	}

	private static TVal GetFieldValue<TVal>(Type type, string fieldName)
	{
		return (TVal)type.GetField(fieldName).GetValue(null);
	}

	private static string GetSTSEndPoint(IHttpWebResponse response)
	{
		return response.Headers["X-NuGet-STS-EndPoint"].SafeTrim();
	}

	private static string GetCacheKey(Uri requestUri)
	{
		return "X-NuGet-STS-Token|" + requestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
	}

	private static string EncodeHeader(string token)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
	}
}
