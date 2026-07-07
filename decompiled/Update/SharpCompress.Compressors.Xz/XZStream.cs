using System;
using System.IO;

namespace SharpCompress.Compressors.Xz;

[CLSCompliant(false)]
internal sealed class XZStream : XZReadOnlyStream
{
	private XZBlock _currentBlock;

	private bool _endOfStream;

	public XZHeader Header { get; private set; }

	public XZIndex Index { get; private set; }

	public XZFooter Footer { get; private set; }

	public bool HeaderIsRead { get; private set; }

	public static bool IsXZStream(Stream stream)
	{
		try
		{
			return XZHeader.FromStream(stream) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void AssertBlockCheckTypeIsSupported()
	{
		switch (Header.BlockCheckType)
		{
		case CheckType.SHA256:
			throw new NotImplementedException();
		default:
			throw new NotSupportedException("Check Type unknown to this version of decoder.");
		case CheckType.NONE:
		case CheckType.CRC32:
		case CheckType.CRC64:
			break;
		}
	}

	public XZStream(Stream stream)
		: base(stream)
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int result = 0;
		if (_endOfStream)
		{
			return result;
		}
		if (!HeaderIsRead)
		{
			ReadHeader();
		}
		result = ReadBlocks(buffer, offset, count);
		if (result < count)
		{
			_endOfStream = true;
			ReadIndex();
			ReadFooter();
		}
		return result;
	}

	private void ReadHeader()
	{
		Header = XZHeader.FromStream(base.BaseStream);
		AssertBlockCheckTypeIsSupported();
		HeaderIsRead = true;
	}

	private void ReadIndex()
	{
		Index = XZIndex.FromStream(base.BaseStream, indexMarkerAlreadyVerified: true);
	}

	private void ReadFooter()
	{
		Footer = XZFooter.FromStream(base.BaseStream);
	}

	private int ReadBlocks(byte[] buffer, int offset, int count)
	{
		int num = 0;
		if (_currentBlock == null)
		{
			NextBlock();
		}
		while (true)
		{
			try
			{
				if (num >= count)
				{
					break;
				}
				int num2 = count - num;
				int offset2 = offset + num;
				int num3 = _currentBlock.Read(buffer, offset2, num2);
				if (num3 < num2)
				{
					NextBlock();
				}
				num += num3;
				continue;
			}
			catch (XZIndexMarkerReachedException)
			{
			}
			break;
		}
		return num;
	}

	private void NextBlock()
	{
		_currentBlock = new XZBlock(base.BaseStream, Header.BlockCheckType, Header.BlockCheckSize);
	}
}
