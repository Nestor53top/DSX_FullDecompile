using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Versioning;

namespace NuGet;

internal static class NetPortableProfileTableSerializer
{
	[DataContract]
	private class PortableProfile
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string FrameworkVersion { get; set; }

		[DataMember]
		public string[] SupportedFrameworks { get; set; }

		[DataMember]
		public string[] OptionalFrameworks { get; set; }
	}

	private static readonly DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(IEnumerable<PortableProfile>));

	internal static void Serialize(NetPortableProfileTable portableProfileTable, Stream output)
	{
		IEnumerable<PortableProfile> enumerable = portableProfileTable.Profiles.Select((NetPortableProfile p) => new PortableProfile
		{
			Name = p.Name,
			FrameworkVersion = p.FrameworkVersion,
			SupportedFrameworks = p.SupportedFrameworks.Select((FrameworkName f) => f.FullName).ToArray(),
			OptionalFrameworks = p.OptionalFrameworks.Select((FrameworkName f) => f.FullName).ToArray()
		});
		((XmlObjectSerializer)_serializer).WriteObject(output, (object)enumerable);
	}

	internal static NetPortableProfileTable Deserialize(Stream input)
	{
		return new NetPortableProfileTable(((IEnumerable<PortableProfile>)((XmlObjectSerializer)_serializer).ReadObject(input)).Select((PortableProfile p) => new NetPortableProfile(p.FrameworkVersion, p.Name, p.SupportedFrameworks.Select((string f) => new FrameworkName(f)), p.OptionalFrameworks.Select((string f) => new FrameworkName(f)))));
	}
}
