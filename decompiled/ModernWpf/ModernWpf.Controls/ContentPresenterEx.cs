using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ModernWpf.Controls;

public class ContentPresenterEx : ContentPresenter
{
	public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty LineHeightProperty = Block.LineHeightProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty LineStackingStrategyProperty = Block.LineStackingStrategyProperty.AddOwner(typeof(ContentPresenterEx));

	public static readonly DependencyProperty TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(typeof(ContentPresenterEx), (PropertyMetadata)new FrameworkPropertyMetadata((object)(TextWrapping)1, new PropertyChangedCallback(OnTextWrappingChanged)));

	private TextBlock _textBlock;

	private AccessText _accessText;

	[Localizability(/*Could not decode attribute arguments.*/)]
	public FontFamily FontFamily
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (FontFamily)((DependencyObject)this).GetValue(FontFamilyProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FontFamilyProperty, (object)value);
		}
	}

	public FontStyle FontStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (FontStyle)((DependencyObject)this).GetValue(FontStyleProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FontStyleProperty, (object)value);
		}
	}

	public FontWeight FontWeight
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (FontWeight)((DependencyObject)this).GetValue(FontWeightProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FontWeightProperty, (object)value);
		}
	}

	public FontStretch FontStretch
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (FontStretch)((DependencyObject)this).GetValue(FontStretchProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FontStretchProperty, (object)value);
		}
	}

	[TypeConverter(typeof(FontSizeConverter))]
	[Localizability(/*Could not decode attribute arguments.*/)]
	public double FontSize
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(FontSizeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FontSizeProperty, (object)value);
		}
	}

	public Brush Foreground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(ForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ForegroundProperty, (object)value);
		}
	}

	[TypeConverter(typeof(LengthConverter))]
	public double LineHeight
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(LineHeightProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(LineHeightProperty, (object)value);
		}
	}

	public LineStackingStrategy LineStackingStrategy
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (LineStackingStrategy)((DependencyObject)this).GetValue(LineStackingStrategyProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(LineStackingStrategyProperty, (object)value);
		}
	}

	public TextWrapping TextWrapping
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (TextWrapping)((DependencyObject)this).GetValue(TextWrappingProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(TextWrappingProperty, (object)value);
		}
	}

	private bool IsUsingDefaultTemplate { get; set; }

	private TextBlock TextBlock
	{
		get
		{
			return _textBlock;
		}
		set
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (_textBlock != null)
			{
				((DependencyObject)_textBlock).ClearValue(TextBlock.TextWrappingProperty);
			}
			_textBlock = value;
			if (_textBlock != null)
			{
				_textBlock.TextWrapping = TextWrapping;
			}
		}
	}

	private AccessText AccessText
	{
		get
		{
			return _accessText;
		}
		set
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (_accessText != null)
			{
				((DependencyObject)_accessText).ClearValue(AccessText.TextWrappingProperty);
			}
			_accessText = value;
			if (_accessText != null)
			{
				_accessText.TextWrapping = TextWrapping;
			}
		}
	}

	private static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ContentPresenterEx contentPresenterEx = (ContentPresenterEx)(object)d;
		if (contentPresenterEx.TextBlock != null)
		{
			contentPresenterEx.TextBlock.TextWrapping = (TextWrapping)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
		else if (contentPresenterEx.AccessText != null)
		{
			contentPresenterEx.AccessText.TextWrapping = (TextWrapping)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		}
	}

	protected override DataTemplate ChooseTemplate()
	{
		DataTemplate val = null;
		object content = ((ContentPresenter)this).Content;
		val = ((ContentPresenter)this).ContentTemplate;
		if (val == null && ((ContentPresenter)this).ContentTemplateSelector != null)
		{
			val = ((ContentPresenter)this).ContentTemplateSelector.SelectTemplate(content, (DependencyObject)(object)this);
		}
		if (val == null)
		{
			val = ((ContentPresenter)this).ChooseTemplate();
			IsUsingDefaultTemplate = true;
		}
		else
		{
			IsUsingDefaultTemplate = false;
		}
		return val;
	}

	protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
	{
		((Visual)this).OnVisualChildrenChanged(visualAdded, visualRemoved);
		if (visualAdded != null && IsUsingDefaultTemplate)
		{
			TextBlock val = (TextBlock)(object)((visualAdded is TextBlock) ? visualAdded : null);
			if (val != null)
			{
				TextBlock = val;
				return;
			}
			AccessText val2 = (AccessText)(object)((visualAdded is AccessText) ? visualAdded : null);
			if (val2 != null)
			{
				AccessText = val2;
			}
		}
		else if (visualRemoved != null)
		{
			if ((object)visualRemoved == TextBlock)
			{
				TextBlock = null;
			}
			else if ((object)visualRemoved == AccessText)
			{
				AccessText = null;
			}
		}
	}
}
