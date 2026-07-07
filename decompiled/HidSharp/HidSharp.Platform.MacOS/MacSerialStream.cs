using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace HidSharp.Platform.MacOS;

internal sealed class MacSerialStream : SerialStream
{
	private object _lock = new object();

	private NativeMethods.termios _oldSettings;

	private NativeMethods.termios _newSettings;

	private SerialSettings _ser = SerialSettings.Default;

	private bool _settingsChanged = true;

	private int _handle;

	public sealed override int BaudRate
	{
		get
		{
			return _ser.BaudRate;
		}
		set
		{
			_ser.SetBaudRate(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override int DataBits
	{
		get
		{
			return _ser.DataBits;
		}
		set
		{
			_ser.SetDataBits(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override SerialParity Parity
	{
		get
		{
			return _ser.Parity;
		}
		set
		{
			_ser.SetParity(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override int StopBits
	{
		get
		{
			return _ser.StopBits;
		}
		set
		{
			_ser.SetStopBits(value, _lock, ref _settingsChanged);
		}
	}

	public sealed override int ReadTimeout { get; set; }

	public sealed override int WriteTimeout { get; set; }

	internal MacSerialStream(MacSerialDevice device)
		: base(device)
	{
		string fileSystemName = device.GetFileSystemName();
		int handle = NativeMethods.retry(() => NativeMethods.open(fileSystemName, (NativeMethods.oflag)131078));
		if (handle < 0)
		{
			NativeMethods.error lastWin32Error = (NativeMethods.error)Marshal.GetLastWin32Error();
			if (lastWin32Error == NativeMethods.error.EACCES)
			{
				throw DeviceException.CreateUnauthorizedAccessException(device, "Not permitted to open serial device at " + fileSystemName + ".");
			}
			throw DeviceException.CreateIOException(device, "Unable to open serial device (" + lastWin32Error.ToString() + ").");
		}
		int num = NativeMethods.retry(() => NativeMethods.ioctl(handle, NativeMethods.TIOCEXCL));
		if (num < 0)
		{
			NativeMethods.retry(() => NativeMethods.close(handle));
			throw new IOException("Unable to open serial device exclusively.");
		}
		num = NativeMethods.retry(() => NativeMethods.tcgetattr(handle, out _oldSettings));
		if (num < 0)
		{
			NativeMethods.retry(() => NativeMethods.ioctl(handle, NativeMethods.TIOCNXCL));
			NativeMethods.retry(() => NativeMethods.close(handle));
			throw new IOException("Unable to get serial port settings.");
		}
		_newSettings = _oldSettings;
		NativeMethods.cfmakeraw(ref _newSettings);
		_handle = handle;
		InitSettings();
		UpdateSettings();
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			lock (_lock)
			{
				int handle = Interlocked.Exchange(ref _handle, -1);
				if (handle >= 0)
				{
					NativeMethods.retry(() => NativeMethods.tcsetattr(handle, 0, ref _oldSettings));
					NativeMethods.retry(() => NativeMethods.ioctl(handle, NativeMethods.TIOCNXCL));
					NativeMethods.retry(() => NativeMethods.close(handle));
				}
			}
		}
		catch
		{
		}
		base.Dispose(disposing);
	}

	public override void Flush()
	{
		int handle = _handle;
		if (handle >= 0)
		{
			NativeMethods.retry(() => NativeMethods.tcdrain(handle));
		}
	}

	private static int GetTimeout(int startTime, int rwTimeout)
	{
		int num;
		if (rwTimeout < 0)
		{
			num = 1000;
		}
		else
		{
			num = Math.Min(1000, startTime + rwTimeout - Environment.TickCount);
			if (num < 0)
			{
				throw new TimeoutException("Read timed out.");
			}
		}
		return num;
	}

	public unsafe override int Read(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		UpdateSettings();
		if (count == 0)
		{
			return 0;
		}
		fixed (byte* ptr = buffer)
		{
			int startTime = Environment.TickCount;
			int readTimeout = ReadTimeout;
			int handle;
			IntPtr bufferPtr;
			int bytesToRead;
			NativeMethods.pollfd fd;
			int num;
			do
			{
				handle = _handle;
				if (handle < 0)
				{
					throw new IOException("Closed.");
				}
				bufferPtr = (IntPtr)(ptr + offset);
				bytesToRead = count;
				fd = new NativeMethods.pollfd
				{
					fd = handle,
					events = NativeMethods.pollev.IN
				};
				num = NativeMethods.retry(() => NativeMethods.poll(ref fd, 1u, GetTimeout(startTime, readTimeout)));
				if (num < 0)
				{
					throw new IOException("Read failed (poll).");
				}
			}
			while (num != 1);
			if (fd.revents != NativeMethods.pollev.IN)
			{
				throw new IOException($"Closed during read ({fd.revents}).");
			}
			int num2 = (int)NativeMethods.retry(() => NativeMethods.read(handle, bufferPtr, (UIntPtr)checked((ulong)bytesToRead)));
			if (num2 <= 0 || num2 > bytesToRead)
			{
				throw new IOException("Read failed.");
			}
			return num2;
		}
	}

	public unsafe override void Write(byte[] buffer, int offset, int count)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		UpdateSettings();
		if (count == 0)
		{
			return;
		}
		fixed (byte* ptr = buffer)
		{
			int startTime = Environment.TickCount;
			int writeTimeout = WriteTimeout;
			int num = 0;
			while (num < count)
			{
				int handle = _handle;
				if (handle < 0)
				{
					throw new IOException("Closed.");
				}
				IntPtr bufferPtr = (IntPtr)(ptr + offset + num);
				int bytesToWrite = count - num;
				NativeMethods.pollfd fd = new NativeMethods.pollfd
				{
					fd = handle,
					events = NativeMethods.pollev.OUT
				};
				int num2 = NativeMethods.retry(() => NativeMethods.poll(ref fd, 1u, GetTimeout(startTime, writeTimeout)));
				if (num2 < 0)
				{
					throw new IOException("Write failed (poll).");
				}
				if (num2 == 1)
				{
					if (fd.revents != NativeMethods.pollev.OUT)
					{
						throw new IOException($"Closed during write ({fd.revents}).");
					}
					int num3 = (int)NativeMethods.retry(() => NativeMethods.write(handle, bufferPtr, (UIntPtr)checked((ulong)bytesToWrite)));
					if (num3 <= 0 || num3 > bytesToWrite)
					{
						throw new IOException("Write failed.");
					}
					num += num3;
				}
			}
		}
	}

	private unsafe void InitSettings()
	{
		ulong num = (ulong)_newSettings.c_iflag;
		num = 0uL;
		_newSettings.c_iflag = (UIntPtr)num;
		ulong num2 = (ulong)_newSettings.c_cflag;
		num2 &= 0xFFFFFFFFFFFFFBFFuL;
		num2 &= 0xFFFFFFFFFFFFFCFFuL;
		num2 &= 0xFFFFFFFFFFFFEFFFuL;
		num2 |= 0x300;
		num2 |= 0x800;
		num2 |= 0x8000;
		num2 &= 0xFFFFFFFFFFFCFFFFuL;
		_newSettings.c_cflag = (UIntPtr)num2;
		ulong num3 = (ulong)_newSettings.c_oflag;
		num3 &= 0xFFFFFFFFFFFFFFFEuL;
		_newSettings.c_oflag = (UIntPtr)num3;
		fixed (byte* c_cc = _newSettings.c_cc)
		{
			c_cc[16] = 1;
			c_cc[17] = 0;
		}
	}

	private void UpdateSettings()
	{
		lock (_lock)
		{
			int handle = _handle;
			if (handle >= 0 && _settingsChanged)
			{
				int baudRate = _ser.BaudRate;
				int dataBits = _ser.DataBits;
				SerialParity parity = _ser.Parity;
				int stopBits = _ser.StopBits;
				int num = NativeMethods.retry(() => NativeMethods.cfsetspeed(ref _newSettings, (UIntPtr)(ulong)Math.Max(1, baudRate)));
				if (num < 0)
				{
					throw new IOException("cfsetspeed failed.");
				}
				ulong num2 = (ulong)_newSettings.c_cflag;
				num2 &= 0xFFFFFFFFFFFFFCFFuL;
				num2 = ((dataBits != 7) ? (num2 | 0x300) : (num2 | 0x200));
				num2 &= 0xFFFFFFFFFFFFCFFFuL;
				switch (parity)
				{
				case SerialParity.Even:
					num2 |= 0x1000;
					break;
				case SerialParity.Odd:
					num2 |= 0x3000;
					break;
				}
				num2 &= 0xFFFFFFFFFFFFFBFFuL;
				if (stopBits == 2)
				{
					num2 |= 0x400;
				}
				_newSettings.c_cflag = (UIntPtr)num2;
				num = NativeMethods.retry(() => NativeMethods.tcsetattr(handle, 0, ref _newSettings));
				if (num < 0)
				{
					throw new IOException("tcsetattr failed.");
				}
				num = NativeMethods.retry(() => NativeMethods.tcflush(handle, 1));
				if (num < 0)
				{
					throw new IOException("tcflush failed.");
				}
				_settingsChanged = false;
			}
		}
	}
}
