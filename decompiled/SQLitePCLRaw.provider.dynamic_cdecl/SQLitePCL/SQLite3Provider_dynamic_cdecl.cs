using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SQLitePCL;

[Preserve(AllMembers = true)]
public sealed class SQLite3Provider_dynamic_cdecl : ISQLite3Provider
{
	private struct sqlite3_vfs
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int SQLiteDeleteDelegate(IntPtr pVfs, byte* zName, int syncDir);

		public int iVersion;

		public int szOsFile;

		public int mxPathname;

		public IntPtr pNext;

		public IntPtr zName;

		public IntPtr pAppData;

		public IntPtr xOpen;

		public SQLiteDeleteDelegate xDelete;

		public IntPtr xAccess;

		public IntPtr xFullPathname;

		public IntPtr xDlOpen;

		public IntPtr xDlError;

		public IntPtr xDlSym;

		public IntPtr xDlClose;

		public IntPtr xRandomness;

		public IntPtr xSleep;

		public IntPtr xCurrentTime;

		public IntPtr xGetLastError;
	}

	private static class NativeMethods
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_log(IntPtr pUserData, int errorCode, IntPtr pMessage);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_scalar_function(IntPtr context, int nArgs, IntPtr argsptr);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_agg_function_step(IntPtr context, int nArgs, IntPtr argsptr);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_agg_function_final(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_destroy(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int callback_collation(IntPtr puser, int len1, IntPtr pv1, int len2, IntPtr pv2);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_update(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int callback_commit(IntPtr puser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_profile(IntPtr puser, IntPtr statement, long elapsed);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int callback_progress_handler(IntPtr puser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int callback_authorizer(IntPtr puser, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_trace(IntPtr puser, IntPtr statement);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void callback_rollback(IntPtr puser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int callback_exec(IntPtr db, int n, IntPtr values, IntPtr names);

		public static MyDelegateTypes.sqlite3_close sqlite3_close;

		public static MyDelegateTypes.sqlite3_close_v2 sqlite3_close_v2;

		public static MyDelegateTypes.sqlite3_enable_shared_cache sqlite3_enable_shared_cache;

		public static MyDelegateTypes.sqlite3_interrupt sqlite3_interrupt;

		public static MyDelegateTypes.sqlite3_finalize sqlite3_finalize;

		public static MyDelegateTypes.sqlite3_reset sqlite3_reset;

		public static MyDelegateTypes.sqlite3_clear_bindings sqlite3_clear_bindings;

		public static MyDelegateTypes.sqlite3_stmt_status sqlite3_stmt_status;

		public static MyDelegateTypes.sqlite3_bind_parameter_name sqlite3_bind_parameter_name;

		public static MyDelegateTypes.sqlite3_column_database_name sqlite3_column_database_name;

		public static MyDelegateTypes.sqlite3_column_decltype sqlite3_column_decltype;

		public static MyDelegateTypes.sqlite3_column_name sqlite3_column_name;

		public static MyDelegateTypes.sqlite3_column_origin_name sqlite3_column_origin_name;

		public static MyDelegateTypes.sqlite3_column_table_name sqlite3_column_table_name;

		public static MyDelegateTypes.sqlite3_column_text sqlite3_column_text;

		public static MyDelegateTypes.sqlite3_errmsg sqlite3_errmsg;

		public static MyDelegateTypes.sqlite3_db_readonly sqlite3_db_readonly;

		public static MyDelegateTypes.sqlite3_db_filename sqlite3_db_filename;

		public static MyDelegateTypes.sqlite3_prepare_v2 sqlite3_prepare_v2;

		public static MyDelegateTypes.sqlite3_prepare_v3 sqlite3_prepare_v3;

		public static MyDelegateTypes.sqlite3_db_status sqlite3_db_status;

		public static MyDelegateTypes.sqlite3_complete sqlite3_complete;

		public static MyDelegateTypes.sqlite3_compileoption_used sqlite3_compileoption_used;

		public static MyDelegateTypes.sqlite3_compileoption_get sqlite3_compileoption_get;

		public static MyDelegateTypes.sqlite3_table_column_metadata sqlite3_table_column_metadata;

		public static MyDelegateTypes.sqlite3_value_text sqlite3_value_text;

		public static MyDelegateTypes.sqlite3_enable_load_extension sqlite3_enable_load_extension;

		public static MyDelegateTypes.sqlite3_load_extension sqlite3_load_extension;

		public static MyDelegateTypes.sqlite3_initialize sqlite3_initialize;

		public static MyDelegateTypes.sqlite3_shutdown sqlite3_shutdown;

		public static MyDelegateTypes.sqlite3_libversion sqlite3_libversion;

		public static MyDelegateTypes.sqlite3_libversion_number sqlite3_libversion_number;

		public static MyDelegateTypes.sqlite3_threadsafe sqlite3_threadsafe;

		public static MyDelegateTypes.sqlite3_sourceid sqlite3_sourceid;

		public static MyDelegateTypes.sqlite3_malloc sqlite3_malloc;

		public static MyDelegateTypes.sqlite3_realloc sqlite3_realloc;

		public static MyDelegateTypes.sqlite3_free sqlite3_free;

		public static MyDelegateTypes.sqlite3_stricmp sqlite3_stricmp;

		public static MyDelegateTypes.sqlite3_strnicmp sqlite3_strnicmp;

		public static MyDelegateTypes.sqlite3_open sqlite3_open;

		public static MyDelegateTypes.sqlite3_open_v2 sqlite3_open_v2;

		public static MyDelegateTypes.sqlite3_vfs_find sqlite3_vfs_find;

		public static MyDelegateTypes.sqlite3_last_insert_rowid sqlite3_last_insert_rowid;

		public static MyDelegateTypes.sqlite3_changes sqlite3_changes;

		public static MyDelegateTypes.sqlite3_total_changes sqlite3_total_changes;

		public static MyDelegateTypes.sqlite3_memory_used sqlite3_memory_used;

		public static MyDelegateTypes.sqlite3_memory_highwater sqlite3_memory_highwater;

		public static MyDelegateTypes.sqlite3_status sqlite3_status;

		public static MyDelegateTypes.sqlite3_busy_timeout sqlite3_busy_timeout;

		public static MyDelegateTypes.sqlite3_bind_blob sqlite3_bind_blob;

		public static MyDelegateTypes.sqlite3_bind_zeroblob sqlite3_bind_zeroblob;

		public static MyDelegateTypes.sqlite3_bind_double sqlite3_bind_double;

		public static MyDelegateTypes.sqlite3_bind_int sqlite3_bind_int;

		public static MyDelegateTypes.sqlite3_bind_int64 sqlite3_bind_int64;

		public static MyDelegateTypes.sqlite3_bind_null sqlite3_bind_null;

		public static MyDelegateTypes.sqlite3_bind_text sqlite3_bind_text;

		public static MyDelegateTypes.sqlite3_bind_parameter_count sqlite3_bind_parameter_count;

		public static MyDelegateTypes.sqlite3_bind_parameter_index sqlite3_bind_parameter_index;

		public static MyDelegateTypes.sqlite3_column_count sqlite3_column_count;

		public static MyDelegateTypes.sqlite3_data_count sqlite3_data_count;

		public static MyDelegateTypes.sqlite3_step sqlite3_step;

		public static MyDelegateTypes.sqlite3_sql sqlite3_sql;

		public static MyDelegateTypes.sqlite3_column_double sqlite3_column_double;

		public static MyDelegateTypes.sqlite3_column_int sqlite3_column_int;

		public static MyDelegateTypes.sqlite3_column_int64 sqlite3_column_int64;

		public static MyDelegateTypes.sqlite3_column_blob sqlite3_column_blob;

		public static MyDelegateTypes.sqlite3_column_bytes sqlite3_column_bytes;

		public static MyDelegateTypes.sqlite3_column_type sqlite3_column_type;

		public static MyDelegateTypes.sqlite3_aggregate_count sqlite3_aggregate_count;

		public static MyDelegateTypes.sqlite3_value_blob sqlite3_value_blob;

		public static MyDelegateTypes.sqlite3_value_bytes sqlite3_value_bytes;

		public static MyDelegateTypes.sqlite3_value_double sqlite3_value_double;

		public static MyDelegateTypes.sqlite3_value_int sqlite3_value_int;

		public static MyDelegateTypes.sqlite3_value_int64 sqlite3_value_int64;

		public static MyDelegateTypes.sqlite3_value_type sqlite3_value_type;

		public static MyDelegateTypes.sqlite3_user_data sqlite3_user_data;

		public static MyDelegateTypes.sqlite3_result_blob sqlite3_result_blob;

		public static MyDelegateTypes.sqlite3_result_double sqlite3_result_double;

		public static MyDelegateTypes.sqlite3_result_error sqlite3_result_error;

		public static MyDelegateTypes.sqlite3_result_int sqlite3_result_int;

		public static MyDelegateTypes.sqlite3_result_int64 sqlite3_result_int64;

		public static MyDelegateTypes.sqlite3_result_null sqlite3_result_null;

		public static MyDelegateTypes.sqlite3_result_text sqlite3_result_text;

		public static MyDelegateTypes.sqlite3_result_zeroblob sqlite3_result_zeroblob;

		public static MyDelegateTypes.sqlite3_result_error_toobig sqlite3_result_error_toobig;

		public static MyDelegateTypes.sqlite3_result_error_nomem sqlite3_result_error_nomem;

		public static MyDelegateTypes.sqlite3_result_error_code sqlite3_result_error_code;

		public static MyDelegateTypes.sqlite3_aggregate_context sqlite3_aggregate_context;

		public static MyDelegateTypes.sqlite3_key sqlite3_key;

		public static MyDelegateTypes.sqlite3_key_v2 sqlite3_key_v2;

		public static MyDelegateTypes.sqlite3_rekey sqlite3_rekey;

		public static MyDelegateTypes.sqlite3_rekey_v2 sqlite3_rekey_v2;

		public static MyDelegateTypes.sqlite3_config_none sqlite3_config_none;

		public static MyDelegateTypes.sqlite3_config_int sqlite3_config_int;

		public static MyDelegateTypes.sqlite3_config_log sqlite3_config_log;

		public static MyDelegateTypes.sqlite3_create_function_v2 sqlite3_create_function_v2;

		public static MyDelegateTypes.sqlite3_create_collation sqlite3_create_collation;

		public static MyDelegateTypes.sqlite3_update_hook sqlite3_update_hook;

		public static MyDelegateTypes.sqlite3_commit_hook sqlite3_commit_hook;

		public static MyDelegateTypes.sqlite3_profile sqlite3_profile;

		public static MyDelegateTypes.sqlite3_progress_handler sqlite3_progress_handler;

		public static MyDelegateTypes.sqlite3_trace sqlite3_trace;

		public static MyDelegateTypes.sqlite3_rollback_hook sqlite3_rollback_hook;

		public static MyDelegateTypes.sqlite3_db_handle sqlite3_db_handle;

		public static MyDelegateTypes.sqlite3_next_stmt sqlite3_next_stmt;

		public static MyDelegateTypes.sqlite3_stmt_busy sqlite3_stmt_busy;

		public static MyDelegateTypes.sqlite3_stmt_readonly sqlite3_stmt_readonly;

		public static MyDelegateTypes.sqlite3_exec sqlite3_exec;

		public static MyDelegateTypes.sqlite3_get_autocommit sqlite3_get_autocommit;

		public static MyDelegateTypes.sqlite3_extended_result_codes sqlite3_extended_result_codes;

		public static MyDelegateTypes.sqlite3_errcode sqlite3_errcode;

		public static MyDelegateTypes.sqlite3_extended_errcode sqlite3_extended_errcode;

		public static MyDelegateTypes.sqlite3_errstr sqlite3_errstr;

		public static MyDelegateTypes.sqlite3_log sqlite3_log;

		public static MyDelegateTypes.sqlite3_file_control sqlite3_file_control;

		public static MyDelegateTypes.sqlite3_backup_init sqlite3_backup_init;

		public static MyDelegateTypes.sqlite3_backup_step sqlite3_backup_step;

		public static MyDelegateTypes.sqlite3_backup_remaining sqlite3_backup_remaining;

		public static MyDelegateTypes.sqlite3_backup_pagecount sqlite3_backup_pagecount;

		public static MyDelegateTypes.sqlite3_backup_finish sqlite3_backup_finish;

		public static MyDelegateTypes.sqlite3_blob_open sqlite3_blob_open;

		public static MyDelegateTypes.sqlite3_blob_write sqlite3_blob_write;

		public static MyDelegateTypes.sqlite3_blob_read sqlite3_blob_read;

		public static MyDelegateTypes.sqlite3_blob_bytes sqlite3_blob_bytes;

		public static MyDelegateTypes.sqlite3_blob_reopen sqlite3_blob_reopen;

		public static MyDelegateTypes.sqlite3_blob_close sqlite3_blob_close;

		public static MyDelegateTypes.sqlite3_wal_autocheckpoint sqlite3_wal_autocheckpoint;

		public static MyDelegateTypes.sqlite3_wal_checkpoint sqlite3_wal_checkpoint;

		public static MyDelegateTypes.sqlite3_wal_checkpoint_v2 sqlite3_wal_checkpoint_v2;

		public static MyDelegateTypes.sqlite3_set_authorizer sqlite3_set_authorizer;

		public static MyDelegateTypes.sqlite3_win32_set_directory8 sqlite3_win32_set_directory8;

		private static Delegate Load(IGetFunctionPointer gf, Type delegate_type)
		{
			string name = delegate_type.Name;
			foreach (Attribute customAttribute in delegate_type.GetTypeInfo().GetCustomAttributes())
			{
				if (customAttribute.GetType() == typeof(EntryPointAttribute))
				{
					name = (customAttribute as EntryPointAttribute).Name;
				}
			}
			IntPtr functionPointer = gf.GetFunctionPointer(name);
			if (functionPointer != IntPtr.Zero)
			{
				return Marshal.GetDelegateForFunctionPointer(functionPointer, delegate_type);
			}
			return null;
		}

		public static void Setup(IGetFunctionPointer gf)
		{
			sqlite3_close = (MyDelegateTypes.sqlite3_close)Load(gf, typeof(MyDelegateTypes.sqlite3_close));
			sqlite3_close_v2 = (MyDelegateTypes.sqlite3_close_v2)Load(gf, typeof(MyDelegateTypes.sqlite3_close_v2));
			sqlite3_enable_shared_cache = (MyDelegateTypes.sqlite3_enable_shared_cache)Load(gf, typeof(MyDelegateTypes.sqlite3_enable_shared_cache));
			sqlite3_interrupt = (MyDelegateTypes.sqlite3_interrupt)Load(gf, typeof(MyDelegateTypes.sqlite3_interrupt));
			sqlite3_finalize = (MyDelegateTypes.sqlite3_finalize)Load(gf, typeof(MyDelegateTypes.sqlite3_finalize));
			sqlite3_reset = (MyDelegateTypes.sqlite3_reset)Load(gf, typeof(MyDelegateTypes.sqlite3_reset));
			sqlite3_clear_bindings = (MyDelegateTypes.sqlite3_clear_bindings)Load(gf, typeof(MyDelegateTypes.sqlite3_clear_bindings));
			sqlite3_stmt_status = (MyDelegateTypes.sqlite3_stmt_status)Load(gf, typeof(MyDelegateTypes.sqlite3_stmt_status));
			sqlite3_bind_parameter_name = (MyDelegateTypes.sqlite3_bind_parameter_name)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_parameter_name));
			sqlite3_column_database_name = (MyDelegateTypes.sqlite3_column_database_name)Load(gf, typeof(MyDelegateTypes.sqlite3_column_database_name));
			sqlite3_column_decltype = (MyDelegateTypes.sqlite3_column_decltype)Load(gf, typeof(MyDelegateTypes.sqlite3_column_decltype));
			sqlite3_column_name = (MyDelegateTypes.sqlite3_column_name)Load(gf, typeof(MyDelegateTypes.sqlite3_column_name));
			sqlite3_column_origin_name = (MyDelegateTypes.sqlite3_column_origin_name)Load(gf, typeof(MyDelegateTypes.sqlite3_column_origin_name));
			sqlite3_column_table_name = (MyDelegateTypes.sqlite3_column_table_name)Load(gf, typeof(MyDelegateTypes.sqlite3_column_table_name));
			sqlite3_column_text = (MyDelegateTypes.sqlite3_column_text)Load(gf, typeof(MyDelegateTypes.sqlite3_column_text));
			sqlite3_errmsg = (MyDelegateTypes.sqlite3_errmsg)Load(gf, typeof(MyDelegateTypes.sqlite3_errmsg));
			sqlite3_db_readonly = (MyDelegateTypes.sqlite3_db_readonly)Load(gf, typeof(MyDelegateTypes.sqlite3_db_readonly));
			sqlite3_db_filename = (MyDelegateTypes.sqlite3_db_filename)Load(gf, typeof(MyDelegateTypes.sqlite3_db_filename));
			sqlite3_prepare_v2 = (MyDelegateTypes.sqlite3_prepare_v2)Load(gf, typeof(MyDelegateTypes.sqlite3_prepare_v2));
			sqlite3_prepare_v3 = (MyDelegateTypes.sqlite3_prepare_v3)Load(gf, typeof(MyDelegateTypes.sqlite3_prepare_v3));
			sqlite3_db_status = (MyDelegateTypes.sqlite3_db_status)Load(gf, typeof(MyDelegateTypes.sqlite3_db_status));
			sqlite3_complete = (MyDelegateTypes.sqlite3_complete)Load(gf, typeof(MyDelegateTypes.sqlite3_complete));
			sqlite3_compileoption_used = (MyDelegateTypes.sqlite3_compileoption_used)Load(gf, typeof(MyDelegateTypes.sqlite3_compileoption_used));
			sqlite3_compileoption_get = (MyDelegateTypes.sqlite3_compileoption_get)Load(gf, typeof(MyDelegateTypes.sqlite3_compileoption_get));
			sqlite3_table_column_metadata = (MyDelegateTypes.sqlite3_table_column_metadata)Load(gf, typeof(MyDelegateTypes.sqlite3_table_column_metadata));
			sqlite3_value_text = (MyDelegateTypes.sqlite3_value_text)Load(gf, typeof(MyDelegateTypes.sqlite3_value_text));
			sqlite3_enable_load_extension = (MyDelegateTypes.sqlite3_enable_load_extension)Load(gf, typeof(MyDelegateTypes.sqlite3_enable_load_extension));
			sqlite3_load_extension = (MyDelegateTypes.sqlite3_load_extension)Load(gf, typeof(MyDelegateTypes.sqlite3_load_extension));
			sqlite3_initialize = (MyDelegateTypes.sqlite3_initialize)Load(gf, typeof(MyDelegateTypes.sqlite3_initialize));
			sqlite3_shutdown = (MyDelegateTypes.sqlite3_shutdown)Load(gf, typeof(MyDelegateTypes.sqlite3_shutdown));
			sqlite3_libversion = (MyDelegateTypes.sqlite3_libversion)Load(gf, typeof(MyDelegateTypes.sqlite3_libversion));
			sqlite3_libversion_number = (MyDelegateTypes.sqlite3_libversion_number)Load(gf, typeof(MyDelegateTypes.sqlite3_libversion_number));
			sqlite3_threadsafe = (MyDelegateTypes.sqlite3_threadsafe)Load(gf, typeof(MyDelegateTypes.sqlite3_threadsafe));
			sqlite3_sourceid = (MyDelegateTypes.sqlite3_sourceid)Load(gf, typeof(MyDelegateTypes.sqlite3_sourceid));
			sqlite3_malloc = (MyDelegateTypes.sqlite3_malloc)Load(gf, typeof(MyDelegateTypes.sqlite3_malloc));
			sqlite3_realloc = (MyDelegateTypes.sqlite3_realloc)Load(gf, typeof(MyDelegateTypes.sqlite3_realloc));
			sqlite3_free = (MyDelegateTypes.sqlite3_free)Load(gf, typeof(MyDelegateTypes.sqlite3_free));
			sqlite3_stricmp = (MyDelegateTypes.sqlite3_stricmp)Load(gf, typeof(MyDelegateTypes.sqlite3_stricmp));
			sqlite3_strnicmp = (MyDelegateTypes.sqlite3_strnicmp)Load(gf, typeof(MyDelegateTypes.sqlite3_strnicmp));
			sqlite3_open = (MyDelegateTypes.sqlite3_open)Load(gf, typeof(MyDelegateTypes.sqlite3_open));
			sqlite3_open_v2 = (MyDelegateTypes.sqlite3_open_v2)Load(gf, typeof(MyDelegateTypes.sqlite3_open_v2));
			sqlite3_vfs_find = (MyDelegateTypes.sqlite3_vfs_find)Load(gf, typeof(MyDelegateTypes.sqlite3_vfs_find));
			sqlite3_last_insert_rowid = (MyDelegateTypes.sqlite3_last_insert_rowid)Load(gf, typeof(MyDelegateTypes.sqlite3_last_insert_rowid));
			sqlite3_changes = (MyDelegateTypes.sqlite3_changes)Load(gf, typeof(MyDelegateTypes.sqlite3_changes));
			sqlite3_total_changes = (MyDelegateTypes.sqlite3_total_changes)Load(gf, typeof(MyDelegateTypes.sqlite3_total_changes));
			sqlite3_memory_used = (MyDelegateTypes.sqlite3_memory_used)Load(gf, typeof(MyDelegateTypes.sqlite3_memory_used));
			sqlite3_memory_highwater = (MyDelegateTypes.sqlite3_memory_highwater)Load(gf, typeof(MyDelegateTypes.sqlite3_memory_highwater));
			sqlite3_status = (MyDelegateTypes.sqlite3_status)Load(gf, typeof(MyDelegateTypes.sqlite3_status));
			sqlite3_busy_timeout = (MyDelegateTypes.sqlite3_busy_timeout)Load(gf, typeof(MyDelegateTypes.sqlite3_busy_timeout));
			sqlite3_bind_blob = (MyDelegateTypes.sqlite3_bind_blob)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_blob));
			sqlite3_bind_zeroblob = (MyDelegateTypes.sqlite3_bind_zeroblob)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_zeroblob));
			sqlite3_bind_double = (MyDelegateTypes.sqlite3_bind_double)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_double));
			sqlite3_bind_int = (MyDelegateTypes.sqlite3_bind_int)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_int));
			sqlite3_bind_int64 = (MyDelegateTypes.sqlite3_bind_int64)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_int64));
			sqlite3_bind_null = (MyDelegateTypes.sqlite3_bind_null)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_null));
			sqlite3_bind_text = (MyDelegateTypes.sqlite3_bind_text)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_text));
			sqlite3_bind_parameter_count = (MyDelegateTypes.sqlite3_bind_parameter_count)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_parameter_count));
			sqlite3_bind_parameter_index = (MyDelegateTypes.sqlite3_bind_parameter_index)Load(gf, typeof(MyDelegateTypes.sqlite3_bind_parameter_index));
			sqlite3_column_count = (MyDelegateTypes.sqlite3_column_count)Load(gf, typeof(MyDelegateTypes.sqlite3_column_count));
			sqlite3_data_count = (MyDelegateTypes.sqlite3_data_count)Load(gf, typeof(MyDelegateTypes.sqlite3_data_count));
			sqlite3_step = (MyDelegateTypes.sqlite3_step)Load(gf, typeof(MyDelegateTypes.sqlite3_step));
			sqlite3_sql = (MyDelegateTypes.sqlite3_sql)Load(gf, typeof(MyDelegateTypes.sqlite3_sql));
			sqlite3_column_double = (MyDelegateTypes.sqlite3_column_double)Load(gf, typeof(MyDelegateTypes.sqlite3_column_double));
			sqlite3_column_int = (MyDelegateTypes.sqlite3_column_int)Load(gf, typeof(MyDelegateTypes.sqlite3_column_int));
			sqlite3_column_int64 = (MyDelegateTypes.sqlite3_column_int64)Load(gf, typeof(MyDelegateTypes.sqlite3_column_int64));
			sqlite3_column_blob = (MyDelegateTypes.sqlite3_column_blob)Load(gf, typeof(MyDelegateTypes.sqlite3_column_blob));
			sqlite3_column_bytes = (MyDelegateTypes.sqlite3_column_bytes)Load(gf, typeof(MyDelegateTypes.sqlite3_column_bytes));
			sqlite3_column_type = (MyDelegateTypes.sqlite3_column_type)Load(gf, typeof(MyDelegateTypes.sqlite3_column_type));
			sqlite3_aggregate_count = (MyDelegateTypes.sqlite3_aggregate_count)Load(gf, typeof(MyDelegateTypes.sqlite3_aggregate_count));
			sqlite3_value_blob = (MyDelegateTypes.sqlite3_value_blob)Load(gf, typeof(MyDelegateTypes.sqlite3_value_blob));
			sqlite3_value_bytes = (MyDelegateTypes.sqlite3_value_bytes)Load(gf, typeof(MyDelegateTypes.sqlite3_value_bytes));
			sqlite3_value_double = (MyDelegateTypes.sqlite3_value_double)Load(gf, typeof(MyDelegateTypes.sqlite3_value_double));
			sqlite3_value_int = (MyDelegateTypes.sqlite3_value_int)Load(gf, typeof(MyDelegateTypes.sqlite3_value_int));
			sqlite3_value_int64 = (MyDelegateTypes.sqlite3_value_int64)Load(gf, typeof(MyDelegateTypes.sqlite3_value_int64));
			sqlite3_value_type = (MyDelegateTypes.sqlite3_value_type)Load(gf, typeof(MyDelegateTypes.sqlite3_value_type));
			sqlite3_user_data = (MyDelegateTypes.sqlite3_user_data)Load(gf, typeof(MyDelegateTypes.sqlite3_user_data));
			sqlite3_result_blob = (MyDelegateTypes.sqlite3_result_blob)Load(gf, typeof(MyDelegateTypes.sqlite3_result_blob));
			sqlite3_result_double = (MyDelegateTypes.sqlite3_result_double)Load(gf, typeof(MyDelegateTypes.sqlite3_result_double));
			sqlite3_result_error = (MyDelegateTypes.sqlite3_result_error)Load(gf, typeof(MyDelegateTypes.sqlite3_result_error));
			sqlite3_result_int = (MyDelegateTypes.sqlite3_result_int)Load(gf, typeof(MyDelegateTypes.sqlite3_result_int));
			sqlite3_result_int64 = (MyDelegateTypes.sqlite3_result_int64)Load(gf, typeof(MyDelegateTypes.sqlite3_result_int64));
			sqlite3_result_null = (MyDelegateTypes.sqlite3_result_null)Load(gf, typeof(MyDelegateTypes.sqlite3_result_null));
			sqlite3_result_text = (MyDelegateTypes.sqlite3_result_text)Load(gf, typeof(MyDelegateTypes.sqlite3_result_text));
			sqlite3_result_zeroblob = (MyDelegateTypes.sqlite3_result_zeroblob)Load(gf, typeof(MyDelegateTypes.sqlite3_result_zeroblob));
			sqlite3_result_error_toobig = (MyDelegateTypes.sqlite3_result_error_toobig)Load(gf, typeof(MyDelegateTypes.sqlite3_result_error_toobig));
			sqlite3_result_error_nomem = (MyDelegateTypes.sqlite3_result_error_nomem)Load(gf, typeof(MyDelegateTypes.sqlite3_result_error_nomem));
			sqlite3_result_error_code = (MyDelegateTypes.sqlite3_result_error_code)Load(gf, typeof(MyDelegateTypes.sqlite3_result_error_code));
			sqlite3_aggregate_context = (MyDelegateTypes.sqlite3_aggregate_context)Load(gf, typeof(MyDelegateTypes.sqlite3_aggregate_context));
			sqlite3_key = (MyDelegateTypes.sqlite3_key)Load(gf, typeof(MyDelegateTypes.sqlite3_key));
			sqlite3_key_v2 = (MyDelegateTypes.sqlite3_key_v2)Load(gf, typeof(MyDelegateTypes.sqlite3_key_v2));
			sqlite3_rekey = (MyDelegateTypes.sqlite3_rekey)Load(gf, typeof(MyDelegateTypes.sqlite3_rekey));
			sqlite3_rekey_v2 = (MyDelegateTypes.sqlite3_rekey_v2)Load(gf, typeof(MyDelegateTypes.sqlite3_rekey_v2));
			sqlite3_config_none = (MyDelegateTypes.sqlite3_config_none)Load(gf, typeof(MyDelegateTypes.sqlite3_config_none));
			sqlite3_config_int = (MyDelegateTypes.sqlite3_config_int)Load(gf, typeof(MyDelegateTypes.sqlite3_config_int));
			sqlite3_config_log = (MyDelegateTypes.sqlite3_config_log)Load(gf, typeof(MyDelegateTypes.sqlite3_config_log));
			sqlite3_create_function_v2 = (MyDelegateTypes.sqlite3_create_function_v2)Load(gf, typeof(MyDelegateTypes.sqlite3_create_function_v2));
			sqlite3_create_collation = (MyDelegateTypes.sqlite3_create_collation)Load(gf, typeof(MyDelegateTypes.sqlite3_create_collation));
			sqlite3_update_hook = (MyDelegateTypes.sqlite3_update_hook)Load(gf, typeof(MyDelegateTypes.sqlite3_update_hook));
			sqlite3_commit_hook = (MyDelegateTypes.sqlite3_commit_hook)Load(gf, typeof(MyDelegateTypes.sqlite3_commit_hook));
			sqlite3_profile = (MyDelegateTypes.sqlite3_profile)Load(gf, typeof(MyDelegateTypes.sqlite3_profile));
			sqlite3_progress_handler = (MyDelegateTypes.sqlite3_progress_handler)Load(gf, typeof(MyDelegateTypes.sqlite3_progress_handler));
			sqlite3_trace = (MyDelegateTypes.sqlite3_trace)Load(gf, typeof(MyDelegateTypes.sqlite3_trace));
			sqlite3_rollback_hook = (MyDelegateTypes.sqlite3_rollback_hook)Load(gf, typeof(MyDelegateTypes.sqlite3_rollback_hook));
			sqlite3_db_handle = (MyDelegateTypes.sqlite3_db_handle)Load(gf, typeof(MyDelegateTypes.sqlite3_db_handle));
			sqlite3_next_stmt = (MyDelegateTypes.sqlite3_next_stmt)Load(gf, typeof(MyDelegateTypes.sqlite3_next_stmt));
			sqlite3_stmt_busy = (MyDelegateTypes.sqlite3_stmt_busy)Load(gf, typeof(MyDelegateTypes.sqlite3_stmt_busy));
			sqlite3_stmt_readonly = (MyDelegateTypes.sqlite3_stmt_readonly)Load(gf, typeof(MyDelegateTypes.sqlite3_stmt_readonly));
			sqlite3_exec = (MyDelegateTypes.sqlite3_exec)Load(gf, typeof(MyDelegateTypes.sqlite3_exec));
			sqlite3_get_autocommit = (MyDelegateTypes.sqlite3_get_autocommit)Load(gf, typeof(MyDelegateTypes.sqlite3_get_autocommit));
			sqlite3_extended_result_codes = (MyDelegateTypes.sqlite3_extended_result_codes)Load(gf, typeof(MyDelegateTypes.sqlite3_extended_result_codes));
			sqlite3_errcode = (MyDelegateTypes.sqlite3_errcode)Load(gf, typeof(MyDelegateTypes.sqlite3_errcode));
			sqlite3_extended_errcode = (MyDelegateTypes.sqlite3_extended_errcode)Load(gf, typeof(MyDelegateTypes.sqlite3_extended_errcode));
			sqlite3_errstr = (MyDelegateTypes.sqlite3_errstr)Load(gf, typeof(MyDelegateTypes.sqlite3_errstr));
			sqlite3_log = (MyDelegateTypes.sqlite3_log)Load(gf, typeof(MyDelegateTypes.sqlite3_log));
			sqlite3_file_control = (MyDelegateTypes.sqlite3_file_control)Load(gf, typeof(MyDelegateTypes.sqlite3_file_control));
			sqlite3_backup_init = (MyDelegateTypes.sqlite3_backup_init)Load(gf, typeof(MyDelegateTypes.sqlite3_backup_init));
			sqlite3_backup_step = (MyDelegateTypes.sqlite3_backup_step)Load(gf, typeof(MyDelegateTypes.sqlite3_backup_step));
			sqlite3_backup_remaining = (MyDelegateTypes.sqlite3_backup_remaining)Load(gf, typeof(MyDelegateTypes.sqlite3_backup_remaining));
			sqlite3_backup_pagecount = (MyDelegateTypes.sqlite3_backup_pagecount)Load(gf, typeof(MyDelegateTypes.sqlite3_backup_pagecount));
			sqlite3_backup_finish = (MyDelegateTypes.sqlite3_backup_finish)Load(gf, typeof(MyDelegateTypes.sqlite3_backup_finish));
			sqlite3_blob_open = (MyDelegateTypes.sqlite3_blob_open)Load(gf, typeof(MyDelegateTypes.sqlite3_blob_open));
			sqlite3_blob_write = (MyDelegateTypes.sqlite3_blob_write)Load(gf, typeof(MyDelegateTypes.sqlite3_blob_write));
			sqlite3_blob_read = (MyDelegateTypes.sqlite3_blob_read)Load(gf, typeof(MyDelegateTypes.sqlite3_blob_read));
			sqlite3_blob_bytes = (MyDelegateTypes.sqlite3_blob_bytes)Load(gf, typeof(MyDelegateTypes.sqlite3_blob_bytes));
			sqlite3_blob_reopen = (MyDelegateTypes.sqlite3_blob_reopen)Load(gf, typeof(MyDelegateTypes.sqlite3_blob_reopen));
			sqlite3_blob_close = (MyDelegateTypes.sqlite3_blob_close)Load(gf, typeof(MyDelegateTypes.sqlite3_blob_close));
			sqlite3_wal_autocheckpoint = (MyDelegateTypes.sqlite3_wal_autocheckpoint)Load(gf, typeof(MyDelegateTypes.sqlite3_wal_autocheckpoint));
			sqlite3_wal_checkpoint = (MyDelegateTypes.sqlite3_wal_checkpoint)Load(gf, typeof(MyDelegateTypes.sqlite3_wal_checkpoint));
			sqlite3_wal_checkpoint_v2 = (MyDelegateTypes.sqlite3_wal_checkpoint_v2)Load(gf, typeof(MyDelegateTypes.sqlite3_wal_checkpoint_v2));
			sqlite3_set_authorizer = (MyDelegateTypes.sqlite3_set_authorizer)Load(gf, typeof(MyDelegateTypes.sqlite3_set_authorizer));
			sqlite3_win32_set_directory8 = (MyDelegateTypes.sqlite3_win32_set_directory8)Load(gf, typeof(MyDelegateTypes.sqlite3_win32_set_directory8));
		}
	}

	private static class MyDelegateTypes
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_close(IntPtr db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_close_v2(IntPtr db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_enable_shared_cache(int enable);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_interrupt(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_finalize(IntPtr stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_reset(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_clear_bindings(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_column_database_name(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_column_decltype(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_column_name(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_column_origin_name(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_column_table_name(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_column_text(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_errmsg(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_db_readonly(sqlite3 db, byte* dbName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_db_filename(sqlite3 db, byte* att);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_prepare_v2(sqlite3 db, byte* pSql, int nBytes, out IntPtr stmt, out byte* ptrRemain);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_prepare_v3(sqlite3 db, byte* pSql, int nBytes, uint flags, out IntPtr stmt, out byte* ptrRemain);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_complete(byte* pSql);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_compileoption_used(byte* pSql);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_compileoption_get(int n);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_table_column_metadata(sqlite3 db, byte* dbName, byte* tblName, byte* colName, out byte* ptrDataType, out byte* ptrCollSeq, out int notNull, out int primaryKey, out int autoInc);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_value_text(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_enable_load_extension(sqlite3 db, int enable);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_load_extension(sqlite3 db, byte[] fileName, byte[] procName, ref IntPtr pError);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_initialize();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_shutdown();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_libversion();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_libversion_number();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_threadsafe();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_sourceid();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_malloc(int n);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_realloc(IntPtr p, int n);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_free(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_stricmp(IntPtr p, IntPtr q);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_strnicmp(IntPtr p, IntPtr q, int n);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_open(byte* filename, out IntPtr db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_open_v2(byte* filename, out IntPtr db, int flags, byte* vfs);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate IntPtr sqlite3_vfs_find(byte* vfs);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate long sqlite3_last_insert_rowid(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_changes(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_total_changes(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate long sqlite3_memory_used();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate long sqlite3_memory_highwater(int resetFlag);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_busy_timeout(sqlite3 db, int ms);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_bind_blob(sqlite3_stmt stmt, int index, byte* val, int nSize, IntPtr nTransient);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_bind_null(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_bind_text(sqlite3_stmt stmt, int index, byte* val, int nlen, IntPtr pvReserved);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_bind_parameter_count(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_bind_parameter_index(sqlite3_stmt stmt, byte* strName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_column_count(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_data_count(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_step(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_sql(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate double sqlite3_column_double(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_column_int(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate long sqlite3_column_int64(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_column_blob(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_column_bytes(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_column_type(sqlite3_stmt stmt, int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_aggregate_count(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_value_blob(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_value_bytes(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate double sqlite3_value_double(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_value_int(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate long sqlite3_value_int64(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_value_type(IntPtr p);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_user_data(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_blob(IntPtr context, IntPtr val, int nSize, IntPtr pvReserved);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_double(IntPtr context, double val);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void sqlite3_result_error(IntPtr context, byte* strErr, int nLen);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_int(IntPtr context, int val);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_int64(IntPtr context, long val);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_null(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void sqlite3_result_text(IntPtr context, byte* val, int nLen, IntPtr pvReserved);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_zeroblob(IntPtr context, int n);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_error_toobig(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_error_nomem(IntPtr context);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void sqlite3_result_error_code(IntPtr context, int code);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_key(sqlite3 db, byte* key, int keylen);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_key_v2(sqlite3 db, byte* dbname, byte* key, int keylen);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_rekey(sqlite3 db, byte* key, int keylen);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_rekey_v2(sqlite3 db, byte* dbname, byte* key, int keylen);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[EntryPoint("sqlite3_config")]
		public delegate int sqlite3_config_none(int op);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[EntryPoint("sqlite3_config")]
		public delegate int sqlite3_config_int(int op, int val);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[EntryPoint("sqlite3_config")]
		public delegate int sqlite3_config_log(int op, NativeMethods.callback_log func, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_create_collation(sqlite3 db, byte[] strName, int nType, hook_handle pvUser, NativeMethods.callback_collation func);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_update_hook(sqlite3 db, NativeMethods.callback_update func, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_commit_hook(sqlite3 db, NativeMethods.callback_commit func, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_profile(sqlite3 db, NativeMethods.callback_profile func, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_progress_handler(sqlite3 db, int instructions, NativeMethods.callback_progress_handler func, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_trace(sqlite3 db, NativeMethods.callback_trace func, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_rollback_hook(sqlite3 db, NativeMethods.callback_rollback func, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_db_handle(IntPtr stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr sqlite3_next_stmt(sqlite3 db, IntPtr stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_stmt_busy(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_stmt_readonly(sqlite3_stmt stmt);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_exec(sqlite3 db, byte* strSql, NativeMethods.callback_exec cb, hook_handle pvParam, out IntPtr errMsg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_get_autocommit(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_extended_result_codes(sqlite3 db, int onoff);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_errcode(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_extended_errcode(sqlite3 db);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate byte* sqlite3_errstr(int rc);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void sqlite3_log(int iErrCode, byte* zFormat);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_file_control(sqlite3 db, byte[] zDbName, int op, IntPtr pArg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate sqlite3_backup sqlite3_backup_init(sqlite3 destDb, byte* zDestName, sqlite3 sourceDb, byte* zSourceName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_backup_step(sqlite3_backup backup, int nPage);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_backup_remaining(sqlite3_backup backup);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_backup_pagecount(sqlite3_backup backup);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_backup_finish(IntPtr backup);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_blob_open(sqlite3 db, byte* sdb, byte* table, byte* col, long rowid, int flags, out sqlite3_blob blob);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_blob_write(sqlite3_blob blob, byte* b, int n, int offset);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_blob_read(sqlite3_blob blob, byte* b, int n, int offset);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_blob_bytes(sqlite3_blob blob);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_blob_reopen(sqlite3_blob blob, long rowid);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_blob_close(IntPtr blob);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_wal_autocheckpoint(sqlite3 db, int n);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_wal_checkpoint(sqlite3 db, byte* dbName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_wal_checkpoint_v2(sqlite3 db, byte* dbName, int eMode, out int logSize, out int framesCheckPointed);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_set_authorizer(sqlite3 db, NativeMethods.callback_authorizer cb, hook_handle pvUser);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate int sqlite3_win32_set_directory8(uint directoryType, byte* directoryPath);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int sqlite3_create_function_v2(sqlite3 db, byte[] strName, int nArgs, int nType, hook_handle pvUser, NativeMethods.callback_scalar_function func, NativeMethods.callback_agg_function_step fstep, NativeMethods.callback_agg_function_final ffinal, NativeMethods.callback_destroy fdestroy);
	}

	private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

	private static string _native_library_name;

	private readonly NativeMethods.callback_commit commit_hook_bridge = commit_hook_bridge_impl;

	private readonly NativeMethods.callback_scalar_function scalar_function_hook_bridge = scalar_function_hook_bridge_impl;

	private static IDisposable disp_log_hook_handle;

	private readonly NativeMethods.callback_log log_hook_bridge = log_hook_bridge_impl;

	private NativeMethods.callback_agg_function_step agg_function_hook_bridge_step = agg_function_hook_bridge_step_impl;

	private NativeMethods.callback_agg_function_final agg_function_hook_bridge_final = agg_function_hook_bridge_final_impl;

	private readonly NativeMethods.callback_collation collation_hook_bridge = collation_hook_bridge_impl;

	private readonly NativeMethods.callback_update update_hook_bridge = update_hook_bridge_impl;

	private readonly NativeMethods.callback_rollback rollback_hook_bridge = rollback_hook_bridge_impl;

	private readonly NativeMethods.callback_trace trace_hook_bridge = trace_hook_bridge_impl;

	private readonly NativeMethods.callback_profile profile_hook_bridge = profile_hook_bridge_impl;

	private readonly NativeMethods.callback_progress_handler progress_hook_bridge = progress_hook_bridge_impl;

	private readonly NativeMethods.callback_authorizer authorizer_hook_bridge = authorizer_hook_bridge_impl;

	public static void Setup(string name, IGetFunctionPointer gf)
	{
		_native_library_name = name;
		NativeMethods.Setup(gf);
	}

	string ISQLite3Provider.GetNativeLibraryName()
	{
		return _native_library_name;
	}

	private bool my_streq(IntPtr p, IntPtr q, int len)
	{
		return NativeMethods.sqlite3_strnicmp(p, q, len) == 0;
	}

	private hook_handles get_hooks(sqlite3 db)
	{
		return db.GetOrCreateExtra(() => new hook_handles(my_streq));
	}

	unsafe int ISQLite3Provider.sqlite3_win32_set_directory(int typ, utf8z path)
	{
		fixed (byte* directoryPath = path)
		{
			return NativeMethods.sqlite3_win32_set_directory8((uint)typ, directoryPath);
		}
	}

	unsafe int ISQLite3Provider.sqlite3_open(utf8z filename, out IntPtr db)
	{
		fixed (byte* filename2 = filename)
		{
			return NativeMethods.sqlite3_open(filename2, out db);
		}
	}

	unsafe int ISQLite3Provider.sqlite3_open_v2(utf8z filename, out IntPtr db, int flags, utf8z vfs)
	{
		fixed (byte* filename2 = filename)
		{
			fixed (byte* vfs2 = vfs)
			{
				return NativeMethods.sqlite3_open_v2(filename2, out db, flags, vfs2);
			}
		}
	}

	unsafe int ISQLite3Provider.sqlite3__vfs__delete(utf8z vfs, utf8z filename, int syncDir)
	{
		fixed (byte* vfs2 = vfs)
		{
			fixed (byte* zName = filename)
			{
				IntPtr intPtr = NativeMethods.sqlite3_vfs_find(vfs2);
				return ((sqlite3_vfs)Marshal.PtrToStructure(intPtr, typeof(sqlite3_vfs))).xDelete(intPtr, zName, 1);
			}
		}
	}

	int ISQLite3Provider.sqlite3_close_v2(IntPtr db)
	{
		return NativeMethods.sqlite3_close_v2(db);
	}

	int ISQLite3Provider.sqlite3_close(IntPtr db)
	{
		return NativeMethods.sqlite3_close(db);
	}

	void ISQLite3Provider.sqlite3_free(IntPtr p)
	{
		NativeMethods.sqlite3_free(p);
	}

	int ISQLite3Provider.sqlite3_stricmp(IntPtr p, IntPtr q)
	{
		return NativeMethods.sqlite3_stricmp(p, q);
	}

	int ISQLite3Provider.sqlite3_strnicmp(IntPtr p, IntPtr q, int n)
	{
		return NativeMethods.sqlite3_strnicmp(p, q, n);
	}

	int ISQLite3Provider.sqlite3_enable_shared_cache(int enable)
	{
		return NativeMethods.sqlite3_enable_shared_cache(enable);
	}

	void ISQLite3Provider.sqlite3_interrupt(sqlite3 db)
	{
		NativeMethods.sqlite3_interrupt(db);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_exec))]
	private static int exec_hook_bridge(IntPtr p, int n, IntPtr values_ptr, IntPtr names_ptr)
	{
		return exec_hook_info.from_ptr(p).call(n, values_ptr, names_ptr);
	}

	unsafe int ISQLite3Provider.sqlite3_exec(sqlite3 db, utf8z sql, delegate_exec func, object user_data, out IntPtr errMsg)
	{
		NativeMethods.callback_exec cb;
		exec_hook_info target;
		if (func != null)
		{
			cb = exec_hook_bridge;
			target = new exec_hook_info(func, user_data);
		}
		else
		{
			cb = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		int result;
		fixed (byte* strSql = sql)
		{
			result = NativeMethods.sqlite3_exec(db, strSql, cb, hook_handle2, out errMsg);
		}
		hook_handle2.Dispose();
		return result;
	}

	unsafe int ISQLite3Provider.sqlite3_complete(utf8z sql)
	{
		fixed (byte* pSql = sql)
		{
			return NativeMethods.sqlite3_complete(pSql);
		}
	}

	unsafe utf8z ISQLite3Provider.sqlite3_compileoption_get(int n)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_compileoption_get(n));
	}

	unsafe int ISQLite3Provider.sqlite3_compileoption_used(utf8z s)
	{
		fixed (byte* pSql = s)
		{
			return NativeMethods.sqlite3_compileoption_used(pSql);
		}
	}

	unsafe int ISQLite3Provider.sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc)
	{
		fixed (byte* dbName2 = dbName)
		{
			fixed (byte* tblName2 = tblName)
			{
				fixed (byte* colName2 = colName)
				{
					byte* ptrDataType;
					byte* ptrCollSeq;
					int result = NativeMethods.sqlite3_table_column_metadata(db, dbName2, tblName2, colName2, out ptrDataType, out ptrCollSeq, out notNull, out primaryKey, out autoInc);
					dataType = utf8z.FromPtr(ptrDataType);
					collSeq = utf8z.FromPtr(ptrCollSeq);
					return result;
				}
			}
		}
	}

	unsafe int ISQLite3Provider.sqlite3_key(sqlite3 db, ReadOnlySpan<byte> k)
	{
		fixed (byte* key = k)
		{
			return NativeMethods.sqlite3_key(db, key, k.Length);
		}
	}

	unsafe int ISQLite3Provider.sqlite3_key_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
	{
		fixed (byte* key = k)
		{
			fixed (byte* dbname = name)
			{
				return NativeMethods.sqlite3_key_v2(db, dbname, key, k.Length);
			}
		}
	}

	unsafe int ISQLite3Provider.sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> k)
	{
		fixed (byte* key = k)
		{
			return NativeMethods.sqlite3_rekey(db, key, k.Length);
		}
	}

	unsafe int ISQLite3Provider.sqlite3_rekey_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
	{
		fixed (byte* key = k)
		{
			fixed (byte* dbname = name)
			{
				return NativeMethods.sqlite3_rekey_v2(db, dbname, key, k.Length);
			}
		}
	}

	unsafe int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out IntPtr stm, out ReadOnlySpan<byte> tail)
	{
		fixed (byte* ptr = sql)
		{
			byte* ptrRemain;
			int result = NativeMethods.sqlite3_prepare_v2(db, ptr, sql.Length, out stm, out ptrRemain);
			int num = (int)(ptrRemain - ptr);
			int num2 = sql.Length - num;
			if (num2 > 0)
			{
				tail = sql.Slice(num, num2);
				return result;
			}
			tail = ReadOnlySpan<byte>.Empty;
			return result;
		}
	}

	unsafe int ISQLite3Provider.sqlite3_prepare_v2(sqlite3 db, utf8z sql, out IntPtr stm, out utf8z tail)
	{
		fixed (byte* pSql = sql)
		{
			byte* ptrRemain;
			int result = NativeMethods.sqlite3_prepare_v2(db, pSql, -1, out stm, out ptrRemain);
			tail = utf8z.FromPtr(ptrRemain);
			return result;
		}
	}

	unsafe int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out IntPtr stm, out ReadOnlySpan<byte> tail)
	{
		fixed (byte* ptr = sql)
		{
			byte* ptrRemain;
			int result = NativeMethods.sqlite3_prepare_v3(db, ptr, sql.Length, flags, out stm, out ptrRemain);
			int num = (int)(ptrRemain - ptr);
			int num2 = sql.Length - num;
			if (num2 > 0)
			{
				tail = sql.Slice(num, num2);
				return result;
			}
			tail = ReadOnlySpan<byte>.Empty;
			return result;
		}
	}

	unsafe int ISQLite3Provider.sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out IntPtr stm, out utf8z tail)
	{
		fixed (byte* pSql = sql)
		{
			byte* ptrRemain;
			int result = NativeMethods.sqlite3_prepare_v3(db, pSql, -1, flags, out stm, out ptrRemain);
			tail = utf8z.FromPtr(ptrRemain);
			return result;
		}
	}

	int ISQLite3Provider.sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
	{
		return NativeMethods.sqlite3_db_status(db, op, out current, out highest, resetFlg);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_sql(sqlite3_stmt stmt)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_sql(stmt));
	}

	IntPtr ISQLite3Provider.sqlite3_db_handle(IntPtr stmt)
	{
		return NativeMethods.sqlite3_db_handle(stmt);
	}

	unsafe int ISQLite3Provider.sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob)
	{
		fixed (byte* sdb = db_utf8)
		{
			fixed (byte* table = table_utf8)
			{
				fixed (byte* col = col_utf8)
				{
					return NativeMethods.sqlite3_blob_open(db, sdb, table, col, rowid, flags, out blob);
				}
			}
		}
	}

	int ISQLite3Provider.sqlite3_blob_bytes(sqlite3_blob blob)
	{
		return NativeMethods.sqlite3_blob_bytes(blob);
	}

	int ISQLite3Provider.sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
	{
		return NativeMethods.sqlite3_blob_reopen(blob, rowid);
	}

	unsafe int ISQLite3Provider.sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset)
	{
		fixed (byte* b2 = b)
		{
			return NativeMethods.sqlite3_blob_read(blob, b2, b.Length, offset);
		}
	}

	unsafe int ISQLite3Provider.sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
	{
		fixed (byte* b2 = b)
		{
			return NativeMethods.sqlite3_blob_write(blob, b2, b.Length, offset);
		}
	}

	int ISQLite3Provider.sqlite3_blob_close(IntPtr blob)
	{
		return NativeMethods.sqlite3_blob_close(blob);
	}

	unsafe sqlite3_backup ISQLite3Provider.sqlite3_backup_init(sqlite3 destDb, utf8z destName, sqlite3 sourceDb, utf8z sourceName)
	{
		fixed (byte* zDestName = destName)
		{
			fixed (byte* zSourceName = sourceName)
			{
				return NativeMethods.sqlite3_backup_init(destDb, zDestName, sourceDb, zSourceName);
			}
		}
	}

	int ISQLite3Provider.sqlite3_backup_step(sqlite3_backup backup, int nPage)
	{
		return NativeMethods.sqlite3_backup_step(backup, nPage);
	}

	int ISQLite3Provider.sqlite3_backup_remaining(sqlite3_backup backup)
	{
		return NativeMethods.sqlite3_backup_remaining(backup);
	}

	int ISQLite3Provider.sqlite3_backup_pagecount(sqlite3_backup backup)
	{
		return NativeMethods.sqlite3_backup_pagecount(backup);
	}

	int ISQLite3Provider.sqlite3_backup_finish(IntPtr backup)
	{
		return NativeMethods.sqlite3_backup_finish(backup);
	}

	IntPtr ISQLite3Provider.sqlite3_next_stmt(sqlite3 db, IntPtr stmt)
	{
		return NativeMethods.sqlite3_next_stmt(db, stmt);
	}

	long ISQLite3Provider.sqlite3_last_insert_rowid(sqlite3 db)
	{
		return NativeMethods.sqlite3_last_insert_rowid(db);
	}

	int ISQLite3Provider.sqlite3_changes(sqlite3 db)
	{
		return NativeMethods.sqlite3_changes(db);
	}

	int ISQLite3Provider.sqlite3_total_changes(sqlite3 db)
	{
		return NativeMethods.sqlite3_total_changes(db);
	}

	int ISQLite3Provider.sqlite3_extended_result_codes(sqlite3 db, int onoff)
	{
		return NativeMethods.sqlite3_extended_result_codes(db, onoff);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_errstr(int rc)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_errstr(rc));
	}

	int ISQLite3Provider.sqlite3_errcode(sqlite3 db)
	{
		return NativeMethods.sqlite3_errcode(db);
	}

	int ISQLite3Provider.sqlite3_extended_errcode(sqlite3 db)
	{
		return NativeMethods.sqlite3_extended_errcode(db);
	}

	int ISQLite3Provider.sqlite3_busy_timeout(sqlite3 db, int ms)
	{
		return NativeMethods.sqlite3_busy_timeout(db, ms);
	}

	int ISQLite3Provider.sqlite3_get_autocommit(sqlite3 db)
	{
		return NativeMethods.sqlite3_get_autocommit(db);
	}

	unsafe int ISQLite3Provider.sqlite3_db_readonly(sqlite3 db, utf8z dbName)
	{
		fixed (byte* dbName2 = dbName)
		{
			return NativeMethods.sqlite3_db_readonly(db, dbName2);
		}
	}

	unsafe utf8z ISQLite3Provider.sqlite3_db_filename(sqlite3 db, utf8z att)
	{
		fixed (byte* att2 = att)
		{
			return utf8z.FromPtr(NativeMethods.sqlite3_db_filename(db, att2));
		}
	}

	unsafe utf8z ISQLite3Provider.sqlite3_errmsg(sqlite3 db)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_errmsg(db));
	}

	unsafe utf8z ISQLite3Provider.sqlite3_libversion()
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_libversion());
	}

	int ISQLite3Provider.sqlite3_libversion_number()
	{
		return NativeMethods.sqlite3_libversion_number();
	}

	int ISQLite3Provider.sqlite3_threadsafe()
	{
		return NativeMethods.sqlite3_threadsafe();
	}

	int ISQLite3Provider.sqlite3_config(int op)
	{
		return NativeMethods.sqlite3_config_none(op);
	}

	int ISQLite3Provider.sqlite3_config(int op, int val)
	{
		return NativeMethods.sqlite3_config_int(op, val);
	}

	int ISQLite3Provider.sqlite3_initialize()
	{
		return NativeMethods.sqlite3_initialize();
	}

	int ISQLite3Provider.sqlite3_shutdown()
	{
		return NativeMethods.sqlite3_shutdown();
	}

	int ISQLite3Provider.sqlite3_enable_load_extension(sqlite3 db, int onoff)
	{
		return NativeMethods.sqlite3_enable_load_extension(db, onoff);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_commit))]
	private static int commit_hook_bridge_impl(IntPtr p)
	{
		return commit_hook_info.from_ptr(p).call();
	}

	void ISQLite3Provider.sqlite3_commit_hook(sqlite3 db, delegate_commit func, object v)
	{
		hook_handles hook_handles2 = get_hooks(db);
		if (hook_handles2.commit != null)
		{
			hook_handles2.commit.Dispose();
			hook_handles2.commit = null;
		}
		NativeMethods.callback_commit func2;
		commit_hook_info target;
		if (func != null)
		{
			func2 = commit_hook_bridge;
			target = new commit_hook_info(func, v);
		}
		else
		{
			func2 = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		NativeMethods.sqlite3_commit_hook(db, func2, hook_handle2);
		hook_handles2.commit = hook_handle2.ForDispose();
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_scalar_function))]
	private static void scalar_function_hook_bridge_impl(IntPtr context, int num_args, IntPtr argsptr)
	{
		function_hook_info.from_ptr(NativeMethods.sqlite3_user_data(context)).call_scalar(context, num_args, argsptr);
	}

	int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_scalar func)
	{
		hook_handles hook_handles2 = get_hooks(db);
		hook_handles2.RemoveScalarFunction(name, nargs);
		int nType = 1 | flags;
		NativeMethods.callback_scalar_function callback_scalar_function;
		function_hook_info target;
		if (func != null)
		{
			callback_scalar_function = scalar_function_hook_bridge;
			target = new function_hook_info(func, v);
		}
		else
		{
			callback_scalar_function = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		int num = NativeMethods.sqlite3_create_function_v2(db, name, nargs, nType, hook_handle2, callback_scalar_function, null, null, null);
		if (num == 0 && callback_scalar_function != null)
		{
			hook_handles2.AddScalarFunction(name, nargs, hook_handle2.ForDispose());
		}
		return num;
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_log))]
	private static void log_hook_bridge_impl(IntPtr p, int rc, IntPtr s)
	{
		log_hook_info.from_ptr(p).call(rc, utf8z.FromIntPtr(s));
	}

	int ISQLite3Provider.sqlite3_config_log(delegate_log func, object v)
	{
		if (disp_log_hook_handle != null)
		{
			disp_log_hook_handle.Dispose();
			disp_log_hook_handle = null;
		}
		NativeMethods.callback_log func2;
		log_hook_info target;
		if (func != null)
		{
			func2 = log_hook_bridge;
			target = new log_hook_info(func, v);
		}
		else
		{
			func2 = null;
			target = null;
		}
		hook_handle pvUser = (hook_handle)(disp_log_hook_handle = new hook_handle(target));
		return NativeMethods.sqlite3_config_log(16, func2, pvUser);
	}

	unsafe void ISQLite3Provider.sqlite3_log(int errcode, utf8z s)
	{
		fixed (byte* zFormat = s)
		{
			NativeMethods.sqlite3_log(errcode, zFormat);
		}
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_agg_function_step))]
	private static void agg_function_hook_bridge_step_impl(IntPtr context, int num_args, IntPtr argsptr)
	{
		IntPtr agg_context = NativeMethods.sqlite3_aggregate_context(context, 8);
		function_hook_info.from_ptr(NativeMethods.sqlite3_user_data(context)).call_step(context, agg_context, num_args, argsptr);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_agg_function_final))]
	private static void agg_function_hook_bridge_final_impl(IntPtr context)
	{
		IntPtr agg_context = NativeMethods.sqlite3_aggregate_context(context, 8);
		function_hook_info.from_ptr(NativeMethods.sqlite3_user_data(context)).call_final(context, agg_context);
	}

	int ISQLite3Provider.sqlite3_create_function(sqlite3 db, byte[] name, int nargs, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
	{
		hook_handles hook_handles2 = get_hooks(db);
		hook_handles2.RemoveAggFunction(name, nargs);
		int nType = 1 | flags;
		NativeMethods.callback_agg_function_step callback_agg_function_step;
		NativeMethods.callback_agg_function_final ffinal;
		function_hook_info target;
		if (func_step != null)
		{
			callback_agg_function_step = agg_function_hook_bridge_step;
			ffinal = agg_function_hook_bridge_final;
			target = new function_hook_info(func_step, func_final, v);
		}
		else
		{
			callback_agg_function_step = null;
			ffinal = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		int num = NativeMethods.sqlite3_create_function_v2(db, name, nargs, nType, hook_handle2, null, callback_agg_function_step, ffinal, null);
		if (num == 0 && callback_agg_function_step != null)
		{
			hook_handles2.AddAggFunction(name, nargs, hook_handle2.ForDispose());
		}
		return num;
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_collation))]
	private unsafe static int collation_hook_bridge_impl(IntPtr p, int len1, IntPtr pv1, int len2, IntPtr pv2)
	{
		collation_hook_info obj = collation_hook_info.from_ptr(p);
		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(pv1.ToPointer(), len1);
		ReadOnlySpan<byte> s2 = new ReadOnlySpan<byte>(pv2.ToPointer(), len2);
		return obj.call(s, s2);
	}

	int ISQLite3Provider.sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation func)
	{
		hook_handles hook_handles2 = get_hooks(db);
		hook_handles2.RemoveCollation(name);
		NativeMethods.callback_collation callback_collation;
		collation_hook_info target;
		if (func != null)
		{
			callback_collation = collation_hook_bridge;
			target = new collation_hook_info(func, v);
		}
		else
		{
			callback_collation = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		int num = NativeMethods.sqlite3_create_collation(db, name, 1, hook_handle2, callback_collation);
		if (num == 0 && callback_collation != null)
		{
			hook_handles2.AddCollation(name, hook_handle2.ForDispose());
		}
		return num;
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_update))]
	private static void update_hook_bridge_impl(IntPtr p, int typ, IntPtr db, IntPtr tbl, long rowid)
	{
		update_hook_info.from_ptr(p).call(typ, utf8z.FromIntPtr(db), utf8z.FromIntPtr(tbl), rowid);
	}

	void ISQLite3Provider.sqlite3_update_hook(sqlite3 db, delegate_update func, object v)
	{
		hook_handles hook_handles2 = get_hooks(db);
		if (hook_handles2.update != null)
		{
			hook_handles2.update.Dispose();
			hook_handles2.update = null;
		}
		NativeMethods.callback_update func2;
		update_hook_info target;
		if (func != null)
		{
			func2 = update_hook_bridge;
			target = new update_hook_info(func, v);
		}
		else
		{
			func2 = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		hook_handles2.update = hook_handle2.ForDispose();
		NativeMethods.sqlite3_update_hook(db, func2, hook_handle2);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_rollback))]
	private static void rollback_hook_bridge_impl(IntPtr p)
	{
		rollback_hook_info.from_ptr(p).call();
	}

	void ISQLite3Provider.sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, object v)
	{
		hook_handles hook_handles2 = get_hooks(db);
		if (hook_handles2.rollback != null)
		{
			hook_handles2.rollback.Dispose();
			hook_handles2.rollback = null;
		}
		NativeMethods.callback_rollback func2;
		rollback_hook_info target;
		if (func != null)
		{
			func2 = rollback_hook_bridge;
			target = new rollback_hook_info(func, v);
		}
		else
		{
			func2 = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		hook_handles2.rollback = hook_handle2.ForDispose();
		NativeMethods.sqlite3_rollback_hook(db, func2, hook_handle2);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_trace))]
	private static void trace_hook_bridge_impl(IntPtr p, IntPtr s)
	{
		trace_hook_info.from_ptr(p).call(utf8z.FromIntPtr(s));
	}

	void ISQLite3Provider.sqlite3_trace(sqlite3 db, delegate_trace func, object v)
	{
		hook_handles hook_handles2 = get_hooks(db);
		if (hook_handles2.trace != null)
		{
			hook_handles2.trace.Dispose();
			hook_handles2.trace = null;
		}
		NativeMethods.callback_trace func2;
		trace_hook_info target;
		if (func != null)
		{
			func2 = trace_hook_bridge;
			target = new trace_hook_info(func, v);
		}
		else
		{
			func2 = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		hook_handles2.trace = hook_handle2.ForDispose();
		NativeMethods.sqlite3_trace(db, func2, hook_handle2);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_profile))]
	private static void profile_hook_bridge_impl(IntPtr p, IntPtr s, long elapsed)
	{
		profile_hook_info.from_ptr(p).call(utf8z.FromIntPtr(s), elapsed);
	}

	void ISQLite3Provider.sqlite3_profile(sqlite3 db, delegate_profile func, object v)
	{
		hook_handles hook_handles2 = get_hooks(db);
		if (hook_handles2.profile != null)
		{
			hook_handles2.profile.Dispose();
			hook_handles2.profile = null;
		}
		NativeMethods.callback_profile func2;
		profile_hook_info target;
		if (func != null)
		{
			func2 = profile_hook_bridge;
			target = new profile_hook_info(func, v);
		}
		else
		{
			func2 = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		hook_handles2.profile = hook_handle2.ForDispose();
		NativeMethods.sqlite3_profile(db, func2, hook_handle2);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_progress_handler))]
	private static int progress_hook_bridge_impl(IntPtr p)
	{
		return progress_hook_info.from_ptr(p).call();
	}

	void ISQLite3Provider.sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v)
	{
		hook_handles hook_handles2 = get_hooks(db);
		if (hook_handles2.progress != null)
		{
			hook_handles2.progress.Dispose();
			hook_handles2.progress = null;
		}
		NativeMethods.callback_progress_handler func2;
		progress_hook_info target;
		if (func != null)
		{
			func2 = progress_hook_bridge;
			target = new progress_hook_info(func, v);
		}
		else
		{
			func2 = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		hook_handles2.progress = hook_handle2.ForDispose();
		NativeMethods.sqlite3_progress_handler(db, instructions, func2, hook_handle2);
	}

	[MonoPInvokeCallback(typeof(NativeMethods.callback_authorizer))]
	private static int authorizer_hook_bridge_impl(IntPtr p, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view)
	{
		return authorizer_hook_info.from_ptr(p).call(action_code, utf8z.FromIntPtr(param0), utf8z.FromIntPtr(param1), utf8z.FromIntPtr(dbName), utf8z.FromIntPtr(inner_most_trigger_or_view));
	}

	int ISQLite3Provider.sqlite3_set_authorizer(sqlite3 db, delegate_authorizer func, object v)
	{
		hook_handles hook_handles2 = get_hooks(db);
		if (hook_handles2.authorizer != null)
		{
			hook_handles2.authorizer.Dispose();
			hook_handles2.authorizer = null;
		}
		NativeMethods.callback_authorizer cb;
		authorizer_hook_info target;
		if (func != null)
		{
			cb = authorizer_hook_bridge;
			target = new authorizer_hook_info(func, v);
		}
		else
		{
			cb = null;
			target = null;
		}
		hook_handle hook_handle2 = new hook_handle(target);
		hook_handles2.authorizer = hook_handle2.ForDispose();
		return NativeMethods.sqlite3_set_authorizer(db, cb, hook_handle2);
	}

	long ISQLite3Provider.sqlite3_memory_used()
	{
		return NativeMethods.sqlite3_memory_used();
	}

	long ISQLite3Provider.sqlite3_memory_highwater(int resetFlag)
	{
		return NativeMethods.sqlite3_memory_highwater(resetFlag);
	}

	int ISQLite3Provider.sqlite3_status(int op, out int current, out int highwater, int resetFlag)
	{
		return NativeMethods.sqlite3_status(op, out current, out highwater, resetFlag);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_sourceid()
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_sourceid());
	}

	void ISQLite3Provider.sqlite3_result_int64(IntPtr ctx, long val)
	{
		NativeMethods.sqlite3_result_int64(ctx, val);
	}

	void ISQLite3Provider.sqlite3_result_int(IntPtr ctx, int val)
	{
		NativeMethods.sqlite3_result_int(ctx, val);
	}

	void ISQLite3Provider.sqlite3_result_double(IntPtr ctx, double val)
	{
		NativeMethods.sqlite3_result_double(ctx, val);
	}

	void ISQLite3Provider.sqlite3_result_null(IntPtr stm)
	{
		NativeMethods.sqlite3_result_null(stm);
	}

	unsafe void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, ReadOnlySpan<byte> val)
	{
		fixed (byte* strErr = val)
		{
			NativeMethods.sqlite3_result_error(ctx, strErr, val.Length);
		}
	}

	unsafe void ISQLite3Provider.sqlite3_result_error(IntPtr ctx, utf8z val)
	{
		fixed (byte* strErr = val)
		{
			NativeMethods.sqlite3_result_error(ctx, strErr, -1);
		}
	}

	unsafe void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, ReadOnlySpan<byte> val)
	{
		fixed (byte* val2 = val)
		{
			NativeMethods.sqlite3_result_text(ctx, val2, val.Length, new IntPtr(-1));
		}
	}

	unsafe void ISQLite3Provider.sqlite3_result_text(IntPtr ctx, utf8z val)
	{
		fixed (byte* val2 = val)
		{
			NativeMethods.sqlite3_result_text(ctx, val2, -1, new IntPtr(-1));
		}
	}

	unsafe void ISQLite3Provider.sqlite3_result_blob(IntPtr ctx, ReadOnlySpan<byte> blob)
	{
		fixed (byte* ptr = blob)
		{
			NativeMethods.sqlite3_result_blob(ctx, (IntPtr)ptr, blob.Length, new IntPtr(-1));
		}
	}

	void ISQLite3Provider.sqlite3_result_zeroblob(IntPtr ctx, int n)
	{
		NativeMethods.sqlite3_result_zeroblob(ctx, n);
	}

	void ISQLite3Provider.sqlite3_result_error_toobig(IntPtr ctx)
	{
		NativeMethods.sqlite3_result_error_toobig(ctx);
	}

	void ISQLite3Provider.sqlite3_result_error_nomem(IntPtr ctx)
	{
		NativeMethods.sqlite3_result_error_nomem(ctx);
	}

	void ISQLite3Provider.sqlite3_result_error_code(IntPtr ctx, int code)
	{
		NativeMethods.sqlite3_result_error_code(ctx, code);
	}

	unsafe ReadOnlySpan<byte> ISQLite3Provider.sqlite3_value_blob(IntPtr p)
	{
		IntPtr intPtr = NativeMethods.sqlite3_value_blob(p);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		int length = NativeMethods.sqlite3_value_bytes(p);
		return new ReadOnlySpan<byte>(intPtr.ToPointer(), length);
	}

	int ISQLite3Provider.sqlite3_value_bytes(IntPtr p)
	{
		return NativeMethods.sqlite3_value_bytes(p);
	}

	double ISQLite3Provider.sqlite3_value_double(IntPtr p)
	{
		return NativeMethods.sqlite3_value_double(p);
	}

	int ISQLite3Provider.sqlite3_value_int(IntPtr p)
	{
		return NativeMethods.sqlite3_value_int(p);
	}

	long ISQLite3Provider.sqlite3_value_int64(IntPtr p)
	{
		return NativeMethods.sqlite3_value_int64(p);
	}

	int ISQLite3Provider.sqlite3_value_type(IntPtr p)
	{
		return NativeMethods.sqlite3_value_type(p);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_value_text(IntPtr p)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_value_text(p));
	}

	int ISQLite3Provider.sqlite3_bind_int(sqlite3_stmt stm, int paramIndex, int val)
	{
		return NativeMethods.sqlite3_bind_int(stm, paramIndex, val);
	}

	int ISQLite3Provider.sqlite3_bind_int64(sqlite3_stmt stm, int paramIndex, long val)
	{
		return NativeMethods.sqlite3_bind_int64(stm, paramIndex, val);
	}

	unsafe int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, ReadOnlySpan<byte> t)
	{
		fixed (byte* val = t)
		{
			return NativeMethods.sqlite3_bind_text(stm, paramIndex, val, t.Length, new IntPtr(-1));
		}
	}

	unsafe int ISQLite3Provider.sqlite3_bind_text(sqlite3_stmt stm, int paramIndex, utf8z t)
	{
		fixed (byte* val = t)
		{
			return NativeMethods.sqlite3_bind_text(stm, paramIndex, val, -1, new IntPtr(-1));
		}
	}

	int ISQLite3Provider.sqlite3_bind_double(sqlite3_stmt stm, int paramIndex, double val)
	{
		return NativeMethods.sqlite3_bind_double(stm, paramIndex, val);
	}

	unsafe int ISQLite3Provider.sqlite3_bind_blob(sqlite3_stmt stm, int paramIndex, ReadOnlySpan<byte> blob)
	{
		if (blob.Length == 0)
		{
			fixed (byte* val = (ReadOnlySpan<byte>)new byte[1] { 42 })
			{
				return NativeMethods.sqlite3_bind_blob(stm, paramIndex, val, 0, new IntPtr(-1));
			}
		}
		fixed (byte* val2 = blob)
		{
			return NativeMethods.sqlite3_bind_blob(stm, paramIndex, val2, blob.Length, new IntPtr(-1));
		}
	}

	int ISQLite3Provider.sqlite3_bind_zeroblob(sqlite3_stmt stm, int paramIndex, int size)
	{
		return NativeMethods.sqlite3_bind_zeroblob(stm, paramIndex, size);
	}

	int ISQLite3Provider.sqlite3_bind_null(sqlite3_stmt stm, int paramIndex)
	{
		return NativeMethods.sqlite3_bind_null(stm, paramIndex);
	}

	int ISQLite3Provider.sqlite3_bind_parameter_count(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_bind_parameter_count(stm);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_bind_parameter_name(sqlite3_stmt stm, int paramIndex)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_bind_parameter_name(stm, paramIndex));
	}

	unsafe int ISQLite3Provider.sqlite3_bind_parameter_index(sqlite3_stmt stm, utf8z paramName)
	{
		fixed (byte* strName = paramName)
		{
			return NativeMethods.sqlite3_bind_parameter_index(stm, strName);
		}
	}

	int ISQLite3Provider.sqlite3_step(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_step(stm);
	}

	int ISQLite3Provider.sqlite3_stmt_busy(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_stmt_busy(stm);
	}

	int ISQLite3Provider.sqlite3_stmt_readonly(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_stmt_readonly(stm);
	}

	int ISQLite3Provider.sqlite3_column_int(sqlite3_stmt stm, int columnIndex)
	{
		return NativeMethods.sqlite3_column_int(stm, columnIndex);
	}

	long ISQLite3Provider.sqlite3_column_int64(sqlite3_stmt stm, int columnIndex)
	{
		return NativeMethods.sqlite3_column_int64(stm, columnIndex);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_column_text(sqlite3_stmt stm, int columnIndex)
	{
		byte* p = NativeMethods.sqlite3_column_text(stm, columnIndex);
		int len = NativeMethods.sqlite3_column_bytes(stm, columnIndex);
		return utf8z.FromPtrLen(p, len);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_column_decltype(sqlite3_stmt stm, int columnIndex)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_column_decltype(stm, columnIndex));
	}

	double ISQLite3Provider.sqlite3_column_double(sqlite3_stmt stm, int columnIndex)
	{
		return NativeMethods.sqlite3_column_double(stm, columnIndex);
	}

	unsafe ReadOnlySpan<byte> ISQLite3Provider.sqlite3_column_blob(sqlite3_stmt stm, int columnIndex)
	{
		IntPtr intPtr = NativeMethods.sqlite3_column_blob(stm, columnIndex);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		int length = NativeMethods.sqlite3_column_bytes(stm, columnIndex);
		return new ReadOnlySpan<byte>(intPtr.ToPointer(), length);
	}

	int ISQLite3Provider.sqlite3_column_type(sqlite3_stmt stm, int columnIndex)
	{
		return NativeMethods.sqlite3_column_type(stm, columnIndex);
	}

	int ISQLite3Provider.sqlite3_column_bytes(sqlite3_stmt stm, int columnIndex)
	{
		return NativeMethods.sqlite3_column_bytes(stm, columnIndex);
	}

	int ISQLite3Provider.sqlite3_column_count(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_column_count(stm);
	}

	int ISQLite3Provider.sqlite3_data_count(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_data_count(stm);
	}

	unsafe utf8z ISQLite3Provider.sqlite3_column_name(sqlite3_stmt stm, int columnIndex)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_column_name(stm, columnIndex));
	}

	unsafe utf8z ISQLite3Provider.sqlite3_column_origin_name(sqlite3_stmt stm, int columnIndex)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_column_origin_name(stm, columnIndex));
	}

	unsafe utf8z ISQLite3Provider.sqlite3_column_table_name(sqlite3_stmt stm, int columnIndex)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_column_table_name(stm, columnIndex));
	}

	unsafe utf8z ISQLite3Provider.sqlite3_column_database_name(sqlite3_stmt stm, int columnIndex)
	{
		return utf8z.FromPtr(NativeMethods.sqlite3_column_database_name(stm, columnIndex));
	}

	int ISQLite3Provider.sqlite3_reset(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_reset(stm);
	}

	int ISQLite3Provider.sqlite3_clear_bindings(sqlite3_stmt stm)
	{
		return NativeMethods.sqlite3_clear_bindings(stm);
	}

	int ISQLite3Provider.sqlite3_stmt_status(sqlite3_stmt stm, int op, int resetFlg)
	{
		return NativeMethods.sqlite3_stmt_status(stm, op, resetFlg);
	}

	int ISQLite3Provider.sqlite3_finalize(IntPtr stm)
	{
		return NativeMethods.sqlite3_finalize(stm);
	}

	int ISQLite3Provider.sqlite3_wal_autocheckpoint(sqlite3 db, int n)
	{
		return NativeMethods.sqlite3_wal_autocheckpoint(db, n);
	}

	unsafe int ISQLite3Provider.sqlite3_wal_checkpoint(sqlite3 db, utf8z dbName)
	{
		fixed (byte* dbName2 = dbName)
		{
			return NativeMethods.sqlite3_wal_checkpoint(db, dbName2);
		}
	}

	unsafe int ISQLite3Provider.sqlite3_wal_checkpoint_v2(sqlite3 db, utf8z dbName, int eMode, out int logSize, out int framesCheckPointed)
	{
		fixed (byte* dbName2 = dbName)
		{
			return NativeMethods.sqlite3_wal_checkpoint_v2(db, dbName2, eMode, out logSize, out framesCheckPointed);
		}
	}
}
