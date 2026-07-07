using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.LZMA.Utilites;
using SharpCompress.IO;

namespace SharpCompress.Common.SevenZip;

internal class ArchiveReader
{
	internal class CExtractFolderInfo
	{
		internal int FileIndex;

		internal int FolderIndex;

		internal List<bool> ExtractStatuses = new List<bool>();

		internal CExtractFolderInfo(int fileIndex, int folderIndex)
		{
			FileIndex = fileIndex;
			FolderIndex = folderIndex;
			if (fileIndex != -1)
			{
				ExtractStatuses.Add(item: true);
			}
		}
	}

	private class FolderUnpackStream : Stream
	{
		private readonly ArchiveDatabase _db;

		private readonly int _startIndex;

		private readonly List<bool> _extractStatuses;

		private Stream _stream;

		private long _rem;

		private int _currentIndex;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public FolderUnpackStream(ArchiveDatabase db, int p, int startIndex, List<bool> list)
		{
			_db = db;
			_startIndex = startIndex;
			_extractStatuses = list;
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		private void ProcessEmptyFiles()
		{
			while (_currentIndex < _extractStatuses.Count && _db.Files[_startIndex + _currentIndex].Size == 0L)
			{
				OpenFile();
				_stream.Dispose();
				_stream = null;
				_currentIndex++;
			}
		}

		private void OpenFile()
		{
			int index = _startIndex + _currentIndex;
			if (_db.Files[index].CrcDefined)
			{
				_stream = new CrcCheckStream(_db.Files[index].Crc.Value);
			}
			else
			{
				_stream = new MemoryStream();
			}
			_rem = _db.Files[index].Size;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			while (count != 0)
			{
				if (_stream != null)
				{
					int num = count;
					if (num > _rem)
					{
						num = (int)_rem;
					}
					_stream.Write(buffer, offset, num);
					count -= num;
					_rem -= num;
					offset += num;
					if (_rem == 0L)
					{
						_stream.Dispose();
						_stream = null;
						_currentIndex++;
						ProcessEmptyFiles();
					}
				}
				else
				{
					ProcessEmptyFiles();
					if (_currentIndex == _extractStatuses.Count)
					{
						Debugger.Break();
						throw new NotSupportedException();
					}
					OpenFile();
				}
			}
		}
	}

	internal Stream _stream;

	internal Stack<DataReader> _readerStack = new Stack<DataReader>();

	internal DataReader _currentReader;

	internal long _streamOrigin;

	internal long _streamEnding;

	internal byte[] _header;

	private readonly Dictionary<int, Stream> _cachedStreams = new Dictionary<int, Stream>();

	internal void AddByteStream(byte[] buffer, int offset, int length)
	{
		_readerStack.Push(_currentReader);
		_currentReader = new DataReader(buffer, offset, length);
	}

	internal void DeleteByteStream()
	{
		_currentReader = _readerStack.Pop();
	}

	internal byte ReadByte()
	{
		return _currentReader.ReadByte();
	}

	private void ReadBytes(byte[] buffer, int offset, int length)
	{
		_currentReader.ReadBytes(buffer, offset, length);
	}

	private ulong ReadNumber()
	{
		return _currentReader.ReadNumber();
	}

	internal int ReadNum()
	{
		return _currentReader.ReadNum();
	}

	private uint ReadUInt32()
	{
		return _currentReader.ReadUInt32();
	}

	private ulong ReadUInt64()
	{
		return _currentReader.ReadUInt64();
	}

	private BlockType? ReadId()
	{
		ulong num = _currentReader.ReadNumber();
		if (num > 25)
		{
			return null;
		}
		return (BlockType)num;
	}

	private void SkipData(long size)
	{
		_currentReader.SkipData(size);
	}

	private void SkipData()
	{
		_currentReader.SkipData();
	}

	private void WaitAttribute(BlockType attribute)
	{
		while (true)
		{
			BlockType? blockType = ReadId();
			if (blockType == attribute)
			{
				return;
			}
			if (blockType == BlockType.End)
			{
				break;
			}
			SkipData();
		}
		throw new InvalidOperationException();
	}

	private void ReadArchiveProperties()
	{
		while (ReadId() != BlockType.End)
		{
			SkipData();
		}
	}

	private BitVector ReadBitVector(int length)
	{
		BitVector bitVector = new BitVector(length);
		byte b = 0;
		byte b2 = 0;
		for (int i = 0; i < length; i++)
		{
			if (b2 == 0)
			{
				b = ReadByte();
				b2 = 128;
			}
			if ((b & b2) != 0)
			{
				bitVector.SetBit(i);
			}
			b2 >>= 1;
		}
		return bitVector;
	}

	private BitVector ReadOptionalBitVector(int length)
	{
		if (ReadByte() != 0)
		{
			return new BitVector(length, initValue: true);
		}
		return ReadBitVector(length);
	}

	private void ReadNumberVector(List<byte[]> dataVector, int numFiles, Action<int, long?> action)
	{
		BitVector bitVector = ReadOptionalBitVector(numFiles);
		using CStreamSwitch cStreamSwitch = default(CStreamSwitch);
		cStreamSwitch.Set(this, dataVector);
		for (int i = 0; i < numFiles; i++)
		{
			if (bitVector[i])
			{
				action(i, checked((long)ReadUInt64()));
			}
			else
			{
				action(i, null);
			}
		}
	}

	private DateTime TranslateTime(long time)
	{
		return DateTime.FromFileTimeUtc(time).ToLocalTime();
	}

	private DateTime? TranslateTime(long? time)
	{
		if (time.HasValue)
		{
			return TranslateTime(time.Value);
		}
		return null;
	}

	private void ReadDateTimeVector(List<byte[]> dataVector, int numFiles, Action<int, DateTime?> action)
	{
		ReadNumberVector(dataVector, numFiles, delegate(int index, long? value)
		{
			action(index, TranslateTime(value));
		});
	}

	private void ReadAttributeVector(List<byte[]> dataVector, int numFiles, Action<int, uint?> action)
	{
		BitVector bitVector = ReadOptionalBitVector(numFiles);
		using CStreamSwitch cStreamSwitch = default(CStreamSwitch);
		cStreamSwitch.Set(this, dataVector);
		for (int i = 0; i < numFiles; i++)
		{
			if (bitVector[i])
			{
				action(i, ReadUInt32());
			}
			else
			{
				action(i, null);
			}
		}
	}

	private void GetNextFolderItem(CFolder folder)
	{
		int num = ReadNum();
		folder.Coders = new List<CCoderInfo>(num);
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			CCoderInfo cCoderInfo = new CCoderInfo();
			folder.Coders.Add(cCoderInfo);
			byte b = ReadByte();
			int num4 = b & 0xF;
			byte[] array = new byte[num4];
			ReadBytes(array, 0, num4);
			if (num4 > 8)
			{
				throw new NotSupportedException();
			}
			ulong num5 = 0uL;
			for (int j = 0; j < num4; j++)
			{
				num5 |= (ulong)array[num4 - 1 - j] << 8 * j;
			}
			cCoderInfo.MethodId = new CMethodId(num5);
			if ((b & 0x10) != 0)
			{
				cCoderInfo.NumInStreams = ReadNum();
				cCoderInfo.NumOutStreams = ReadNum();
			}
			else
			{
				cCoderInfo.NumInStreams = 1;
				cCoderInfo.NumOutStreams = 1;
			}
			if ((b & 0x20) != 0)
			{
				int num6 = ReadNum();
				cCoderInfo.Props = new byte[num6];
				ReadBytes(cCoderInfo.Props, 0, num6);
			}
			if ((b & 0x80) != 0)
			{
				throw new NotSupportedException();
			}
			num2 += cCoderInfo.NumInStreams;
			num3 += cCoderInfo.NumOutStreams;
		}
		int num7 = num3 - 1;
		folder.BindPairs = new List<CBindPair>(num7);
		for (int k = 0; k < num7; k++)
		{
			CBindPair cBindPair = new CBindPair();
			cBindPair.InIndex = ReadNum();
			cBindPair.OutIndex = ReadNum();
			folder.BindPairs.Add(cBindPair);
		}
		if (num2 < num7)
		{
			throw new NotSupportedException();
		}
		int num8 = num2 - num7;
		if (num8 == 1)
		{
			for (int l = 0; l < num2; l++)
			{
				if (folder.FindBindPairForInStream(l) < 0)
				{
					folder.PackStreams.Add(l);
					break;
				}
			}
			if (folder.PackStreams.Count != 1)
			{
				throw new NotSupportedException();
			}
		}
		else
		{
			for (int m = 0; m < num8; m++)
			{
				int item = ReadNum();
				folder.PackStreams.Add(item);
			}
		}
	}

	private List<uint?> ReadHashDigests(int count)
	{
		BitVector bitVector = ReadOptionalBitVector(count);
		List<uint?> list = new List<uint?>(count);
		for (int i = 0; i < count; i++)
		{
			if (bitVector[i])
			{
				uint value = ReadUInt32();
				list.Add(value);
			}
			else
			{
				list.Add(null);
			}
		}
		return list;
	}

	private void ReadPackInfo(out long dataOffset, out List<long> packSizes, out List<uint?> packCRCs)
	{
		packCRCs = null;
		dataOffset = checked((long)ReadNumber());
		int num = ReadNum();
		WaitAttribute(BlockType.Size);
		packSizes = new List<long>(num);
		for (int i = 0; i < num; i++)
		{
			long item = checked((long)ReadNumber());
			packSizes.Add(item);
		}
		while (true)
		{
			BlockType? blockType = ReadId();
			if (blockType == BlockType.End)
			{
				break;
			}
			if (blockType == BlockType.CRC)
			{
				packCRCs = ReadHashDigests(num);
			}
			else
			{
				SkipData();
			}
		}
		if (packCRCs == null)
		{
			packCRCs = new List<uint?>(num);
			for (int j = 0; j < num; j++)
			{
				packCRCs.Add(null);
			}
		}
	}

	private void ReadUnpackInfo(List<byte[]> dataVector, out List<CFolder> folders)
	{
		WaitAttribute(BlockType.Folder);
		int num = ReadNum();
		using (CStreamSwitch cStreamSwitch = default(CStreamSwitch))
		{
			cStreamSwitch.Set(this, dataVector);
			folders = new List<CFolder>(num);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				CFolder cFolder = new CFolder
				{
					FirstPackStreamId = num2
				};
				folders.Add(cFolder);
				GetNextFolderItem(cFolder);
				num2 += cFolder.PackStreams.Count;
			}
		}
		WaitAttribute(BlockType.CodersUnpackSize);
		for (int j = 0; j < num; j++)
		{
			CFolder cFolder2 = folders[j];
			int numOutStreams = cFolder2.GetNumOutStreams();
			for (int k = 0; k < numOutStreams; k++)
			{
				long item = checked((long)ReadNumber());
				cFolder2.UnpackSizes.Add(item);
			}
		}
		while (true)
		{
			BlockType? blockType = ReadId();
			if (blockType == BlockType.End)
			{
				break;
			}
			if (blockType == BlockType.CRC)
			{
				List<uint?> list = ReadHashDigests(num);
				for (int l = 0; l < num; l++)
				{
					folders[l].UnpackCRC = list[l];
				}
			}
			else
			{
				SkipData();
			}
		}
	}

	private void ReadSubStreamsInfo(List<CFolder> folders, out List<int> numUnpackStreamsInFolders, out List<long> unpackSizes, out List<uint?> digests)
	{
		numUnpackStreamsInFolders = null;
		BlockType? blockType;
		while (true)
		{
			blockType = ReadId();
			if (blockType == BlockType.NumUnpackStream)
			{
				numUnpackStreamsInFolders = new List<int>(folders.Count);
				for (int i = 0; i < folders.Count; i++)
				{
					int item = ReadNum();
					numUnpackStreamsInFolders.Add(item);
				}
			}
			else
			{
				if (blockType == BlockType.CRC || blockType == BlockType.Size || blockType == BlockType.End)
				{
					break;
				}
				SkipData();
			}
		}
		if (numUnpackStreamsInFolders == null)
		{
			numUnpackStreamsInFolders = new List<int>(folders.Count);
			for (int j = 0; j < folders.Count; j++)
			{
				numUnpackStreamsInFolders.Add(1);
			}
		}
		unpackSizes = new List<long>(folders.Count);
		for (int k = 0; k < numUnpackStreamsInFolders.Count; k++)
		{
			int num = numUnpackStreamsInFolders[k];
			if (num == 0)
			{
				continue;
			}
			long num2 = 0L;
			for (int l = 1; l < num; l++)
			{
				if (blockType == BlockType.Size)
				{
					long num3 = checked((long)ReadNumber());
					unpackSizes.Add(num3);
					num2 += num3;
				}
			}
			unpackSizes.Add(folders[k].GetUnpackSize() - num2);
		}
		if (blockType == BlockType.Size)
		{
			blockType = ReadId();
		}
		int num4 = 0;
		int num5 = 0;
		for (int m = 0; m < folders.Count; m++)
		{
			int num6 = numUnpackStreamsInFolders[m];
			if (num6 != 1 || !folders[m].UnpackCRCDefined)
			{
				num4 += num6;
			}
			num5 += num6;
		}
		digests = null;
		while (true)
		{
			if (blockType == BlockType.CRC)
			{
				digests = new List<uint?>(num5);
				List<uint?> list = ReadHashDigests(num4);
				int num7 = 0;
				for (int n = 0; n < folders.Count; n++)
				{
					int num8 = numUnpackStreamsInFolders[n];
					CFolder cFolder = folders[n];
					if (num8 == 1 && cFolder.UnpackCRCDefined)
					{
						digests.Add(cFolder.UnpackCRC.Value);
						continue;
					}
					int num9 = 0;
					while (num9 < num8)
					{
						digests.Add(list[num7]);
						num9++;
						num7++;
					}
				}
				if (num7 != num4 || num5 != digests.Count)
				{
					Debugger.Break();
				}
			}
			else
			{
				if (blockType == BlockType.End)
				{
					break;
				}
				SkipData();
			}
			blockType = ReadId();
		}
		if (digests == null)
		{
			digests = new List<uint?>(num5);
			for (int num10 = 0; num10 < num5; num10++)
			{
				digests.Add(null);
			}
		}
	}

	private void ReadStreamsInfo(List<byte[]> dataVector, out long dataOffset, out List<long> packSizes, out List<uint?> packCRCs, out List<CFolder> folders, out List<int> numUnpackStreamsInFolders, out List<long> unpackSizes, out List<uint?> digests)
	{
		dataOffset = long.MinValue;
		packSizes = null;
		packCRCs = null;
		folders = null;
		numUnpackStreamsInFolders = null;
		unpackSizes = null;
		digests = null;
		while (true)
		{
			switch (ReadId())
			{
			case BlockType.End:
				return;
			case BlockType.PackInfo:
				ReadPackInfo(out dataOffset, out packSizes, out packCRCs);
				break;
			case BlockType.UnpackInfo:
				ReadUnpackInfo(dataVector, out folders);
				break;
			case BlockType.SubStreamsInfo:
				ReadSubStreamsInfo(folders, out numUnpackStreamsInFolders, out unpackSizes, out digests);
				break;
			default:
				throw new InvalidOperationException();
			}
		}
	}

	private List<byte[]> ReadAndDecodePackedStreams(long baseOffset, IPasswordProvider pass)
	{
		ReadStreamsInfo(null, out var dataOffset, out var packSizes, out var _, out var folders, out var _, out var _, out var _);
		dataOffset += baseOffset;
		List<byte[]> list = new List<byte[]>(folders.Count);
		int num = 0;
		foreach (CFolder item in folders)
		{
			long startPos = dataOffset;
			long[] array = new long[item.PackStreams.Count];
			for (int i = 0; i < array.Length; i++)
			{
				dataOffset += (array[i] = packSizes[num + i]);
			}
			Stream stream = DecoderStreamHelper.CreateDecoderStream(_stream, startPos, array, item, pass);
			int num2 = checked((int)item.GetUnpackSize());
			byte[] array2 = new byte[num2];
			stream.ReadExact(array2, 0, array2.Length);
			if (stream.ReadByte() >= 0)
			{
				throw new InvalidOperationException("Decoded stream is longer than expected.");
			}
			list.Add(array2);
			if (item.UnpackCRCDefined && CRC.Finish(CRC.Update(uint.MaxValue, array2, 0, num2)) != item.UnpackCRC)
			{
				throw new InvalidOperationException("Decoded stream does not match expected CRC.");
			}
		}
		return list;
	}

	private void ReadHeader(ArchiveDatabase db, IPasswordProvider getTextPassword)
	{
		BlockType? blockType = ReadId();
		if (blockType == BlockType.ArchiveProperties)
		{
			ReadArchiveProperties();
			blockType = ReadId();
		}
		List<byte[]> dataVector = null;
		if (blockType == BlockType.AdditionalStreamsInfo)
		{
			dataVector = ReadAndDecodePackedStreams(db.StartPositionAfterHeader, getTextPassword);
			blockType = ReadId();
		}
		List<long> unpackSizes;
		List<uint?> digests;
		if (blockType == BlockType.MainStreamsInfo)
		{
			ReadStreamsInfo(dataVector, out db.DataStartPosition, out db.PackSizes, out db.PackCRCs, out db.Folders, out db.NumUnpackStreamsVector, out unpackSizes, out digests);
			db.DataStartPosition += db.StartPositionAfterHeader;
			blockType = ReadId();
		}
		else
		{
			unpackSizes = new List<long>(db.Folders.Count);
			digests = new List<uint?>(db.Folders.Count);
			db.NumUnpackStreamsVector = new List<int>(db.Folders.Count);
			for (int i = 0; i < db.Folders.Count; i++)
			{
				CFolder cFolder = db.Folders[i];
				unpackSizes.Add(cFolder.GetUnpackSize());
				digests.Add(cFolder.UnpackCRC);
				db.NumUnpackStreamsVector.Add(1);
			}
		}
		db.Files.Clear();
		if (blockType == BlockType.End)
		{
			return;
		}
		if (blockType != BlockType.FilesInfo)
		{
			throw new InvalidOperationException();
		}
		int num = ReadNum();
		db.Files = new List<CFileItem>(num);
		for (int j = 0; j < num; j++)
		{
			db.Files.Add(new CFileItem());
		}
		BitVector bitVector = new BitVector(num);
		BitVector bitVector2 = null;
		BitVector bitVector3 = null;
		int num2 = 0;
		while (true)
		{
			blockType = ReadId();
			if (blockType == BlockType.End)
			{
				break;
			}
			long num3 = checked((long)ReadNumber());
			int offset = _currentReader.Offset;
			switch (blockType)
			{
			case BlockType.Name:
			{
				using (CStreamSwitch cStreamSwitch = default(CStreamSwitch))
				{
					cStreamSwitch.Set(this, dataVector);
					for (int num5 = 0; num5 < db.Files.Count; num5++)
					{
						db.Files[num5].Name = _currentReader.ReadString();
					}
				}
				break;
			}
			case BlockType.WinAttributes:
				ReadAttributeVector(dataVector, num, delegate(int index, uint? attr)
				{
					if (attr.HasValue && attr.Value >> 16 != 0)
					{
						attr = attr.Value & 0x7FFF;
					}
					db.Files[index].Attrib = attr;
				});
				break;
			case BlockType.EmptyStream:
			{
				bitVector = ReadBitVector(num);
				for (int num6 = 0; num6 < bitVector.Length; num6++)
				{
					if (bitVector[num6])
					{
						num2++;
					}
				}
				bitVector2 = new BitVector(num2);
				bitVector3 = new BitVector(num2);
				break;
			}
			case BlockType.EmptyFile:
				bitVector2 = ReadBitVector(num2);
				break;
			case BlockType.Anti:
				bitVector3 = ReadBitVector(num2);
				break;
			case BlockType.StartPos:
				ReadNumberVector(dataVector, num, delegate(int index, long? startPos)
				{
					db.Files[index].StartPos = startPos;
				});
				break;
			case BlockType.CTime:
				ReadDateTimeVector(dataVector, num, delegate(int index, DateTime? time)
				{
					db.Files[index].CTime = time;
				});
				break;
			case BlockType.ATime:
				ReadDateTimeVector(dataVector, num, delegate(int index, DateTime? time)
				{
					db.Files[index].ATime = time;
				});
				break;
			case BlockType.MTime:
				ReadDateTimeVector(dataVector, num, delegate(int index, DateTime? time)
				{
					db.Files[index].MTime = time;
				});
				break;
			case BlockType.Dummy:
			{
				for (long num4 = 0L; num4 < num3; num4++)
				{
					if (ReadByte() != 0)
					{
						throw new InvalidOperationException();
					}
				}
				break;
			}
			default:
				SkipData(num3);
				break;
			}
			if ((db.MajorVersion > 0 || db.MinorVersion > 2) && _currentReader.Offset - offset != num3)
			{
				throw new InvalidOperationException();
			}
		}
		int num7 = 0;
		int num8 = 0;
		for (int num9 = 0; num9 < num; num9++)
		{
			CFileItem cFileItem = db.Files[num9];
			cFileItem.HasStream = !bitVector[num9];
			if (cFileItem.HasStream)
			{
				cFileItem.IsDir = false;
				cFileItem.IsAnti = false;
				cFileItem.Size = unpackSizes[num8];
				cFileItem.Crc = digests[num8];
				num8++;
			}
			else
			{
				cFileItem.IsDir = !bitVector2[num7];
				cFileItem.IsAnti = bitVector3[num7];
				num7++;
				cFileItem.Size = 0L;
				cFileItem.Crc = null;
			}
		}
	}

	public void Open(Stream stream)
	{
		Close();
		_streamOrigin = stream.Position;
		_streamEnding = stream.Length;
		_header = new byte[32];
		int num;
		for (int i = 0; i < 32; i += num)
		{
			num = stream.Read(_header, i, 32 - i);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
		}
		_stream = stream;
	}

	public void Close()
	{
		if (_stream != null)
		{
			_stream.Dispose();
		}
		foreach (Stream value in _cachedStreams.Values)
		{
			value.Dispose();
		}
		_cachedStreams.Clear();
	}

	public ArchiveDatabase ReadDatabase(IPasswordProvider pass)
	{
		ArchiveDatabase archiveDatabase = new ArchiveDatabase();
		archiveDatabase.Clear();
		archiveDatabase.MajorVersion = _header[6];
		archiveDatabase.MinorVersion = _header[7];
		if (archiveDatabase.MajorVersion != 0)
		{
			throw new InvalidOperationException();
		}
		uint num = DataReader.Get32(_header, 8);
		long num2 = (long)DataReader.Get64(_header, 12);
		long num3 = (long)DataReader.Get64(_header, 20);
		uint num4 = DataReader.Get32(_header, 28);
		if (CRC.Finish(CRC.Update(CRC.Update(CRC.Update(uint.MaxValue, num2), num3), num4)) != num)
		{
			throw new InvalidOperationException();
		}
		archiveDatabase.StartPositionAfterHeader = _streamOrigin + 32;
		if (num3 == 0L)
		{
			archiveDatabase.Fill();
			return archiveDatabase;
		}
		if (num2 < 0 || num3 < 0 || num3 > int.MaxValue)
		{
			throw new InvalidOperationException();
		}
		if (num2 > _streamEnding - archiveDatabase.StartPositionAfterHeader)
		{
			throw new IndexOutOfRangeException();
		}
		_stream.Seek(num2, SeekOrigin.Current);
		byte[] array = new byte[num3];
		_stream.ReadExact(array, 0, array.Length);
		if (CRC.Finish(CRC.Update(uint.MaxValue, array, 0, array.Length)) != num4)
		{
			throw new InvalidOperationException();
		}
		using (CStreamSwitch cStreamSwitch = default(CStreamSwitch))
		{
			cStreamSwitch.Set(this, array);
			BlockType? blockType = ReadId();
			if (blockType != BlockType.Header)
			{
				if (blockType != BlockType.EncodedHeader)
				{
					throw new InvalidOperationException();
				}
				List<byte[]> list = ReadAndDecodePackedStreams(archiveDatabase.StartPositionAfterHeader, pass);
				if (list.Count == 0)
				{
					archiveDatabase.Fill();
					return archiveDatabase;
				}
				if (list.Count != 1)
				{
					throw new InvalidOperationException();
				}
				cStreamSwitch.Set(this, list[0]);
				if (ReadId() != BlockType.Header)
				{
					throw new InvalidOperationException();
				}
			}
			ReadHeader(archiveDatabase, pass);
		}
		archiveDatabase.Fill();
		return archiveDatabase;
	}

	private Stream GetCachedDecoderStream(ArchiveDatabase _db, int folderIndex, IPasswordProvider pw)
	{
		if (!_cachedStreams.TryGetValue(folderIndex, out var value))
		{
			CFolder cFolder = _db.Folders[folderIndex];
			int firstPackStreamId = _db.Folders[folderIndex].FirstPackStreamId;
			long folderStreamPos = _db.GetFolderStreamPos(cFolder, 0);
			List<long> list = new List<long>();
			for (int i = 0; i < cFolder.PackStreams.Count; i++)
			{
				list.Add(_db.PackSizes[firstPackStreamId + i]);
			}
			value = DecoderStreamHelper.CreateDecoderStream(_stream, folderStreamPos, list.ToArray(), cFolder, pw);
			_cachedStreams.Add(folderIndex, value);
		}
		return value;
	}

	public Stream OpenStream(ArchiveDatabase _db, int fileIndex, IPasswordProvider pw)
	{
		int num = _db.FileIndexToFolderIndexMap[fileIndex];
		int num2 = _db.NumUnpackStreamsVector[num];
		int num3 = _db.FolderStartFileIndex[num];
		if (num3 > fileIndex || fileIndex - num3 >= num2)
		{
			throw new InvalidOperationException();
		}
		int num4 = fileIndex - num3;
		long num5 = 0L;
		for (int i = 0; i < num4; i++)
		{
			num5 += _db.Files[num3 + i].Size;
		}
		Stream cachedDecoderStream = GetCachedDecoderStream(_db, num, pw);
		cachedDecoderStream.Position = num5;
		return new ReadOnlySubStream(cachedDecoderStream, _db.Files[fileIndex].Size);
	}

	public void Extract(ArchiveDatabase _db, int[] indices, IPasswordProvider pw)
	{
		bool flag = indices == null;
		int num = ((!flag) ? indices.Length : _db.Files.Count);
		if (num == 0)
		{
			return;
		}
		List<CExtractFolderInfo> list = new List<CExtractFolderInfo>();
		for (int i = 0; i < num; i++)
		{
			int num2 = (flag ? i : indices[i]);
			int num3 = _db.FileIndexToFolderIndexMap[num2];
			if (num3 == -1)
			{
				list.Add(new CExtractFolderInfo(num2, -1));
				continue;
			}
			if (list.Count == 0 || num3 != list.Last().FolderIndex)
			{
				list.Add(new CExtractFolderInfo(-1, num3));
			}
			CExtractFolderInfo cExtractFolderInfo = list.Last();
			int num4 = _db.FolderStartFileIndex[num3];
			for (int j = cExtractFolderInfo.ExtractStatuses.Count; j <= num2 - num4; j++)
			{
				cExtractFolderInfo.ExtractStatuses.Add(j == num2 - num4);
			}
		}
		foreach (CExtractFolderInfo item in list)
		{
			int startIndex = ((item.FileIndex == -1) ? _db.FolderStartFileIndex[item.FolderIndex] : item.FileIndex);
			FolderUnpackStream folderUnpackStream = new FolderUnpackStream(_db, 0, startIndex, item.ExtractStatuses);
			if (item.FileIndex != -1)
			{
				continue;
			}
			int folderIndex = item.FolderIndex;
			CFolder cFolder = _db.Folders[folderIndex];
			int firstPackStreamId = _db.Folders[folderIndex].FirstPackStreamId;
			long folderStreamPos = _db.GetFolderStreamPos(cFolder, 0);
			List<long> list2 = new List<long>();
			for (int k = 0; k < cFolder.PackStreams.Count; k++)
			{
				list2.Add(_db.PackSizes[firstPackStreamId + k]);
			}
			Stream stream = DecoderStreamHelper.CreateDecoderStream(_stream, folderStreamPos, list2.ToArray(), cFolder, pw);
			byte[] array = new byte[4096];
			while (true)
			{
				int num5 = stream.Read(array, 0, array.Length);
				if (num5 == 0)
				{
					break;
				}
				folderUnpackStream.Write(array, 0, num5);
			}
		}
	}

	public IEnumerable<CFileItem> GetFiles(ArchiveDatabase db)
	{
		return db.Files;
	}

	public int GetFileIndex(ArchiveDatabase db, CFileItem item)
	{
		return db.Files.IndexOf(item);
	}
}
