using System.Collections.Generic;
using HidSharp.Reports.Encodings;

namespace HidSharp.Reports;

internal sealed class ReportDescriptorParseState
{
	public DescriptorCollectionItem CurrentCollectionItem { get; set; }

	public DescriptorCollectionItem RootItem { get; private set; }

	public IDictionary<GlobalItemTag, EncodedItem> GlobalItemState => GlobalItemStateStack[GlobalItemStateStack.Count - 1];

	public IList<IDictionary<GlobalItemTag, EncodedItem>> GlobalItemStateStack { get; private set; }

	public IList<KeyValuePair<LocalItemTag, uint>> LocalItemState { get; private set; }

	public ReportDescriptorParseState()
	{
		RootItem = new DescriptorCollectionItem();
		GlobalItemStateStack = new List<IDictionary<GlobalItemTag, EncodedItem>>();
		LocalItemState = new List<KeyValuePair<LocalItemTag, uint>>();
		Reset();
	}

	public void Reset()
	{
		CurrentCollectionItem = RootItem;
		RootItem.ChildItems.Clear();
		RootItem.CollectionType = CollectionType.Physical;
		GlobalItemStateStack.Clear();
		GlobalItemStateStack.Add(new Dictionary<GlobalItemTag, EncodedItem>());
		LocalItemState.Clear();
	}

	public EncodedItem GetGlobalItem(GlobalItemTag tag)
	{
		GlobalItemState.TryGetValue(tag, out var value);
		return value;
	}

	public uint GetGlobalItemValue(GlobalItemTag tag)
	{
		return GetGlobalItem(tag)?.DataValue ?? 0;
	}

	public bool IsGlobalItemSet(GlobalItemTag tag)
	{
		return GlobalItemState.ContainsKey(tag);
	}
}
