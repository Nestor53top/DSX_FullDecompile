using System;

namespace HidSharp.Reports.Units;

public struct Unit(uint rawValue) : IEquatable<Unit>
{
	private uint _rawValue = rawValue;

	public UnitSystem System
	{
		get
		{
			return (UnitSystem)GetElement((UnitKind)0);
		}
		set
		{
			SetElement((UnitKind)0, (uint)value);
		}
	}

	public int LengthExponent
	{
		get
		{
			return GetExponent(UnitKind.Length);
		}
		set
		{
			SetExponent(UnitKind.Length, value);
		}
	}

	public LengthUnit LengthUnit => System switch
	{
		UnitSystem.SILinear => LengthUnit.Centimeter, 
		UnitSystem.SIRotation => LengthUnit.Radians, 
		UnitSystem.EnglishLinear => LengthUnit.Inch, 
		UnitSystem.EnglishRotation => LengthUnit.Degrees, 
		_ => LengthUnit.None, 
	};

	public int MassExponent
	{
		get
		{
			return GetExponent(UnitKind.Mass);
		}
		set
		{
			SetExponent(UnitKind.Mass, value);
		}
	}

	public MassUnit MassUnit
	{
		get
		{
			switch (System)
			{
			case UnitSystem.SILinear:
			case UnitSystem.SIRotation:
				return MassUnit.Gram;
			case UnitSystem.EnglishLinear:
			case UnitSystem.EnglishRotation:
				return MassUnit.Slug;
			default:
				return MassUnit.None;
			}
		}
	}

	public int TimeExponent
	{
		get
		{
			return GetExponent(UnitKind.Time);
		}
		set
		{
			SetExponent(UnitKind.Time, value);
		}
	}

	public TimeUnit TimeUnit
	{
		get
		{
			if (System == UnitSystem.None)
			{
				return TimeUnit.None;
			}
			return TimeUnit.Seconds;
		}
	}

	public int TemperatureExponent
	{
		get
		{
			return GetExponent(UnitKind.Temperature);
		}
		set
		{
			SetExponent(UnitKind.Temperature, value);
		}
	}

	public TemperatureUnit TemperatureUnit
	{
		get
		{
			switch (System)
			{
			case UnitSystem.SILinear:
			case UnitSystem.SIRotation:
				return TemperatureUnit.Kelvin;
			case UnitSystem.EnglishLinear:
			case UnitSystem.EnglishRotation:
				return TemperatureUnit.Fahrenheit;
			default:
				return TemperatureUnit.None;
			}
		}
	}

	public int CurrentExponent
	{
		get
		{
			return GetExponent(UnitKind.Current);
		}
		set
		{
			SetExponent(UnitKind.Current, value);
		}
	}

	public CurrentUnit CurrentUnit
	{
		get
		{
			if (System == UnitSystem.None)
			{
				return CurrentUnit.None;
			}
			return CurrentUnit.Ampere;
		}
	}

	public int LuminousIntensityExponent
	{
		get
		{
			return GetExponent(UnitKind.LuminousIntensity);
		}
		set
		{
			SetExponent(UnitKind.LuminousIntensity, value);
		}
	}

	public LuminousIntensityUnit LuminousIntensityUnit
	{
		get
		{
			if (System == UnitSystem.None)
			{
				return LuminousIntensityUnit.None;
			}
			return LuminousIntensityUnit.Candela;
		}
	}

	public uint RawValue
	{
		get
		{
			return _rawValue;
		}
		set
		{
			_rawValue = value;
		}
	}

	public override bool Equals(object obj)
	{
		if (obj is Unit)
		{
			return Equals((Unit)obj);
		}
		return false;
	}

	public bool Equals(Unit other)
	{
		return RawValue == other.RawValue;
	}

	public override int GetHashCode()
	{
		return _rawValue.GetHashCode();
	}

	private uint GetElement(UnitKind kind)
	{
		return (RawValue >> ((int)kind << 2)) & 0xF;
	}

	public int GetExponent(UnitKind kind)
	{
		return DecodeExponent(GetElement(kind));
	}

	public static int DecodeExponent(uint value)
	{
		switch (value)
		{
		default:
			throw new ArgumentOutOfRangeException("value", "Value range is [0, 15].");
		case 0u:
		case 1u:
		case 2u:
		case 3u:
		case 4u:
		case 5u:
		case 6u:
		case 7u:
			return (int)value;
		case 8u:
		case 9u:
		case 10u:
		case 11u:
		case 12u:
		case 13u:
		case 14u:
		case 15u:
			return (int)(value - 16);
		}
	}

	private void SetElement(UnitKind kind, uint value)
	{
		RawValue &= (uint)(15 << ((int)kind << 2));
		RawValue |= (value & 0xF) << ((int)kind << 2);
	}

	public static uint EncodeExponent(int value)
	{
		switch (value)
		{
		default:
			throw new ArgumentOutOfRangeException("value", "Exponent range is [-8, 7].");
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			return (uint)value;
		case -8:
		case -7:
		case -6:
		case -5:
		case -4:
		case -3:
		case -2:
		case -1:
			return (uint)(value + 16);
		}
	}

	public void SetExponent(UnitKind kind, int value)
	{
		SetElement(kind, EncodeExponent(value));
	}

	public static bool operator ==(Unit unit1, Unit unit2)
	{
		return unit1.Equals(unit2);
	}

	public static bool operator !=(Unit unit1, Unit unit2)
	{
		return !unit1.Equals(unit2);
	}
}
