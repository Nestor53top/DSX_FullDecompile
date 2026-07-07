namespace SharpCompress.Compressors.Rar.VM;

internal class VMStandardFilterSignature
{
	internal int Length { get; }

	internal uint CRC { get; }

	internal VMStandardFilters Type { get; }

	internal VMStandardFilterSignature(int length, uint crc, VMStandardFilters type)
	{
		Length = length;
		CRC = crc;
		Type = type;
	}
}
