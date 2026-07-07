using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace HidSharp.Platform;

internal abstract class SysHidStream : HidStream
{
	internal class CommonOutputReport
	{
		public byte[] Bytes;

		public bool DoneOK;

		public bool Feature;

		public volatile bool Done;
	}

	private SysRefCountHelper _rch;

	public sealed override int ReadTimeout { get; set; }

	public sealed override int WriteTimeout { get; set; }

	protected SysHidStream(HidDevice device)
		: base(device)
	{
	}

	internal static int GetTimeout(int startTime, int timeout)
	{
		return Math.Min(timeout, Math.Max(0, startTime + timeout - Environment.TickCount));
	}

	internal void CommonDisconnected(Queue<byte[]> readQueue)
	{
		lock (readQueue)
		{
			if (readQueue.Count == 0 || readQueue.Peek() != null)
			{
				readQueue.Enqueue(null);
				Monitor.PulseAll(readQueue);
			}
		}
	}

	internal int CommonRead(byte[] buffer, int offset, int count, Queue<byte[]> queue)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		if (count == 0)
		{
			return 0;
		}
		int readTimeout = ReadTimeout;
		int tickCount = Environment.TickCount;
		HandleAcquireIfOpenOrFail();
		try
		{
			lock (queue)
			{
				int timeout;
				do
				{
					if (queue.Count > 0)
					{
						if (queue.Peek() == null)
						{
							throw new IOException("I/O disconnected.");
						}
						byte[] array = queue.Dequeue();
						count = Math.Min(count, array.Length);
						Array.Copy(array, 0, buffer, offset, count);
						return count;
					}
					timeout = GetTimeout(tickCount, readTimeout);
					_rch.ThrowIfClosed();
				}
				while (Monitor.Wait(queue, timeout));
				throw new TimeoutException();
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	internal void CommonWrite(byte[] buffer, int offset, int count, Queue<CommonOutputReport> queue, bool feature, int maxOutputReportLength)
	{
		Throw.If.OutOfRange(buffer, offset, count);
		count = Math.Min(count, maxOutputReportLength);
		if (count == 0)
		{
			return;
		}
		int writeTimeout = WriteTimeout;
		int tickCount = Environment.TickCount;
		HandleAcquireIfOpenOrFail();
		try
		{
			lock (queue)
			{
				int timeout;
				do
				{
					if (queue.Count == 0)
					{
						byte[] array = new byte[count];
						Array.Copy(buffer, offset, array, 0, count);
						CommonOutputReport commonOutputReport = new CommonOutputReport();
						commonOutputReport.Bytes = array;
						commonOutputReport.Feature = feature;
						CommonOutputReport commonOutputReport2 = commonOutputReport;
						queue.Enqueue(commonOutputReport2);
						Monitor.PulseAll(queue);
						do
						{
							if (commonOutputReport2.Done)
							{
								if (!commonOutputReport2.DoneOK)
								{
									throw new IOException("I/O output report failed.");
								}
								return;
							}
							timeout = GetTimeout(tickCount, writeTimeout);
							_rch.ThrowIfClosed();
						}
						while (Monitor.Wait(queue, timeout));
						throw new TimeoutException();
					}
					timeout = GetTimeout(tickCount, writeTimeout);
					_rch.ThrowIfClosed();
				}
				while (Monitor.Wait(queue, timeout));
				throw new TimeoutException();
			}
		}
		finally
		{
			HandleRelease();
		}
	}

	internal void HandleInitAndOpen()
	{
		_rch.HandleInitAndOpen();
	}

	internal bool HandleClose()
	{
		return _rch.HandleClose();
	}

	internal bool HandleAcquire()
	{
		return _rch.HandleAcquire();
	}

	internal void HandleAcquireIfOpenOrFail()
	{
		_rch.HandleAcquireIfOpenOrFail();
	}

	internal void HandleRelease()
	{
		if (_rch.HandleRelease())
		{
			HandleFree();
		}
	}

	internal abstract void HandleFree();
}
