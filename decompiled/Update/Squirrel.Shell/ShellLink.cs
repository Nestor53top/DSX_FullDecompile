using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Squirrel.Shell;

internal class ShellLink : IDisposable
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("0000010C-0000-0000-C000-000000000046")]
	private interface IPersist
	{
		[PreserveSig]
		void GetClassID(out Guid pClassID);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("0000010B-0000-0000-C000-000000000046")]
	private interface IPersistFile
	{
		[PreserveSig]
		void GetClassID(out Guid pClassID);

		void IsDirty();

		void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

		void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);

		void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

		void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
	}

	public struct PropVariant
	{
		public short variantType;

		public short Reserved1;

		public short Reserved2;

		public short Reserved3;

		public IntPtr pointerValue;

		public static PropVariant FromString(string str)
		{
			return new PropVariant
			{
				variantType = 31,
				pointerValue = Marshal.StringToCoTaskMemUni(str)
			};
		}

		public static PropVariant FromGuid(Guid guid)
		{
			byte[] array = guid.ToByteArray();
			PropVariant result = new PropVariant
			{
				variantType = 72,
				pointerValue = Marshal.AllocCoTaskMem(array.Length)
			};
			Marshal.Copy(array, 0, result.pointerValue, array.Length);
			return result;
		}

		[DllImport("ole32.dll")]
		private static extern int PropVariantClear(ref PropVariant pvar);

		public void Clear()
		{
			PropVariant pvar = this;
			PropVariantClear(ref pvar);
			variantType = 0;
			Reserved1 = (Reserved2 = (Reserved3 = 0));
			pointerValue = IntPtr.Zero;
		}
	}

	public struct PROPERTYKEY
	{
		public Guid fmtid;

		public UIntPtr pid;

		public static PROPERTYKEY PKEY_AppUserModel_ID => new PROPERTYKEY
		{
			fmtid = Guid.ParseExact("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}", "B"),
			pid = new UIntPtr(5u)
		};

		public static PROPERTYKEY PKEY_AppUserModel_ToastActivatorCLSID => new PROPERTYKEY
		{
			fmtid = Guid.ParseExact("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}", "B"),
			pid = new UIntPtr(26u)
		};
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
	private interface IPropertyStore
	{
		[PreserveSig]
		int GetCount(out uint cProps);

		[PreserveSig]
		int GetAt([In] uint iProp, out PROPERTYKEY pkey);

		[PreserveSig]
		int GetValue([In] ref PROPERTYKEY key, out PropVariant pv);

		[PreserveSig]
		int SetValue([In] ref PROPERTYKEY key, [In] ref PropVariant pv);

		[PreserveSig]
		int Commit();
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214EE-0000-0000-C000-000000000046")]
	private interface IShellLinkA
	{
		void GetPath([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, int cchMaxPath, ref _WIN32_FIND_DATAA pfd, uint fFlags);

		void GetIDList(out IntPtr ppidl);

		void SetIDList(IntPtr pidl);

		void GetDescription([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, int cchMaxName);

		void SetDescription([MarshalAs(UnmanagedType.LPStr)] string pszName);

		void GetWorkingDirectory([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir, int cchMaxPath);

		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPStr)] string pszDir);

		void GetArguments([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs, int cchMaxPath);

		void SetArguments([MarshalAs(UnmanagedType.LPStr)] string pszArgs);

		void GetHotkey(out short pwHotkey);

		void SetHotkey(short pwHotkey);

		void GetShowCmd(out uint piShowCmd);

		void SetShowCmd(uint piShowCmd);

		void GetIconLocation([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

		void SetIconLocation([MarshalAs(UnmanagedType.LPStr)] string pszIconPath, int iIcon);

		void SetRelativePath([MarshalAs(UnmanagedType.LPStr)] string pszPathRel, uint dwReserved);

		void Resolve(IntPtr hWnd, uint fFlags);

		void SetPath([MarshalAs(UnmanagedType.LPStr)] string pszFile);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	private interface IShellLinkW
	{
		void GetPath([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, ref _WIN32_FIND_DATAW pfd, uint fFlags);

		void GetIDList(out IntPtr ppidl);

		void SetIDList(IntPtr pidl);

		void GetDescription([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);

		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

		void GetWorkingDirectory([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

		void GetArguments([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

		void GetHotkey(out short pwHotkey);

		void SetHotkey(short pwHotkey);

		void GetShowCmd(out uint piShowCmd);

		void SetShowCmd(uint piShowCmd);

		void GetIconLocation([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

		void Resolve(IntPtr hWnd, uint fFlags);

		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}

	[ComImport]
	[ClassInterface(ClassInterfaceType.None)]
	[Guid("00021401-0000-0000-C000-000000000046")]
	private class CShellLink
	{
	}

	private enum EShellLinkGP : uint
	{
		SLGP_SHORTPATH = 1u,
		SLGP_UNCPRIORITY
	}

	[Flags]
	private enum EShowWindowFlags : uint
	{
		SW_HIDE = 0u,
		SW_SHOWNORMAL = 1u,
		SW_NORMAL = 1u,
		SW_SHOWMINIMIZED = 2u,
		SW_SHOWMAXIMIZED = 3u,
		SW_MAXIMIZE = 3u,
		SW_SHOWNOACTIVATE = 4u,
		SW_SHOW = 5u,
		SW_MINIMIZE = 6u,
		SW_SHOWMINNOACTIVE = 7u,
		SW_SHOWNA = 8u,
		SW_RESTORE = 9u,
		SW_SHOWDEFAULT = 0xAu,
		SW_MAX = 0xAu
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
	private struct _WIN32_FIND_DATAW
	{
		public uint dwFileAttributes;

		public _FILETIME ftCreationTime;

		public _FILETIME ftLastAccessTime;

		public _FILETIME ftLastWriteTime;

		public uint nFileSizeHigh;

		public uint nFileSizeLow;

		public uint dwReserved0;

		public uint dwReserved1;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string cFileName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct _WIN32_FIND_DATAA
	{
		public uint dwFileAttributes;

		public _FILETIME ftCreationTime;

		public _FILETIME ftLastAccessTime;

		public _FILETIME ftLastWriteTime;

		public uint nFileSizeHigh;

		public uint nFileSizeLow;

		public uint dwReserved0;

		public uint dwReserved1;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string cFileName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct _FILETIME
	{
		public uint dwLowDateTime;

		public uint dwHighDateTime;
	}

	private class UnManagedMethods
	{
		[DllImport("Shell32", CharSet = CharSet.Auto)]
		internal static extern int ExtractIconEx([MarshalAs(UnmanagedType.LPTStr)] string lpszFile, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, int nIcons);

		[DllImport("user32")]
		internal static extern int DestroyIcon(IntPtr hIcon);
	}

	[Flags]
	public enum EShellLinkResolveFlags : uint
	{
		SLR_ANY_MATCH = 2u,
		SLR_INVOKE_MSI = 0x80u,
		SLR_NOLINKINFO = 0x40u,
		SLR_NO_UI = 1u,
		SLR_NO_UI_WITH_MSG_PUMP = 0x101u,
		SLR_NOUPDATE = 8u,
		SLR_NOSEARCH = 0x10u,
		SLR_NOTRACK = 0x20u,
		SLR_UPDATE = 4u
	}

	public enum LinkDisplayMode : uint
	{
		edmNormal = 1u,
		edmMinimized = 7u,
		edmMaximized = 3u
	}

	private IShellLinkW linkW;

	private IShellLinkA linkA;

	private string shortcutFile = "";

	public string ShortCutFile
	{
		get
		{
			return shortcutFile;
		}
		set
		{
			shortcutFile = value;
		}
	}

	public Icon LargeIcon => getIcon(large: true);

	public Icon SmallIcon => getIcon(large: false);

	public string IconPath
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			return stringBuilder.ToString();
		}
		set
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			if (linkA == null)
			{
				linkW.SetIconLocation(value, piIcon);
			}
			else
			{
				linkA.SetIconLocation(value, piIcon);
			}
		}
	}

	public int IconIndex
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			return piIcon;
		}
		set
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			if (linkA == null)
			{
				linkW.SetIconLocation(stringBuilder.ToString(), value);
			}
			else
			{
				linkA.SetIconLocation(stringBuilder.ToString(), value);
			}
		}
	}

	public string Target
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			if (linkA == null)
			{
				_WIN32_FIND_DATAW pfd = default(_WIN32_FIND_DATAW);
				linkW.GetPath(stringBuilder, stringBuilder.Capacity, ref pfd, 2u);
			}
			else
			{
				_WIN32_FIND_DATAA pfd2 = default(_WIN32_FIND_DATAA);
				linkA.GetPath(stringBuilder, stringBuilder.Capacity, ref pfd2, 2u);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetPath(value);
			}
			else
			{
				linkA.SetPath(value);
			}
		}
	}

	public string WorkingDirectory
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			if (linkA == null)
			{
				linkW.GetWorkingDirectory(stringBuilder, stringBuilder.Capacity);
			}
			else
			{
				linkA.GetWorkingDirectory(stringBuilder, stringBuilder.Capacity);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetWorkingDirectory(value);
			}
			else
			{
				linkA.SetWorkingDirectory(value);
			}
		}
	}

	public string Description
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(1024, 1024);
			if (linkA == null)
			{
				linkW.GetDescription(stringBuilder, stringBuilder.Capacity);
			}
			else
			{
				linkA.GetDescription(stringBuilder, stringBuilder.Capacity);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetDescription(value);
			}
			else
			{
				linkA.SetDescription(value);
			}
		}
	}

	public string Arguments
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			if (linkA == null)
			{
				linkW.GetArguments(stringBuilder, stringBuilder.Capacity);
			}
			else
			{
				linkA.GetArguments(stringBuilder, stringBuilder.Capacity);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetArguments(value);
			}
			else
			{
				linkA.SetArguments(value);
			}
		}
	}

	public LinkDisplayMode DisplayMode
	{
		get
		{
			uint piShowCmd = 0u;
			if (linkA == null)
			{
				linkW.GetShowCmd(out piShowCmd);
			}
			else
			{
				linkA.GetShowCmd(out piShowCmd);
			}
			return (LinkDisplayMode)piShowCmd;
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetShowCmd((uint)value);
			}
			else
			{
				linkA.SetShowCmd((uint)value);
			}
		}
	}

	public short HotKey
	{
		get
		{
			short pwHotkey = 0;
			if (linkA == null)
			{
				linkW.GetHotkey(out pwHotkey);
			}
			else
			{
				linkA.GetHotkey(out pwHotkey);
			}
			return pwHotkey;
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetHotkey(value);
			}
			else
			{
				linkA.SetHotkey(value);
			}
		}
	}

	public ShellLink()
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			linkW = (IShellLinkW)new CShellLink();
		}
		else
		{
			linkA = (IShellLinkA)new CShellLink();
		}
	}

	public ShellLink(string linkFile)
		: this()
	{
		Open(linkFile);
	}

	~ShellLink()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (linkW != null)
		{
			Marshal.ReleaseComObject(linkW);
			linkW = null;
		}
		if (linkA != null)
		{
			Marshal.ReleaseComObject(linkA);
			linkA = null;
		}
	}

	private Icon getIcon(bool large)
	{
		int piIcon = 0;
		StringBuilder stringBuilder = new StringBuilder(260, 260);
		if (linkA == null)
		{
			linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
		}
		else
		{
			linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
		}
		string text = stringBuilder.ToString();
		if (text.Length == 0)
		{
			FileIcon.SHGetFileInfoConstants sHGetFileInfoConstants = FileIcon.SHGetFileInfoConstants.SHGFI_ICON | FileIcon.SHGetFileInfoConstants.SHGFI_ATTRIBUTES;
			sHGetFileInfoConstants = ((!large) ? (sHGetFileInfoConstants | FileIcon.SHGetFileInfoConstants.SHGFI_SMALLICON) : (sHGetFileInfoConstants | FileIcon.SHGetFileInfoConstants.SHGFI_LARGEICON));
			return new FileIcon(Target, sHGetFileInfoConstants).ShellIcon;
		}
		IntPtr[] array = new IntPtr[1] { IntPtr.Zero };
		if (large)
		{
			UnManagedMethods.ExtractIconEx(text, piIcon, array, null, 1);
		}
		else
		{
			UnManagedMethods.ExtractIconEx(text, piIcon, null, array, 1);
		}
		Icon result = null;
		if (array[0] != IntPtr.Zero)
		{
			result = Icon.FromHandle(array[0]);
		}
		return result;
	}

	public void SetAppUserModelId(string appId)
	{
		IPropertyStore obj = (IPropertyStore)linkW;
		PROPERTYKEY key = PROPERTYKEY.PKEY_AppUserModel_ID;
		PropVariant pv = PropVariant.FromString(appId);
		obj.SetValue(ref key, ref pv);
	}

	public void SetToastActivatorCLSID(string clsid)
	{
		Guid toastActivatorCLSID = Guid.Parse(clsid);
		SetToastActivatorCLSID(toastActivatorCLSID);
	}

	public void SetToastActivatorCLSID(Guid clsid)
	{
		IPropertyStore propertyStore = (IPropertyStore)linkW;
		PROPERTYKEY key = PROPERTYKEY.PKEY_AppUserModel_ToastActivatorCLSID;
		PropVariant pv = PropVariant.FromGuid(clsid);
		try
		{
			Marshal.ThrowExceptionForHR(propertyStore.SetValue(ref key, ref pv));
			Marshal.ThrowExceptionForHR(propertyStore.Commit());
		}
		finally
		{
			pv.Clear();
		}
	}

	public void Save()
	{
		Save(shortcutFile);
	}

	public void Save(string linkFile)
	{
		if (linkA == null)
		{
			((IPersistFile)linkW).Save(linkFile, fRemember: true);
			shortcutFile = linkFile;
		}
		else
		{
			((IPersistFile)linkA).Save(linkFile, fRemember: true);
			shortcutFile = linkFile;
		}
	}

	public void Open(string linkFile)
	{
		Open(linkFile, IntPtr.Zero, EShellLinkResolveFlags.SLR_ANY_MATCH | EShellLinkResolveFlags.SLR_NO_UI, 1);
	}

	public void Open(string linkFile, IntPtr hWnd, EShellLinkResolveFlags resolveFlags)
	{
		Open(linkFile, hWnd, resolveFlags, 1);
	}

	public void Open(string linkFile, IntPtr hWnd, EShellLinkResolveFlags resolveFlags, ushort timeOut)
	{
		uint fFlags = (((resolveFlags & EShellLinkResolveFlags.SLR_NO_UI) != EShellLinkResolveFlags.SLR_NO_UI) ? ((uint)resolveFlags) : ((uint)resolveFlags | (uint)(timeOut << 16)));
		if (linkA == null)
		{
			((IPersistFile)linkW).Load(linkFile, 0u);
			linkW.Resolve(hWnd, fFlags);
			shortcutFile = linkFile;
		}
		else
		{
			((IPersistFile)linkA).Load(linkFile, 0u);
			linkA.Resolve(hWnd, fFlags);
			shortcutFile = linkFile;
		}
	}
}
