using System;
using System.Collections.Generic;
using System.IO;

namespace SharpCompress.Compressors.Xz.Filters;

internal abstract class BlockFilter : ReadOnlyStream
{
	public enum FilterTypes : ulong
	{
		DELTA = 3uL,
		ARCH_x86_FILTER = 4uL,
		ARCH_PowerPC_FILTER = 5uL,
		ARCH_IA64_FILTER = 6uL,
		ARCH_ARM_FILTER = 7uL,
		ARCH_ARMTHUMB_FILTER = 8uL,
		ARCH_SPARC_FILTER = 9uL,
		LZMA2 = 33uL
	}

	private static Dictionary<FilterTypes, Type> FilterMap = new Dictionary<FilterTypes, Type> { 
	{
		FilterTypes.LZMA2,
		typeof(Lzma2Filter)
	} };

	public abstract bool AllowAsLast { get; }

	public abstract bool AllowAsNonLast { get; }

	public abstract bool ChangesDataSize { get; }

	public FilterTypes FilterType { get; set; }

	public BlockFilter()
	{
	}

	public abstract void Init(byte[] properties);

	public abstract void ValidateFilter();

	public static BlockFilter Read(BinaryReader reader)
	{
		FilterTypes filterTypes = (FilterTypes)reader.ReadXZInteger();
		if (!FilterMap.ContainsKey(filterTypes))
		{
			throw new NotImplementedException($"Filter {filterTypes} has not yet been implemented");
		}
		BlockFilter obj = Activator.CreateInstance(FilterMap[filterTypes]) as BlockFilter;
		ulong num = reader.ReadXZInteger();
		if (num > int.MaxValue)
		{
			throw new InvalidDataException("Block filter information too large");
		}
		byte[] properties = reader.ReadBytes((int)num);
		obj.Init(properties);
		return obj;
	}

	public abstract void SetBaseStream(Stream stream);
}
