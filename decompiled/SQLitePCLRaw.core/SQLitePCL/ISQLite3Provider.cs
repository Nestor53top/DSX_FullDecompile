using System;

namespace SQLitePCL;

public interface ISQLite3Provider
{
	string GetNativeLibraryName();

	int sqlite3_open(utf8z filename, out IntPtr db);

	int sqlite3_open_v2(utf8z filename, out IntPtr db, int flags, utf8z vfs);

	int sqlite3_close_v2(IntPtr db);

	int sqlite3_close(IntPtr db);

	int sqlite3_enable_shared_cache(int enable);

	void sqlite3_interrupt(sqlite3 db);

	int sqlite3__vfs__delete(utf8z vfs, utf8z pathname, int syncDir);

	int sqlite3_threadsafe();

	utf8z sqlite3_libversion();

	int sqlite3_libversion_number();

	utf8z sqlite3_sourceid();

	long sqlite3_memory_used();

	long sqlite3_memory_highwater(int resetFlag);

	int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

	int sqlite3_db_readonly(sqlite3 db, utf8z dbName);

	utf8z sqlite3_db_filename(sqlite3 db, utf8z att);

	utf8z sqlite3_errmsg(sqlite3 db);

	long sqlite3_last_insert_rowid(sqlite3 db);

	int sqlite3_changes(sqlite3 db);

	int sqlite3_total_changes(sqlite3 db);

	int sqlite3_get_autocommit(sqlite3 db);

	int sqlite3_busy_timeout(sqlite3 db, int ms);

	int sqlite3_extended_result_codes(sqlite3 db, int onoff);

	int sqlite3_errcode(sqlite3 db);

	int sqlite3_extended_errcode(sqlite3 db);

	utf8z sqlite3_errstr(int rc);

	int sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out IntPtr stmt, out ReadOnlySpan<byte> remain);

	int sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out IntPtr stmt, out ReadOnlySpan<byte> remain);

	int sqlite3_prepare_v2(sqlite3 db, utf8z sql, out IntPtr stmt, out utf8z remain);

	int sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out IntPtr stmt, out utf8z remain);

	int sqlite3_step(sqlite3_stmt stmt);

	int sqlite3_finalize(IntPtr stmt);

	int sqlite3_reset(sqlite3_stmt stmt);

	int sqlite3_clear_bindings(sqlite3_stmt stmt);

	int sqlite3_stmt_status(sqlite3_stmt stmt, int op, int resetFlg);

	utf8z sqlite3_sql(sqlite3_stmt stmt);

	IntPtr sqlite3_db_handle(IntPtr stmt);

	IntPtr sqlite3_next_stmt(sqlite3 db, IntPtr stmt);

	int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size);

	utf8z sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index);

	int sqlite3_bind_blob(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> blob);

	int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val);

	int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val);

	int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val);

	int sqlite3_bind_null(sqlite3_stmt stmt, int index);

	int sqlite3_bind_text(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> text);

	int sqlite3_bind_text(sqlite3_stmt stmt, int index, utf8z text);

	int sqlite3_bind_parameter_count(sqlite3_stmt stmt);

	int sqlite3_bind_parameter_index(sqlite3_stmt stmt, utf8z strName);

	utf8z sqlite3_column_database_name(sqlite3_stmt stmt, int index);

	utf8z sqlite3_column_name(sqlite3_stmt stmt, int index);

	utf8z sqlite3_column_origin_name(sqlite3_stmt stmt, int index);

	utf8z sqlite3_column_table_name(sqlite3_stmt stmt, int index);

	utf8z sqlite3_column_text(sqlite3_stmt stmt, int index);

	int sqlite3_data_count(sqlite3_stmt stmt);

	int sqlite3_column_count(sqlite3_stmt stmt);

	double sqlite3_column_double(sqlite3_stmt stmt, int index);

	int sqlite3_column_int(sqlite3_stmt stmt, int index);

	long sqlite3_column_int64(sqlite3_stmt stmt, int index);

	ReadOnlySpan<byte> sqlite3_column_blob(sqlite3_stmt stmt, int index);

	int sqlite3_column_bytes(sqlite3_stmt stmt, int index);

	int sqlite3_column_type(sqlite3_stmt stmt, int index);

	utf8z sqlite3_column_decltype(sqlite3_stmt stmt, int index);

	sqlite3_backup sqlite3_backup_init(sqlite3 destDb, utf8z destName, sqlite3 sourceDb, utf8z sourceName);

	int sqlite3_backup_step(sqlite3_backup backup, int nPage);

	int sqlite3_backup_remaining(sqlite3_backup backup);

	int sqlite3_backup_pagecount(sqlite3_backup backup);

	int sqlite3_backup_finish(IntPtr backup);

	int sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob);

	int sqlite3_blob_bytes(sqlite3_blob blob);

	int sqlite3_blob_reopen(sqlite3_blob blob, long rowid);

	int sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset);

	int sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset);

	int sqlite3_blob_close(IntPtr blob);

	int sqlite3_config_log(delegate_log func, object v);

	void sqlite3_log(int errcode, utf8z s);

	void sqlite3_commit_hook(sqlite3 db, delegate_commit func, object v);

	void sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, object v);

	void sqlite3_trace(sqlite3 db, delegate_trace func, object v);

	void sqlite3_profile(sqlite3 db, delegate_profile func, object v);

	void sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v);

	void sqlite3_update_hook(sqlite3 db, delegate_update func, object v);

	int sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation func);

	int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_scalar func);

	int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final);

	int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg);

	void sqlite3_result_blob(IntPtr context, ReadOnlySpan<byte> val);

	void sqlite3_result_double(IntPtr context, double val);

	void sqlite3_result_error(IntPtr context, ReadOnlySpan<byte> strErr);

	void sqlite3_result_error(IntPtr context, utf8z strErr);

	void sqlite3_result_int(IntPtr context, int val);

	void sqlite3_result_int64(IntPtr context, long val);

	void sqlite3_result_null(IntPtr context);

	void sqlite3_result_text(IntPtr context, ReadOnlySpan<byte> val);

	void sqlite3_result_text(IntPtr context, utf8z val);

	void sqlite3_result_zeroblob(IntPtr context, int n);

	void sqlite3_result_error_toobig(IntPtr context);

	void sqlite3_result_error_nomem(IntPtr context);

	void sqlite3_result_error_code(IntPtr context, int code);

	ReadOnlySpan<byte> sqlite3_value_blob(IntPtr p);

	int sqlite3_value_bytes(IntPtr p);

	double sqlite3_value_double(IntPtr p);

	int sqlite3_value_int(IntPtr p);

	long sqlite3_value_int64(IntPtr p);

	int sqlite3_value_type(IntPtr p);

	utf8z sqlite3_value_text(IntPtr p);

	int sqlite3_stmt_busy(sqlite3_stmt stmt);

	int sqlite3_stmt_readonly(sqlite3_stmt stmt);

	int sqlite3_exec(sqlite3 db, utf8z sql, delegate_exec callback, object user_data, out IntPtr errMsg);

	int sqlite3_complete(utf8z sql);

	int sqlite3_compileoption_used(utf8z sql);

	utf8z sqlite3_compileoption_get(int n);

	int sqlite3_wal_autocheckpoint(sqlite3 db, int n);

	int sqlite3_wal_checkpoint(sqlite3 db, utf8z dbName);

	int sqlite3_wal_checkpoint_v2(sqlite3 db, utf8z dbName, int eMode, out int logSize, out int framesCheckPointed);

	int sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc);

	int sqlite3_set_authorizer(sqlite3 db, delegate_authorizer authorizer, object user_data);

	int sqlite3_stricmp(IntPtr p, IntPtr q);

	int sqlite3_strnicmp(IntPtr p, IntPtr q, int n);

	void sqlite3_free(IntPtr p);

	int sqlite3_key(sqlite3 db, ReadOnlySpan<byte> key);

	int sqlite3_key_v2(sqlite3 db, utf8z dbname, ReadOnlySpan<byte> key);

	int sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> key);

	int sqlite3_rekey_v2(sqlite3 db, utf8z dbname, ReadOnlySpan<byte> key);

	int sqlite3_initialize();

	int sqlite3_shutdown();

	int sqlite3_config(int op);

	int sqlite3_config(int op, int val);

	int sqlite3_enable_load_extension(sqlite3 db, int enable);

	int sqlite3_win32_set_directory(int typ, utf8z path);
}
