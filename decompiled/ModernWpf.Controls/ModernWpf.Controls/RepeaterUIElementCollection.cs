using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls;

internal class RepeaterUIElementCollection : UIElementCollection
{
	private readonly VisualCollection _visualChildren;

	private readonly UIElement _visualParent;

	private readonly FrameworkElement _logicalParent;

	public override int Count => _visualChildren.Count;

	public override bool IsSynchronized => _visualChildren.IsSynchronized;

	public override object SyncRoot => _visualChildren.SyncRoot;

	public override int Capacity
	{
		get
		{
			return _visualChildren.Capacity;
		}
		set
		{
			_visualChildren.Capacity = value;
		}
	}

	public override UIElement this[int index]
	{
		get
		{
			Visual obj = _visualChildren[index];
			return (UIElement)(object)((obj is UIElement) ? obj : null);
		}
		set
		{
			ValidateElement(value);
			VisualCollection visualChildren = _visualChildren;
			if ((object)visualChildren[index] != value)
			{
				Visual obj = visualChildren[index];
				UIElement val = (UIElement)(object)((obj is UIElement) ? obj : null);
				if (val != null)
				{
					((UIElementCollection)this).ClearLogicalParent(val);
				}
				visualChildren[index] = (Visual)(object)value;
				((UIElementCollection)this).SetLogicalParent(value);
			}
		}
	}

	public RepeaterUIElementCollection(UIElement visualParent, FrameworkElement logicalParent)
		: base(visualParent, logicalParent)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		_visualChildren = new VisualCollection((Visual)(object)visualParent);
		_visualParent = visualParent;
		_logicalParent = logicalParent;
	}

	public override void CopyTo(Array array, int index)
	{
		_visualChildren.CopyTo(array, index);
	}

	public override void CopyTo(UIElement[] array, int index)
	{
		_visualChildren.CopyTo((Visual[])(object)array, index);
	}

	public override int Add(UIElement element)
	{
		return AddInternal(element);
	}

	internal int AddInternal(UIElement element)
	{
		ValidateElement(element);
		((UIElementCollection)this).SetLogicalParent(element);
		return _visualChildren.Add((Visual)(object)element);
	}

	public override int IndexOf(UIElement element)
	{
		return _visualChildren.IndexOf((Visual)(object)element);
	}

	public override void Remove(UIElement element)
	{
		RemoveInternal(element);
	}

	internal void RemoveInternal(UIElement element)
	{
		_visualChildren.Remove((Visual)(object)element);
		((UIElementCollection)this).ClearLogicalParent(element);
	}

	public override bool Contains(UIElement element)
	{
		return _visualChildren.Contains((Visual)(object)element);
	}

	public override void Clear()
	{
		ClearInternal();
	}

	internal void ClearInternal()
	{
		VisualCollection visualChildren = _visualChildren;
		int count = visualChildren.Count;
		if (count <= 0)
		{
			return;
		}
		Visual[] array = (Visual[])(object)new Visual[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = visualChildren[i];
		}
		visualChildren.Clear();
		for (int j = 0; j < count; j++)
		{
			Visual obj = array[j];
			UIElement val = (UIElement)(object)((obj is UIElement) ? obj : null);
			if (val != null)
			{
				((UIElementCollection)this).ClearLogicalParent(val);
			}
		}
	}

	public override void Insert(int index, UIElement element)
	{
		InsertInternal(index, element);
	}

	internal void InsertInternal(int index, UIElement element)
	{
		ValidateElement(element);
		((UIElementCollection)this).SetLogicalParent(element);
		_visualChildren.Insert(index, (Visual)(object)element);
	}

	public override void RemoveAt(int index)
	{
		VisualCollection visualChildren = _visualChildren;
		Visual obj = visualChildren[index];
		UIElement val = (UIElement)(object)((obj is UIElement) ? obj : null);
		visualChildren.RemoveAt(index);
		if (val != null)
		{
			((UIElementCollection)this).ClearLogicalParent(val);
		}
	}

	public override void RemoveRange(int index, int count)
	{
		RemoveRangeInternal(index, count);
	}

	internal void RemoveRangeInternal(int index, int count)
	{
		VisualCollection visualChildren = _visualChildren;
		int count2 = visualChildren.Count;
		if (count > count2 - index)
		{
			count = count2 - index;
		}
		if (count <= 0)
		{
			return;
		}
		Visual[] array = (Visual[])(object)new Visual[count];
		int num = index;
		for (int i = 0; i < count; i++)
		{
			array[i] = visualChildren[num];
			num++;
		}
		visualChildren.RemoveRange(index, count);
		for (num = 0; num < count; num++)
		{
			Visual obj = array[num];
			UIElement val = (UIElement)(object)((obj is UIElement) ? obj : null);
			if (val != null)
			{
				((UIElementCollection)this).ClearLogicalParent(val);
			}
		}
	}

	private void ValidateElement(UIElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException($"Children of '{((object)this).GetType()}' cannot be null. Object derived from UIElement expected.");
		}
	}

	public override IEnumerator GetEnumerator()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return (IEnumerator)(object)_visualChildren.GetEnumerator();
	}
}
