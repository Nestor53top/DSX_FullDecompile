using System;
using System.Collections.Generic;

namespace ModernWpf.Controls;

internal class SplitDataSourceBase<T, SplitVectorID, AttachedDataType> where SplitVectorID : Enum
{
	private static readonly int SplitVectorSize = Enum.GetNames(typeof(SplitVectorID)).Length;

	private List<SplitVectorID> m_flags = new List<SplitVectorID>();

	private List<AttachedDataType> m_attachedData = new List<AttachedDataType>();

	private SplitVector<T, SplitVectorID>[] m_splitVectors = new SplitVector<T, SplitVectorID>[SplitVectorSize];

	public SplitVectorID GetVectorIDForItem(int index)
	{
		return m_flags[index];
	}

	public AttachedDataType AttachedData(int index)
	{
		return m_attachedData[index];
	}

	public void AttachedData(int index, AttachedDataType attachedData)
	{
		m_attachedData[index] = attachedData;
	}

	public void ResetAttachedData()
	{
		ResetAttachedData(DefaultAttachedData());
	}

	public void ResetAttachedData(AttachedDataType attachedData)
	{
		for (int i = 0; i < RawDataSize(); i++)
		{
			m_attachedData[i] = attachedData;
		}
	}

	public SplitVector<T, SplitVectorID> GetVectorForItem(int index)
	{
		if (index >= 0 && index < RawDataSize())
		{
			return m_splitVectors[Convert.ToInt32(m_flags[index])];
		}
		return null;
	}

	public void MoveItemsToVector(SplitVectorID newVectorID)
	{
		MoveItemsToVector(0, RawDataSize(), newVectorID);
	}

	public void MoveItemsToVector(int start, int end, SplitVectorID newVectorID)
	{
		for (int i = start; i < end; i++)
		{
			MoveItemToVector(i, newVectorID);
		}
	}

	public void MoveItemToVector(int index, SplitVectorID newVectorID)
	{
		if (!object.Equals(m_flags[index], newVectorID))
		{
			GetVectorForItem(index)?.RemoveAt(index);
			m_flags[index] = newVectorID;
			SplitVector<T, SplitVectorID> splitVector = m_splitVectors[Convert.ToInt32(newVectorID)];
			if (splitVector != null)
			{
				int preferIndex = GetPreferIndex(index, newVectorID);
				T at = GetAt(index);
				splitVector.InsertAt(preferIndex, index, at);
			}
		}
	}

	internal virtual int IndexOf(T value)
	{
		return 0;
	}

	internal virtual T GetAt(int index)
	{
		return default(T);
	}

	internal virtual int Size()
	{
		return 0;
	}

	internal virtual SplitVectorID DefaultVectorIDOnInsert()
	{
		return default(SplitVectorID);
	}

	internal virtual AttachedDataType DefaultAttachedData()
	{
		return default(AttachedDataType);
	}

	public int IndexOfImpl(T value, SplitVectorID vectorID)
	{
		int num = IndexOf(value);
		int result = -1;
		if (num != -1)
		{
			SplitVector<T, SplitVectorID> vectorForItem = GetVectorForItem(num);
			if (vectorForItem != null && object.Equals(vectorForItem.GetVectorIDForItem(), vectorID))
			{
				result = vectorForItem.IndexFromIndexInOriginalVector(num);
			}
		}
		return result;
	}

	public void InitializeSplitVectors(List<SplitVector<T, SplitVectorID>> vectors)
	{
		foreach (SplitVector<T, SplitVectorID> vector in vectors)
		{
			m_splitVectors[Convert.ToInt32(vector.GetVectorIDForItem())] = vector;
		}
	}

	public SplitVector<T, SplitVectorID> GetVector(SplitVectorID vectorID)
	{
		return m_splitVectors[Convert.ToInt32(vectorID)];
	}

	public void OnClear()
	{
		SplitVector<T, SplitVectorID>[] splitVectors = m_splitVectors;
		for (int i = 0; i < splitVectors.Length; i++)
		{
			splitVectors[i]?.Clear();
		}
		m_flags.Clear();
		m_attachedData.Clear();
	}

	public void OnRemoveAt(int startIndex, int count)
	{
		for (int num = startIndex + count - 1; num >= startIndex; num--)
		{
			OnRemoveAt(num);
		}
	}

	public void OnInsertAt(int startIndex, int count)
	{
		for (int i = startIndex; i < startIndex + count; i++)
		{
			OnInsertAt(i);
		}
	}

	public int RawDataSize()
	{
		return m_flags.Count;
	}

	public void SyncAndInitVectorFlagsWithID(SplitVectorID defaultID, AttachedDataType defaultAttachedData)
	{
		for (int i = 0; i < Size(); i++)
		{
			m_flags.Add(defaultID);
			m_attachedData.Add(defaultAttachedData);
		}
	}

	public void Clear()
	{
		OnClear();
	}

	private void OnRemoveAt(int index)
	{
		SplitVectorID vectorID = m_flags[index];
		SplitVector<T, SplitVectorID>[] splitVectors = m_splitVectors;
		for (int i = 0; i < splitVectors.Length; i++)
		{
			splitVectors[i]?.OnRawDataRemove(index, vectorID);
		}
		m_flags.RemoveAt(index);
		m_attachedData.RemoveAt(index);
	}

	private void OnReplace(int index)
	{
		SplitVector<T, SplitVectorID> vectorForItem = GetVectorForItem(index);
		if (vectorForItem != null)
		{
			T at = GetAt(index);
			vectorForItem.Replace(index, at);
		}
	}

	private void OnInsertAt(int index)
	{
		SplitVectorID val = DefaultVectorIDOnInsert();
		AttachedDataType item = DefaultAttachedData();
		int preferIndex = GetPreferIndex(index, val);
		T at = GetAt(index);
		SplitVector<T, SplitVectorID>[] splitVectors = m_splitVectors;
		for (int i = 0; i < splitVectors.Length; i++)
		{
			splitVectors[i]?.OnRawDataInsert(preferIndex, index, at, val);
		}
		m_flags.Insert(index, val);
		m_attachedData.Insert(index, item);
	}

	private int GetPreferIndex(int index, SplitVectorID vectorID)
	{
		return RangeCount(0, index, vectorID);
	}

	private int RangeCount(int start, int end, SplitVectorID vectorID)
	{
		int num = 0;
		for (int i = start; i < end; i++)
		{
			if (object.Equals(m_flags[i], vectorID))
			{
				num++;
			}
		}
		return num;
	}
}
