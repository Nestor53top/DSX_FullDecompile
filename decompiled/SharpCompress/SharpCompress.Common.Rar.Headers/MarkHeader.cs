using SharpCompress.IO;

namespace SharpCompress.Common.Rar.Headers;

internal class MarkHeader : RarHeader
{
	internal bool OldFormat { get; private set; }

	protected override void ReadFromReader(MarkingBinaryReader reader)
	{
	}

	internal bool IsValid()
	{
		if (base.HeadCRC == 24914 && base.HeaderType == HeaderType.MarkHeader && base.Flags == 6689)
		{
			return base.HeaderSize == 7;
		}
		return false;
	}
}
