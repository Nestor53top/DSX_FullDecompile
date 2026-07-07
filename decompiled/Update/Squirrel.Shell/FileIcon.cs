using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Squirrel.Shell;

internal class FileIcon
{
	private struct SHFILEINFO
	{
		public IntPtr hIcon;

		public int iIcon;

		public int dwAttributes;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	}

	[Flags]
	public enum SHGetFileInfoConstants
	{
		SHGFI_ICON = 0x100,
		SHGFI_DISPLAYNAME = 0x200,
		SHGFI_TYPENAME = 0x400,
		SHGFI_ATTRIBUTES = 0x800,
		SHGFI_ICONLOCATION = 0x1000,
		SHGFI_EXETYPE = 0x2000,
		SHGFI_SYSICONINDEX = 0x4000,
		SHGFI_LINKOVERLAY = 0x8000,
		SHGFI_SELECTED = 0x10000,
		SHGFI_ATTR_SPECIFIED = 0x20000,
		SHGFI_LARGEICON = 0,
		SHGFI_SMALLICON = 1,
		SHGFI_OPENICON = 2,
		SHGFI_SHELLICONSIZE = 4,
		SHGFI_USEFILEATTRIBUTES = 0x10,
		SHGFI_ADDOVERLAYS = 0x20,
		SHGFI_OVERLAYINDEX = 0x40
	}

	private const int MAX_PATH = 260;

	private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 256;

	private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

	private const int FORMAT_MESSAGE_FROM_HMODULE = 2048;

	private const int FORMAT_MESSAGE_FROM_STRING = 1024;

	private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

	private const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

	private const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 255;

	private string fileName;

	private string displayName;

	private string typeName;

	private SHGetFileInfoConstants flags;

	private Icon fileIcon;

	public SHGetFileInfoConstants Flags
	{
		get
		{
			return flags;
		}
		set
		{
			flags = value;
		}
	}

	public string FileName
	{
		get
		{
			return fileName;
		}
		set
		{
			fileName = value;
		}
	}

	public Icon ShellIcon => fileIcon;

	public string DisplayName => displayName;

	public string TypeName => typeName;

	[DllImport("shell32")]
	private static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

	[DllImport("user32.dll")]
	private static extern int DestroyIcon(IntPtr hIcon);

	[DllImport("kernel32")]
	private static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, string lpBuffer, uint nSize, IntPtr argumentsLong);

	[DllImport("kernel32")]
	private static extern int GetLastError();

	public void GetInfo()
	{
		fileIcon = null;
		typeName = "";
		displayName = "";
		SHFILEINFO psfi = default(SHFILEINFO);
		uint cbFileInfo = (uint)Marshal.SizeOf(psfi.GetType());
		if (SHGetFileInfo(fileName, 0, ref psfi, cbFileInfo, (uint)flags) != IntPtr.Zero)
		{
			if (psfi.hIcon != IntPtr.Zero)
			{
				fileIcon = Icon.FromHandle(psfi.hIcon);
			}
			typeName = psfi.szTypeName;
			displayName = psfi.szDisplayName;
		}
		else
		{
			int lastError = GetLastError();
			Console.WriteLine("Error {0}", lastError);
			string text = new string('\0', 256);
			int num = FormatMessage(4608, IntPtr.Zero, lastError, 0, text, 256u, IntPtr.Zero);
			Console.WriteLine("Len {0} text {1}", num, text);
		}
	}

	public FileIcon()
	{
		flags = SHGetFileInfoConstants.SHGFI_ICON | SHGetFileInfoConstants.SHGFI_DISPLAYNAME | SHGetFileInfoConstants.SHGFI_TYPENAME | SHGetFileInfoConstants.SHGFI_ATTRIBUTES | SHGetFileInfoConstants.SHGFI_EXETYPE;
	}

	public FileIcon(string fileName)
		: this()
	{
		this.fileName = fileName;
		GetInfo();
	}

	public FileIcon(string fileName, SHGetFileInfoConstants flags)
	{
		this.fileName = fileName;
		this.flags = flags;
		GetInfo();
	}
}
