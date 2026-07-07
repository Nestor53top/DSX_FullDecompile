using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nefarius.ViGEm.Client.Targets.DualShock4;

public abstract class DualShock4Property : IComparable
{
	public string Name { get; }

	public int Id { get; }

	protected DualShock4Property()
	{
	}

	protected DualShock4Property(int id, string name)
	{
		Id = id;
		Name = name;
	}

	public int CompareTo(object other)
	{
		return Id.CompareTo(((DualShock4Property)other).Id);
	}

	public override string ToString()
	{
		return Name;
	}

	public static IEnumerable<T> GetAll<T>() where T : DualShock4Property
	{
		return (from f in typeof(T).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public)
			select f.GetValue(null)).Cast<T>();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DualShock4Property dualShock4Property))
		{
			return false;
		}
		bool num = GetType() == obj.GetType();
		bool flag = Id.Equals(dualShock4Property.Id);
		return num && flag;
	}

	protected bool Equals(DualShock4Property other)
	{
		return Id == other.Id;
	}

	public override int GetHashCode()
	{
		return Id;
	}
}
