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

		private static object Init()
		{
			return null;
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

		public static readonly object forInit = Init();

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
	}
}
