using System.Windows;

namespace ModernWpf.Controls;

internal class OrientationBasedMeasures
{
	public ScrollOrientation ScrollOrientation { get; set; }

	public double Major(Size size)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Size)(ref size)).Width;
		}
		return ((Size)(ref size)).Height;
	}

	public double Minor(Size size)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Size)(ref size)).Height;
		}
		return ((Size)(ref size)).Width;
	}

	public double MajorSize(Rect rect)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Rect)(ref rect)).Width;
		}
		return ((Rect)(ref rect)).Height;
	}

	public double MinorSize(Rect rect)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Rect)(ref rect)).Height;
		}
		return ((Rect)(ref rect)).Width;
	}

	public double MajorStart(Rect rect)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Rect)(ref rect)).X;
		}
		return ((Rect)(ref rect)).Y;
	}

	public double MajorEnd(Rect rect)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Rect)(ref rect)).X + ((Rect)(ref rect)).Width;
		}
		return ((Rect)(ref rect)).Y + ((Rect)(ref rect)).Height;
	}

	public double MinorStart(Rect rect)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Rect)(ref rect)).Y;
		}
		return ((Rect)(ref rect)).X;
	}

	public double MinorEnd(Rect rect)
	{
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return ((Rect)(ref rect)).Y + ((Rect)(ref rect)).Height;
		}
		return ((Rect)(ref rect)).X + ((Rect)(ref rect)).Width;
	}

	public void SetMajorSize(ref Rect rect, double value)
	{
		if (ScrollOrientation == ScrollOrientation.Vertical)
		{
			((Rect)(ref rect)).Height = value;
		}
		else
		{
			((Rect)(ref rect)).Width = value;
		}
	}

	public void SetMinorSize(ref Rect rect, double value)
	{
		if (ScrollOrientation == ScrollOrientation.Vertical)
		{
			((Rect)(ref rect)).Width = value;
		}
		else
		{
			((Rect)(ref rect)).Height = value;
		}
	}

	public void SetMajorStart(ref Rect rect, double value)
	{
		if (ScrollOrientation == ScrollOrientation.Vertical)
		{
			((Rect)(ref rect)).Y = value;
		}
		else
		{
			((Rect)(ref rect)).X = value;
		}
	}

	public void SetMinorStart(ref Rect rect, double value)
	{
		if (ScrollOrientation == ScrollOrientation.Vertical)
		{
			((Rect)(ref rect)).X = value;
		}
		else
		{
			((Rect)(ref rect)).Y = value;
		}
	}

	public Rect MinorMajorRect(double minor, double major, double minorSize, double majorSize)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return new Rect(major, minor, majorSize, minorSize);
		}
		return new Rect(minor, major, minorSize, majorSize);
	}

	public Point MinorMajorPoint(double minor, double major)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return new Point(major, minor);
		}
		return new Point(minor, major);
	}

	public Size MinorMajorSize(double minor, double major)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ScrollOrientation != ScrollOrientation.Vertical)
		{
			return new Size(major, minor);
		}
		return new Size(minor, major);
	}
}
