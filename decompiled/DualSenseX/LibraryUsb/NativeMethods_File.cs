using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace LibraryUsb;

public class NativeMethods_File
{
	public enum FileDesiredAccess : uint
	{
		GENERIC_READ = 2147483648u,
		GENERIC_WRITE = 1073741824u,
		GENERIC_EXECUTE = 536870912u,
		GENERIC_ALL = 268435456u
	}

	public enum FileShareMode : uint
	{
		FILE_SHARE_NONE = 0u,
		FILE_SHARE_READ = 1u,
		FILE_SHARE_WRITE = 2u,
		FILE_SHARE_READ_WRITE = 3u,
		FILE_SHARE_DELETE = 4u,
		FILE_SHARE_VALID_FLAGS = 7u
	}

	public enum FileCreationDisposition : uint
	{
		CREATE_NEW = 1u,
		CREATE_ALWAYS,
		OPEN_EXISTING,
		OPEN_ALWAYS,
		TRUNCATE_EXISTING
	}

	public enum FileFlagsAndAttributes : uint
	{
		FILE_ATTRIBUTE_READONLY = 1u,
		FILE_ATTRIBUTE_HIDDEN = 2u,
		FILE_ATTRIBUTE_SYSTEM = 4u,
		FILE_ATTRIBUTE_DIRECTORY = 16u,
		FILE_ATTRIBUTE_ARCHIVE = 32u,
		FILE_ATTRIBUTE_DEVICE = 64u,
		FILE_ATTRIBUTE_NORMAL = 128u,
		FILE_ATTRIBUTE_TEMPORARY = 256u,
		FILE_ATTRIBUTE_SPARSE_FILE = 512u,
		FILE_ATTRIBUTE_REPARSE_POINT = 1024u,
		FILE_ATTRIBUTE_COMPRESSED = 2048u,
		FILE_ATTRIBUTE_OFFLINE = 4096u,
		FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192u,
		FILE_ATTRIBUTE_ENCRYPTED = 16384u,
		FILE_ATTRIBUTE_INTEGRITY_STREAM = 32768u,
		FILE_ATTRIBUTE_VIRTUAL = 65536u,
		FILE_ATTRIBUTE_NO_SCRUB_DATA = 131072u,
		FILE_ATTRIBUTE_RECALL_ON_OPEN = 262144u,
		FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = 4194304u,
		FILE_FLAG_NORMAL = 0u,
		FILE_FLAG_OPEN_REQUIRING_OPLOCK = 262144u,
		FILE_FLAG_FIRST_PIPE_INSTANCE = 524288u,
		FILE_FLAG_OPEN_NO_RECALL = 1048576u,
		FILE_FLAG_OPEN_REPARSE_POINT = 2097152u,
		FILE_FLAG_SESSION_AWARE = 8388608u,
		FILE_FLAG_POSIX_SEMANTICS = 16777216u,
		FILE_FLAG_BACKUP_SEMANTICS = 33554432u,
		FILE_FLAG_DELETE_ON_CLOSE = 67108864u,
		FILE_FLAG_SEQUENTIAL_SCAN = 134217728u,
		FILE_FLAG_RANDOM_ACCESS = 268435456u,
		FILE_FLAG_NO_BUFFERING = 536870912u,
		FILE_FLAG_OVERLAPPED = 1073741824u,
		FILE_FLAG_WRITE_THROUGH = 2147483648u
	}

	public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

	[DllImport("kernel32.dll")]
	public static extern bool ReadFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesReaded, IntPtr lpOverlapped);

	[DllImport("kernel32.dll")]
	public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);

	[DllImport("kernel32.dll")]
	public static extern SafeFileHandle CreateFile(string lpFileName, FileDesiredAccess dwDesiredAccess, FileShareMode dwShareMode, IntPtr lpSecurityAttributes, FileCreationDisposition dwCreationDisposition, FileFlagsAndAttributes dwFlagsAndAttributes, uint hTemplateFile);

	[DllImport("kernel32.dll")]
	public static extern bool CloseHandle(IntPtr hObject);

	[DllImport("kernel32.dll")]
	public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);
}
