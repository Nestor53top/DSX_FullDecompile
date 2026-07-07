using System.Text;

namespace SharpCompress.Common;

internal static class ArchiveEncoding
{
	public static Encoding Default { get; set; }

	public static Encoding Password { get; set; }

	static ArchiveEncoding()
	{
		Default = Encoding.UTF8;
		Password = Encoding.UTF8;
	}
}
