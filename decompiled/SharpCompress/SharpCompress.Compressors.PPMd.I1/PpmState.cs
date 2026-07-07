namespace SharpCompress.Compressors.PPMd.I1;

internal struct PpmState(uint address, byte[] memory)
{
	public uint Address = address;

	public byte[] Memory = memory;

	public static readonly PpmState Zero = new PpmState(0u, null);

	public const int Size = 6;

	public byte Symbol
	{
		get
		{
			return Memory[Address];
		}
		set
		{
			Memory[Address] = value;
		}
	}

	public byte Frequency
	{
		get
		{
			return Memory[Address + 1];
		}
		set
		{
			Memory[Address + 1] = value;
		}
	}

	public Model.PpmContext Successor
	{
		get
		{
			return new Model.PpmContext((uint)(Memory[Address + 2] | (Memory[Address + 3] << 8) | (Memory[Address + 4] << 16) | (Memory[Address + 5] << 24)), Memory);
		}
		set
		{
			Memory[Address + 2] = (byte)value.Address;
			Memory[Address + 3] = (byte)(value.Address >> 8);
			Memory[Address + 4] = (byte)(value.Address >> 16);
			Memory[Address + 5] = (byte)(value.Address >> 24);
		}
	}

	public PpmState this[int offset] => new PpmState((uint)(Address + offset * 6), Memory);

	public static implicit operator PpmState(Pointer pointer)
	{
		return new PpmState(pointer.Address, pointer.Memory);
	}

	public static PpmState operator +(PpmState state, int offset)
	{
		state.Address = (uint)(state.Address + offset * 6);
		return state;
	}

	public static PpmState operator ++(PpmState state)
	{
		state.Address += 6u;
		return state;
	}

	public static PpmState operator -(PpmState state, int offset)
	{
		state.Address = (uint)(state.Address - offset * 6);
		return state;
	}

	public static PpmState operator --(PpmState state)
	{
		state.Address -= 6u;
		return state;
	}

	public static bool operator <=(PpmState state1, PpmState state2)
	{
		return state1.Address <= state2.Address;
	}

	public static bool operator >=(PpmState state1, PpmState state2)
	{
		return state1.Address >= state2.Address;
	}

	public static bool operator ==(PpmState state1, PpmState state2)
	{
		return state1.Address == state2.Address;
	}

	public static bool operator !=(PpmState state1, PpmState state2)
	{
		return state1.Address != state2.Address;
	}

	public override bool Equals(object obj)
	{
		if (obj is PpmState)
		{
			return ((PpmState)obj).Address == Address;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return Address.GetHashCode();
	}
}
