using System;
using System.Text;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;

namespace Mono.Cecil.Cil;

internal sealed class PortablePdbWriter : ISymbolWriter, IDisposable
{
	private readonly MetadataBuilder pdb_metadata;

	private readonly ModuleDefinition module;

	private readonly ImageWriter writer;

	private MetadataBuilder module_metadata;

	private bool IsEmbedded => writer == null;

	internal PortablePdbWriter(MetadataBuilder pdb_metadata, ModuleDefinition module)
	{
		this.pdb_metadata = pdb_metadata;
		this.module = module;
		module_metadata = module.metadata_builder;
		if (module_metadata != pdb_metadata)
		{
			this.pdb_metadata.metadata_builder = module_metadata;
		}
		pdb_metadata.AddCustomDebugInformations(module);
	}

	internal PortablePdbWriter(MetadataBuilder pdb_metadata, ModuleDefinition module, ImageWriter writer)
		: this(pdb_metadata, module)
	{
		this.writer = writer;
	}

	public ISymbolReaderProvider GetReaderProvider()
	{
		return new PortablePdbReaderProvider();
	}

	public ImageDebugHeader GetDebugHeader()
	{
		if (IsEmbedded)
		{
			return new ImageDebugHeader();
		}
		ImageDebugDirectory directory = new ImageDebugDirectory
		{
			MajorVersion = 256,
			MinorVersion = 20557,
			Type = ImageDebugType.CodeView,
			TimeDateStamp = (int)module.timestamp
		};
		ByteBuffer byteBuffer = new ByteBuffer();
		byteBuffer.WriteUInt32(1396986706u);
		byteBuffer.WriteBytes(module.Mvid.ToByteArray());
		byteBuffer.WriteUInt32(1u);
		byteBuffer.WriteBytes(Encoding.UTF8.GetBytes(writer.BaseStream.GetFileName()));
		byteBuffer.WriteByte(0);
		byte[] array = new byte[byteBuffer.length];
		Buffer.BlockCopy(byteBuffer.buffer, 0, array, 0, byteBuffer.length);
		directory.SizeOfData = array.Length;
		return new ImageDebugHeader(new ImageDebugHeaderEntry(directory, array));
	}

	public void Write(MethodDebugInformation info)
	{
		CheckMethodDebugInformationTable();
		pdb_metadata.AddMethodDebugInformation(info);
	}

	private void CheckMethodDebugInformationTable()
	{
		MethodDebugInformationTable table = pdb_metadata.table_heap.GetTable<MethodDebugInformationTable>(Table.MethodDebugInformation);
		if (table.length <= 0)
		{
			table.rows = new Row<uint, uint>[module_metadata.method_rid - 1];
			table.length = table.rows.Length;
		}
	}

	public void Dispose()
	{
		if (!IsEmbedded)
		{
			WritePdbFile();
		}
	}

	private void WritePdbFile()
	{
		WritePdbHeap();
		WriteTableHeap();
		writer.BuildMetadataTextMap();
		writer.WriteMetadataHeader();
		writer.WriteMetadata();
		writer.Flush();
		writer.stream.Dispose();
	}

	private void WritePdbHeap()
	{
		PdbHeapBuffer pdb_heap = pdb_metadata.pdb_heap;
		pdb_heap.WriteBytes(module.Mvid.ToByteArray());
		pdb_heap.WriteUInt32(module_metadata.timestamp);
		pdb_heap.WriteUInt32(module_metadata.entry_point.ToUInt32());
		MetadataTable[] tables = module_metadata.table_heap.tables;
		ulong num = 0uL;
		for (int i = 0; i < tables.Length; i++)
		{
			if (tables[i] != null && tables[i].Length != 0)
			{
				num |= (ulong)(1L << i);
			}
		}
		pdb_heap.WriteUInt64(num);
		for (int j = 0; j < tables.Length; j++)
		{
			if (tables[j] != null && tables[j].Length != 0)
			{
				pdb_heap.WriteUInt32((uint)tables[j].Length);
			}
		}
	}

	private void WriteTableHeap()
	{
		pdb_metadata.table_heap.string_offsets = pdb_metadata.string_heap.WriteStrings();
		pdb_metadata.table_heap.ComputeTableInformations();
		pdb_metadata.table_heap.WriteTableHeap();
	}
}
