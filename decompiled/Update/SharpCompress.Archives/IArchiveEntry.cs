using System.IO;
using SharpCompress.Common;

namespace SharpCompress.Archives;

internal interface IArchiveEntry : IEntry
{
	bool IsComplete { get; }

	IArchive Archive { get; }

	Stream OpenEntryStream();
}
