using System;

namespace SQLitePCL;

public interface IGetFunctionPointer
{
	IntPtr GetFunctionPointer(string name);
}
