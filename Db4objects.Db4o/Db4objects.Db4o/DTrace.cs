namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class DTrace
	{
		public const bool enabled = false;

		private static void BreakPoint()
		{
		}

		private static void Configure()
		{
		}

		private static void Init()
		{
			return;
			ADD_TO_CLASS_INDEX = new Db4objects.Db4o.DTrace(true, true, "add to class index tree"
				, true);
			BEGIN_TOP_LEVEL_CALL = new Db4objects.Db4o.DTrace(true, true, "begin top level call"
				, true);
			BIND = new Db4objects.Db4o.DTrace(true, true, "bind", true);
			BTREE_NODE_REMOVE = new Db4objects.Db4o.DTrace(true, true, "btreenode remove", true
				);
			BTREE_NODE_COMMIT_OR_ROLLBACK = new Db4objects.Db4o.DTrace(true, true, "btreenode commit or rollback"
				, true);
			CANDIDATE_READ = new Db4objects.Db4o.DTrace(true, true, "candidate read", true);
			CLOSE = new Db4objects.Db4o.DTrace(true, true, "close", true);
			COLLECT_CHILDREN = new Db4objects.Db4o.DTrace(true, true, "collect children", true
				);
			COMMIT = new Db4objects.Db4o.DTrace(false, false, "commit", true);
			CONTINUESET = new Db4objects.Db4o.DTrace(true, true, "continueset", true);
			CREATE_CANDIDATE = new Db4objects.Db4o.DTrace(true, true, "create candidate", true
				);
			DELETE = new Db4objects.Db4o.DTrace(true, true, "delete", true);
			DONOTINCLUDE = new Db4objects.Db4o.DTrace(true, true, "donotinclude", true);
			END_TOP_LEVEL_CALL = new Db4objects.Db4o.DTrace(true, true, "end top level call", 
				true);
			EVALUATE_SELF = new Db4objects.Db4o.DTrace(true, true, "evaluate self", true);
			FREE = new Db4objects.Db4o.DTrace(true, true, "free", true);
			FILE_FREE = new Db4objects.Db4o.DTrace(true, true, "fileFree", true);
			FREE_RAM = new Db4objects.Db4o.DTrace(true, true, "freeRAM", true);
			FREE_ON_COMMIT = new Db4objects.Db4o.DTrace(true, true, "trans freeOnCommit", true
				);
			FREE_ON_ROLLBACK = new Db4objects.Db4o.DTrace(true, true, "trans freeOnRollback", 
				true);
			GET_SLOT = new Db4objects.Db4o.DTrace(true, true, "getSlot", true);
			GET_FREESPACE = new Db4objects.Db4o.DTrace(true, true, "getFreespace", true);
			GET_FREESPACE_RAM = new Db4objects.Db4o.DTrace(true, true, "getFreespaceRam", true
				);
			GET_YAPOBJECT = new Db4objects.Db4o.DTrace(true, true, "get yapObject", true);
			ID_TREE_ADD = new Db4objects.Db4o.DTrace(true, true, "id tree add", true);
			ID_TREE_REMOVE = new Db4objects.Db4o.DTrace(true, true, "id tree remove", true);
			IO_COPY = new Db4objects.Db4o.DTrace(true, true, "io copy", true);
			JUST_SET = new Db4objects.Db4o.DTrace(true, true, "just set", true);
			NEW_INSTANCE = new Db4objects.Db4o.DTrace(true, true, "newInstance", true);
			PRODUCE_SLOT_CHANGE = new Db4objects.Db4o.DTrace(true, true, "produce slot change"
				, true);
			QUERY_PROCESS = new Db4objects.Db4o.DTrace(true, true, "query process", true);
			READ_ARRAY_WRAPPER = new Db4objects.Db4o.DTrace(true, true, "read array wrapper", 
				true);
			READ_BYTES = new Db4objects.Db4o.DTrace(true, true, "readBytes", true);
			READ_ID = new Db4objects.Db4o.DTrace(true, true, "read ID", true);
			READ_SLOT = new Db4objects.Db4o.DTrace(true, true, "read slot", true);
			REFERENCE_REMOVED = new Db4objects.Db4o.DTrace(true, true, "reference removed", true
				);
			REGULAR_SEEK = new Db4objects.Db4o.DTrace(true, true, "regular seek", true);
			REMOVE_FROM_CLASS_INDEX = new Db4objects.Db4o.DTrace(true, true, "trans removeFromClassIndexTree"
				, true);
			REREAD_OLD_UUID = new Db4objects.Db4o.DTrace(true, true, "reread old uuid", true);
			SLOT_SET_POINTER = new Db4objects.Db4o.DTrace(true, true, "slot set pointer", true
				);
			SLOT_DELETE = new Db4objects.Db4o.DTrace(true, true, "slot delete", true);
			SLOT_FREE_ON_COMMIT = new Db4objects.Db4o.DTrace(true, true, "slot free on commit"
				, true);
			SLOT_FREE_ON_ROLLBACK_ID = new Db4objects.Db4o.DTrace(true, true, "slot free on rollback id"
				, true);
			SLOT_FREE_ON_ROLLBACK_ADDRESS = new Db4objects.Db4o.DTrace(true, true, "slot free on rollback address"
				, true);
			TRANS_COMMIT = new Db4objects.Db4o.DTrace(false, false, "trans commit", false);
			TRANS_DELETE = new Db4objects.Db4o.DTrace(true, true, "trans delete", true);
			TRANS_DONT_DELETE = new Db4objects.Db4o.DTrace(true, true, "trans dontDelete", true
				);
			TRANS_FLUSH = new Db4objects.Db4o.DTrace(true, true, "trans flush", true);
			YAPMETA_WRITE = new Db4objects.Db4o.DTrace(true, true, "yapmeta write", true);
			YAPCLASS_BY_ID = new Db4objects.Db4o.DTrace(true, true, "yapclass by id", true);
			YAPCLASS_INIT = new Db4objects.Db4o.DTrace(true, true, "yapclass init", true);
			YAPMETA_SET_ID = new Db4objects.Db4o.DTrace(true, true, "yapmeta setid", true);
			WRITE_BYTES = new Db4objects.Db4o.DTrace(true, true, "writeBytes", true);
			WRITE_POINTER = new Db4objects.Db4o.DTrace(true, true, "write pointer", true);
			WRITE_UPDATE_DELETE_MEMBERS = new Db4objects.Db4o.DTrace(true, true, "trans writeUpdateDeleteMembers"
				, true);
			WRITE_XBYTES = new Db4objects.Db4o.DTrace(true, true, "writeXBytes", true);
			Configure();
		}

		private static void TrackEventsWithoutRange()
		{
			_trackEventsWithoutRange = true;
		}

		private DTrace(bool enabled_, bool break_, string tag_, bool log_)
		{
		}

		private bool _enabled;

		private bool _break;

		private bool _log;

		private string _tag;

		private static long[] _rangeStart;

		private static long[] _rangeEnd;

		private static int _rangeCount;

		public static long _eventNr;

		private static long[] _breakEventNrs;

		private static int _breakEventCount;

		private static bool _trackEventsWithoutRange;

		public static Db4objects.Db4o.DTrace ADD_TO_CLASS_INDEX;

		public static Db4objects.Db4o.DTrace BEGIN_TOP_LEVEL_CALL;

		public static Db4objects.Db4o.DTrace BIND;

		public static Db4objects.Db4o.DTrace BTREE_NODE_COMMIT_OR_ROLLBACK;

		public static Db4objects.Db4o.DTrace BTREE_NODE_REMOVE;

		public static Db4objects.Db4o.DTrace CANDIDATE_READ;

		public static Db4objects.Db4o.DTrace CLOSE;

		public static Db4objects.Db4o.DTrace COLLECT_CHILDREN;

		public static Db4objects.Db4o.DTrace COMMIT;

		public static Db4objects.Db4o.DTrace CONTINUESET;

		public static Db4objects.Db4o.DTrace CREATE_CANDIDATE;

		public static Db4objects.Db4o.DTrace DELETE;

		public static Db4objects.Db4o.DTrace DONOTINCLUDE;

		public static Db4objects.Db4o.DTrace END_TOP_LEVEL_CALL;

		public static Db4objects.Db4o.DTrace EVALUATE_SELF;

		public static Db4objects.Db4o.DTrace FILE_FREE;

		public static Db4objects.Db4o.DTrace FREE;

		public static Db4objects.Db4o.DTrace FREE_RAM;

		public static Db4objects.Db4o.DTrace FREE_ON_COMMIT;

		public static Db4objects.Db4o.DTrace FREE_ON_ROLLBACK;

		public static Db4objects.Db4o.DTrace GET_SLOT;

		public static Db4objects.Db4o.DTrace GET_FREESPACE;

		public static Db4objects.Db4o.DTrace GET_FREESPACE_RAM;

		public static Db4objects.Db4o.DTrace GET_YAPOBJECT;

		public static Db4objects.Db4o.DTrace ID_TREE_ADD;

		public static Db4objects.Db4o.DTrace ID_TREE_REMOVE;

		public static Db4objects.Db4o.DTrace IO_COPY;

		public static Db4objects.Db4o.DTrace JUST_SET;

		public static Db4objects.Db4o.DTrace NEW_INSTANCE;

		public static Db4objects.Db4o.DTrace PRODUCE_SLOT_CHANGE;

		public static Db4objects.Db4o.DTrace QUERY_PROCESS;

		public static Db4objects.Db4o.DTrace READ_ARRAY_WRAPPER;

		public static Db4objects.Db4o.DTrace READ_BYTES;

		public static Db4objects.Db4o.DTrace READ_ID;

		public static Db4objects.Db4o.DTrace READ_SLOT;

		public static Db4objects.Db4o.DTrace REFERENCE_REMOVED;

		public static Db4objects.Db4o.DTrace REGULAR_SEEK;

		public static Db4objects.Db4o.DTrace REMOVE_FROM_CLASS_INDEX;

		public static Db4objects.Db4o.DTrace REREAD_OLD_UUID;

		public static Db4objects.Db4o.DTrace SLOT_SET_POINTER;

		public static Db4objects.Db4o.DTrace SLOT_DELETE;

		public static Db4objects.Db4o.DTrace SLOT_FREE_ON_COMMIT;

		public static Db4objects.Db4o.DTrace SLOT_FREE_ON_ROLLBACK_ID;

		public static Db4objects.Db4o.DTrace SLOT_FREE_ON_ROLLBACK_ADDRESS;

		public static Db4objects.Db4o.DTrace TRANS_COMMIT;

		public static Db4objects.Db4o.DTrace TRANS_DONT_DELETE;

		public static Db4objects.Db4o.DTrace TRANS_DELETE;

		public static Db4objects.Db4o.DTrace TRANS_FLUSH;

		public static Db4objects.Db4o.DTrace YAPCLASS_BY_ID;

		public static Db4objects.Db4o.DTrace YAPCLASS_INIT;

		public static Db4objects.Db4o.DTrace YAPMETA_SET_ID;

		public static Db4objects.Db4o.DTrace YAPMETA_WRITE;

		public static Db4objects.Db4o.DTrace WRITE_BYTES;

		public static Db4objects.Db4o.DTrace WRITE_POINTER;

		public static Db4objects.Db4o.DTrace WRITE_XBYTES;

		public static Db4objects.Db4o.DTrace WRITE_UPDATE_DELETE_MEMBERS;

		static DTrace()
		{
			Init();
		}

		private static Db4objects.Db4o.DTrace all;

		private static int current;

		public virtual void Log()
		{
		}

		public virtual void Log(long p)
		{
		}

		public virtual void LogInfo(string info)
		{
		}

		public virtual void Log(long p, string info)
		{
		}

		public virtual void LogLength(long start, long length)
		{
		}

		public virtual void LogEnd(long start, long end)
		{
		}

		public virtual void LogEnd(long start, long end, string info)
		{
		}

		public static void AddRange(long pos)
		{
		}

		public static void AddRangeWithLength(long start, long length)
		{
		}

		public static void AddRangeWithEnd(long start, long end)
		{
		}

		private static void BreakOnEvent(long eventNr)
		{
		}

		private string FormatInt(long i, int len)
		{
			return null;
		}

		private string FormatInt(long i)
		{
			return null;
		}

		private static void TurnAllOffExceptFor(Db4objects.Db4o.DTrace[] these)
		{
		}

		public static void NoWarnings()
		{
			BreakOnEvent(0);
			TrackEventsWithoutRange();
		}
	}
}
