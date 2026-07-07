using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nefarius.ViGEm.Client.Targets.Xbox360;

public abstract class Xbox360Property : IComparable
{
	public string Name { get; }

	public int Id { get; }

	protected Xbox360Property()
	{
	}

	protected Xbox360Property(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int CompareTo(object other)
	{
		return Id.CompareTo(((Xbox360Property)other).Id);
	}

	public override string ToString()
	{
		return Name;
	}

	public static IEnumerable<T> GetAll<T>() where T : Xbox360Property
	{
		return (from f in typeof(T).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public)
			select f.GetValue(null)).Cast<T>();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Xbox360Property xbox360Property))
		{
			return false;
		}
		bool num = GetType() == obj.GetType();
		bool flag = Id.Equals(xbox360Property.Id);
		return num && flag;
	}

	protected bool Equals(Xbox360Property other)
	{
		return Id == other.Id;
	}

	public override int GetHashCode()
	{
		return Id;
	}
}
