using System;
using System.Threading;

namespace HidSharp.Reports.Input;

public class HidDeviceInputReceiver
{
	private byte[] _buffer;

	private int _bufferOffset;

	private int _bufferCount;

	private int _maxInputReportLength;

	private ReportDescriptor _reportDescriptor;

	private volatile bool _running;

	private HidStream _stream;

	private object _syncRoot;

	private ManualResetEvent _waitHandle;

	public bool IsRunning => _running;

	public ReportDescriptor ReportDescriptor => _reportDescriptor;

	public HidStream Stream => _stream;

	public WaitHandle WaitHandle => _waitHandle;

	public event EventHandler Started;

	public event EventHandler Received;

	public event EventHandler Stopped;

	public HidDeviceInputReceiver(ReportDescriptor reportDescriptor)
	{
		Throw.If.Null(reportDescriptor, "reportDescriptor");
		_maxInputReportLength = reportDescriptor.MaxInputReportLength;
		_buffer = new byte[_maxInputReportLength * 16];
		_reportDescriptor = reportDescriptor;
		_syncRoot = new object();
		_waitHandle = new ManualResetEvent(initialState: true);
	}

	public void Start(HidStream stream)
	{
		Throw.If.Null(stream);
		lock (_syncRoot)
		{
			int maxInputReportLength = _maxInputReportLength;
			if (maxInputReportLength == 0)
			{
				return;
			}
			stream.ReadTimeout = -1;
			if (_running)
			{
				throw new InvalidOperationException("The receiver is already running.");
			}
			_running = true;
			_stream = stream;
			byte[] buffer = new byte[maxInputReportLength * 16];
			Action beginRead = null;
			AsyncCallback endRead = null;
			beginRead = delegate
			{
				try
				{
					stream.BeginRead(buffer, 0, buffer.Length, endRead, null);
				}
				catch
				{
					Stop();
				}
			};
			endRead = delegate(IAsyncResult ar)
			{
				int num;
				try
				{
					num = stream.EndRead(ar);
				}
				catch
				{
					Stop();
					return;
				}
				if (num == 0)
				{
					Stop();
				}
				else
				{
					ProvideReceivedData(buffer, 0, num);
					beginRead();
				}
			};
			beginRead();
			_waitHandle.Reset();
		}
		this.Started?.Invoke(this, EventArgs.Empty);
	}

	private void Stop()
	{
		lock (_syncRoot)
		{
			_running = false;
			_stream = null;
			_waitHandle.Set();
		}
		this.Stopped?.Invoke(this, EventArgs.Empty);
	}

	private void ClearReceivedData()
	{
		lock (_syncRoot)
		{
			_bufferOffset = 0;
			_bufferCount = 0;
			_waitHandle.Reset();
		}
	}

	private void ProvideReceivedData(byte[] buffer, int offset, int count)
	{
		Throw.If.Null(buffer, "buffer").OutOfRange(buffer, offset, count);
		lock (_syncRoot)
		{
			if (_maxInputReportLength == 0)
			{
				return;
			}
			checked
			{
				int num = _bufferCount + count;
				int num2;
				for (num2 = _buffer.Length; num2 < num; num2 *= 2)
				{
				}
				Array.Resize(ref _buffer, num2);
				Array.Copy(buffer, 0, _buffer, _bufferCount, count);
			}
			_bufferCount += count;
			_waitHandle.Set();
		}
		this.Received?.Invoke(this, EventArgs.Empty);
	}

	public bool TryRead(byte[] buffer, int offset, out Report report)
	{
		Throw.If.Null(buffer).OutOfRange(buffer, offset, _maxInputReportLength);
		lock (_syncRoot)
		{
			if (!_running)
			{
				report = null;
				return false;
			}
			if (_bufferOffset >= _bufferCount)
			{
				_waitHandle.Reset();
				report = null;
				return false;
			}
			if (!_reportDescriptor.TryGetReport(ReportType.Input, _buffer[_bufferOffset], out report))
			{
				ClearReceivedData();
				report = null;
				return false;
			}
			int length = report.Length;
			int num = _bufferOffset + length;
			if (num > _bufferCount)
			{
				_waitHandle.Reset();
				report = null;
				return false;
			}
			Array.Copy(_buffer, _bufferOffset, buffer, offset, length);
			if (num == _bufferCount)
			{
				ClearReceivedData();
			}
			else
			{
				_bufferOffset = num;
			}
			return true;
		}
	}
}
