using System;
using System.Threading.Tasks;
using Device.Net;

namespace Hid.Net;

public interface IHidDevice : IDevice, IDisposable
{
	byte DefaultReportId { get; }

	Task<ReadReport> ReadReportAsync();

	Task WriteReportAsync(byte[] data, byte? reportId);
}
