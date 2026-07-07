namespace SQLitePCL;

public delegate void delegate_function_scalar(sqlite3_context ctx, object user_data, sqlite3_value[] args);
