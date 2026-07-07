namespace SharpCompress.Common.Zip.Headers;

internal enum ExtraDataType : ushort
{
	WinZipAes = 39169,
	NotImplementedExtraData = ushort.MaxValue,
	UnicodePathExtraField = 28789,
	Zip64ExtendedInformationExtraField = 1
}
