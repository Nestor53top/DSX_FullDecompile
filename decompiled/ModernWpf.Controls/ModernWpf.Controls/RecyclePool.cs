using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;

namespace ModernWpf.Controls;

public class RecyclePool
{
	private class ElementInfo
	{
		public UIElement Element { get; }

		public Panel Owner { get; }

		public ElementInfo(UIElement element, Panel owner)
		{
			Element = element;
			Owner = owner;
		}
	}

	internal static readonly DependencyProperty ReuseKeyProperty = DependencyProperty.RegisterAttached("ReuseKey", typeof(string), typeof(RecyclePool), new PropertyMetadata((object)string.Empty));

	private static readonly AttachableMemberIdentifier PoolInstanceProperty = new AttachableMemberIdentifier(typeof(RecyclePool), "PoolInstance");

	internal static readonly DependencyProperty OriginTemplateProperty = DependencyProperty.RegisterAttached("OriginTemplate", typeof(DataTemplate), typeof(RecyclePool), (PropertyMetadata)null);

	private readonly Dictionary<string, List<ElementInfo>> m_elements = new Dictionary<string, List<ElementInfo>>();

	public void PutElement(UIElement element, string key)
	{
		PutElementCore(element, key, null);
	}

	public void PutElement(UIElement element, string key, UIElement owner)
	{
		PutElementCore(element, key, owner);
	}

	public UIElement TryGetElement(string key)
	{
		return TryGetElementCore(key, null);
	}

	public UIElement TryGetElement(string key, UIElement owner)
	{
		return TryGetElementCore(key, owner);
	}

	protected virtual void PutElementCore(UIElement element, string key, UIElement owner)
	{
		Panel owner2 = EnsureOwnerIsPanelOrNull(owner);
		ElementInfo item = new ElementInfo(element, owner2);
		if (m_elements.TryGetValue(key, out var value))
		{
			value.Add(item);
			return;
		}
		List<ElementInfo> list = new List<ElementInfo>();
		list.Add(item);
		m_elements.Add(key, list);
	}

	protected virtual UIElement TryGetElementCore(string key, UIElement owner)
	{
		if (m_elements.TryGetValue(key, out var value) && value.Count > 0)
		{
			ElementInfo elementInfo = new ElementInfo(null, null);
			UIElement winrtOwner = owner;
			int num = value.FindIndex((ElementInfo elemInfo) => (object)elemInfo.Owner == winrtOwner || elemInfo.Owner == null);
			if (num >= 0)
			{
				elementInfo = value[num];
				value.RemoveAt(num);
			}
			else
			{
				elementInfo = value.Last();
				value.RemoveLast();
			}
			Panel val = EnsureOwnerIsPanelOrNull(winrtOwner);
			if (elementInfo.Owner != null && elementInfo.Owner != val)
			{
				Panel owner2 = elementInfo.Owner;
				if (owner2 != null)
				{
					int num2 = owner2.Children.IndexOf(elementInfo.Element);
					if (num2 < 0)
					{
						throw new Exception("ItemsRepeater's child not found in its Children collection.");
					}
					owner2.Children.RemoveAt(num2);
				}
			}
			return elementInfo.Element;
		}
		return null;
	}

	internal static string GetReuseKey(UIElement element)
	{
		return (string)((DependencyObject)element).GetValue(ReuseKeyProperty);
	}

	internal static void SetReuseKey(UIElement element, string value)
	{
		((DependencyObject)element).SetValue(ReuseKeyProperty, (object)value);
	}

	public static RecyclePool GetPoolInstance(DataTemplate dataTemplate)
	{
		RecyclePool result = default(RecyclePool);
		AttachablePropertyServices.TryGetProperty<RecyclePool>((object)dataTemplate, PoolInstanceProperty, ref result);
		return result;
	}

	public static void SetPoolInstance(DataTemplate dataTemplate, RecyclePool value)
	{
		AttachablePropertyServices.SetProperty((object)dataTemplate, PoolInstanceProperty, (object)value);
	}

	private Panel EnsureOwnerIsPanelOrNull(UIElement owner)
	{
		Panel val = null;
		if (owner != null)
		{
			val = (Panel)(object)((owner is Panel) ? owner : null);
			if (val == null)
			{
				throw new ArgumentException("owner must to be a Panel or null.");
			}
		}
		return val;
	}
}
