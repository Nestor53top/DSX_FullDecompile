using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ModernWpf.Controls;

internal class ItemTemplateWrapper : IElementFactoryShim
{
	public DataTemplate Template { get; set; }

	public DataTemplateSelector TemplateSelector { get; set; }

	public ItemTemplateWrapper(DataTemplate dataTemplate)
	{
		Template = dataTemplate;
	}

	public ItemTemplateWrapper(DataTemplateSelector dataTemplateSelector)
	{
		TemplateSelector = dataTemplateSelector;
	}

	public UIElement GetElement(ElementFactoryGetArgs args)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		DataTemplate val = Template ?? TemplateSelector.SelectTemplate(args.Data, (DependencyObject)null);
		if (val == null)
		{
			val = TemplateSelector.SelectTemplate(args.Data, (DependencyObject)null);
			if (val == null)
			{
				throw new InvalidOperationException("Null encountered as data template. That is not a valid value for a data template, and can not be used.");
			}
		}
		RecyclePool poolInstance = RecyclePool.GetPoolInstance(val);
		UIElement val2 = null;
		if (poolInstance != null)
		{
			string empty = string.Empty;
			UIElement parent = args.Parent;
			val2 = poolInstance.TryGetElement(empty, (parent is FrameworkElement) ? parent : null);
		}
		if (val2 == null)
		{
			DependencyObject obj = ((FrameworkTemplate)val).LoadContent();
			val2 = (UIElement)(object)((obj is FrameworkElement) ? obj : null);
			if (val2 == null)
			{
				val2 = (UIElement)new Rectangle
				{
					Width = 0.0,
					Height = 0.0
				};
			}
			((DependencyObject)val2).SetValue(RecyclePool.OriginTemplateProperty, (object)val);
		}
		return val2;
	}

	public void RecycleElement(ElementFactoryRecycleArgs args)
	{
		UIElement element = args.Element;
		object obj = Template;
		if (obj == null)
		{
			object value = ((DependencyObject)element).GetValue(RecyclePool.OriginTemplateProperty);
			obj = ((value is DataTemplate) ? value : null);
		}
		DataTemplate dataTemplate = (DataTemplate)obj;
		RecyclePool recyclePool = RecyclePool.GetPoolInstance(dataTemplate);
		if (recyclePool == null)
		{
			recyclePool = new RecyclePool();
			RecyclePool.SetPoolInstance(dataTemplate, recyclePool);
		}
		recyclePool.PutElement(args.Element, string.Empty, args.Parent);
	}
}
