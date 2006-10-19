namespace Db4objects.Db4o
{
	/// <summary>Messages for Client/Server Communication</summary>
	internal class Msg : Sharpen.Lang.ICloneable
	{
		internal static int _idGenerator = 1;

		private static Db4objects.Db4o.Msg[] _messages = new Db4objects.Db4o.Msg[60];

		internal int _msgID;

		internal string _name;

		internal Db4objects.Db4o.Transaction _trans;

		public static readonly Db4objects.Db4o.MsgD CLASS_NAME_FOR_ID = new Db4objects.Db4o.MClassNameForID
			();

		public static readonly Db4objects.Db4o.Msg CLOSE = new Db4objects.Db4o.Msg("CLOSE"
			);

		public static readonly Db4objects.Db4o.Msg COMMIT = new Db4objects.Db4o.MCommit();

		public static readonly Db4objects.Db4o.Msg COMMIT_OK = new Db4objects.Db4o.MCommitOK
			();

		public static readonly Db4objects.Db4o.MsgD CREATE_CLASS = new Db4objects.Db4o.MCreateClass
			();

		public static readonly Db4objects.Db4o.Msg CURRENT_VERSION = new Db4objects.Db4o.Msg
			("VERSION");

		public static readonly Db4objects.Db4o.MsgD DELETE = new Db4objects.Db4o.MDelete(
			);

		public static readonly Db4objects.Db4o.Msg ERROR = new Db4objects.Db4o.Msg("ERROR"
			);

		public static readonly Db4objects.Db4o.Msg FAILED = new Db4objects.Db4o.Msg("FAILED"
			);

		public static readonly Db4objects.Db4o.Msg GET_ALL = new Db4objects.Db4o.MGetAll(
			);

		public static readonly Db4objects.Db4o.MsgD GET_CLASSES = new Db4objects.Db4o.MGetClasses
			();

		public static readonly Db4objects.Db4o.MsgD GET_INTERNAL_IDS = new Db4objects.Db4o.MGetInternalIDs
			();

		public static readonly Db4objects.Db4o.Msg GET_THREAD_ID = new Db4objects.Db4o.Msg
			("GET_THREAD_ID");

		public static readonly Db4objects.Db4o.MsgD ID_LIST = new Db4objects.Db4o.MsgD("ID_LIST"
			);

		public static readonly Db4objects.Db4o.Msg IDENTITY = new Db4objects.Db4o.Msg("IDENTITY"
			);

		public static readonly Db4objects.Db4o.MsgD LENGTH = new Db4objects.Db4o.MsgD("LENGTH"
			);

		public static readonly Db4objects.Db4o.MsgD LOGIN = new Db4objects.Db4o.MsgD("LOGIN"
			);

		public static readonly Db4objects.Db4o.MsgD LOGIN_OK = new Db4objects.Db4o.MsgD("LOGIN_OK"
			);

		public static readonly Db4objects.Db4o.Msg NULL = new Db4objects.Db4o.Msg("NULL");

		public static readonly Db4objects.Db4o.MsgD OBJECT_BY_UUID = new Db4objects.Db4o.MObjectByUuid
			();

		public static readonly Db4objects.Db4o.MsgObject OBJECT_TO_CLIENT = new Db4objects.Db4o.MsgObject
			();

		public static readonly Db4objects.Db4o.Msg OK = new Db4objects.Db4o.Msg("OK");

		public static readonly Db4objects.Db4o.Msg PING = new Db4objects.Db4o.Msg("PING");

		public static readonly Db4objects.Db4o.MsgD PREFETCH_IDS = new Db4objects.Db4o.MPrefetchIDs
			();

		public static readonly Db4objects.Db4o.MsgObject QUERY_EXECUTE = new Db4objects.Db4o.MQueryExecute
			();

		public static readonly Db4objects.Db4o.MsgD RAISE_VERSION = new Db4objects.Db4o.MsgD
			("RAISE_VERSION");

		public static readonly Db4objects.Db4o.MsgBlob READ_BLOB = new Db4objects.Db4o.MReadBlob
			();

		public static readonly Db4objects.Db4o.MsgD READ_BYTES = new Db4objects.Db4o.MReadBytes
			();

		public static readonly Db4objects.Db4o.MsgD READ_MULTIPLE_OBJECTS = new Db4objects.Db4o.MReadMultipleObjects
			();

		public static readonly Db4objects.Db4o.MsgD READ_OBJECT = new Db4objects.Db4o.MReadObject
			();

		public static readonly Db4objects.Db4o.MsgD RELEASE_SEMAPHORE = new Db4objects.Db4o.MReleaseSemaphore
			();

		public static readonly Db4objects.Db4o.Msg ROLLBACK = new Db4objects.Db4o.MRollback
			();

		public static readonly Db4objects.Db4o.MsgD SET_SEMAPHORE = new Db4objects.Db4o.MSetSemaphore
			();

		public static readonly Db4objects.Db4o.Msg SUCCESS = new Db4objects.Db4o.Msg("SUCCESS"
			);

		public static readonly Db4objects.Db4o.MsgD SWITCH_TO_FILE = new Db4objects.Db4o.MsgD
			("SWITCH_F");

		public static readonly Db4objects.Db4o.Msg SWITCH_TO_MAIN_FILE = new Db4objects.Db4o.Msg
			("SWITCH_M");

		public static readonly Db4objects.Db4o.Msg TA_BEGIN_END_SET = new Db4objects.Db4o.MTaBeginEndSet
			();

		public static readonly Db4objects.Db4o.MsgD TA_DELETE = new Db4objects.Db4o.MTaDelete
			();

		public static readonly Db4objects.Db4o.MsgD TA_DONT_DELETE = new Db4objects.Db4o.MTaDontDelete
			();

		public static readonly Db4objects.Db4o.MsgD TA_IS_DELETED = new Db4objects.Db4o.MTaIsDeleted
			();

		public static readonly Db4objects.Db4o.MsgD USER_MESSAGE = new Db4objects.Db4o.MUserMessage
			();

		public static readonly Db4objects.Db4o.MsgD USE_TRANSACTION = new Db4objects.Db4o.MUseTransaction
			();

		public static readonly Db4objects.Db4o.MsgBlob WRITE_BLOB = new Db4objects.Db4o.MWriteBlob
			();

		public static readonly Db4objects.Db4o.MWriteNew WRITE_NEW = new Db4objects.Db4o.MWriteNew
			();

		public static readonly Db4objects.Db4o.MsgObject WRITE_UPDATE = new Db4objects.Db4o.MWriteUpdate
			();

		public static readonly Db4objects.Db4o.MsgD WRITE_UPDATE_DELETE_MEMBERS = new Db4objects.Db4o.MWriteUpdateDeleteMembers
			();

		internal Msg()
		{
			_msgID = _idGenerator++;
			_messages[_msgID] = this;
		}

		internal Msg(string aName) : this()
		{
			_name = aName;
		}

		internal Db4objects.Db4o.Msg Clone(Db4objects.Db4o.Transaction a_trans)
		{
			Db4objects.Db4o.Msg msg = null;
			try
			{
				msg = (Db4objects.Db4o.Msg)MemberwiseClone();
				msg._trans = a_trans;
			}
			catch (Sharpen.Lang.CloneNotSupportedException e)
			{
			}
			return msg;
		}

		public sealed override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			return _msgID == ((Db4objects.Db4o.Msg)obj)._msgID;
		}

		internal virtual void FakePayLoad(Db4objects.Db4o.Transaction a_trans)
		{
			_trans = a_trans;
		}

		/// <summary>
		/// dummy method to allow clean override handling
		/// without casting
		/// </summary>
		internal virtual Db4objects.Db4o.YapReader GetByteLoad()
		{
			return null;
		}

		internal string GetName()
		{
			if (_name == null)
			{
				return GetType().FullName;
			}
			return _name;
		}

		internal virtual Db4objects.Db4o.Transaction GetTransaction()
		{
			return _trans;
		}

		internal virtual Db4objects.Db4o.YapStream GetStream()
		{
			return GetTransaction().Stream();
		}

		/// <summary>server side execution</summary>
		internal virtual bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 socket)
		{
			return false;
		}

		internal static Db4objects.Db4o.Msg ReadMessage(Db4objects.Db4o.Transaction a_trans
			, Db4objects.Db4o.Foundation.Network.IYapSocket sock)
		{
			Db4objects.Db4o.YapWriter reader = new Db4objects.Db4o.YapWriter(a_trans, Db4objects.Db4o.YapConst
				.MESSAGE_LENGTH);
			if (!reader.Read(sock))
			{
				return null;
			}
			Db4objects.Db4o.Msg message = _messages[reader.ReadInt()].ReadPayLoad(a_trans, sock
				, reader);
			return message;
		}

		internal virtual Db4objects.Db4o.Msg ReadPayLoad(Db4objects.Db4o.Transaction a_trans
			, Db4objects.Db4o.Foundation.Network.IYapSocket sock, Db4objects.Db4o.YapReader 
			reader)
		{
			if (reader.ReadByte() == Db4objects.Db4o.YapConst.SYSTEM_TRANS && a_trans.i_parentTransaction
				 != null)
			{
				a_trans = a_trans.i_parentTransaction;
			}
			return Clone(a_trans);
		}

		internal void SetTransaction(Db4objects.Db4o.Transaction aTrans)
		{
			_trans = aTrans;
		}

		public sealed override string ToString()
		{
			return GetName();
		}

		internal void Write(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			lock (sock)
			{
				try
				{
					sock.Write(GetPayLoad()._buffer);
					sock.Flush();
				}
				catch (System.Exception e)
				{
				}
			}
		}

		internal virtual Db4objects.Db4o.YapWriter GetPayLoad()
		{
			Db4objects.Db4o.YapWriter writer = new Db4objects.Db4o.YapWriter(GetTransaction()
				, Db4objects.Db4o.YapConst.MESSAGE_LENGTH);
			writer.WriteInt(_msgID);
			return writer;
		}

		internal void WriteQueryResult(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.Inside.Query.IQueryResult
			 qr, Db4objects.Db4o.Foundation.Network.IYapSocket sock)
		{
			int size = qr.Size();
			Db4objects.Db4o.MsgD message = ID_LIST.GetWriterForLength(a_trans, Db4objects.Db4o.YapConst
				.ID_LENGTH * (size + 1));
			Db4objects.Db4o.YapWriter writer = message.GetPayLoad();
			writer.WriteQueryResult(qr);
			message.Write(a_trans.Stream(), sock);
		}
	}
}
