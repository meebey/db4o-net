/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <summary>Messages for Client/Server Communication</summary>
	public abstract class Msg : Sharpen.Lang.ICloneable
	{
		internal static int _messageIdGenerator = 1;

		private static Db4objects.Db4o.Internal.CS.Messages.Msg[] _messages = new Db4objects.Db4o.Internal.CS.Messages.Msg
			[70];

		internal int _msgID;

		internal string _name;

		private Db4objects.Db4o.Internal.Transaction _trans;

		private IMessageDispatcher _messageDispatcher;

		public static readonly MRuntimeException RUNTIME_EXCEPTION = new MRuntimeException
			();

		public static readonly MClassID CLASS_ID = new MClassID();

		public static readonly MClassMetadataIdForName CLASS_METADATA_ID_FOR_NAME = new MClassMetadataIdForName
			();

		public static readonly MClassNameForID CLASS_NAME_FOR_ID = new MClassNameForID();

		public static readonly MClose CLOSE = new MClose();

		public static readonly MCloseSocket CLOSE_SOCKET = new MCloseSocket();

		public static readonly MCommit COMMIT = new MCommit();

		public static readonly MCommittedCallBackRegistry COMMITTED_CALLBACK_REGISTER = new 
			MCommittedCallBackRegistry();

		public static readonly MCommittedInfo COMMITTED_INFO = new MCommittedInfo();

		public static readonly MCommitSystemTransaction COMMIT_SYSTEMTRANS = new MCommitSystemTransaction
			();

		public static readonly MCreateClass CREATE_CLASS = new MCreateClass();

		public static readonly MClassMeta CLASS_META = new MClassMeta();

		public static readonly MVersion CURRENT_VERSION = new MVersion();

		public static readonly MDelete DELETE = new MDelete();

		public static readonly MError ERROR = new MError();

		public static readonly MFailed FAILED = new MFailed();

		public static readonly MGetAll GET_ALL = new MGetAll();

		public static readonly MGetClasses GET_CLASSES = new MGetClasses();

		public static readonly MGetInternalIDs GET_INTERNAL_IDS = new MGetInternalIDs();

		public static readonly MGetThreadID GET_THREAD_ID = new MGetThreadID();

		public static readonly MIDList ID_LIST = new MIDList();

		public static readonly MIdentity IDENTITY = new MIdentity();

		public static readonly MIsAlive IS_ALIVE = new MIsAlive();

		public static readonly MLength LENGTH = new MLength();

		public static readonly MLogin LOGIN = new MLogin();

		public static readonly MLoginOK LOGIN_OK = new MLoginOK();

		public static readonly MNull NULL = new MNull();

		public static readonly MObjectByUuid OBJECT_BY_UUID = new MObjectByUuid();

		public static readonly MsgObject OBJECT_TO_CLIENT = new MsgObject();

		public static readonly MObjectSetFetch OBJECTSET_FETCH = new MObjectSetFetch();

		public static readonly MObjectSetFinalized OBJECTSET_FINALIZED = new MObjectSetFinalized
			();

		public static readonly MObjectSetGetId OBJECTSET_GET_ID = new MObjectSetGetId();

		public static readonly MObjectSetIndexOf OBJECTSET_INDEXOF = new MObjectSetIndexOf
			();

		public static readonly MObjectSetReset OBJECTSET_RESET = new MObjectSetReset();

		public static readonly MObjectSetSize OBJECTSET_SIZE = new MObjectSetSize();

		public static readonly MOK OK = new MOK();

		public static readonly MPing PING = new MPing();

		public static readonly MPong PONG = new MPong();

		public static readonly MPrefetchIDs PREFETCH_IDS = new MPrefetchIDs();

		public static readonly MProcessDeletes PROCESS_DELETES = new MProcessDeletes();

		public static readonly MQueryExecute QUERY_EXECUTE = new MQueryExecute();

		public static readonly MQueryResult QUERY_RESULT = new MQueryResult();

		public static readonly MRaiseVersion RAISE_VERSION = new MRaiseVersion();

		public static readonly MReadBlob READ_BLOB = new MReadBlob();

		public static readonly MReadBytes READ_BYTES = new MReadBytes();

		public static readonly MReadMultipleObjects READ_MULTIPLE_OBJECTS = new MReadMultipleObjects
			();

		public static readonly MReadObject READ_OBJECT = new MReadObject();

		public static readonly MReleaseSemaphore RELEASE_SEMAPHORE = new MReleaseSemaphore
			();

		public static readonly MRollback ROLLBACK = new MRollback();

		public static readonly MSetSemaphore SET_SEMAPHORE = new MSetSemaphore();

		public static readonly MSuccess SUCCESS = new MSuccess();

		public static readonly MSwitchToFile SWITCH_TO_FILE = new MSwitchToFile();

		public static readonly MSwitchToMainFile SWITCH_TO_MAIN_FILE = new MSwitchToMainFile
			();

		public static readonly MTaDelete TA_DELETE = new MTaDelete();

		public static readonly MTaIsDeleted TA_IS_DELETED = new MTaIsDeleted();

		public static readonly MUserMessage USER_MESSAGE = new MUserMessage();

		public static readonly MUseTransaction USE_TRANSACTION = new MUseTransaction();

		public static readonly MWriteBlob WRITE_BLOB = new MWriteBlob();

		public static readonly MWriteNew WRITE_NEW = new MWriteNew();

		public static readonly MWriteUpdate WRITE_UPDATE = new MWriteUpdate();

		public static readonly MWriteUpdateDeleteMembers WRITE_UPDATE_DELETE_MEMBERS = new 
			MWriteUpdateDeleteMembers();

		public static readonly MWriteBatchedMessages WRITE_BATCHED_MESSAGES = new MWriteBatchedMessages
			();

		public static readonly MsgBlob DELETE_BLOB_FILE = new MDeleteBlobFile();

		internal Msg()
		{
			_msgID = _messageIdGenerator++;
			_messages[_msgID] = this;
		}

		internal Msg(string aName) : this()
		{
			_name = aName;
		}

		public static Db4objects.Db4o.Internal.CS.Messages.Msg GetMessage(int id)
		{
			return _messages[id];
		}

		public Db4objects.Db4o.Internal.CS.Messages.Msg PublicClone()
		{
			try
			{
				return (Db4objects.Db4o.Internal.CS.Messages.Msg)MemberwiseClone();
			}
			catch (CloneNotSupportedException)
			{
				Exceptions4.ShouldNeverHappen();
				return null;
			}
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
			return _msgID == ((Db4objects.Db4o.Internal.CS.Messages.Msg)obj)._msgID;
		}

		public override int GetHashCode()
		{
			return _msgID;
		}

		/// <summary>
		/// dummy method to allow clean override handling
		/// without casting
		/// </summary>
		public virtual BufferImpl GetByteLoad()
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

		protected virtual LocalTransaction ServerTransaction()
		{
			return (LocalTransaction)_trans;
		}

		protected virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _trans;
		}

		protected virtual LocalObjectContainer File()
		{
			return (LocalObjectContainer)Stream();
		}

		protected virtual ObjectContainerBase Stream()
		{
			return Transaction().Container();
		}

		protected virtual object StreamLock()
		{
			return Stream().Lock();
		}

		protected virtual Config4Impl Config()
		{
			return Stream().Config();
		}

		/// <exception cref="Db4oIOException"></exception>
		protected static StatefulBuffer ReadMessageBuffer(Db4objects.Db4o.Internal.Transaction
			 trans, ISocket4 sock)
		{
			return ReadMessageBuffer(trans, sock, Const4.MESSAGE_LENGTH);
		}

		/// <exception cref="Db4oIOException"></exception>
		protected static StatefulBuffer ReadMessageBuffer(Db4objects.Db4o.Internal.Transaction
			 trans, ISocket4 sock, int length)
		{
			StatefulBuffer buffer = new StatefulBuffer(trans, length);
			int offset = 0;
			while (length > 0)
			{
				int read = sock.Read(buffer._buffer, offset, length);
				if (read < 0)
				{
					throw new Db4oIOException();
				}
				offset += read;
				length -= read;
			}
			return buffer;
		}

		/// <exception cref="Db4oIOException"></exception>
		public static Db4objects.Db4o.Internal.CS.Messages.Msg ReadMessage(IMessageDispatcher
			 messageDispatcher, Db4objects.Db4o.Internal.Transaction trans, ISocket4 sock)
		{
			StatefulBuffer reader = ReadMessageBuffer(trans, sock);
			Db4objects.Db4o.Internal.CS.Messages.Msg message = _messages[reader.ReadInt()].ReadPayLoad
				(messageDispatcher, trans, sock, reader);
			return message;
		}

		/// <param name="sock"></param>
		internal virtual Db4objects.Db4o.Internal.CS.Messages.Msg ReadPayLoad(IMessageDispatcher
			 messageDispatcher, Db4objects.Db4o.Internal.Transaction a_trans, ISocket4 sock, 
			BufferImpl reader)
		{
			Db4objects.Db4o.Internal.CS.Messages.Msg msg = PublicClone();
			msg.SetMessageDispatcher(messageDispatcher);
			msg.SetTransaction(CheckParentTransaction(a_trans, reader));
			return msg;
		}

		protected Db4objects.Db4o.Internal.Transaction CheckParentTransaction(Db4objects.Db4o.Internal.Transaction
			 a_trans, BufferImpl reader)
		{
			if (reader.ReadByte() == Const4.SYSTEM_TRANS && a_trans.ParentTransaction() != null
				)
			{
				return a_trans.ParentTransaction();
			}
			return a_trans;
		}

		public void SetTransaction(Db4objects.Db4o.Internal.Transaction aTrans)
		{
			_trans = aTrans;
		}

		public sealed override string ToString()
		{
			return GetName();
		}

		public virtual void Write(Db4objects.Db4o.Internal.CS.Messages.Msg msg)
		{
			_messageDispatcher.Write(msg);
		}

		public virtual void WriteException(Exception e)
		{
			Write(RUNTIME_EXCEPTION.GetWriterForSingleObject(Transaction(), e));
		}

		public virtual void RespondInt(int response)
		{
			Write(ID_LIST.GetWriterForInt(Transaction(), response));
		}

		public virtual bool Write(ISocket4 sock)
		{
			if (null == sock)
			{
				throw new ArgumentNullException();
			}
			lock (sock)
			{
				try
				{
					sock.Write(PayLoad()._buffer);
					sock.Flush();
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public virtual StatefulBuffer PayLoad()
		{
			StatefulBuffer writer = new StatefulBuffer(Transaction(), Const4.MESSAGE_LENGTH);
			writer.WriteInt(_msgID);
			return writer;
		}

		public virtual IMessageDispatcher MessageDispatcher()
		{
			return _messageDispatcher;
		}

		public virtual IServerMessageDispatcher ServerMessageDispatcher()
		{
			if (_messageDispatcher is IServerMessageDispatcher)
			{
				return (IServerMessageDispatcher)_messageDispatcher;
			}
			throw new InvalidOperationException();
		}

		public virtual IClientMessageDispatcher ClientMessageDispatcher()
		{
			if (_messageDispatcher is IClientMessageDispatcher)
			{
				return (IClientMessageDispatcher)_messageDispatcher;
			}
			throw new InvalidOperationException();
		}

		public virtual void SetMessageDispatcher(IMessageDispatcher messageDispatcher)
		{
			_messageDispatcher = messageDispatcher;
		}

		public virtual void LogMsg(int msgCode, string msg)
		{
			Stream().LogMsg(msgCode, msg);
		}
	}
}
