using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public class DataGridCellPresenter : ContentPresenter
{
	private class BorderHelper
	{
		private readonly UIElement _owner;

		private Size RenderSize => _owner.RenderSize;

		private Pen PenCache { get; set; }

		public BorderHelper(UIElement owner)
		{
			_owner = owner;
		}

		public void ClearPenCache()
		{
			PenCache = null;
		}

		public void DrawBorder(DrawingContext dc, Brush brush, double thickness, double margin = 0.0)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			if (!(thickness > 0.0) || brush == null)
			{
				return;
			}
			Pen val = PenCache;
			if (val == null)
			{
				val = new Pen(brush, thickness);
				if (((Freezable)brush).IsFrozen)
				{
					((Freezable)val).Freeze();
				}
				PenCache = val;
			}
			double num = thickness * 0.5;
			Point val2 = new Point(margin + num, margin + num);
			Size renderSize = RenderSize;
			double num2 = ((Size)(ref renderSize)).Width - margin - num;
			renderSize = RenderSize;
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(val2, new Point(num2, ((Size)(ref renderSize)).Height - margin - num));
			dc.DrawRectangle((Brush)null, val, val3);
		}
	}

	public static readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, (FrameworkPropertyMetadataOptions)2064));

	public static readonly DependencyProperty CurrencyVisualBrushProperty = DependencyProperty.Register("CurrencyVisualBrush", typeof(Brush), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, (FrameworkPropertyMetadataOptions)16));

	public static readonly DependencyProperty CurrencyVisualThicknessProperty = DependencyProperty.Register("CurrencyVisualThickness", typeof(double), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.0, (FrameworkPropertyMetadataOptions)16, new PropertyChangedCallback(OnCurrencyVisualThicknessChanged)));

	public static readonly DependencyProperty FocusVisualPrimaryBrushProperty = DependencyProperty.Register("FocusVisualPrimaryBrush", typeof(Brush), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, (FrameworkPropertyMetadataOptions)16));

	public static readonly DependencyProperty FocusVisualPrimaryThicknessProperty = DependencyProperty.Register("FocusVisualPrimaryThickness", typeof(double), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.0, (FrameworkPropertyMetadataOptions)16, new PropertyChangedCallback(OnFocusVisualPrimaryThicknessChanged)));

	public static readonly DependencyProperty FocusVisualSecondaryBrushProperty = DependencyProperty.Register("FocusVisualSecondaryBrush", typeof(Brush), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, (FrameworkPropertyMetadataOptions)16));

	public static readonly DependencyProperty FocusVisualSecondaryThicknessProperty = DependencyProperty.Register("FocusVisualSecondaryThickness", typeof(double), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.0, (FrameworkPropertyMetadataOptions)16, new PropertyChangedCallback(OnFocusVisualSecondaryThicknessChanged)));

	public static readonly DependencyProperty IsCurrencyVisualVisibleProperty = DependencyProperty.Register("IsCurrencyVisualVisible", typeof(bool), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (FrameworkPropertyMetadataOptions)16));

	public static readonly DependencyProperty IsFocusVisualVisibleProperty = DependencyProperty.Register("IsFocusVisualVisible", typeof(bool), typeof(DataGridCellPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (FrameworkPropertyMetadataOptions)16));

	private readonly BorderHelper _currencyVisualHelper;

	private readonly BorderHelper _focusVisualPrimaryHelper;

	private readonly BorderHelper _focusVisualSecondaryHelper;

	public Brush Background
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(BackgroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BackgroundProperty, (object)value);
		}
	}

	public Brush CurrencyVisualBrush
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(CurrencyVisualBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CurrencyVisualBrushProperty, (object)value);
		}
	}

	public double CurrencyVisualThickness
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(CurrencyVisualThicknessProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CurrencyVisualThicknessProperty, (object)value);
		}
	}

	public Brush FocusVisualPrimaryBrush
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(FocusVisualPrimaryBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FocusVisualPrimaryBrushProperty, (object)value);
		}
	}

	public double FocusVisualPrimaryThickness
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(FocusVisualPrimaryThicknessProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FocusVisualPrimaryThicknessProperty, (object)value);
		}
	}

	public Brush FocusVisualSecondaryBrush
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(FocusVisualSecondaryBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FocusVisualSecondaryBrushProperty, (object)value);
		}
	}

	public double FocusVisualSecondaryThickness
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(FocusVisualSecondaryThicknessProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FocusVisualSecondaryThicknessProperty, (object)value);
		}
	}

	public bool IsCurrencyVisualVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsCurrencyVisualVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsCurrencyVisualVisibleProperty, (object)value);
		}
	}

	public bool IsFocusVisualVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsFocusVisualVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsFocusVisualVisibleProperty, (object)value);
		}
	}

	public DataGridCellPresenter()
	{
		_currencyVisualHelper = new BorderHelper((UIElement)(object)this);
		_focusVisualPrimaryHelper = new BorderHelper((UIElement)(object)this);
		_focusVisualSecondaryHelper = new BorderHelper((UIElement)(object)this);
	}

	private static void OnCurrencyVisualThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((DataGridCellPresenter)(object)d)._currencyVisualHelper.ClearPenCache();
	}

	private static void OnFocusVisualPrimaryThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((DataGridCellPresenter)(object)d)._focusVisualPrimaryHelper.ClearPenCache();
	}

	private static void OnFocusVisualSecondaryThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((DataGridCellPresenter)(object)d)._focusVisualSecondaryHelper.ClearPenCache();
	}

	protected override void OnRender(DrawingContext dc)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Brush background = Background;
		if (background != null)
		{
			dc.DrawRectangle(background, (Pen)null, new Rect(((UIElement)this).RenderSize));
		}
		((UIElement)this).OnRender(dc);
		if (IsCurrencyVisualVisible)
		{
			_currencyVisualHelper.DrawBorder(dc, CurrencyVisualBrush, CurrencyVisualThickness);
		}
		if (IsFocusVisualVisible)
		{
			double focusVisualPrimaryThickness = FocusVisualPrimaryThickness;
			_focusVisualPrimaryHelper.DrawBorder(dc, FocusVisualPrimaryBrush, focusVisualPrimaryThickness);
			_focusVisualSecondaryHelper.DrawBorder(dc, FocusVisualSecondaryBrush, FocusVisualSecondaryThickness, focusVisualPrimaryThickness);
		}
	}
}
