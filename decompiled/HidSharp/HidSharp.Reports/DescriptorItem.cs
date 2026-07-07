using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HidSharp.Reports;

public class DescriptorItem
{
	private static readonly IList<DescriptorItem> _noChildren = new ReadOnlyCollection<DescriptorItem>(new DescriptorItem[0]);

	private Indexes _designator;

	private Indexes _string;

	private Indexes _usage;

	public virtual IList<DescriptorItem> ChildItems => _noChildren;

	public DescriptorCollectionItem ParentItem { get; internal set; }

	public Indexes Designators
	{
		get
		{
			return _designator ?? Indexes.Unset;
		}
		set
		{
			_designator = value;
		}
	}

	public Indexes Strings
	{
		get
		{
			return _string ?? Indexes.Unset;
		}
		set
		{
			_string = value;
		}
	}

	public Indexes Usages
	{
		get
		{
			return _usage ?? Indexes.Unset;
		}
		set
		{
			_usage = value;
		}
	}
}
