/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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

		private static int Unused = -1;

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
				// breakOnEvent(5);
				// addRange(4874);
				// addRangeWithEnd(3835808, 3836267);
				//        	 addRangeWithLength(6539,1);
				//        	addRangeWithLength(17673,1);
				//            addRangeWithLength(455404,1);
				//            
				//            addRangeWithLength(455570,1);
				//            
				//            addRangeWithLength(455926,1);
				//
				AddRangeWithLength(1647603457, 1);
				// trackEventsWithoutRange();
				TurnAllOffExceptFor(new Db4objects.Db4o.DTrace[] { ObjectReferenceCreated });
			}
		}

		//            turnAllOffExceptFor(new DTrace[] {
		//                PERSISTENT_OWN_LENGTH,
		//                });
		//            turnAllOffExceptFor(new DTrace[] {
		//                FREESPACEMANAGER_GET_SLOT,
		//                FREESPACEMANAGER_RAM_FREE,
		//                FREESPACEMANAGER_BTREE_FREE,
		//                });
		//          turnAllOffExceptFor(new DTrace[] {BTREE_NODE_COMMIT_OR_ROLLBACK });
		//            turnAllOffExceptFor(new DTrace[] {BTREE_NODE_REMOVE, BTREE_NODE_COMMIT_OR_ROLLBACK YAPMETA_SET_ID});
		private static void Init()
		{
			if (enabled)
			{
				AddToClassIndex = new Db4objects.Db4o.DTrace(true, true, "add to class index tree"
					, true);
				BeginTopLevelCall = new Db4objects.Db4o.DTrace(true, true, "begin top level call"
					, true);
				Bind = new Db4objects.Db4o.DTrace(true, true, "bind", true);
				BlockingQueueStoppedException = new Db4objects.Db4o.DTrace(true, true, "blocking queue stopped exception"
					, true);
				BtreeNodeRemove = new Db4objects.Db4o.DTrace(true, true, "btreenode remove", true
					);
				BtreeNodeCommitOrRollback = new Db4objects.Db4o.DTrace(true, true, "btreenode commit or rollback"
					, true);
				CandidateRead = new Db4objects.Db4o.DTrace(true, true, "candidate read", true);
				ClassmetadataById = new Db4objects.Db4o.DTrace(true, true, "classmetadata by id", 
					true);
				ClassmetadataInit = new Db4objects.Db4o.DTrace(true, true, "classmetadata init", 
					true);
				ClientMessageLoopException = new Db4objects.Db4o.DTrace(true, true, "client message loop exception"
					, true);
				Close = new Db4objects.Db4o.DTrace(true, true, "close", true);
				CloseCalled = new Db4objects.Db4o.DTrace(true, true, "close called", true);
				CollectChildren = new Db4objects.Db4o.DTrace(true, true, "collect children", true
					);
				Commit = new Db4objects.Db4o.DTrace(false, false, "commit", true);
				Continueset = new Db4objects.Db4o.DTrace(true, true, "continueset", true);
				CreateCandidate = new Db4objects.Db4o.DTrace(true, true, "create candidate", true
					);
				Delete = new Db4objects.Db4o.DTrace(true, true, "delete", true);
				Donotinclude = new Db4objects.Db4o.DTrace(true, true, "donotinclude", true);
				EndTopLevelCall = new Db4objects.Db4o.DTrace(true, true, "end top level call", true
					);
				EvaluateSelf = new Db4objects.Db4o.DTrace(true, true, "evaluate self", true);
				FatalException = new Db4objects.Db4o.DTrace(true, true, "fatal exception", true);
				Free = new Db4objects.Db4o.DTrace(true, true, "free", true);
				FileFree = new Db4objects.Db4o.DTrace(true, true, "fileFree", true);
				FreespacemanagerGetSlot = new Db4objects.Db4o.DTrace(true, true, "FreespaceManager getSlot"
					, true);
				FreespacemanagerRamFree = new Db4objects.Db4o.DTrace(true, true, "RamFreeSpaceManager free"
					, true);
				FreespacemanagerBtreeFree = new Db4objects.Db4o.DTrace(true, true, "BTreeFreeSpaceManager free"
					, true);
				FreeOnCommit = new Db4objects.Db4o.DTrace(true, true, "trans freeOnCommit", true);
				FreeOnRollback = new Db4objects.Db4o.DTrace(true, true, "trans freeOnRollback", true
					);
				FreePointerOnRollback = new Db4objects.Db4o.DTrace(true, true, "freePointerOnRollback"
					, true);
				GetPointerSlot = new Db4objects.Db4o.DTrace(true, true, "getPointerSlot", true);
				GetSlot = new Db4objects.Db4o.DTrace(true, true, "getSlot", true);
				GetFreespaceRam = new Db4objects.Db4o.DTrace(true, true, "getFreespaceRam", true);
				GetYapobject = new Db4objects.Db4o.DTrace(true, true, "get yapObject", true);
				IdTreeAdd = new Db4objects.Db4o.DTrace(true, true, "id tree add", true);
				IdTreeRemove = new Db4objects.Db4o.DTrace(true, true, "id tree remove", true);
				IoCopy = new Db4objects.Db4o.DTrace(true, true, "io copy", true);
				JustSet = new Db4objects.Db4o.DTrace(true, true, "just set", true);
				NewInstance = new Db4objects.Db4o.DTrace(true, true, "newInstance", true);
				ObjectReferenceCreated = new Db4objects.Db4o.DTrace(true, true, "new ObjectReference"
					, true);
				PersistentOwnLength = new Db4objects.Db4o.DTrace(true, true, "Persistent own length"
					, true);
				PersistentbaseWrite = new Db4objects.Db4o.DTrace(true, true, "persistentbase write"
					, true);
				PersistentbaseSetId = new Db4objects.Db4o.DTrace(true, true, "persistentbase setid"
					, true);
				ProduceSlotChange = new Db4objects.Db4o.DTrace(true, true, "produce slot change", 
					true);
				QueryProcess = new Db4objects.Db4o.DTrace(true, true, "query process", true);
				ReadArrayWrapper = new Db4objects.Db4o.DTrace(true, true, "read array wrapper", true
					);
				ReadBytes = new Db4objects.Db4o.DTrace(true, true, "readBytes", true);
				ReadId = new Db4objects.Db4o.DTrace(true, true, "read ID", true);
				ReadSlot = new Db4objects.Db4o.DTrace(true, true, "read slot", true);
				ReferenceRemoved = new Db4objects.Db4o.DTrace(true, true, "reference removed", true
					);
				RegularSeek = new Db4objects.Db4o.DTrace(true, true, "regular seek", true);
				RemoveFromClassIndex = new Db4objects.Db4o.DTrace(true, true, "trans removeFromClassIndexTree"
					, true);
				RereadOldUuid = new Db4objects.Db4o.DTrace(true, true, "reread old uuid", true);
				ServerMessageLoopException = new Db4objects.Db4o.DTrace(true, true, "server message loop exception"
					, true);
				SlotSetPointer = new Db4objects.Db4o.DTrace(true, true, "slot set pointer", true);
				SlotDelete = new Db4objects.Db4o.DTrace(true, true, "slot delete", true);
				SlotFreeOnCommit = new Db4objects.Db4o.DTrace(true, true, "slot free on commit", 
					true);
				SlotFreeOnRollbackId = new Db4objects.Db4o.DTrace(true, true, "slot free on rollback id"
					, true);
				SlotFreeOnRollbackAddress = new Db4objects.Db4o.DTrace(true, true, "slot free on rollback address"
					, true);
				TransCommit = new Db4objects.Db4o.DTrace(false, false, "trans commit", false);
				TransDelete = new Db4objects.Db4o.DTrace(true, true, "trans delete", true);
				TransDontDelete = new Db4objects.Db4o.DTrace(true, true, "trans dontDelete", true
					);
				TransFlush = new Db4objects.Db4o.DTrace(true, true, "trans flush", true);
				WriteBytes = new Db4objects.Db4o.DTrace(true, true, "writeBytes", true);
				WritePointer = new Db4objects.Db4o.DTrace(true, true, "write pointer", true);
				WriteUpdateDeleteMembers = new Db4objects.Db4o.DTrace(true, true, "trans writeUpdateDeleteMembers"
					, true);
				WriteXbytes = new Db4objects.Db4o.DTrace(true, true, "writeXBytes", true);
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

		public static Db4objects.Db4o.DTrace AddToClassIndex;

		public static Db4objects.Db4o.DTrace BeginTopLevelCall;

		public static Db4objects.Db4o.DTrace Bind;

		public static Db4objects.Db4o.DTrace BlockingQueueStoppedException;

		public static Db4objects.Db4o.DTrace BtreeNodeCommitOrRollback;

		public static Db4objects.Db4o.DTrace BtreeNodeRemove;

		public static Db4objects.Db4o.DTrace CandidateRead;

		public static Db4objects.Db4o.DTrace ClassmetadataById;

		public static Db4objects.Db4o.DTrace ClassmetadataInit;

		public static Db4objects.Db4o.DTrace ClientMessageLoopException;

		public static Db4objects.Db4o.DTrace Close;

		public static Db4objects.Db4o.DTrace CloseCalled;

		public static Db4objects.Db4o.DTrace CollectChildren;

		public static Db4objects.Db4o.DTrace Commit;

		public static Db4objects.Db4o.DTrace Continueset;

		public static Db4objects.Db4o.DTrace CreateCandidate;

		public static Db4objects.Db4o.DTrace Delete;

		public static Db4objects.Db4o.DTrace Donotinclude;

		public static Db4objects.Db4o.DTrace EndTopLevelCall;

		public static Db4objects.Db4o.DTrace EvaluateSelf;

		public static Db4objects.Db4o.DTrace FatalException;

		public static Db4objects.Db4o.DTrace FileFree;

		public static Db4objects.Db4o.DTrace Free;

		public static Db4objects.Db4o.DTrace FreespacemanagerGetSlot;

		public static Db4objects.Db4o.DTrace FreespacemanagerRamFree;

		public static Db4objects.Db4o.DTrace FreespacemanagerBtreeFree;

		public static Db4objects.Db4o.DTrace FreeOnCommit;

		public static Db4objects.Db4o.DTrace FreeOnRollback;

		public static Db4objects.Db4o.DTrace FreePointerOnRollback;

		public static Db4objects.Db4o.DTrace GetSlot;

		public static Db4objects.Db4o.DTrace GetPointerSlot;

		public static Db4objects.Db4o.DTrace GetFreespaceRam;

		public static Db4objects.Db4o.DTrace GetYapobject;

		public static Db4objects.Db4o.DTrace IdTreeAdd;

		public static Db4objects.Db4o.DTrace IdTreeRemove;

		public static Db4objects.Db4o.DTrace IoCopy;

		public static Db4objects.Db4o.DTrace JustSet;

		public static Db4objects.Db4o.DTrace NewInstance;

		public static Db4objects.Db4o.DTrace ObjectReferenceCreated;

		public static Db4objects.Db4o.DTrace PersistentOwnLength;

		public static Db4objects.Db4o.DTrace PersistentbaseSetId;

		public static Db4objects.Db4o.DTrace PersistentbaseWrite;

		public static Db4objects.Db4o.DTrace ProduceSlotChange;

		public static Db4objects.Db4o.DTrace QueryProcess;

		public static Db4objects.Db4o.DTrace ReadArrayWrapper;

		public static Db4objects.Db4o.DTrace ReadBytes;

		public static Db4objects.Db4o.DTrace ReadId;

		public static Db4objects.Db4o.DTrace ReadSlot;

		public static Db4objects.Db4o.DTrace ReferenceRemoved;

		public static Db4objects.Db4o.DTrace RegularSeek;

		public static Db4objects.Db4o.DTrace RemoveFromClassIndex;

		public static Db4objects.Db4o.DTrace RereadOldUuid;

		public static Db4objects.Db4o.DTrace ServerMessageLoopException;

		public static Db4objects.Db4o.DTrace SlotSetPointer;

		public static Db4objects.Db4o.DTrace SlotDelete;

		public static Db4objects.Db4o.DTrace SlotFreeOnCommit;

		public static Db4objects.Db4o.DTrace SlotFreeOnRollbackId;

		public static Db4objects.Db4o.DTrace SlotFreeOnRollbackAddress;

		public static Db4objects.Db4o.DTrace TransCommit;

		public static Db4objects.Db4o.DTrace TransDontDelete;

		public static Db4objects.Db4o.DTrace TransDelete;

		public static Db4objects.Db4o.DTrace TransFlush;

		public static Db4objects.Db4o.DTrace WriteBytes;

		public static Db4objects.Db4o.DTrace WritePointer;

		public static Db4objects.Db4o.DTrace WriteXbytes;

		public static Db4objects.Db4o.DTrace WriteUpdateDeleteMembers;

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
				Log(Unused);
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
				Log(Unused, msg);
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
				LogEnd(Unused, 0, info);
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
					// Case 1 start in range
					if (start >= _rangeStart[i] && start <= _rangeEnd[i])
					{
						inRange = true;
						break;
					}
					if (end != 0)
					{
						// Case 2 end in range
						if (end >= _rangeStart[i] && end <= _rangeEnd[i])
						{
							inRange = true;
							break;
						}
						// Case 3 start before range, end after range
						if (start <= _rangeStart[i] && end >= _rangeEnd[i])
						{
							inRange = true;
							break;
						}
					}
				}
				if (inRange || (_trackEventsWithoutRange && (start == Unused)))
				{
					if (_log)
					{
						_eventNr++;
						StringBuilder sb = new StringBuilder(":");
						sb.Append(FormatInt(_eventNr, 6));
						sb.Append(":");
						sb.Append(FormatInt(start));
						sb.Append(":");
						if (end != 0 && start != end)
						{
							sb.Append(FormatInt(end));
							sb.Append(":");
							sb.Append(FormatInt(end - start + 1));
						}
						else
						{
							sb.Append(FormatUnused());
							sb.Append(":");
							sb.Append(FormatUnused());
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

		private string FormatUnused()
		{
			return FormatInt(Unused);
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
				if (i != Unused)
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
