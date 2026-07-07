using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace HidSharp.Platform.SystemEvents;

internal abstract class PosixEventManager : EventManager
{
	private sealed class PosixEvent : SystemEvent
	{
		private struct LockStructure
		{
			public int Ttl;

			public int RefCount;

			public int SetID;

			public int ResetID;
		}

		private bool _createdNew;

		private ManualResetEvent _event;

		private PosixEventManager _manager;

		private object _refreshHandle;

		public override bool CreatedNew => _createdNew;

		public override WaitHandle WaitHandle => _event;

		public PosixEvent(PosixEventManager manager, string name)
			: base(name)
		{
			if (manager == null)
			{
				throw new ArgumentNullException();
			}
			_manager = manager;
			_event = new ManualResetEvent(initialState: false);
			UpdateEventStruct(delegate(LockStructure inL)
			{
				if (inL.RefCount == 0)
				{
					_createdNew = true;
				}
				LockStructure value = inL;
				value.RefCount++;
				value.Ttl = _manager.GetTickCount();
				return value;
			});
			manager.RegisterRefreshCallback(Refresh, out _refreshHandle, GetEventFilename("Events", base.Name), GetSHMFilename("Events", base.Name));
		}

		public override void Dispose()
		{
			if (_event != null)
			{
				_manager.UnregisterRefreshCallback(ref _refreshHandle);
				UpdateEventStruct(delegate(LockStructure inL)
				{
					LockStructure value = inL;
					value.RefCount--;
					return value;
				});
				_event.Close();
				_event = null;
			}
		}

		private void Refresh()
		{
			UpdateEventStruct(delegate(LockStructure inL)
			{
				int tickCount = _manager.GetTickCount();
				if ((uint)(tickCount - inL.Ttl) < 4000u)
				{
					return (LockStructure?)null;
				}
				LockStructure value = inL;
				value.Ttl = tickCount;
				return value;
			});
		}

		public override void Reset()
		{
			UpdateEventStruct(delegate(LockStructure inL)
			{
				if (inL.ResetID == inL.SetID)
				{
					return (LockStructure?)null;
				}
				LockStructure value = inL;
				value.ResetID = value.SetID;
				value.Ttl = _manager.GetTickCount();
				return value;
			}, updateFSW: true);
		}

		public override void Set()
		{
			UpdateEventStruct(delegate(LockStructure inL)
			{
				if (inL.ResetID != inL.SetID)
				{
					return (LockStructure?)null;
				}
				LockStructure value = inL;
				value.SetID++;
				value.Ttl = _manager.GetTickCount();
				return value;
			}, updateFSW: true);
		}

		private void UpdateEventStream(Func<byte[], bool> editCallback)
		{
			_manager.UpdateEventStream("Events", base.Name, 16, editCallback);
		}

		private void UpdateEventStruct(Func<LockStructure, LockStructure?> editCallback, bool updateFSW = false)
		{
			UpdateEventStream(delegate(byte[] buffer)
			{
				int tickCount = _manager.GetTickCount();
				LockStructure arg = new LockStructure
				{
					Ttl = BitConverter.ToInt32(buffer, 0),
					RefCount = BitConverter.ToInt32(buffer, 4),
					SetID = BitConverter.ToInt32(buffer, 8),
					ResetID = BitConverter.ToInt32(buffer, 12)
				};
				if (arg.RefCount <= 0)
				{
					arg = default(LockStructure);
				}
				int num = tickCount - arg.Ttl;
				if (arg.Ttl != 0 && (num >= 30000 || num <= -1000))
				{
					arg = default(LockStructure);
				}
				LockStructure? lockStructure = editCallback(arg);
				if (!lockStructure.HasValue)
				{
					UpdateEvent(arg.SetID != arg.ResetID);
					return false;
				}
				LockStructure value = lockStructure.Value;
				Array.Copy(BitConverter.GetBytes(value.Ttl), 0, buffer, 0, 4);
				Array.Copy(BitConverter.GetBytes(value.RefCount), 0, buffer, 4, 4);
				Array.Copy(BitConverter.GetBytes(value.SetID), 0, buffer, 8, 4);
				Array.Copy(BitConverter.GetBytes(value.ResetID), 0, buffer, 12, 4);
				UpdateEvent(value.SetID != arg.ResetID);
				return updateFSW;
			});
		}

		private void UpdateEvent(bool set)
		{
			if (set)
			{
				_event.Set();
			}
			else
			{
				_event.Reset();
			}
		}
	}

	private sealed class PosixMutex : SystemMutex
	{
		private struct LockStructure
		{
			public int Ttl;

			public int RefCount;

			public int LockTtl;

			public Guid LockGuid;
		}

		private bool _createdNew;

		private Guid _guid;

		private PosixEventManager _manager;

		private object _refreshHandle;

		public override bool CreatedNew => _createdNew;

		public PosixMutex(PosixEventManager manager, string name)
			: base(name)
		{
			if (manager == null)
			{
				throw new ArgumentNullException();
			}
			_guid = Guid.NewGuid();
			_manager = manager;
			UpdateEventStruct(delegate(LockStructure inL)
			{
				if (inL.RefCount == 0)
				{
					_createdNew = true;
				}
				LockStructure value = inL;
				value.RefCount++;
				return value;
			});
			_manager.RegisterRefreshCallback(Refresh, out _refreshHandle, null, null);
		}

		public override void Dispose()
		{
			_manager.UnregisterRefreshCallback(ref _refreshHandle);
			UpdateEventStruct(delegate(LockStructure inL)
			{
				LockStructure value = inL;
				if (value.LockGuid == _guid)
				{
					value.LockGuid = Guid.Empty;
				}
				value.RefCount--;
				return value;
			});
		}

		private void Refresh()
		{
			UpdateEventStruct(delegate(LockStructure inL)
			{
				_manager.GetTickCount();
				return inL;
			});
		}

		protected override bool WaitOne(int timeout)
		{
			int tickCount = _manager.GetTickCount();
			do
			{
				bool locked = false;
				UpdateEventStruct(delegate(LockStructure inL)
				{
					if (inL.LockGuid != Guid.Empty)
					{
						if (inL.LockGuid == _guid)
						{
							throw new InvalidOperationException("Already locked by this mutex.");
						}
						return (LockStructure?)null;
					}
					LockStructure value = inL;
					value.LockGuid = _guid;
					locked = true;
					return value;
				});
				if (locked)
				{
					return true;
				}
				Thread.Sleep(50);
			}
			while ((uint)(_manager.GetTickCount() - tickCount) <= (uint)timeout);
			return false;
		}

		protected override void ReleaseMutex()
		{
			UpdateEventStruct(delegate(LockStructure inL)
			{
				if (inL.LockGuid == Guid.Empty)
				{
					throw new InvalidOperationException("Not locked by anyone.");
				}
				if (inL.LockGuid != _guid)
				{
					throw new InvalidOperationException("Not locked by this mutex.");
				}
				LockStructure value = inL;
				value.LockGuid = Guid.Empty;
				return value;
			});
		}

		private void UpdateEventStream(Func<byte[], bool> editCallback)
		{
			_manager.UpdateEventStream("Mutexes", base.Name, 32, editCallback);
		}

		private void UpdateEventStruct(Func<LockStructure, LockStructure?> editCallback)
		{
			UpdateEventStream(delegate(byte[] buffer)
			{
				int tickCount = _manager.GetTickCount();
				LockStructure arg = new LockStructure
				{
					Ttl = BitConverter.ToInt32(buffer, 0),
					RefCount = BitConverter.ToInt32(buffer, 4),
					LockTtl = BitConverter.ToInt32(buffer, 12)
				};
				byte[] array = new byte[16];
				Array.Copy(buffer, 16, array, 0, array.Length);
				arg.LockGuid = new Guid(array);
				if (arg.RefCount <= 0)
				{
					arg = default(LockStructure);
				}
				int num = tickCount - arg.Ttl;
				if (arg.Ttl != 0 && (num >= 30000 || num <= -1000))
				{
					arg = default(LockStructure);
				}
				int num2 = tickCount - arg.LockTtl;
				if ((arg.LockGuid != Guid.Empty || arg.LockTtl != 0) && (num2 >= 30000 || num2 < -1000))
				{
					arg.LockGuid = Guid.Empty;
					arg.LockTtl = 0;
				}
				LockStructure? lockStructure = editCallback(arg);
				if (!lockStructure.HasValue)
				{
					return false;
				}
				LockStructure value = lockStructure.Value;
				value.Ttl = tickCount;
				if (value.LockGuid == _guid)
				{
					value.LockTtl = tickCount;
				}
				Array.Copy(BitConverter.GetBytes(value.Ttl), 0, buffer, 0, 4);
				Array.Copy(BitConverter.GetBytes(value.RefCount), 0, buffer, 4, 4);
				Array.Copy(BitConverter.GetBytes(value.LockTtl), 0, buffer, 12, 4);
				Array.Copy(value.LockGuid.ToByteArray(), 0, buffer, 16, 16);
				return false;
			});
		}
	}

	private const string EventsKind = "Events";

	private const string MutexesKind = "Mutexes";

	private const int DontRefreshInterval = 4000;

	private const int RefreshInterval = 5000;

	private const int TimeoutInterval = 30000;

	private const int TimeTravelInterval = 1000;

	private Dictionary<object, Action> _jobs;

	private ManualResetEvent _jobThreadReady;

	private Thread _jobThread;

	protected PosixNativeMethods NativeMethods { get; private set; }

	protected object SyncRoot { get; private set; }

	public PosixEventManager()
	{
		SyncRoot = new object();
		NativeMethods = CreateNativeMethods();
	}

	internal override void Start()
	{
		_jobs = new Dictionary<object, Action>();
		_jobThreadReady = new ManualResetEvent(initialState: false);
		_jobThread = new Thread(RunJobThread)
		{
			IsBackground = true,
			Name = "HID System Events Job Manager"
		};
		_jobThread.Start();
		_jobThreadReady.WaitOne();
	}

	public override SystemEvent CreateEvent(string name)
	{
		return new PosixEvent(this, name);
	}

	public override SystemMutex CreateMutex(string name)
	{
		return new PosixMutex(this, name);
	}

	protected abstract PosixNativeMethods CreateNativeMethods();

	internal void UpdateEventStream(string kind, string name, int length, Func<byte[], bool> editCallback)
	{
		string[] eventDirectoryParts = GetEventDirectoryParts(kind);
		string directory;
		for (int i = 0; i < eventDirectoryParts.Length; i++)
		{
			directory = eventDirectoryParts[i];
			try
			{
				Directory.CreateDirectory(directory);
			}
			catch
			{
			}
			try
			{
				NativeMethods.retry(() => NativeMethods.chmod(directory, 511));
			}
			catch
			{
			}
		}
		string eventFilename = GetEventFilename(kind, name);
		using FileStream fileStream = File.Open(eventFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
		int streamHandle = (int)fileStream.SafeFileHandle.DangerousGetHandle();
		try
		{
			NativeMethods.retry(() => NativeMethods.fchmod(streamHandle, 438));
		}
		catch
		{
		}
		while (true)
		{
			try
			{
				fileStream.Lock(0L, 0L);
			}
			catch (IOException)
			{
				Thread.Sleep(50);
				continue;
			}
			break;
		}
		string shmName = GetSHMFilename(kind, name);
		int shm;
		try
		{
			shm = NativeMethods.retry(() => NativeMethods.shm_open(shmName, NativeMethods.O_CREAT | NativeMethods.O_RDWR, 438));
		}
		catch (DllNotFoundException)
		{
			shm = -1;
		}
		if (shm < 0)
		{
			throw new InvalidOperationException($"Failed to open shared memory {shmName}: {NativeMethods.GetLastError()}");
		}
		NativeMethods.retry(() => NativeMethods.fchmod(shm, 438));
		try
		{
			NativeMethods.retry(() => NativeMethods.ftruncate(shm, length));
			IntPtr intPtr = NativeMethods.mmap(IntPtr.Zero, (UIntPtr)(ulong)length, NativeMethods.PROT_READ | NativeMethods.PROT_WRITE, NativeMethods.MAP_SHARED, shm, 0L);
			if (intPtr == PosixNativeMethods.IntPtrNegativeOne)
			{
				throw new InvalidOperationException("Failed to map memory: " + NativeMethods.GetLastError());
			}
			try
			{
				byte[] array = new byte[length];
				Marshal.Copy(intPtr, array, 0, length);
				bool flag = editCallback(array);
				Marshal.Copy(array, 0, intPtr, length);
				if (flag)
				{
					RunNotify(fileStream, eventFilename, shmName);
				}
			}
			finally
			{
				NativeMethods.munmap(intPtr, (UIntPtr)(ulong)length);
			}
		}
		finally
		{
			NativeMethods.retry(() => NativeMethods.close(shm));
		}
	}

	protected abstract object CreateJobObject();

	protected abstract void RegisterJobObjectNotify(object jobObject, string eventName, string shmName);

	protected abstract void UnregisterJobObjectNotify(object jobObject);

	protected void RunJobObject(object jobObject)
	{
		if (_jobs.TryGetValue(jobObject, out var value))
		{
			value();
		}
	}

	protected abstract void RunNotify(FileStream eventStream, string eventName, string shmName);

	private void RegisterRefreshCallback(Action callback, out object jobObject, string eventName, string shmName)
	{
		jobObject = CreateJobObject();
		lock (SyncRoot)
		{
			_jobs.Add(jobObject, callback);
			RegisterJobObjectNotify(jobObject, eventName, shmName);
			Monitor.Pulse(SyncRoot);
		}
	}

	private void UnregisterRefreshCallback(ref object jobObject)
	{
		lock (SyncRoot)
		{
			if (jobObject != null)
			{
				UnregisterJobObjectNotify(jobObject);
				_jobs.Remove(jobObject);
				Monitor.Pulse(SyncRoot);
				jobObject = null;
			}
		}
	}

	private void RunJobThread()
	{
		_jobThreadReady.Set();
		lock (SyncRoot)
		{
			while (true)
			{
				if (_jobs.Count == 0)
				{
					Monitor.Wait(SyncRoot);
				}
				else
				{
					Monitor.Exit(SyncRoot);
					try
					{
						Thread.Sleep(5000);
					}
					finally
					{
						Monitor.Enter(SyncRoot);
					}
				}
				Action[] array = _jobs.Values.ToArray();
				foreach (Action action in array)
				{
					action();
				}
			}
		}
	}

	private static string GetEventDirectory(string kind)
	{
		return Path.Combine(Path.Combine(Path.Combine(Path.GetTempPath(), "HIDSharp"), "SystemEvents"), kind);
	}

	private static string[] GetEventDirectoryParts(string kind)
	{
		return new string[3]
		{
			Path.Combine(Path.GetTempPath(), "HIDSharp"),
			Path.Combine(Path.Combine(Path.GetTempPath(), "HIDSharp"), "SystemEvents"),
			Path.Combine(Path.Combine(Path.Combine(Path.GetTempPath(), "HIDSharp"), "SystemEvents"), kind)
		};
	}

	private static string GetEventFilename(string kind, string name)
	{
		string text = Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(name))).Replace('+', '-').Replace('/', '_')
			.Replace("=", "");
		string eventDirectory = GetEventDirectory(kind);
		return Path.Combine(eventDirectory, text + ".tmp");
	}

	private static string GetSHMFilename(string kind, string name)
	{
		return $"/HS.{kind}.{Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(name))).Substring(0, 16).Replace('+', '-').Replace('/', '_')}";
	}

	private int GetTickCount()
	{
		int num = NativeMethods.GetTickCount();
		if (num == 0)
		{
			num = 1;
		}
		return num;
	}
}
