using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using NuGet.Resources;

namespace NuGet;

internal static class ManifestSchemaUtility
{
	internal const string SchemaVersionV1 = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

	internal const string SchemaVersionV2 = "http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd";

	internal const string SchemaVersionV3 = "http://schemas.microsoft.com/packaging/2011/10/nuspec.xsd";

	internal const string SchemaVersionV4 = "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd";

	internal const string SchemaVersionV5 = "http://schemas.microsoft.com/packaging/2013/01/nuspec.xsd";

	internal const string SchemaVersionV6 = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";

	private static readonly string[] VersionToSchemaMappings = new string[6] { "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd", "http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd", "http://schemas.microsoft.com/packaging/2011/10/nuspec.xsd", "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd", "http://schemas.microsoft.com/packaging/2013/01/nuspec.xsd", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd" };

	private static ConcurrentDictionary<string, XmlSchemaSet> _manifestSchemaSetCache = new ConcurrentDictionary<string, XmlSchemaSet>(StringComparer.OrdinalIgnoreCase);

	public static int GetVersionFromNamespace(string @namespace)
	{
		return Math.Max(0, Array.IndexOf(VersionToSchemaMappings, @namespace)) + 1;
	}

	public static string GetSchemaNamespace(int version)
	{
		if (version <= 0 || version > VersionToSchemaMappings.Length)
		{
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnknownSchemaVersion, new object[1] { version }));
		}
		return VersionToSchemaMappings[version - 1];
	}

	public static XmlSchemaSet GetManifestSchemaSet(string schemaNamespace)
	{
		return _manifestSchemaSetCache.GetOrAdd(schemaNamespace, (Func<string, XmlSchemaSet>)delegate(string schema)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			string s;
			using (StreamReader streamReader = new StreamReader(typeof(Manifest).Assembly.GetManifestResourceStream("NuGet.Authoring.nuspec.xsd")))
			{
				string format = streamReader.ReadToEnd();
				s = string.Format(CultureInfo.InvariantCulture, format, new object[1] { schema });
			}
			using StringReader stringReader = new StringReader(s);
			XmlSchemaSet val = new XmlSchemaSet();
			XmlReaderSettings val2 = new XmlReaderSettings
			{
				DtdProcessing = (DtdProcessing)0,
				XmlResolver = null
			};
			val.Add(schema, XmlReader.Create((TextReader)stringReader, val2));
			return val;
		});
	}

	public static bool IsKnownSchema(string schemaNamespace)
	{
		return Enumerable.Contains<string>(VersionToSchemaMappings, schemaNamespace, StringComparer.OrdinalIgnoreCase);
	}
}
