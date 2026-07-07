using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HidSharp.Reports.Encodings;

namespace HidSharp.Reports;

public class DescriptorCollectionItem : DescriptorItem
{
	private sealed class ReportCollectionItemChildren : Collection<DescriptorItem>
	{
		private DescriptorCollectionItem _item;

		public ReportCollectionItemChildren(DescriptorCollectionItem item)
		{
			_item = item;
		}

		protected override void ClearItems()
		{
			using (IEnumerator<DescriptorItem> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DescriptorItem current = enumerator.Current;
					current.ParentItem = null;
				}
			}
			base.ClearItems();
		}

		protected override void InsertItem(int index, DescriptorItem item)
		{
			Throw.If.Null(item).False(item.ParentItem == null);
			item.ParentItem = _item;
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			base[index].ParentItem = null;
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, DescriptorItem item)
		{
			throw new NotImplementedException();
		}
	}

	private ReportCollectionItemChildren _children;

	public override IList<DescriptorItem> ChildItems => _children;

	public CollectionType CollectionType { get; set; }

	public DescriptorCollectionItem()
	{
		_children = new ReportCollectionItemChildren(this);
	}
}
