using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ModernWpf.Controls;

public class RecyclingElementFactory : ElementFactory
{
	private Dictionary<string, DataTemplate> m_templates;

	private SelectTemplateEventArgs m_args;

	public RecyclePool RecyclePool { get; set; }

	public Dictionary<string, DataTemplate> Templates
	{
		get
		{
			return m_templates;
		}
		set
		{
			m_templates = value;
		}
	}

	public event TypedEventHandler<RecyclingElementFactory, SelectTemplateEventArgs> SelectTemplateKey;

	public RecyclingElementFactory()
	{
		m_templates = new Dictionary<string, DataTemplate>();
	}

	protected virtual string OnSelectTemplateKeyCore(object dataContext, UIElement owner)
	{
		if (m_args == null)
		{
			m_args = new SelectTemplateEventArgs();
		}
		SelectTemplateEventArgs args = m_args;
		args.TemplateKey = string.Empty;
		args.DataContext = dataContext;
		args.Owner = owner;
		this.SelectTemplateKey?.Invoke(this, args);
		string templateKey = args.TemplateKey;
		if (string.IsNullOrEmpty(templateKey))
		{
			throw new InvalidOperationException("Please provide a valid template identifier in the handler for the SelectTemplateKey event.");
		}
		return templateKey;
	}

	protected override UIElement GetElementCore(ElementFactoryGetArgs args)
	{
		if (m_templates == null || m_templates.Count == 0)
		{
			throw new InvalidOperationException("Templates property cannot be null or empty.");
		}
		UIElement parent = args.Parent;
		string text = ((m_templates.Count == 1) ? m_templates.First().Key : OnSelectTemplateKeyCore(args.Data, parent));
		if (string.IsNullOrEmpty(text))
		{
			throw new InvalidOperationException("Template key cannot be empty or null.");
		}
		UIElement obj = RecyclePool.TryGetElement(text, parent);
		FrameworkElement val = (FrameworkElement)(object)((obj is FrameworkElement) ? obj : null);
		if (val == null)
		{
			if (m_templates.Count > 1 && !m_templates.ContainsKey(text))
			{
				throw new InvalidOperationException("No templates of key " + text + " were found in the templates collection.");
			}
			DependencyObject obj2 = ((FrameworkTemplate)m_templates[text]).LoadContent();
			val = (FrameworkElement)(object)((obj2 is FrameworkElement) ? obj2 : null);
			RecyclePool.SetReuseKey((UIElement)(object)val, text);
		}
		return (UIElement)(object)val;
	}

	protected override void RecycleElementCore(ElementFactoryRecycleArgs args)
	{
		UIElement element = args.Element;
		string reuseKey = RecyclePool.GetReuseKey(element);
		RecyclePool.PutElement(element, reuseKey, args.Parent);
	}
}
