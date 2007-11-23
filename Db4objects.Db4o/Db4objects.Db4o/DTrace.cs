/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using System.Text;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.IO;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class DTrace
	{
		public static bool enabled = false;

		public static bool writeToLogFile = false;

		public static bool writeToConsole = true;

		private static readonly string logFilePath = "C://";

		private static string logFileName;

		private static readonly object Lock = new object();

		private static readonly LatinStringIO stringIO = new LatinStringIO();

		public static RandomAccessFile _logFile;

		private static void BreakPoint()
		{
			if (enabled)
			{
			}
		}

		private static void Configure()
		{
			if (enabled)
			{
				TrackEventsWithoutRange();
				TurnAllOffExceptFor(new Db4objects.Db4o.DTrace[] { BLOCKING_QUEUE_STOPPED_EXCEPTION
					, CLIENT_MESSAGE_LOOP_EXCEPTION, CLOSE, CLOSE_CALLED, FATAL_EXCEPTION, SERVER_MESSAGE_LOOP_EXCEPTION
					 });
			}
		}

		private static void Init()
		{
			if (enabled)
			{
				ADD_TO_CLASS_INDEX = new Db4objects.Db4o.DTrace(true, true, "add to class index tree"
					, true);
				BEGIN_TOP_LEVEL_CALL = new Db4objects.Db4o.DTrace(true, true, "begin top level call"
					, true);
				BIND = new Db4objects.Db4o.DTrace(true, true, "bind", true);
				BLOCKING_QUEUE_STOPPED_EXCEPTION = new Db4objects.Db4o.DTrace(true, true, "blocking queue stopped exception"
					, true);
				BTREE_NODE_REMOVE = new Db4objects.Db4o.DTrace(true, true, "btreenode remove", true
					);
				BTREE_NODE_COMMIT_OR_ROLLBACK = new Db4objects.Db4o.DTrace(true, true, "btreenode commit or rollback"
					, true);
				CANDIDATE_READ = new Db4objects.Db4o.DTrace(true, true, "candidate read", true);
				CLIENT_MESSAGE_LOOP_EXCEPTION = new Db4objects.Db4o.DTrace(true, true, "client message loop exception"
					, true);
				CLOSE = new Db4objects.Db4o.DTrace(true, true, "close", true);
				CLOSE_CALLED = new Db4objects.Db4o.DTrace(true, true, "close called", true);
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
				FATAL_EXCEPTION = new Db4objects.Db4o.DTrace(true, true, "fatal exception", true);
				FREE = new Db4objects.Db4o.DTrace(true, true, "free", true);
				FILE_FREE = new Db4objects.Db4o.DTrace(true, true, "fileFree", true);
				FREE_RAM = new Db4objects.Db4o.DTrace(true, true, "freeRAM", true);
				FREE_ON_COMMIT = new Db4objects.Db4o.DTrace(true, true, "trans freeOnCommit", true
					);
				FREE_ON_ROLLBACK = new Db4objects.Db4o.DTrace(true, true, "trans freeOnRollback", 
					true);
				FREE_POINTER_ON_ROLLBACK = new Db4objects.Db4o.DTrace(true, true, "freePointerOnRollback"
					, true);
				GET_POINTER_SLOT = new Db4objects.Db4o.DTrace(true, true, "getPointerSlot", true);
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
				SERVER_MESSAGE_LOOP_EXCEPTION = new Db4objects.Db4o.DTrace(true, true, "server message loop exception"
					, true);
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
		}

		private static void TrackEventsWithoutRange()
		{
			_trackEventsWithoutRange = true;
		}

		private DTrace(bool enabled_, bool break_, string tag_, bool log_)
		{
			if (enabled)
			{
				_enabled = enabled_;
				_break = break_;
				_tag = tag_;
				_log = log_;
				if (all == null)
				{
					all = new Db4objects.Db4o.DTrace[100];
				}
				all[current++] = this;
			}
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

		public static Db4objects.Db4o.DTrace BLOCKING_QUEUE_STOPPED_EXCEPTION;

		public static Db4objects.Db4o.DTrace BTREE_NODE_COMMIT_OR_ROLLBACK;

		public static Db4objects.Db4o.DTrace BTREE_NODE_REMOVE;

		public static Db4objects.Db4o.DTrace CANDIDATE_READ;

		public static Db4objects.Db4o.DTrace CLIENT_MESSAGE_LOOP_EXCEPTION;

		public static Db4objects.Db4o.DTrace CLOSE;

		public static Db4objects.Db4o.DTrace CLOSE_CALLED;

		public static Db4objects.Db4o.DTrace COLLECT_CHILDREN;

		public static Db4objects.Db4o.DTrace COMMIT;

		public static Db4objects.Db4o.DTrace CONTINUESET;

		public static Db4objects.Db4o.DTrace CREATE_CANDIDATE;

		public static Db4objects.Db4o.DTrace DELETE;

		public static Db4objects.Db4o.DTrace DONOTINCLUDE;

		public static Db4objects.Db4o.DTrace END_TOP_LEVEL_CALL;

		public static Db4objects.Db4o.DTrace EVALUATE_SELF;

		public static Db4objects.Db4o.DTrace FATAL_EXCEPTION;

		public static Db4objects.Db4o.DTrace FILE_FREE;

		public static Db4objects.Db4o.DTrace FREE;

		public static Db4objects.Db4o.DTrace FREE_RAM;

		public static Db4objects.Db4o.DTrace FREE_ON_COMMIT;

		public static Db4objects.Db4o.DTrace FREE_ON_ROLLBACK;

		public static Db4objects.Db4o.DTrace FREE_POINTER_ON_ROLLBACK;

		public static Db4objects.Db4o.DTrace GET_SLOT;

		public static Db4objects.Db4o.DTrace GET_POINTER_SLOT;

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

		public static Db4objects.Db4o.DTrace SERVER_MESSAGE_LOOP_EXCEPTION;

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

		private static Db4objects.Db4o.DTrace[] all;

		private static int current;

		public virtual void Log()
		{
			if (enabled)
			{
				Log(-1);
			}
		}

		public virtual void LogStack(string msg)
		{
			if (enabled)
			{
				Log(msg);
				Log(Platform4.StackTrace());
			}
		}

		public virtual void Log(string msg)
		{
			if (enabled)
			{
				Log(-1, msg);
			}
		}

		public virtual void Log(long p)
		{
			if (enabled)
			{
				LogLength(p, 1);
			}
		}

		public virtual void LogInfo(string info)
		{
			if (enabled)
			{
				LogEnd(-1, 0, info);
			}
		}

		public virtual void Log(long p, string info)
		{
			if (enabled)
			{
				LogEnd(p, 0, info);
			}
		}

		public virtual void LogLength(long start, long length)
		{
			if (enabled)
			{
				LogEnd(start, start + length - 1);
			}
		}

		public virtual void LogLength(Slot slot)
		{
			if (enabled)
			{
				LogLength(slot.Address(), slot.Length());
			}
		}

		public virtual void LogEnd(long start, long end)
		{
			if (enabled)
			{
				LogEnd(start, end, null);
			}
		}

		public virtual void LogEnd(long start, long end, string info)
		{
			if (enabled)
			{
				if (!_enabled)
				{
					return;
				}
				bool inRange = false;
				if (_rangeCount == 0)
				{
					inRange = true;
				}
				for (int i = 0; i < _rangeCount; i++)
				{
					if (start >= _rangeStart[i] && start <= _rangeEnd[i])
					{
						inRange = true;
						break;
					}
					if (end != 0)
					{
						if (end >= _rangeStart[i] && end <= _rangeEnd[i])
						{
							inRange = true;
							break;
						}
						if (start <= _rangeStart[i] && end >= _rangeEnd[i])
						{
							inRange = true;
							break;
						}
					}
				}
				if (inRange || (_trackEventsWithoutRange && (start == -1)))
				{
					if (_log)
					{
						_eventNr++;
						StringBuilder sb = new StringBuilder(":");
						sb.Append(FormatInt(_eventNr, 6));
						sb.Append(":");
						if (start != 0)
						{
							sb.Append(FormatInt(start));
							sb.Append(":");
						}
						if (end != 0 && start != end)
						{
							sb.Append(FormatInt(end));
							sb.Append(":");
							sb.Append(FormatInt(end - start + 1));
						}
						else
						{
							sb.Append(FormatInt(0));
						}
						sb.Append(":");
						if (info != null)
						{
							sb.Append(" " + info + " ");
							sb.Append(":");
						}
						sb.Append(" ");
						sb.Append(_tag);
						LogToOutput(sb.ToString());
					}
					if (_break)
					{
						if (_breakEventCount > 0)
						{
							for (int i = 0; i < _breakEventCount; i++)
							{
								if (_breakEventNrs[i] == _eventNr)
								{
									BreakPoint();
									break;
								}
							}
						}
						else
						{
							BreakPoint();
						}
					}
				}
			}
		}

		private static void LogToOutput(string msg)
		{
			if (enabled)
			{
				LogToFile(msg);
				LogToConsole(msg);
			}
		}

		private static void LogToConsole(string msg)
		{
			if (enabled)
			{
				if (writeToConsole)
				{
					Sharpen.Runtime.Out.WriteLine(msg);
				}
			}
		}

		private static void LogToFile(string msg)
		{
			if (enabled)
			{
				if (!writeToLogFile)
				{
					return;
				}
				lock (Lock)
				{
					if (_logFile == null)
					{
						try
						{
							_logFile = new RandomAccessFile(LogFile(), "rw");
							LogToFile("\r\n\r\n ********** BEGIN LOG ********** \r\n\r\n ");
						}
						catch (IOException e)
						{
							Sharpen.Runtime.PrintStackTrace(e);
						}
					}
					msg = DateHandlerBase.Now() + "\r\n" + msg + "\r\n";
					byte[] bytes = stringIO.Write(msg);
					try
					{
						_logFile.Write(bytes);
					}
					catch (IOException e)
					{
						Sharpen.Runtime.PrintStackTrace(e);
					}
				}
			}
		}

		private static string LogFile()
		{
			if (enabled)
			{
				if (logFileName != null)
				{
					return logFileName;
				}
				logFileName = "db4oDTrace_" + DateHandlerBase.Now() + "_" + SignatureGenerator.GenerateSignature
					() + ".log";
				logFileName = logFileName.Replace(' ', '_');
				logFileName = logFileName.Replace(':', '_');
				logFileName = logFileName.Replace('-', '_');
				return logFilePath + logFileName;
			}
			return null;
		}

		public static void AddRange(long pos)
		{
			if (enabled)
			{
				AddRangeWithEnd(pos, pos);
			}
		}

		public static void AddRangeWithLength(long start, long length)
		{
			if (enabled)
			{
				AddRangeWithEnd(start, start + length - 1);
			}
		}

		public static void AddRangeWithEnd(long start, long end)
		{
			if (enabled)
			{
				if (_rangeStart == null)
				{
					_rangeStart = new long[100];
					_rangeEnd = new long[100];
				}
				_rangeStart[_rangeCount] = start;
				_rangeEnd[_rangeCount] = end;
				_rangeCount++;
			}
		}

		private static void BreakOnEvent(long eventNr)
		{
			if (enabled)
			{
				if (_breakEventNrs == null)
				{
					_breakEventNrs = new long[100];
				}
				_breakEventNrs[_breakEventCount] = eventNr;
				_breakEventCount++;
			}
		}

		private string FormatInt(long i, int len)
		{
			if (enabled)
			{
				string str = "              ";
				if (i != 0)
				{
					str += i + " ";
				}
				return Sharpen.Runtime.Substring(str, str.Length - len);
			}
			return null;
		}

		private string FormatInt(long i)
		{
			if (enabled)
			{
				return FormatInt(i, 10);
			}
			return null;
		}

		private static void TurnAllOffExceptFor(Db4objects.Db4o.DTrace[] these)
		{
			if (enabled)
			{
				for (int i = 0; i < all.Length; i++)
				{
					if (all[i] == null)
					{
						break;
					}
					bool turnOff = true;
					for (int j = 0; j < these.Length; j++)
					{
						if (all[i] == these[j])
						{
							turnOff = false;
							break;
						}
					}
					if (turnOff)
					{
						all[i]._break = false;
						all[i]._enabled = false;
						all[i]._log = false;
					}
				}
			}
		}

		public static void NoWarnings()
		{
			BreakOnEvent(0);
			TrackEventsWithoutRange();
		}
	}
}
