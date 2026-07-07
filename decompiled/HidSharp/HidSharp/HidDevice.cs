using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using HidSharp.Reports;
using HidSharp.Utility;

namespace HidSharp;

[ComVisible(true)]
[Guid("4D8A9A1A-D5CC-414e-8356-5A025EDA098D")]
public abstract class HidDevice : Device
{
	public abstract int ProductID { get; }

	public Version ReleaseNumber => BcdHelper.ToVersion(ReleaseNumberBcd);

	public abstract int ReleaseNumberBcd { get; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use ReleaseNumberBcd instead.")]
	public virtual int ProductVersion => ReleaseNumberBcd;

	public abstract int VendorID { get; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete]
	public virtual string Manufacturer
	{
		get
		{
			try
			{
				return GetManufacturer() ?? "";
			}
			catch (IOException)
			{
				return "";
			}
			catch (UnauthorizedAccessException)
			{
				return "";
			}
		}
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual string ProductName
	{
		get
		{
			try
			{
				return GetProductName() ?? "";
			}
			catch (IOException)
			{
				return "";
			}
			catch (UnauthorizedAccessException)
			{
				return "";
			}
		}
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual string SerialNumber
	{
		get
		{
			try
			{
				return GetSerialNumber() ?? "";
			}
			catch (IOException)
			{
				return "";
			}
			catch (UnauthorizedAccessException)
			{
				return "";
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete]
	public virtual int MaxInputReportLength
	{
		get
		{
			try
			{
				return GetMaxInputReportLength();
			}
			catch (IOException)
			{
				return 0;
			}
			catch (UnauthorizedAccessException)
			{
				return 0;
			}
		}
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual int MaxOutputReportLength
	{
		get
		{
			try
			{
				return GetMaxOutputReportLength();
			}
			catch (IOException)
			{
				return 0;
			}
			catch (UnauthorizedAccessException)
			{
				return 0;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete]
	public virtual int MaxFeatureReportLength
	{
		get
		{
			try
			{
				return GetMaxFeatureReportLength();
			}
			catch (IOException)
			{
				return 0;
			}
			catch (UnauthorizedAccessException)
			{
				return 0;
			}
		}
	}

	public new HidStream Open()
	{
		return (HidStream)base.Open();
	}

	public new HidStream Open(OpenConfiguration openConfig)
	{
		return (HidStream)base.Open(openConfig);
	}

	public override string GetFriendlyName()
	{
		return GetProductName();
	}

	public abstract string GetManufacturer();

	public abstract string GetProductName();

	public abstract string GetSerialNumber();

	public abstract int GetMaxInputReportLength();

	public abstract int GetMaxOutputReportLength();

	public abstract int GetMaxFeatureReportLength();

	public ReportDescriptor GetReportDescriptor()
	{
		return new ReportDescriptor(GetRawReportDescriptor());
	}

	public virtual byte[] GetRawReportDescriptor()
	{
		throw new NotSupportedException();
	}

	public virtual string[] GetSerialPorts()
	{
		throw new NotSupportedException();
	}

	public override string ToString()
	{
		string text = "(unnamed manufacturer)";
		try
		{
			text = GetManufacturer();
		}
		catch
		{
		}
		string text2 = "(unnamed product)";
		try
		{
			text2 = GetProductName();
		}
		catch
		{
		}
		string text3 = "(no serial number)";
		try
		{
			text3 = GetSerialNumber();
		}
		catch
		{
		}
		return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} (VID {3}, PID {4}, version {5})", new object[6] { text, text2, text3, VendorID, ProductID, ReleaseNumber });
	}

	public bool TryOpen(out HidStream stream)
	{
		return TryOpen(null, out stream);
	}

	public bool TryOpen(OpenConfiguration openConfig, out HidStream stream)
	{
		DeviceStream stream2;
		bool result = TryOpen(openConfig, out stream2);
		stream = (HidStream)stream2;
		return result;
	}

	public override bool HasImplementationDetail(Guid detail)
	{
		if (!base.HasImplementationDetail(detail))
		{
			return detail == ImplementationDetail.HidDevice;
		}
		return true;
	}
}
