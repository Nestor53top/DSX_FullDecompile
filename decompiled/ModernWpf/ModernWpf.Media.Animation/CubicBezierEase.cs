using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ModernWpf.Media.Animation;

public class CubicBezierEase : EasingFunctionBase
{
	public static readonly DependencyProperty ControlPoint1Property;

	public static readonly DependencyProperty ControlPoint2Property;

	private bool _isSpecified;

	private bool _isDirty;

	private double _parameter;

	private double _Bx;

	private double _Cx;

	private double _Cx_Bx;

	private double _three_Cx;

	private double _By;

	private double _Cy;

	private const double accuracy = 0.001;

	private const double fuzz = 1E-06;

	public Point ControlPoint1
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Point)((DependencyObject)this).GetValue(ControlPoint1Property);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(ControlPoint1Property, (object)value);
		}
	}

	public Point ControlPoint2
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Point)((DependencyObject)this).GetValue(ControlPoint2Property);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(ControlPoint2Property, (object)value);
		}
	}

	static CubicBezierEase()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		ControlPoint1Property = DependencyProperty.Register("ControlPoint1", typeof(Point), typeof(CubicBezierEase), new PropertyMetadata((object)new Point(0.0, 0.0), new PropertyChangedCallback(OnControlPointChanged)));
		ControlPoint2Property = DependencyProperty.Register("ControlPoint2", typeof(Point), typeof(CubicBezierEase), new PropertyMetadata((object)new Point(1.0, 1.0), new PropertyChangedCallback(OnControlPointChanged)));
		EasingFunctionBase.EasingModeProperty.OverrideMetadata(typeof(CubicBezierEase), new PropertyMetadata((object)(EasingMode)0));
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new CubicBezierEase();
	}

	protected override double EaseInCore(double normalizedTime)
	{
		return GetSplineProgress(normalizedTime);
	}

	protected override void OnChanged()
	{
		_isDirty = true;
		((Freezable)this).OnChanged();
	}

	private double GetSplineProgress(double linearProgress)
	{
		((Freezable)this).ReadPreamble();
		if (_isDirty)
		{
			Build();
		}
		if (!_isSpecified)
		{
			return linearProgress;
		}
		SetParameterFromX(linearProgress);
		return GetBezierValue(_By, _Cy, _parameter);
	}

	private void Build()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Point controlPoint = ControlPoint1;
		Point controlPoint2 = ControlPoint2;
		if (controlPoint == new Point(0.0, 0.0) && controlPoint2 == new Point(1.0, 1.0))
		{
			_isSpecified = false;
		}
		else
		{
			_isSpecified = true;
			_parameter = 0.0;
			_Bx = 3.0 * ((Point)(ref controlPoint)).X;
			_Cx = 3.0 * ((Point)(ref controlPoint2)).X;
			_Cx_Bx = 2.0 * (_Cx - _Bx);
			_three_Cx = 3.0 - _Cx;
			_By = 3.0 * ((Point)(ref controlPoint)).Y;
			_Cy = 3.0 * ((Point)(ref controlPoint2)).Y;
		}
		_isDirty = false;
	}

	private static double GetBezierValue(double b, double c, double t)
	{
		double num = 1.0 - t;
		double num2 = t * t;
		return b * t * num * num + c * num2 * num + num2 * t;
	}

	private void GetXAndDx(double t, out double x, out double dx)
	{
		double num = 1.0 - t;
		double num2 = t * t;
		double num3 = num * num;
		x = _Bx * t * num3 + _Cx * num2 * num + num2 * t;
		dx = _Bx * num3 + _Cx_Bx * num * t + _three_Cx * num2;
	}

	private void SetParameterFromX(double time)
	{
		double num = 0.0;
		double num2 = 1.0;
		if (time == 0.0)
		{
			_parameter = 0.0;
			return;
		}
		if (time == 1.0)
		{
			_parameter = 1.0;
			return;
		}
		while (num2 - num > 1E-06)
		{
			GetXAndDx(_parameter, out var x, out var dx);
			double num3 = Math.Abs(dx);
			if (x > time)
			{
				num2 = _parameter;
			}
			else
			{
				num = _parameter;
			}
			if (Math.Abs(x - time) < 0.001 * num3)
			{
				break;
			}
			if (num3 > 1E-06)
			{
				double num4 = _parameter - (x - time) / dx;
				if (num4 >= num2)
				{
					_parameter = (_parameter + num2) / 2.0;
				}
				else if (num4 <= num)
				{
					_parameter = (_parameter + num) / 2.0;
				}
				else
				{
					_parameter = num4;
				}
			}
			else
			{
				_parameter = (num + num2) / 2.0;
			}
		}
	}

	private static void OnControlPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
	}
}
