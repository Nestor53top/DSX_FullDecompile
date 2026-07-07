using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Compressors.Xz.Filters;

namespace SharpCompress.Compressors.Xz;

internal sealed class XZBlock : XZReadOnlyStream
{
	private CheckType _checkType;

	private int _checkSize;

	private bool _streamConnected;

	private int _numFilters;

	private byte _blockHeaderSizeByte;

	private Stream _decomStream;

	private bool _endOfStream;

	private bool _paddingSkipped;

	private bool _crcChecked;

	private ulong _bytesRead;

	public int BlockHeaderSize => (_blockHeaderSizeByte + 1) * 4;

	public ulong? CompressedSize { get; private set; }

	public ulong? UncompressedSize { get; private set; }

	public Stack<BlockFilter> Filters { get; private set; } = new Stack<BlockFilter>();

	public bool HeaderIsLoaded { get; private set; }

	public XZBlock(Stream stream, CheckType checkType, int checkSize)
		: base(stream)
	{
		_checkType = checkType;
		_checkSize = checkSize;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = 0;
		if (!HeaderIsLoaded)
		{
			LoadHeader();
		}
		if (!_streamConnected)
		{
			ConnectStream();
		}
		if (!_endOfStream)
		{
			num = _decomStream.Read(buffer, offset, count);
		}
		if (num != count)
		{
			_endOfStream = true;
		}
		if (_endOfStream && !_paddingSkipped)
		{
			SkipPadding();
		}
		if (_endOfStream && !_crcChecked)
		{
			CheckCrc();
		}
		_bytesRead += (ulong)num;
		return num;
	}

	private void SkipPadding()
	{
		int num = (int)(_bytesRead % 4);
		if (num > 0)
		{
			byte[] array = new byte[num];
			base.BaseStream.Read(array, 0, num);
			if (array.Any((byte b) => b != 0))
			{
				throw new InvalidDataException("Padding bytes were non-null");
			}
		}
		_paddingSkipped = true;
	}

	private void CheckCrc()
	{
		byte[] buffer = new byte[_checkSize];
		base.BaseStream.Read(buffer, 0, _checkSize);
		_crcChecked = true;
	}

	private void ConnectStream()
	{
		_decomStream = base.BaseStream;
		while (Filters.Any())
		{
			BlockFilter blockFilter = Filters.Pop();
			blockFilter.SetBaseStream(_decomStream);
			_decomStream = blockFilter;
		}
		_streamConnected = true;
	}

	private void LoadHeader()
	{
		ReadHeaderSize();
		using (MemoryStream input = new MemoryStream(CacheHeader()))
		{
			using BinaryReader binaryReader = new BinaryReader(input);
			binaryReader.BaseStream.Position = 1L;
			ReadBlockFlags(binaryReader);
			ReadFilters(binaryReader, 0L);
		}
		HeaderIsLoaded = true;
	}

	private void ReadHeaderSize()
	{
		_blockHeaderSizeByte = (byte)base.BaseStream.ReadByte();
		if (_blockHeaderSizeByte == 0)
		{
			throw new XZIndexMarkerReachedException();
		}
	}

	private byte[] CacheHeader()
	{
		byte[] array = new byte[BlockHeaderSize - 4];
		array[0] = _blockHeaderSizeByte;
		if (base.BaseStream.Read(array, 1, BlockHeaderSize - 5) != BlockHeaderSize - 5)
		{
			throw new EndOfStreamException("Reached end of stream unexectedly");
		}
		uint num = base.BaseStream.ReadLittleEndianUInt32();
		uint num2 = Crc32.Compute(array);
		if (num != num2)
		{
			throw new InvalidDataException("Block header corrupt");
		}
		return array;
	}

	private void ReadBlockFlags(BinaryReader reader)
	{
		byte b = reader.ReadByte();
		_numFilters = (b & 3) + 1;
		if ((byte)(b & 0x3C) != 0)
		{
			throw new InvalidDataException("Reserved bytes used, perhaps an unknown XZ implementation");
		}
		bool num = (b & 0x40) != 0;
		bool flag = (b & 0x80) != 0;
		if (num)
		{
			CompressedSize = reader.ReadXZInteger();
		}
		if (flag)
		{
			UncompressedSize = reader.ReadXZInteger();
		}
	}

	private void ReadFilters(BinaryReader reader, long baseStreamOffset = 0L)
	{
		int num = 0;
		for (int i = 0; i < _numFilters; i++)
		{
			BlockFilter blockFilter = BlockFilter.Read(reader);
			if ((i + 1 == _numFilters && !blockFilter.AllowAsLast) || (i + 1 < _numFilters && !blockFilter.AllowAsNonLast))
			{
				throw new InvalidDataException("Block Filters in bad order");
			}
			if (blockFilter.ChangesDataSize && i + 1 < _numFilters)
			{
				num++;
			}
			blockFilter.ValidateFilter();
			Filters.Push(blockFilter);
		}
		if (num > 2)
		{
			throw new InvalidDataException("More than two non-last block filters cannot change stream size");
		}
		int count = BlockHeaderSize - (4 + (int)(reader.BaseStream.Position - baseStreamOffset));
		if (!reader.ReadBytes(count).All((byte b) => b == 0))
		{
			throw new InvalidDataException("Block header contains unknown fields");
		}
	}
}
