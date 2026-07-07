using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

internal class RadioButtonsElementFactory : ElementFactory
{
	private IElementFactoryShim m_itemTemplateWrapper;

	internal void UserElementFactory(object newValue)
	{
		m_itemTemplateWrapper = newValue as IElementFactoryShim;
		if (m_itemTemplateWrapper == null)
		{
			DataTemplate val = (DataTemplate)((newValue is DataTemplate) ? newValue : null);
			if (val != null)
			{
				m_itemTemplateWrapper = new ItemTemplateWrapper(val);
			}
		}
	}

	protected override UIElement GetElementCore(ElementFactoryGetArgs args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		object obj = ((m_itemTemplateWrapper == null) ? args.Data : m_itemTemplateWrapper.GetElement(args));
		RadioButton val = (RadioButton)((obj is RadioButton) ? obj : null);
		if (val != null)
		{
			return (UIElement)(object)val;
		}
		RadioButton val2 = new RadioButton();
		((ContentControl)val2).Content = args.Data;
		if (m_itemTemplateWrapper is ItemTemplateWrapper itemTemplateWrapper)
		{
			((ContentControl)val2).ContentTemplate = itemTemplateWrapper.Template;
		}
		return (UIElement)(object)val2;
	}

	protected override void RecycleElementCore(ElementFactoryRecycleArgs args)
	{
	}
}
