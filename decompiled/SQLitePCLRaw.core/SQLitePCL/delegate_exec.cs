using System;

namespace SQLitePCL;

public delegate int delegate_exec(object user_data, IntPtr[] values, IntPtr[] names);
