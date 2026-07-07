using System;
using System.Text;

namespace SQLitePCL;

public static class raw
{
	private static ISQLite3Provider _imp;

	private static bool _frozen;

	public const int SQLITE_UTF8 = 1;

	public const int SQLITE_UTF16LE = 2;

	public const int SQLITE_UTF16BE = 3;

	public const int SQLITE_UTF16 = 4;

	public const int SQLITE_ANY = 5;

	public const int SQLITE_UTF16_ALIGNED = 8;

	public const int SQLITE_DETERMINISTIC = 2048;

	public const int SQLITE_CONFIG_SINGLETHREAD = 1;

	public const int SQLITE_CONFIG_MULTITHREAD = 2;

	public const int SQLITE_CONFIG_SERIALIZED = 3;

	public const int SQLITE_CONFIG_MALLOC = 4;

	public const int SQLITE_CONFIG_GETMALLOC = 5;

	public const int SQLITE_CONFIG_SCRATCH = 6;

	public const int SQLITE_CONFIG_PAGECACHE = 7;

	public const int SQLITE_CONFIG_HEAP = 8;

	public const int SQLITE_CONFIG_MEMSTATUS = 9;

	public const int SQLITE_CONFIG_MUTEX = 10;

	public const int SQLITE_CONFIG_GETMUTEX = 11;

	public const int SQLITE_CONFIG_LOOKASIDE = 13;

	public const int SQLITE_CONFIG_PCACHE = 14;

	public const int SQLITE_CONFIG_GETPCACHE = 15;

	public const int SQLITE_CONFIG_LOG = 16;

	public const int SQLITE_CONFIG_URI = 17;

	public const int SQLITE_CONFIG_PCACHE2 = 18;

	public const int SQLITE_CONFIG_GETPCACHE2 = 19;

	public const int SQLITE_CONFIG_COVERING_INDEX_SCAN = 20;

	public const int SQLITE_CONFIG_SQLLOG = 21;

	public const int SQLITE_OPEN_READONLY = 1;

	public const int SQLITE_OPEN_READWRITE = 2;

	public const int SQLITE_OPEN_CREATE = 4;

	public const int SQLITE_OPEN_DELETEONCLOSE = 8;

	public const int SQLITE_OPEN_EXCLUSIVE = 16;

	public const int SQLITE_OPEN_AUTOPROXY = 32;

	public const int SQLITE_OPEN_URI = 64;

	public const int SQLITE_OPEN_MEMORY = 128;

	public const int SQLITE_OPEN_MAIN_DB = 256;

	public const int SQLITE_OPEN_TEMP_DB = 512;

	public const int SQLITE_OPEN_TRANSIENT_DB = 1024;

	public const int SQLITE_OPEN_MAIN_JOURNAL = 2048;

	public const int SQLITE_OPEN_TEMP_JOURNAL = 4096;

	public const int SQLITE_OPEN_SUBJOURNAL = 8192;

	public const int SQLITE_OPEN_MASTER_JOURNAL = 16384;

	public const int SQLITE_OPEN_NOMUTEX = 32768;

	public const int SQLITE_OPEN_FULLMUTEX = 65536;

	public const int SQLITE_OPEN_SHAREDCACHE = 131072;

	public const int SQLITE_OPEN_PRIVATECACHE = 262144;

	public const int SQLITE_OPEN_WAL = 524288;

	public const int SQLITE_PREPARE_PERSISTENT = 1;

	public const int SQLITE_PREPARE_NORMALIZE = 2;

	public const int SQLITE_PREPARE_NO_VTAB = 4;

	public const int SQLITE_INTEGER = 1;

	public const int SQLITE_FLOAT = 2;

	public const int SQLITE_TEXT = 3;

	public const int SQLITE_BLOB = 4;

	public const int SQLITE_NULL = 5;

	public const int SQLITE_OK = 0;

	public const int SQLITE_ERROR = 1;

	public const int SQLITE_INTERNAL = 2;

	public const int SQLITE_PERM = 3;

	public const int SQLITE_ABORT = 4;

	public const int SQLITE_BUSY = 5;

	public const int SQLITE_LOCKED = 6;

	public const int SQLITE_NOMEM = 7;

	public const int SQLITE_READONLY = 8;

	public const int SQLITE_INTERRUPT = 9;

	public const int SQLITE_IOERR = 10;

	public const int SQLITE_CORRUPT = 11;

	public const int SQLITE_NOTFOUND = 12;

	public const int SQLITE_FULL = 13;

	public const int SQLITE_CANTOPEN = 14;

	public const int SQLITE_PROTOCOL = 15;

	public const int SQLITE_EMPTY = 16;

	public const int SQLITE_SCHEMA = 17;

	public const int SQLITE_TOOBIG = 18;

	public const int SQLITE_CONSTRAINT = 19;

	public const int SQLITE_MISMATCH = 20;

	public const int SQLITE_MISUSE = 21;

	public const int SQLITE_NOLFS = 22;

	public const int SQLITE_AUTH = 23;

	public const int SQLITE_FORMAT = 24;

	public const int SQLITE_RANGE = 25;

	public const int SQLITE_NOTADB = 26;

	public const int SQLITE_NOTICE = 27;

	public const int SQLITE_WARNING = 28;

	public const int SQLITE_ROW = 100;

	public const int SQLITE_DONE = 101;

	public const int SQLITE_IOERR_READ = 266;

	public const int SQLITE_IOERR_SHORT_READ = 522;

	public const int SQLITE_IOERR_WRITE = 778;

	public const int SQLITE_IOERR_FSYNC = 1034;

	public const int SQLITE_IOERR_DIR_FSYNC = 1290;

	public const int SQLITE_IOERR_TRUNCATE = 1546;

	public const int SQLITE_IOERR_FSTAT = 1802;

	public const int SQLITE_IOERR_UNLOCK = 2058;

	public const int SQLITE_IOERR_RDLOCK = 2314;

	public const int SQLITE_IOERR_DELETE = 2570;

	public const int SQLITE_IOERR_BLOCKED = 2826;

	public const int SQLITE_IOERR_NOMEM = 3082;

	public const int SQLITE_IOERR_ACCESS = 3338;

	public const int SQLITE_IOERR_CHECKRESERVEDLOCK = 3594;

	public const int SQLITE_IOERR_LOCK = 3850;

	public const int SQLITE_IOERR_CLOSE = 4106;

	public const int SQLITE_IOERR_DIR_CLOSE = 4362;

	public const int SQLITE_IOERR_SHMOPEN = 4618;

	public const int SQLITE_IOERR_SHMSIZE = 4874;

	public const int SQLITE_IOERR_SHMLOCK = 5130;

	public const int SQLITE_IOERR_SHMMAP = 5386;

	public const int SQLITE_IOERR_SEEK = 5642;

	public const int SQLITE_IOERR_DELETE_NOENT = 5898;

	public const int SQLITE_IOERR_MMAP = 6154;

	public const int SQLITE_IOERR_GETTEMPPATH = 6410;

	public const int SQLITE_IOERR_CONVPATH = 6666;

	public const int SQLITE_LOCKED_SHAREDCACHE = 262;

	public const int SQLITE_BUSY_RECOVERY = 261;

	public const int SQLITE_BUSY_SNAPSHOT = 517;

	public const int SQLITE_CANTOPEN_NOTEMPDIR = 270;

	public const int SQLITE_CANTOPEN_ISDIR = 526;

	public const int SQLITE_CANTOPEN_FULLPATH = 782;

	public const int SQLITE_CANTOPEN_CONVPATH = 1038;

	public const int SQLITE_CORRUPT_VTAB = 267;

	public const int SQLITE_READONLY_RECOVERY = 264;

	public const int SQLITE_READONLY_CANTLOCK = 520;

	public const int SQLITE_READONLY_ROLLBACK = 776;

	public const int SQLITE_READONLY_DBMOVED = 1032;

	public const int SQLITE_ABORT_ROLLBACK = 516;

	public const int SQLITE_CONSTRAINT_CHECK = 275;

	public const int SQLITE_CONSTRAINT_COMMITHOOK = 531;

	public const int SQLITE_CONSTRAINT_FOREIGNKEY = 787;

	public const int SQLITE_CONSTRAINT_FUNCTION = 1043;

	public const int SQLITE_CONSTRAINT_NOTNULL = 1299;

	public const int SQLITE_CONSTRAINT_PRIMARYKEY = 1555;

	public const int SQLITE_CONSTRAINT_TRIGGER = 1811;

	public const int SQLITE_CONSTRAINT_UNIQUE = 2067;

	public const int SQLITE_CONSTRAINT_VTAB = 2323;

	public const int SQLITE_CONSTRAINT_ROWID = 2579;

	public const int SQLITE_NOTICE_RECOVER_WAL = 283;

	public const int SQLITE_NOTICE_RECOVER_ROLLBACK = 539;

	public const int SQLITE_WARNING_AUTOINDEX = 284;

	public const int SQLITE_CREATE_INDEX = 1;

	public const int SQLITE_CREATE_TABLE = 2;

	public const int SQLITE_CREATE_TEMP_INDEX = 3;

	public const int SQLITE_CREATE_TEMP_TABLE = 4;

	public const int SQLITE_CREATE_TEMP_TRIGGER = 5;

	public const int SQLITE_CREATE_TEMP_VIEW = 6;

	public const int SQLITE_CREATE_TRIGGER = 7;

	public const int SQLITE_CREATE_VIEW = 8;

	public const int SQLITE_DELETE = 9;

	public const int SQLITE_DROP_INDEX = 10;

	public const int SQLITE_DROP_TABLE = 11;

	public const int SQLITE_DROP_TEMP_INDEX = 12;

	public const int SQLITE_DROP_TEMP_TABLE = 13;

	public const int SQLITE_DROP_TEMP_TRIGGER = 14;

	public const int SQLITE_DROP_TEMP_VIEW = 15;

	public const int SQLITE_DROP_TRIGGER = 16;

	public const int SQLITE_DROP_VIEW = 17;

	public const int SQLITE_INSERT = 18;

	public const int SQLITE_PRAGMA = 19;

	public const int SQLITE_READ = 20;

	public const int SQLITE_SELECT = 21;

	public const int SQLITE_TRANSACTION = 22;

	public const int SQLITE_UPDATE = 23;

	public const int SQLITE_ATTACH = 24;

	public const int SQLITE_DETACH = 25;

	public const int SQLITE_ALTER_TABLE = 26;

	public const int SQLITE_REINDEX = 27;

	public const int SQLITE_ANALYZE = 28;

	public const int SQLITE_CREATE_VTABLE = 29;

	public const int SQLITE_DROP_VTABLE = 30;

	public const int SQLITE_FUNCTION = 31;

	public const int SQLITE_SAVEPOINT = 32;

	public const int SQLITE_COPY = 0;

	public const int SQLITE_RECURSIVE = 33;

	public const int SQLITE_CHECKPOINT_PASSIVE = 0;

	public const int SQLITE_CHECKPOINT_FULL = 1;

	public const int SQLITE_CHECKPOINT_RESTART = 2;

	public const int SQLITE_CHECKPOINT_TRUNCATE = 3;

	public const int SQLITE_DBSTATUS_LOOKASIDE_USED = 0;

	public const int SQLITE_DBSTATUS_CACHE_USED = 1;

	public const int SQLITE_DBSTATUS_SCHEMA_USED = 2;

	public const int SQLITE_DBSTATUS_STMT_USED = 3;

	public const int SQLITE_DBSTATUS_LOOKASIDE_HIT = 4;

	public const int SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE = 5;

	public const int SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL = 6;

	public const int SQLITE_DBSTATUS_CACHE_HIT = 7;

	public const int SQLITE_DBSTATUS_CACHE_MISS = 8;

	public const int SQLITE_DBSTATUS_CACHE_WRITE = 9;

	public const int SQLITE_DBSTATUS_DEFERRED_FKS = 10;

	public const int SQLITE_STATUS_MEMORY_USED = 0;

	public const int SQLITE_STATUS_PAGECACHE_USED = 1;

	public const int SQLITE_STATUS_PAGECACHE_OVERFLOW = 2;

	public const int SQLITE_STATUS_SCRATCH_USED = 3;

	public const int SQLITE_STATUS_SCRATCH_OVERFLOW = 4;

	public const int SQLITE_STATUS_MALLOC_SIZE = 5;

	public const int SQLITE_STATUS_PARSER_STACK = 6;

	public const int SQLITE_STATUS_PAGECACHE_SIZE = 7;

	public const int SQLITE_STATUS_SCRATCH_SIZE = 8;

	public const int SQLITE_STATUS_MALLOC_COUNT = 9;

	public const int SQLITE_STMTSTATUS_FULLSCAN_STEP = 1;

	public const int SQLITE_STMTSTATUS_SORT = 2;

	public const int SQLITE_STMTSTATUS_AUTOINDEX = 3;

	public const int SQLITE_STMTSTATUS_VM_STEP = 4;

	public const int SQLITE_DENY = 1;

	public const int SQLITE_IGNORE = 2;

	public const int SQLITE_TRACE_STMT = 1;

	public const int SQLITE_TRACE_PROFILE = 2;

	public const int SQLITE_TRACE_ROW = 4;

	public const int SQLITE_TRACE_CLOSE = 8;

	static raw()
	{
		_frozen = false;
	}

	public static void SetProvider(ISQLite3Provider imp)
	{
		if (!_frozen)
		{
			imp.sqlite3_libversion_number();
			_imp = imp;
		}
	}

	public static void FreezeProvider(bool b = true)
	{
		_frozen = b;
	}

	public static string GetNativeLibraryName()
	{
		return _imp.GetNativeLibraryName();
	}

	public static int sqlite3_open(utf8z filename, out sqlite3 db)
	{
		IntPtr db2;
		int result = _imp.sqlite3_open(filename, out db2);
		db = sqlite3.New(db2);
		return result;
	}

	public static int sqlite3_open(string filename, out sqlite3 db)
	{
		return sqlite3_open(filename.to_utf8z(), out db);
	}

	public static int sqlite3_open_v2(utf8z filename, out sqlite3 db, int flags, utf8z vfs)
	{
		IntPtr db2;
		int result = _imp.sqlite3_open_v2(filename, out db2, flags, vfs);
		db = sqlite3.New(db2);
		return result;
	}

	public static int sqlite3_open_v2(string filename, out sqlite3 db, int flags, string vfs)
	{
		return sqlite3_open_v2(filename.to_utf8z(), out db, flags, vfs.to_utf8z());
	}

	public static int sqlite3__vfs__delete(utf8z vfs, utf8z pathname, int syncdir)
	{
		return _imp.sqlite3__vfs__delete(vfs, pathname, syncdir);
	}

	public static int sqlite3__vfs__delete(string vfs, string pathname, int syncdir)
	{
		return sqlite3__vfs__delete(vfs.to_utf8z(), pathname.to_utf8z(), syncdir);
	}

	internal static int internal_sqlite3_close_v2(IntPtr p)
	{
		return _imp.sqlite3_close_v2(p);
	}

	internal static int internal_sqlite3_close(IntPtr p)
	{
		return _imp.sqlite3_close(p);
	}

	public static int sqlite3_close_v2(sqlite3 db)
	{
		return db.manual_close_v2();
	}

	public static int sqlite3_close(sqlite3 db)
	{
		return db.manual_close();
	}

	public static int sqlite3_enable_shared_cache(int enable)
	{
		return _imp.sqlite3_enable_shared_cache(enable);
	}

	public static void sqlite3_interrupt(sqlite3 db)
	{
		_imp.sqlite3_interrupt(db);
	}

	public static int sqlite3_config_log(delegate_log f, object v)
	{
		return _imp.sqlite3_config_log(f, v);
	}

	public static int sqlite3_config_log(strdelegate_log f, object v)
	{
		delegate_log f2 = ((f != null) ? ((delegate_log)delegate(object ob, int e, utf8z msg)
		{
			f(ob, e, msg.utf8_to_string());
		}) : null);
		return sqlite3_config_log(f2, v);
	}

	public static void sqlite3_log(int errcode, utf8z s)
	{
		_imp.sqlite3_log(errcode, s);
	}

	public static void sqlite3_log(int errcode, string s)
	{
		sqlite3_log(errcode, s.to_utf8z());
	}

	public static void sqlite3_commit_hook(sqlite3 db, delegate_commit f, object v)
	{
		_imp.sqlite3_commit_hook(db, f, v);
	}

	public static void sqlite3_rollback_hook(sqlite3 db, delegate_rollback f, object v)
	{
		_imp.sqlite3_rollback_hook(db, f, v);
	}

	public static void sqlite3_trace(sqlite3 db, delegate_trace f, object v)
	{
		_imp.sqlite3_trace(db, f, v);
	}

	public static void sqlite3_trace(sqlite3 db, strdelegate_trace f, object v)
	{
		delegate_trace f2 = ((f != null) ? ((delegate_trace)delegate(object ob, utf8z sp)
		{
			f(v, sp.utf8_to_string());
		}) : null);
		sqlite3_trace(db, f2, v);
	}

	public static void sqlite3_profile(sqlite3 db, delegate_profile f, object v)
	{
		_imp.sqlite3_profile(db, f, v);
	}

	public static void sqlite3_profile(sqlite3 db, strdelegate_profile f, object v)
	{
		delegate_profile f2 = ((f != null) ? ((delegate_profile)delegate(object ob, utf8z sp, long ns)
		{
			f(v, sp.utf8_to_string(), ns);
		}) : null);
		sqlite3_profile(db, f2, v);
	}

	public static void sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v)
	{
		_imp.sqlite3_progress_handler(db, instructions, func, v);
	}

	public static void sqlite3_update_hook(sqlite3 db, delegate_update f, object v)
	{
		_imp.sqlite3_update_hook(db, f, v);
	}

	public static void sqlite3_update_hook(sqlite3 db, strdelegate_update f, object v)
	{
		delegate_update f2 = ((f != null) ? ((delegate_update)delegate(object ob, int typ, utf8z dbname, utf8z tbl, long rowid)
		{
			f(ob, typ, dbname.utf8_to_string(), tbl.utf8_to_string(), rowid);
		}) : null);
		sqlite3_update_hook(db, f2, v);
	}

	public static int sqlite3_create_collation(sqlite3 db, string name, object v, strdelegate_collation f)
	{
		byte[] name2 = name.to_utf8_with_z();
		delegate_collation func = ((f != null) ? ((delegate_collation)((object ob, ReadOnlySpan<byte> s1, ReadOnlySpan<byte> s2) => f(ob, s1.utf8_span_to_string(), s2.utf8_span_to_string()))) : null);
		return _imp.sqlite3_create_collation(db, name2, v, func);
	}

	public static int sqlite3_create_function(sqlite3 db, string name, int nArg, int flags, object v, delegate_function_scalar func)
	{
		byte[] name2 = name.to_utf8_with_z();
		return _imp.sqlite3_create_function(db, name2, nArg, flags, v, func);
	}

	public static int sqlite3_create_function(sqlite3 db, string name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
	{
		byte[] name2 = name.to_utf8_with_z();
		return _imp.sqlite3_create_function(db, name2, nArg, flags, v, func_step, func_final);
	}

	public static int sqlite3_create_function(sqlite3 db, string name, int nArg, object v, delegate_function_scalar func)
	{
		return sqlite3_create_function(db, name, nArg, 0, v, func);
	}

	public static int sqlite3_create_function(sqlite3 db, string name, int nArg, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final)
	{
		return sqlite3_create_function(db, name, nArg, 0, v, func_step, func_final);
	}

	public static int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg)
	{
		return _imp.sqlite3_db_status(db, op, out current, out highest, resetFlg);
	}

	public unsafe static string utf8_span_to_string(this ReadOnlySpan<byte> p)
	{
		if (p.Length == 0)
		{
			return "";
		}
		fixed (byte* bytes = p)
		{
			return Encoding.UTF8.GetString(bytes, p.Length);
		}
	}

	public static int sqlite3_key(sqlite3 db, ReadOnlySpan<byte> k)
	{
		return _imp.sqlite3_key(db, k);
	}

	public static int sqlite3_key_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
	{
		return _imp.sqlite3_key_v2(db, name, k);
	}

	public static int sqlite3_rekey(sqlite3 db, ReadOnlySpan<byte> k)
	{
		return _imp.sqlite3_rekey(db, k);
	}

	public static int sqlite3_rekey_v2(sqlite3 db, utf8z name, ReadOnlySpan<byte> k)
	{
		return _imp.sqlite3_rekey_v2(db, name, k);
	}

	public static utf8z sqlite3_libversion()
	{
		return _imp.sqlite3_libversion();
	}

	public static int sqlite3_libversion_number()
	{
		return _imp.sqlite3_libversion_number();
	}

	public static int sqlite3_threadsafe()
	{
		return _imp.sqlite3_threadsafe();
	}

	public static int sqlite3_initialize()
	{
		return _imp.sqlite3_initialize();
	}

	public static int sqlite3_shutdown()
	{
		return _imp.sqlite3_shutdown();
	}

	public static int sqlite3_config(int op)
	{
		return _imp.sqlite3_config(op);
	}

	public static int sqlite3_config(int op, int val)
	{
		return _imp.sqlite3_config(op, val);
	}

	public static int sqlite3_enable_load_extension(sqlite3 db, int onoff)
	{
		return _imp.sqlite3_enable_load_extension(db, onoff);
	}

	public static utf8z sqlite3_sourceid()
	{
		return _imp.sqlite3_sourceid();
	}

	public static long sqlite3_memory_used()
	{
		return _imp.sqlite3_memory_used();
	}

	public static long sqlite3_memory_highwater(int resetFlag)
	{
		return _imp.sqlite3_memory_highwater(resetFlag);
	}

	public static int sqlite3_status(int op, out int current, out int highwater, int resetFlag)
	{
		return _imp.sqlite3_status(op, out current, out highwater, resetFlag);
	}

	public static utf8z sqlite3_errmsg(sqlite3 db)
	{
		return _imp.sqlite3_errmsg(db);
	}

	public static int sqlite3_db_readonly(sqlite3 db, utf8z dbName)
	{
		return _imp.sqlite3_db_readonly(db, dbName);
	}

	public static int sqlite3_db_readonly(sqlite3 db, string dbName)
	{
		return sqlite3_db_readonly(db, dbName.to_utf8z());
	}

	public static utf8z sqlite3_db_filename(sqlite3 db, utf8z att)
	{
		return _imp.sqlite3_db_filename(db, att);
	}

	public static utf8z sqlite3_db_filename(sqlite3 db, string att)
	{
		return sqlite3_db_filename(db, att.to_utf8z());
	}

	public static long sqlite3_last_insert_rowid(sqlite3 db)
	{
		return _imp.sqlite3_last_insert_rowid(db);
	}

	public static int sqlite3_changes(sqlite3 db)
	{
		return _imp.sqlite3_changes(db);
	}

	public static int sqlite3_total_changes(sqlite3 db)
	{
		return _imp.sqlite3_total_changes(db);
	}

	public static int sqlite3_get_autocommit(sqlite3 db)
	{
		return _imp.sqlite3_get_autocommit(db);
	}

	public static int sqlite3_busy_timeout(sqlite3 db, int ms)
	{
		return _imp.sqlite3_busy_timeout(db, ms);
	}

	public static int sqlite3_extended_result_codes(sqlite3 db, int onoff)
	{
		return _imp.sqlite3_extended_result_codes(db, onoff);
	}

	public static int sqlite3_errcode(sqlite3 db)
	{
		return _imp.sqlite3_errcode(db);
	}

	public static int sqlite3_extended_errcode(sqlite3 db)
	{
		return _imp.sqlite3_extended_errcode(db);
	}

	public static utf8z sqlite3_errstr(int rc)
	{
		return _imp.sqlite3_errstr(rc);
	}

	public static int sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out sqlite3_stmt stmt)
	{
		IntPtr stmt2;
		ReadOnlySpan<byte> remain;
		int result = _imp.sqlite3_prepare_v2(db, sql, out stmt2, out remain);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v2(sqlite3 db, utf8z sql, out sqlite3_stmt stmt)
	{
		IntPtr stmt2;
		utf8z remain;
		int result = _imp.sqlite3_prepare_v2(db, sql, out stmt2, out remain);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v2(sqlite3 db, string sql, out sqlite3_stmt stmt)
	{
		return sqlite3_prepare_v2(db, sql.to_utf8z(), out stmt);
	}

	public static int sqlite3_prepare_v2(sqlite3 db, ReadOnlySpan<byte> sql, out sqlite3_stmt stmt, out ReadOnlySpan<byte> tail)
	{
		IntPtr stmt2;
		int result = _imp.sqlite3_prepare_v2(db, sql, out stmt2, out tail);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v2(sqlite3 db, utf8z sql, out sqlite3_stmt stmt, out utf8z tail)
	{
		IntPtr stmt2;
		int result = _imp.sqlite3_prepare_v2(db, sql, out stmt2, out tail);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v2(sqlite3 db, string sql, out sqlite3_stmt stmt, out string tail)
	{
		utf8z tail2;
		int result = sqlite3_prepare_v2(db, sql.to_utf8z(), out stmt, out tail2);
		tail = tail2.utf8_to_string();
		return result;
	}

	public static int sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out sqlite3_stmt stmt)
	{
		IntPtr stmt2;
		ReadOnlySpan<byte> remain;
		int result = _imp.sqlite3_prepare_v3(db, sql, flags, out stmt2, out remain);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out sqlite3_stmt stmt)
	{
		IntPtr stmt2;
		utf8z remain;
		int result = _imp.sqlite3_prepare_v3(db, sql, flags, out stmt2, out remain);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v3(sqlite3 db, string sql, uint flags, out sqlite3_stmt stmt)
	{
		return sqlite3_prepare_v3(db, sql.to_utf8z(), flags, out stmt);
	}

	public static int sqlite3_prepare_v3(sqlite3 db, ReadOnlySpan<byte> sql, uint flags, out sqlite3_stmt stmt, out ReadOnlySpan<byte> tail)
	{
		IntPtr stmt2;
		int result = _imp.sqlite3_prepare_v3(db, sql, flags, out stmt2, out tail);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v3(sqlite3 db, utf8z sql, uint flags, out sqlite3_stmt stmt, out utf8z tail)
	{
		IntPtr stmt2;
		int result = _imp.sqlite3_prepare_v3(db, sql, flags, out stmt2, out tail);
		stmt = sqlite3_stmt.From(stmt2, db);
		return result;
	}

	public static int sqlite3_prepare_v3(sqlite3 db, string sql, uint flags, out sqlite3_stmt stmt, out string tail)
	{
		utf8z tail2;
		int result = sqlite3_prepare_v3(db, sql.to_utf8z(), flags, out stmt, out tail2);
		tail = tail2.utf8_to_string();
		return result;
	}

	public static int sqlite3_exec(sqlite3 db, string sql, strdelegate_exec callback, object user_data, out string errMsg)
	{
		delegate_exec callback2 = ((callback == null) ? null : ((delegate_exec)delegate(object ob, IntPtr[] values, IntPtr[] names)
		{
			string[] array = new string[values.Length];
			string[] array2 = new string[names.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = util.from_utf8_z(values[i]);
				array2[i] = util.from_utf8_z(names[i]);
			}
			return callback(ob, array, array2);
		}));
		IntPtr errMsg2;
		int result = _imp.sqlite3_exec(db, sql.to_utf8z(), callback2, user_data, out errMsg2);
		if (errMsg2 == IntPtr.Zero)
		{
			errMsg = null;
			return result;
		}
		errMsg = util.from_utf8_z(errMsg2);
		_imp.sqlite3_free(errMsg2);
		return result;
	}

	public static int sqlite3_exec(sqlite3 db, string sql, out string errMsg)
	{
		IntPtr errMsg2;
		int result = _imp.sqlite3_exec(db, sql.to_utf8z(), null, null, out errMsg2);
		if (errMsg2 == IntPtr.Zero)
		{
			errMsg = null;
			return result;
		}
		errMsg = util.from_utf8_z(errMsg2);
		_imp.sqlite3_free(errMsg2);
		return result;
	}

	public static int sqlite3_exec(sqlite3 db, string sql)
	{
		IntPtr errMsg;
		int result = _imp.sqlite3_exec(db, sql.to_utf8z(), null, null, out errMsg);
		if (!(errMsg == IntPtr.Zero))
		{
			_imp.sqlite3_free(errMsg);
		}
		return result;
	}

	public static int sqlite3_step(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_step(stmt);
	}

	public static int sqlite3_finalize(sqlite3_stmt stmt)
	{
		return stmt.manual_close();
	}

	public static int internal_sqlite3_finalize(IntPtr stmt)
	{
		return _imp.sqlite3_finalize(stmt);
	}

	public static int sqlite3_reset(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_reset(stmt);
	}

	public static int sqlite3_clear_bindings(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_clear_bindings(stmt);
	}

	public static int sqlite3_stmt_status(sqlite3_stmt stmt, int op, int resetFlg)
	{
		return _imp.sqlite3_stmt_status(stmt, op, resetFlg);
	}

	public static int sqlite3_complete(utf8z sql)
	{
		return _imp.sqlite3_complete(sql);
	}

	public static int sqlite3_complete(string sql)
	{
		return sqlite3_complete(sql.to_utf8z());
	}

	public static int sqlite3_compileoption_used(utf8z s)
	{
		return _imp.sqlite3_compileoption_used(s);
	}

	public static int sqlite3_compileoption_used(string s)
	{
		return sqlite3_compileoption_used(s.to_utf8z());
	}

	public static utf8z sqlite3_compileoption_get(int n)
	{
		return _imp.sqlite3_compileoption_get(n);
	}

	public static int sqlite3_table_column_metadata(sqlite3 db, utf8z dbName, utf8z tblName, utf8z colName, out utf8z dataType, out utf8z collSeq, out int notNull, out int primaryKey, out int autoInc)
	{
		return _imp.sqlite3_table_column_metadata(db, dbName, tblName, colName, out dataType, out collSeq, out notNull, out primaryKey, out autoInc);
	}

	public static int sqlite3_table_column_metadata(sqlite3 db, string dbName, string tblName, string colName, out string dataType, out string collSeq, out int notNull, out int primaryKey, out int autoInc)
	{
		utf8z dataType2;
		utf8z collSeq2;
		int result = sqlite3_table_column_metadata(db, dbName.to_utf8z(), tblName.to_utf8z(), colName.to_utf8z(), out dataType2, out collSeq2, out notNull, out primaryKey, out autoInc);
		dataType = dataType2.utf8_to_string();
		collSeq = collSeq2.utf8_to_string();
		return result;
	}

	public static utf8z sqlite3_sql(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_sql(stmt);
	}

	public static sqlite3 sqlite3_db_handle(sqlite3_stmt stmt)
	{
		return stmt.db;
	}

	public static sqlite3_stmt sqlite3_next_stmt(sqlite3 db, sqlite3_stmt stmt)
	{
		IntPtr intPtr = _imp.sqlite3_next_stmt(db, stmt?.ptr ?? IntPtr.Zero);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		return db.find_stmt(intPtr);
	}

	public static int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size)
	{
		return _imp.sqlite3_bind_zeroblob(stmt, index, size);
	}

	public static utf8z sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_bind_parameter_name(stmt, index);
	}

	public static object sqlite3_user_data(sqlite3_context context)
	{
		return context.user_data;
	}

	public static void sqlite3_result_null(sqlite3_context context)
	{
		_imp.sqlite3_result_null(context.ptr);
	}

	public static void sqlite3_result_blob(sqlite3_context context, ReadOnlySpan<byte> val)
	{
		_imp.sqlite3_result_blob(context.ptr, val);
	}

	public static void sqlite3_result_error(sqlite3_context context, ReadOnlySpan<byte> val)
	{
		_imp.sqlite3_result_error(context.ptr, val);
	}

	public static void sqlite3_result_error(sqlite3_context context, utf8z val)
	{
		_imp.sqlite3_result_error(context.ptr, val);
	}

	public static void sqlite3_result_error(sqlite3_context context, string val)
	{
		sqlite3_result_error(context, val.to_utf8z());
	}

	public static void sqlite3_result_text(sqlite3_context context, ReadOnlySpan<byte> val)
	{
		_imp.sqlite3_result_text(context.ptr, val);
	}

	public static void sqlite3_result_text(sqlite3_context context, utf8z val)
	{
		_imp.sqlite3_result_text(context.ptr, val);
	}

	public static void sqlite3_result_text(sqlite3_context context, string val)
	{
		sqlite3_result_text(context, val.to_utf8z());
	}

	public static void sqlite3_result_double(sqlite3_context context, double val)
	{
		_imp.sqlite3_result_double(context.ptr, val);
	}

	public static void sqlite3_result_int(sqlite3_context context, int val)
	{
		_imp.sqlite3_result_int(context.ptr, val);
	}

	public static void sqlite3_result_int64(sqlite3_context context, long val)
	{
		_imp.sqlite3_result_int64(context.ptr, val);
	}

	public static void sqlite3_result_zeroblob(sqlite3_context context, int n)
	{
		_imp.sqlite3_result_zeroblob(context.ptr, n);
	}

	public static void sqlite3_result_error_toobig(sqlite3_context context)
	{
		_imp.sqlite3_result_error_toobig(context.ptr);
	}

	public static void sqlite3_result_error_nomem(sqlite3_context context)
	{
		_imp.sqlite3_result_error_nomem(context.ptr);
	}

	public static void sqlite3_result_error_code(sqlite3_context context, int code)
	{
		_imp.sqlite3_result_error_code(context.ptr, code);
	}

	public static ReadOnlySpan<byte> sqlite3_value_blob(sqlite3_value val)
	{
		return _imp.sqlite3_value_blob(val.ptr);
	}

	public static int sqlite3_value_bytes(sqlite3_value val)
	{
		return _imp.sqlite3_value_bytes(val.ptr);
	}

	public static double sqlite3_value_double(sqlite3_value val)
	{
		return _imp.sqlite3_value_double(val.ptr);
	}

	public static int sqlite3_value_int(sqlite3_value val)
	{
		return _imp.sqlite3_value_int(val.ptr);
	}

	public static long sqlite3_value_int64(sqlite3_value val)
	{
		return _imp.sqlite3_value_int64(val.ptr);
	}

	public static int sqlite3_value_type(sqlite3_value val)
	{
		return _imp.sqlite3_value_type(val.ptr);
	}

	public static utf8z sqlite3_value_text(sqlite3_value val)
	{
		return _imp.sqlite3_value_text(val.ptr);
	}

	public static int sqlite3_bind_blob(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> blob)
	{
		return _imp.sqlite3_bind_blob(stmt, index, blob);
	}

	public static int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val)
	{
		return _imp.sqlite3_bind_double(stmt, index, val);
	}

	public static int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val)
	{
		return _imp.sqlite3_bind_int(stmt, index, val);
	}

	public static int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val)
	{
		return _imp.sqlite3_bind_int64(stmt, index, val);
	}

	public static int sqlite3_bind_null(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_bind_null(stmt, index);
	}

	public static int sqlite3_bind_text(sqlite3_stmt stmt, int index, ReadOnlySpan<byte> val)
	{
		return _imp.sqlite3_bind_text(stmt, index, val);
	}

	public static int sqlite3_bind_text(sqlite3_stmt stmt, int index, utf8z val)
	{
		return _imp.sqlite3_bind_text(stmt, index, val);
	}

	public static int sqlite3_bind_text(sqlite3_stmt stmt, int index, string val)
	{
		return sqlite3_bind_text(stmt, index, val.to_utf8z());
	}

	public static int sqlite3_bind_parameter_count(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_bind_parameter_count(stmt);
	}

	public static int sqlite3_bind_parameter_index(sqlite3_stmt stmt, utf8z strName)
	{
		return _imp.sqlite3_bind_parameter_index(stmt, strName);
	}

	public static int sqlite3_bind_parameter_index(sqlite3_stmt stmt, string strName)
	{
		return sqlite3_bind_parameter_index(stmt, strName.to_utf8z());
	}

	public static int sqlite3_stmt_busy(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_stmt_busy(stmt);
	}

	public static int sqlite3_stmt_readonly(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_stmt_readonly(stmt);
	}

	public static utf8z sqlite3_column_database_name(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_database_name(stmt, index);
	}

	public static utf8z sqlite3_column_name(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_name(stmt, index);
	}

	public static utf8z sqlite3_column_origin_name(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_origin_name(stmt, index);
	}

	public static utf8z sqlite3_column_table_name(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_table_name(stmt, index);
	}

	public static utf8z sqlite3_column_text(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_text(stmt, index);
	}

	public static int sqlite3_column_count(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_column_count(stmt);
	}

	public static int sqlite3_data_count(sqlite3_stmt stmt)
	{
		return _imp.sqlite3_data_count(stmt);
	}

	public static double sqlite3_column_double(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_double(stmt, index);
	}

	public static int sqlite3_column_int(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_int(stmt, index);
	}

	public static long sqlite3_column_int64(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_int64(stmt, index);
	}

	public static ReadOnlySpan<byte> sqlite3_column_blob(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_blob(stmt, index);
	}

	public static int sqlite3_column_bytes(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_bytes(stmt, index);
	}

	public static int sqlite3_column_type(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_type(stmt, index);
	}

	public static utf8z sqlite3_column_decltype(sqlite3_stmt stmt, int index)
	{
		return _imp.sqlite3_column_decltype(stmt, index);
	}

	public static sqlite3_backup sqlite3_backup_init(sqlite3 destDb, string destName, sqlite3 sourceDb, string sourceName)
	{
		return _imp.sqlite3_backup_init(destDb, destName.to_utf8z(), sourceDb, sourceName.to_utf8z());
	}

	public static int sqlite3_backup_step(sqlite3_backup backup, int nPage)
	{
		return _imp.sqlite3_backup_step(backup, nPage);
	}

	public static int sqlite3_backup_remaining(sqlite3_backup backup)
	{
		return _imp.sqlite3_backup_remaining(backup);
	}

	public static int sqlite3_backup_pagecount(sqlite3_backup backup)
	{
		return _imp.sqlite3_backup_pagecount(backup);
	}

	public static int sqlite3_backup_finish(sqlite3_backup backup)
	{
		return backup.manual_close();
	}

	internal static int internal_sqlite3_backup_finish(IntPtr p)
	{
		return _imp.sqlite3_backup_finish(p);
	}

	public static int sqlite3_blob_open(sqlite3 db, utf8z db_utf8, utf8z table_utf8, utf8z col_utf8, long rowid, int flags, out sqlite3_blob blob)
	{
		return _imp.sqlite3_blob_open(db, db_utf8, table_utf8, col_utf8, rowid, flags, out blob);
	}

	public static int sqlite3_blob_open(sqlite3 db, string sdb, string table, string col, long rowid, int flags, out sqlite3_blob blob)
	{
		return sqlite3_blob_open(db, sdb.to_utf8z(), table.to_utf8z(), col.to_utf8z(), rowid, flags, out blob);
	}

	public static int sqlite3_blob_bytes(sqlite3_blob blob)
	{
		return _imp.sqlite3_blob_bytes(blob);
	}

	public static int sqlite3_blob_reopen(sqlite3_blob blob, long rowid)
	{
		return _imp.sqlite3_blob_reopen(blob, rowid);
	}

	public static int sqlite3_blob_write(sqlite3_blob blob, ReadOnlySpan<byte> b, int offset)
	{
		return _imp.sqlite3_blob_write(blob, b, offset);
	}

	public static int sqlite3_blob_read(sqlite3_blob blob, Span<byte> b, int offset)
	{
		return _imp.sqlite3_blob_read(blob, b, offset);
	}

	public static int sqlite3_blob_close(sqlite3_blob blob)
	{
		return blob.manual_close();
	}

	internal static int internal_sqlite3_blob_close(IntPtr blob)
	{
		return _imp.sqlite3_blob_close(blob);
	}

	public static int sqlite3_wal_autocheckpoint(sqlite3 db, int n)
	{
		return _imp.sqlite3_wal_autocheckpoint(db, n);
	}

	public static int sqlite3_wal_checkpoint(sqlite3 db, string dbName)
	{
		return _imp.sqlite3_wal_checkpoint(db, dbName.to_utf8z());
	}

	public static int sqlite3_wal_checkpoint_v2(sqlite3 db, string dbName, int eMode, out int logSize, out int framesCheckPointed)
	{
		return _imp.sqlite3_wal_checkpoint_v2(db, dbName.to_utf8z(), eMode, out logSize, out framesCheckPointed);
	}

	public static int sqlite3_set_authorizer(sqlite3 db, delegate_authorizer f, object user_data)
	{
		return _imp.sqlite3_set_authorizer(db, f, user_data);
	}

	public static int sqlite3_set_authorizer(sqlite3 db, strdelegate_authorizer f, object user_data)
	{
		delegate_authorizer f2 = ((f != null) ? ((delegate_authorizer)((object ob, int a, utf8z p0, utf8z p1, utf8z dbname, utf8z v) => f(ob, a, p0.utf8_to_string(), p1.utf8_to_string(), dbname.utf8_to_string(), v.utf8_to_string()))) : null);
		return sqlite3_set_authorizer(db, f2, user_data);
	}

	public static int sqlite3_win32_set_directory(int typ, string path)
	{
		return _imp.sqlite3_win32_set_directory(typ, path.to_utf8z());
	}
}
