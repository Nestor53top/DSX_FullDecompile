using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using MS.Internal;
using MS.Win32;

namespace ModernWpf.Controls.Primitives;

internal class PopupPositioner : DependencyObject, IDisposable
{
	private struct PointCombination(InterestPoint targetInterestPoint, InterestPoint childInterestPoint)
	{
		public InterestPoint TargetInterestPoint = targetInterestPoint;

		public InterestPoint ChildInterestPoint = childInterestPoint;
	}

	private class PositionInfo
	{
		public int X;

		public int Y;

		public Size ChildSize;
	}

	private class PopupSecurityHelper
	{
		private HwndSource _window;

		internal bool AttachedToWindow => _window != null;

		private IntPtr Handle => GetHandle(_window);

		internal PopupSecurityHelper()
		{
		}

		internal void AttachToWindow(HwndSource window, AutoResizedEventHandler handler)
		{
			if (_window == null)
			{
				_window = window;
				window.AutoResized += handler;
			}
		}

		internal void DetachFromWindow(AutoResizedEventHandler onAutoResizedEventHandler)
		{
			if (_window != null)
			{
				HwndSource window = _window;
				_window = null;
				window.AutoResized -= onAutoResizedEventHandler;
			}
		}

		internal bool IsWindowAlive()
		{
			if (_window != null)
			{
				HwndSource window = _window;
				if (window != null)
				{
					return !((PresentationSource)window).IsDisposed;
				}
				return false;
			}
			return false;
		}

		internal void SetPopupPos(bool position, int x, int y, bool size, int width, int height)
		{
			int num = 20;
			if (!position)
			{
				num |= 2;
			}
			if (!size)
			{
				num |= 1;
			}
			UnsafeNativeMethods.SetWindowPos(new HandleRef(null, Handle), new HandleRef(null, IntPtr.Zero), x, y, width, height, num);
		}

		internal Rect GetWindowRect()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			NativeMethods.RECT rect = new NativeMethods.RECT(0, 0, 0, 0);
			if (IsWindowAlive())
			{
				SafeNativeMethods.GetWindowRect(_window.CreateHandleRef(), ref rect);
			}
			return PointUtil.ToRect(rect);
		}

		internal Matrix GetTransformToDevice()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			CompositionTarget compositionTarget = (CompositionTarget)(object)_window.CompositionTarget;
			if (compositionTarget != null)
			{
				try
				{
					return compositionTarget.TransformToDevice;
				}
				catch (ObjectDisposedException)
				{
				}
			}
			return Matrix.Identity;
		}

		internal Matrix GetTransformFromDevice()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			CompositionTarget compositionTarget = (CompositionTarget)(object)_window.CompositionTarget;
			if (compositionTarget != null)
			{
				try
				{
					return compositionTarget.TransformFromDevice;
				}
				catch (ObjectDisposedException)
				{
				}
			}
			return Matrix.Identity;
		}

		private static IntPtr GetHandle(HwndSource hwnd)
		{
			if (hwnd == null)
			{
				return IntPtr.Zero;
			}
			return hwnd.Handle;
		}
	}

	private static class Delegates
	{
		public static Func<Popup, PlacementMode> GetPlacementInternal { get; }

		public static Func<Popup, bool> GetDropOpposite { get; }

		public static Func<Popup, PlacementMode, Point[]> GetPlacementTargetInterestPoints { get; }

		public static Func<Popup, PlacementMode, Point[]> GetChildInterestPoints { get; }

		public static Func<Popup, Rect, Point, Rect> GetScreenBounds { get; }

		static Delegates()
		{
			try
			{
				GetPlacementInternal = DelegateHelper.CreatePropertyGetter<Popup, PlacementMode>("PlacementInternal", BindingFlags.Instance | BindingFlags.NonPublic, nonPublic: true);
				GetDropOpposite = DelegateHelper.CreatePropertyGetter<Popup, bool>("DropOpposite", BindingFlags.Instance | BindingFlags.NonPublic, nonPublic: true);
				GetPlacementTargetInterestPoints = DelegateHelper.CreateDelegate<Func<Popup, PlacementMode, Point[]>>(typeof(Popup), "GetPlacementTargetInterestPoints", BindingFlags.Instance | BindingFlags.NonPublic);
				GetChildInterestPoints = DelegateHelper.CreateDelegate<Func<Popup, PlacementMode, Point[]>>(typeof(Popup), "GetChildInterestPoints", BindingFlags.Instance | BindingFlags.NonPublic);
				GetScreenBounds = DelegateHelper.CreateDelegate<Func<Popup, Rect, Point, Rect>>(typeof(Popup), "GetScreenBounds", BindingFlags.Instance | BindingFlags.NonPublic);
			}
			catch
			{
			}
		}
	}

	internal const double Tolerance = 0.01;

	private PositionInfo _positionInfo;

	private FrameworkElement _popupRoot;

	private PopupSecurityHelper _secHelper;

	private static readonly DependencyProperty PositionerProperty;

	private readonly Popup _popup;

	private bool _isDisposed;

	public static bool IsSupported { get; }

	public bool IsOpen => _popup.IsOpen;

	public PlacementMode Placement => _popup.Placement;

	internal PlacementMode PlacementInternal => Delegates.GetPlacementInternal(_popup);

	public CustomPopupPlacementCallback CustomPopupPlacementCallback => _popup.CustomPopupPlacementCallback;

	public double HorizontalOffset => _popup.HorizontalOffset;

	public double VerticalOffset => _popup.VerticalOffset;

	internal bool DropOpposite => Delegates.GetDropOpposite(_popup);

	private bool IsTransparent => _popup.AllowsTransparency;

	static PopupPositioner()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		PositionerProperty = DependencyProperty.RegisterAttached("Positioner", typeof(PopupPositioner), typeof(PopupPositioner), new PropertyMetadata(new PropertyChangedCallback(OnPositionerChanged)));
		IsSupported = Delegates.GetPlacementInternal != null && Delegates.GetDropOpposite != null && Delegates.GetPlacementTargetInterestPoints != null && Delegates.GetChildInterestPoints != null && Delegates.GetScreenBounds != null;
	}

	public PopupPositioner(Popup popup)
	{
		if (!IsSupported)
		{
			throw new NotSupportedException();
		}
		_popup = popup;
		_secHelper = new PopupSecurityHelper();
		SetPositioner(popup, this);
		popup.Opened += OnPopupOpened;
		popup.Closed += OnPopupClosed;
		if (popup.IsOpen)
		{
			OnPopupOpened(null, null);
		}
	}

	public void Dispose()
	{
		if (!_isDisposed)
		{
			_isDisposed = true;
			if (_popup != null)
			{
				_popup.Opened -= OnPopupOpened;
				_popup.Closed -= OnPopupClosed;
				((DependencyObject)_popup).ClearValue(PositionerProperty);
			}
			OnPopupClosed(null, null);
		}
	}

	private void OnWindowResize(object sender, AutoResizedEventArgs e)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (_positionInfo != null && e.Size != _positionInfo.ChildSize)
		{
			_positionInfo.ChildSize = e.Size;
			Reposition();
		}
	}

	internal void Reposition()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		if (!IsOpen || !_secHelper.IsWindowAlive())
		{
			return;
		}
		if (((DispatcherObject)this).CheckAccess())
		{
			UpdatePosition();
			return;
		}
		((DispatcherObject)this).Dispatcher.BeginInvoke((DispatcherPriority)9, (Delegate)(DispatcherOperationCallback)delegate
		{
			Reposition();
			return (object)null;
		}, (object)null);
	}

	private void UpdatePosition()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Invalid comparison between Unknown and I4
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Invalid comparison between Unknown and I4
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		if (_popupRoot == null)
		{
			return;
		}
		PlacementMode placementInternal = PlacementInternal;
		Point[] placementTargetInterestPoints = GetPlacementTargetInterestPoints(placementInternal);
		Point[] childInterestPoints = GetChildInterestPoints(placementInternal);
		Rect bounds = GetBounds(placementTargetInterestPoints);
		Rect bounds2 = GetBounds(childInterestPoints);
		double num = ((Rect)(ref bounds2)).Width * ((Rect)(ref bounds2)).Height;
		Rect windowRect = _secHelper.GetWindowRect();
		if (_positionInfo == null)
		{
			_positionInfo = new PositionInfo();
		}
		_positionInfo.X = (int)((Rect)(ref windowRect)).X;
		_positionInfo.Y = (int)((Rect)(ref windowRect)).Y;
		_positionInfo.ChildSize = ((Rect)(ref windowRect)).Size;
		Vector val = default(Vector);
		((Vector)(ref val))._002Ector((double)_positionInfo.X, (double)_positionInfo.Y);
		double num2 = -1.0;
		CustomPopupPlacement[] array = null;
		int num3;
		if ((int)placementInternal == 11)
		{
			CustomPopupPlacementCallback customPopupPlacementCallback = CustomPopupPlacementCallback;
			if (customPopupPlacementCallback != null)
			{
				array = customPopupPlacementCallback.Invoke(((Rect)(ref bounds2)).Size, ((Rect)(ref bounds)).Size, new Point(HorizontalOffset, VerticalOffset));
			}
			num3 = ((array != null) ? array.Length : 0);
			if (!IsOpen)
			{
				return;
			}
		}
		else
		{
			num3 = GetNumberOfCombinations(placementInternal);
		}
		Rect screenBounds;
		for (int i = 0; i < num3; i++)
		{
			Vector val2;
			PopupPrimaryAxis axis;
			if ((int)placementInternal == 11)
			{
				val2 = (Vector)placementTargetInterestPoints[0] + (Vector)((CustomPopupPlacement)(ref array[i])).Point;
				axis = ((CustomPopupPlacement)(ref array[i])).PrimaryAxis;
			}
			else
			{
				PointCombination pointCombination = GetPointCombination(placementInternal, i, out axis);
				InterestPoint targetInterestPoint = pointCombination.TargetInterestPoint;
				InterestPoint childInterestPoint = pointCombination.ChildInterestPoint;
				val2 = placementTargetInterestPoints[(int)targetInterestPoint] - childInterestPoints[(int)childInterestPoint];
			}
			Rect val3 = Rect.Offset(bounds2, val2);
			screenBounds = GetScreenBounds(bounds, placementTargetInterestPoints[0]);
			Rect val4 = Rect.Intersect(screenBounds, val3);
			double num4 = ((val4 != Rect.Empty) ? (((Rect)(ref val4)).Width * ((Rect)(ref val4)).Height) : 0.0);
			if (num4 - num2 > 0.01)
			{
				val = val2;
				num2 = num4;
				if (Math.Abs(num4 - num) < 0.01)
				{
					break;
				}
			}
		}
		Matrix transformToDevice = _secHelper.GetTransformToDevice();
		((Rect)(ref bounds2))._002Ector((Size)((Matrix)(ref transformToDevice)).Transform((Point)GetChildSize()));
		((Rect)(ref bounds2)).Offset(val);
		Vector val5 = (Vector)((Matrix)(ref transformToDevice)).Transform(GetChildTranslation());
		((Rect)(ref bounds2)).Offset(val5);
		screenBounds = GetScreenBounds(bounds, placementTargetInterestPoints[0]);
		Rect val6 = Rect.Intersect(screenBounds, bounds2);
		if (Math.Abs(((Rect)(ref val6)).Width - ((Rect)(ref bounds2)).Width) > 0.01 || Math.Abs(((Rect)(ref val6)).Height - ((Rect)(ref bounds2)).Height) > 0.01)
		{
			Point val7 = placementTargetInterestPoints[0];
			Vector val8 = placementTargetInterestPoints[1] - val7;
			((Vector)(ref val8)).Normalize();
			if (!IsTransparent || double.IsNaN(((Vector)(ref val8)).Y) || Math.Abs(((Vector)(ref val8)).Y) < 0.01)
			{
				if (((Rect)(ref bounds2)).Right > ((Rect)(ref screenBounds)).Right)
				{
					((Vector)(ref val)).X = ((Rect)(ref screenBounds)).Right - ((Rect)(ref bounds2)).Width;
					((Vector)(ref val)).X = ((Vector)(ref val)).X - ((Vector)(ref val5)).X;
				}
				else if (((Rect)(ref bounds2)).Left < ((Rect)(ref screenBounds)).Left)
				{
					((Vector)(ref val)).X = ((Rect)(ref screenBounds)).Left;
					((Vector)(ref val)).X = ((Vector)(ref val)).X - ((Vector)(ref val5)).X;
				}
			}
			else if (IsTransparent && Math.Abs(((Vector)(ref val8)).X) < 0.01)
			{
				if (((Rect)(ref bounds2)).Bottom > ((Rect)(ref screenBounds)).Bottom)
				{
					((Vector)(ref val)).Y = ((Rect)(ref screenBounds)).Bottom - ((Rect)(ref bounds2)).Height;
					((Vector)(ref val)).Y = ((Vector)(ref val)).Y - ((Vector)(ref val5)).Y;
				}
				else if (((Rect)(ref bounds2)).Top < ((Rect)(ref screenBounds)).Top)
				{
					((Vector)(ref val)).Y = ((Rect)(ref screenBounds)).Top;
					((Vector)(ref val)).Y = ((Vector)(ref val)).Y - ((Vector)(ref val5)).Y;
				}
			}
			Point val9 = placementTargetInterestPoints[2];
			Vector val10 = val7 - val9;
			((Vector)(ref val10)).Normalize();
			if (!IsTransparent || double.IsNaN(((Vector)(ref val10)).X) || Math.Abs(((Vector)(ref val10)).X) < 0.01)
			{
				if (((Rect)(ref bounds2)).Bottom > ((Rect)(ref screenBounds)).Bottom)
				{
					((Vector)(ref val)).Y = ((Rect)(ref screenBounds)).Bottom - ((Rect)(ref bounds2)).Height;
					((Vector)(ref val)).Y = ((Vector)(ref val)).Y - ((Vector)(ref val5)).Y;
				}
				else if (((Rect)(ref bounds2)).Top < ((Rect)(ref screenBounds)).Top)
				{
					((Vector)(ref val)).Y = ((Rect)(ref screenBounds)).Top;
					((Vector)(ref val)).Y = ((Vector)(ref val)).Y - ((Vector)(ref val5)).Y;
				}
			}
			else if (IsTransparent && Math.Abs(((Vector)(ref val10)).Y) < 0.01)
			{
				if (((Rect)(ref bounds2)).Right > ((Rect)(ref screenBounds)).Right)
				{
					((Vector)(ref val)).X = ((Rect)(ref screenBounds)).Right - ((Rect)(ref bounds2)).Width;
					((Vector)(ref val)).X = ((Vector)(ref val)).X - ((Vector)(ref val5)).X;
				}
				else if (((Rect)(ref bounds2)).Left < ((Rect)(ref screenBounds)).Left)
				{
					((Vector)(ref val)).X = ((Rect)(ref screenBounds)).Left;
					((Vector)(ref val)).X = ((Vector)(ref val)).X - ((Vector)(ref val5)).X;
				}
			}
		}
		int num5 = DoubleUtil.DoubleToInt(((Vector)(ref val)).X);
		int num6 = DoubleUtil.DoubleToInt(((Vector)(ref val)).Y);
		if (num5 != _positionInfo.X || num6 != _positionInfo.Y)
		{
			_positionInfo.X = num5;
			_positionInfo.Y = num6;
			_secHelper.SetPopupPos(position: true, num5, num6, size: false, 0, 0);
		}
		Size GetChildSize()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			UIElement child = _popup.Child;
			if (child != null)
			{
				return child.RenderSize;
			}
			return ((UIElement)_popupRoot).RenderSize;
		}
		Point GetChildTranslation()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			UIElement child = _popup.Child;
			if (child != null)
			{
				return child.TranslatePoint(default(Point), (UIElement)(object)_popupRoot);
			}
			return default(Point);
		}
	}

	private Point[] GetPlacementTargetInterestPoints(PlacementMode placement)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Delegates.GetPlacementTargetInterestPoints(_popup, placement);
	}

	private PointCombination GetPointCombination(PlacementMode placement, int i, out PopupPrimaryAxis axis)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected I4, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Invalid comparison between Unknown and I4
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Invalid comparison between Unknown and I4
		bool menuDropAlignment = SystemParameters.MenuDropAlignment;
		switch ((int)placement)
		{
		case 2:
		case 7:
			axis = (PopupPrimaryAxis)1;
			if (menuDropAlignment)
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.BottomRight, InterestPoint.TopRight);
				case 1:
					return new PointCombination(InterestPoint.TopRight, InterestPoint.BottomRight);
				}
			}
			else
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.BottomLeft, InterestPoint.TopLeft);
				case 1:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
				}
			}
			break;
		case 10:
			axis = (PopupPrimaryAxis)1;
			if (menuDropAlignment)
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.TopRight, InterestPoint.BottomRight);
				case 1:
					return new PointCombination(InterestPoint.BottomRight, InterestPoint.TopRight);
				}
			}
			else
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
				case 1:
					return new PointCombination(InterestPoint.BottomLeft, InterestPoint.TopLeft);
				}
			}
			break;
		case 4:
		case 9:
			axis = (PopupPrimaryAxis)2;
			menuDropAlignment |= DropOpposite;
			if ((menuDropAlignment && (int)placement == 4) || (!menuDropAlignment && (int)placement == 9))
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
				case 1:
					return new PointCombination(InterestPoint.BottomLeft, InterestPoint.BottomRight);
				case 2:
					return new PointCombination(InterestPoint.TopRight, InterestPoint.TopLeft);
				case 3:
					return new PointCombination(InterestPoint.BottomRight, InterestPoint.BottomLeft);
				}
			}
			else
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.TopRight, InterestPoint.TopLeft);
				case 1:
					return new PointCombination(InterestPoint.BottomRight, InterestPoint.BottomLeft);
				case 2:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
				case 3:
					return new PointCombination(InterestPoint.BottomLeft, InterestPoint.BottomRight);
				}
			}
			break;
		case 1:
		case 5:
		case 6:
		case 8:
			axis = (PopupPrimaryAxis)1;
			if (menuDropAlignment)
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
				case 1:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
				case 2:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomRight);
				case 3:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
				}
			}
			else
			{
				switch (i)
				{
				case 0:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
				case 1:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
				case 2:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
				case 3:
					return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomRight);
				}
			}
			break;
		case 3:
			axis = (PopupPrimaryAxis)0;
			return new PointCombination(InterestPoint.Center, InterestPoint.Center);
		default:
			axis = (PopupPrimaryAxis)0;
			return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
		}
		return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
	}

	private Point[] GetChildInterestPoints(PlacementMode placement)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Delegates.GetChildInterestPoints(_popup, placement);
	}

	private Rect GetBounds(Point[] interestPoints)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		double num2;
		double num = (num2 = ((Point)(ref interestPoints[0])).X);
		double num4;
		double num3 = (num4 = ((Point)(ref interestPoints[0])).Y);
		for (int i = 1; i < interestPoints.Length; i++)
		{
			double x = ((Point)(ref interestPoints[i])).X;
			double y = ((Point)(ref interestPoints[i])).Y;
			if (x < num)
			{
				num = x;
			}
			if (x > num2)
			{
				num2 = x;
			}
			if (y < num3)
			{
				num3 = y;
			}
			if (y > num4)
			{
				num4 = y;
			}
		}
		return new Rect(num, num3, num2 - num, num4 - num3);
	}

	private static int GetNumberOfCombinations(PlacementMode placement)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected I4, but got Unknown
		switch ((int)placement)
		{
		case 2:
		case 7:
		case 10:
			return 2;
		case 4:
		case 5:
		case 6:
		case 8:
		case 9:
			return 4;
		case 11:
			return 0;
		default:
			return 1;
		}
	}

	private Rect GetScreenBounds(Rect boundingBox, Point p)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return Delegates.GetScreenBounds(_popup, boundingBox, p);
	}

	internal static PopupPositioner GetPositioner(Popup popup)
	{
		return (PopupPositioner)((DependencyObject)popup).GetValue(PositionerProperty);
	}

	private static void SetPositioner(Popup popup, PopupPositioner value)
	{
		((DependencyObject)popup).SetValue(PositionerProperty, (object)value);
	}

	private static void OnPositionerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is PopupPositioner popupPositioner)
		{
			popupPositioner.Dispose();
		}
	}

	private void OnPopupOpened(object sender, EventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		if (_secHelper.AttachedToWindow)
		{
			return;
		}
		UIElement child = _popup.Child;
		if (child != null)
		{
			PresentationSource obj = PresentationSource.FromVisual((Visual)(object)child);
			HwndSource val = (HwndSource)(object)((obj is HwndSource) ? obj : null);
			if (val != null)
			{
				_secHelper.AttachToWindow(val, new AutoResizedEventHandler(OnWindowResize));
				Visual rootVisual = ((PresentationSource)val).RootVisual;
				_popupRoot = (FrameworkElement)(object)((rootVisual is FrameworkElement) ? rootVisual : null);
				((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.ChildProperty, typeof(Popup))).AddValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
				((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.PlacementProperty, typeof(Popup))).AddValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
				((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.HorizontalOffsetProperty, typeof(Popup))).AddValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
				((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.VerticalOffsetProperty, typeof(Popup))).AddValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
				((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.PlacementRectangleProperty, typeof(Popup))).AddValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
				Reposition();
			}
		}
	}

	private void OnPopupClosed(object sender, EventArgs e)
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		if (_secHelper.AttachedToWindow)
		{
			((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.ChildProperty, typeof(Popup))).RemoveValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
			((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.PlacementProperty, typeof(Popup))).RemoveValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
			((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.HorizontalOffsetProperty, typeof(Popup))).RemoveValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
			((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.VerticalOffsetProperty, typeof(Popup))).RemoveValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
			((PropertyDescriptor)(object)DependencyPropertyDescriptor.FromProperty(Popup.PlacementRectangleProperty, typeof(Popup))).RemoveValueChanged((object)_popup, (EventHandler)OnPopupPropertyChanged);
			_secHelper.DetachFromWindow(new AutoResizedEventHandler(OnWindowResize));
			_popupRoot = null;
			_positionInfo = null;
		}
	}

	private void OnPopupPropertyChanged(object sender, EventArgs e)
	{
		Reposition();
	}
}
