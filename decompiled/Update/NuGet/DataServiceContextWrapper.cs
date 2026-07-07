using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace NuGet;

[CLSCompliant(false)]
internal class DataServiceContextWrapper : IDataServiceContext, IWeakEventListener
{
	internal sealed class DataServiceMetadata
	{
		public HashSet<string> SupportedMethodNames { get; set; }

		public HashSet<string> SupportedProperties { get; set; }
	}

	private const string MetadataKey = "DataServiceMetadata|";

	private static readonly MethodInfo _executeMethodInfo = typeof(DataServiceContext).GetMethod("Execute", new Type[1] { typeof(Uri) });

	private readonly DataServiceContext _context;

	private readonly Uri _metadataUri;

	public Uri BaseUri => _context.BaseUri;

	public bool IgnoreMissingProperties
	{
		get
		{
			return _context.IgnoreMissingProperties;
		}
		set
		{
			_context.IgnoreMissingProperties = value;
		}
	}

	private DataServiceMetadata ServiceMetadata => MemoryCache.Instance.GetOrAdd(GetServiceMetadataKey(), () => GetDataServiceMetadata(_metadataUri), TimeSpan.FromMinutes(15.0));

	public event EventHandler<SendingRequest2EventArgs> SendingRequest
	{
		add
		{
			_context.SendingRequest2 += value;
		}
		remove
		{
			_context.SendingRequest2 -= value;
		}
	}

	public event EventHandler<ReadingWritingEntityEventArgs> ReadingEntity
	{
		add
		{
			_context.ReadingEntity += value;
		}
		remove
		{
			_context.ReadingEntity -= value;
		}
	}

	public DataServiceContextWrapper(Uri serviceRoot)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (serviceRoot == null)
		{
			throw new ArgumentNullException("serviceRoot");
		}
		_context = new DataServiceContext(serviceRoot)
		{
			MergeOption = (MergeOption)3
		};
		_metadataUri = _context.GetMetadataUri();
		AttachEvents();
	}

	private DataServiceClientRequestMessage ShimWebRequests(DataServiceClientRequestMessageArgs args)
	{
		return HttpShim.Instance.ShimDataServiceRequest(args);
	}

	public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
	{
		if (managerType == typeof(Func<DataServiceClientRequestMessage, DataServiceClientRequestMessageArgs>))
		{
			return true;
		}
		return false;
	}

	private void AttachEvents()
	{
		DataServiceClientRequestPipelineConfiguration requestPipeline = _context.Configurations.RequestPipeline;
		requestPipeline.OnMessageCreating = (Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage>)Delegate.Combine(requestPipeline.OnMessageCreating, new Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage>(ShimWebRequests));
	}

	public IDataServiceQuery<T> CreateQuery<T>(string entitySetName, IDictionary<string, object> queryOptions)
	{
		DataServiceQuery<T> val = _context.CreateQuery<T>(entitySetName);
		foreach (KeyValuePair<string, object> queryOption in queryOptions)
		{
			val = val.AddQueryOption(queryOption.Key, queryOption.Value);
		}
		return new DataServiceQueryWrapper<T>(this, (DataServiceQuery)(object)val);
	}

	public IDataServiceQuery<T> CreateQuery<T>(string entitySetName)
	{
		return new DataServiceQueryWrapper<T>(this, (DataServiceQuery)(object)_context.CreateQuery<T>(entitySetName));
	}

	public IEnumerable<T> Execute<T>(Type elementType, DataServiceQueryContinuation continuation)
	{
		return (IEnumerable<T>)_executeMethodInfo.MakeGenericMethod(elementType).Invoke(_context, new object[1] { continuation.NextLinkUri });
	}

	public IEnumerable<T> ExecuteBatch<T>(DataServiceRequest request)
	{
		return ((IEnumerable)_context.ExecuteBatch((DataServiceRequest[])(object)new DataServiceRequest[1] { request })).Cast<QueryOperationResponse>().SelectMany((QueryOperationResponse o) => ((IEnumerable)o).Cast<T>());
	}

	public bool SupportsServiceMethod(string methodName)
	{
		if (ServiceMetadata != null)
		{
			return ServiceMetadata.SupportedMethodNames.Contains(methodName);
		}
		return false;
	}

	public bool SupportsProperty(string propertyName)
	{
		if (ServiceMetadata != null)
		{
			return ServiceMetadata.SupportedProperties.Contains(propertyName);
		}
		return false;
	}

	private string GetServiceMetadataKey()
	{
		return "DataServiceMetadata|" + _metadataUri.OriginalString;
	}

	private static DataServiceMetadata GetDataServiceMetadata(Uri metadataUri)
	{
		if (metadataUri == null)
		{
			return null;
		}
		HttpClient httpClient = new HttpClient(metadataUri);
		using MemoryStream memoryStream = new MemoryStream();
		httpClient.DownloadData(memoryStream);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return ExtractMetadataFromSchema(memoryStream);
	}

	internal static DataServiceMetadata ExtractMetadataFromSchema(Stream schemaStream)
	{
		if (schemaStream == null)
		{
			return null;
		}
		XDocument schemaDocument;
		try
		{
			schemaDocument = XmlUtility.LoadSafe(schemaStream);
		}
		catch
		{
			return null;
		}
		return ExtractMetadataInternal(schemaDocument);
	}

	private static DataServiceMetadata ExtractMetadataInternal(XDocument schemaDocument)
	{
		var anon = (from e in ((XContainer)schemaDocument).Descendants()
			where e.Name.LocalName == "EntityContainer"
			let entitySet = ((XContainer)e).Elements().FirstOrDefault((XElement el) => el.Name.LocalName == "EntitySet")
			let name = (entitySet != null) ? entitySet.Attribute(XName.op_Implicit("Name")).Value : null
			where name != null && name.Equals("Packages", StringComparison.OrdinalIgnoreCase)
			select new
			{
				Container = e,
				EntitySet = entitySet
			}).FirstOrDefault();
		if (anon == null)
		{
			return null;
		}
		XElement container = anon.Container;
		XAttribute val = anon.EntitySet.Attribute(XName.op_Implicit("EntityType"));
		string packageEntityName = null;
		if (val != null)
		{
			packageEntityName = val.Value;
		}
		return new DataServiceMetadata
		{
			SupportedMethodNames = new HashSet<string>(from e in ((XContainer)container).Elements()
				where e.Name.LocalName == "FunctionImport"
				select e.Attribute(XName.op_Implicit("Name")).Value, StringComparer.OrdinalIgnoreCase),
			SupportedProperties = new HashSet<string>(ExtractSupportedProperties(schemaDocument, packageEntityName), StringComparer.OrdinalIgnoreCase)
		};
	}

	private static IEnumerable<string> ExtractSupportedProperties(XDocument schemaDocument, string packageEntityName)
	{
		packageEntityName = TrimNamespace(packageEntityName);
		XElement val = (from e in ((XContainer)schemaDocument).Descendants()
			where e.Name.LocalName == "EntityType"
			let attribute = e.Attribute(XName.op_Implicit("Name"))
			where attribute != null && attribute.Value.Equals(packageEntityName, StringComparison.OrdinalIgnoreCase)
			select e).FirstOrDefault();
		if (val != null)
		{
			return from e in ((XContainer)val).Elements()
				where e.Name.LocalName == "Property"
				select e.Attribute(XName.op_Implicit("Name")).Value;
		}
		return Enumerable.Empty<string>();
	}

	private static string TrimNamespace(string packageEntityName)
	{
		int num = packageEntityName.LastIndexOf('.');
		if (num > 0 && num < packageEntityName.Length)
		{
			packageEntityName = packageEntityName.Substring(num + 1);
		}
		return packageEntityName;
	}
}
