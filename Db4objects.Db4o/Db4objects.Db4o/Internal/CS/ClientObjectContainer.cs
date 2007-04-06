using System;
using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Reflect;
using Sharpen;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class ClientObjectContainer : ObjectContainerBase, IExtClient, IBlobTransport
		, IClientMessageDispatcher
	{
		internal readonly object blobLock = new object();

		private BlobProcessor blobThread;

		private ISocket4 i_socket;

		private BlockingQueue _messageQueue = new BlockingQueue();

		private string password;

		internal int[] _prefetchedIDs;

		internal IClientMessageDispatcher _messageDispatcher;

		internal int remainingIDs;

		private string switchedToFile;

		private bool _singleThreaded;

		private string userName;

		private Db4oDatabase i_db;

		protected bool _doFinalize = true;

		private int _blockSize = 1;

		private Collection4 _batchedMessages = new Collection4();

		private int _batchedQueueLength = Const4.INT_LENGTH;

		private string _fakeServerFile;

		private bool _login;

		public ClientObjectContainer(string fakeServerFile) : base(Db4oFactory.CloneConfiguration
			(), null)
		{
			throw new InvalidOperationException();
			_fakeServerFile = fakeServerFile;
			Open();
		}

		public ClientObjectContainer(IConfiguration config, ISocket4 socket, string user, 
			string password_, bool login) : base(config, null)
		{
			if (password_ == null)
			{
				throw new InvalidPasswordException();
			}
			userName = user;
			password = password_;
			i_socket = socket;
			_login = login;
			Open();
		}

		protected sealed override void OpenImpl()
		{
			_singleThreaded = ConfigImpl().SingleThreadedClient();
			if (_login)
			{
				LoginToServer(i_socket);
			}
			if (!_singleThreaded)
			{
				StartDispatcherThread(i_socket, userName);
			}
			LogMsg(36, ToString());
			ReadThis();
		}

		private void StartDispatcherThread(ISocket4 socket, string user)
		{
			_messageDispatcher = new ClientMessageDispatcherImpl(this, socket, _messageQueue);
			_messageDispatcher.SetDispatcherName(user);
			_messageDispatcher.StartDispatcher();
		}

		public override void Backup(string path)
		{
			throw new NotSupportedException();
		}

		public virtual void BlockSize(int blockSize)
		{
			_blockSize = blockSize;
		}

		public override byte BlockSize()
		{
			return (byte)_blockSize;
		}

		protected override void Close2()
		{
			if (_messageDispatcher == null || !_messageDispatcher.IsMessageDispatcherAlive())
			{
				ShutdownObjectContainer();
				return;
			}
			try
			{
				Commit1();
			}
			catch (Exception e)
			{
				Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				WriteMsg(Msg.CLOSE, true);
			}
			catch (Exception e)
			{
				Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				if (!_singleThreaded)
				{
					_messageDispatcher.Close();
				}
			}
			catch (Exception e)
			{
				Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				i_socket.Close();
			}
			catch (Exception e)
			{
				Exceptions4.CatchAllExceptDb4oException(e);
			}
			ShutdownObjectContainer();
		}

		public sealed override void Commit1()
		{
			i_trans.Commit();
		}

		public override int ConverterVersion()
		{
			return Converter.VERSION;
		}

		internal virtual ISocket4 CreateParalellSocket()
		{
			WriteMsg(Msg.GET_THREAD_ID, true);
			int serverThreadID = ExpectedByteResponse(Msg.ID_LIST).ReadInt();
			ISocket4 sock = i_socket.OpenParalellSocket();
			if (!(i_socket is LoopbackSocket))
			{
				LoginToServer(sock);
			}
			if (switchedToFile != null)
			{
				MsgD message = Msg.SWITCH_TO_FILE.GetWriterForString(SystemTransaction(), switchedToFile
					);
				message.Write(this, sock);
				if (!(Msg.OK.Equals(Msg.ReadMessage(this, SystemTransaction(), sock))))
				{
					throw new IOException(Db4objects.Db4o.Internal.Messages.Get(42));
				}
			}
			Msg.USE_TRANSACTION.GetWriterForInt(i_trans, serverThreadID).Write(this, sock);
			return sock;
		}

		public override AbstractQueryResult NewQueryResult(Transaction trans, QueryEvaluationMode
			 mode)
		{
			throw new InvalidOperationException();
		}

		public sealed override Transaction NewTransaction(Transaction parentTransaction)
		{
			return new ClientTransaction(this, parentTransaction);
		}

		public override bool CreateClassMetadata(ClassMetadata a_yapClass, IReflectClass 
			a_class, ClassMetadata a_superYapClass)
		{
			WriteMsg(Msg.CREATE_CLASS.GetWriterForString(SystemTransaction(), a_class.GetName
				()), true);
			Msg resp = GetResponse();
			if (resp == null)
			{
				return false;
			}
			if (resp.Equals(Msg.FAILED))
			{
				SendClassMeta(a_class);
				resp = GetResponse();
			}
			if (resp.Equals(Msg.FAILED))
			{
				if (ConfigImpl().ExceptionsOnNotStorable())
				{
					throw new ObjectNotStorableException(a_class);
				}
				return false;
			}
			if (!resp.Equals(Msg.OBJECT_TO_CLIENT))
			{
				return false;
			}
			MsgObject message = (MsgObject)resp;
			StatefulBuffer bytes = message.Unmarshall();
			if (bytes == null)
			{
				return false;
			}
			bytes.SetTransaction(SystemTransaction());
			if (!base.CreateClassMetadata(a_yapClass, a_class, a_superYapClass))
			{
				return false;
			}
			a_yapClass.SetID(message.GetId());
			a_yapClass.ReadName1(SystemTransaction(), bytes);
			ClassCollection().AddYapClass(a_yapClass);
			ClassCollection().ReadYapClass(a_yapClass, a_class);
			return true;
		}

		private void SendClassMeta(IReflectClass reflectClass)
		{
			ClassInfo classMeta = _classMetaHelper.GetClassMeta(reflectClass);
			WriteMsg(Msg.CLASS_META.GetWriter(Serializer.Marshall(SystemTransaction(), classMeta
				)), true);
		}

		public override long CurrentVersion()
		{
			WriteMsg(Msg.CURRENT_VERSION, true);
			return ((MsgD)ExpectedResponse(Msg.ID_LIST)).ReadLong();
		}

		public sealed override bool Delete4(Transaction ta, ObjectReference yo, int a_cascade
			, bool userCall)
		{
			MsgD msg = Msg.DELETE.GetWriterForInts(i_trans, new int[] { yo.GetID(), userCall ? 
				1 : 0 });
			WriteMsg(msg, false);
			return true;
		}

		public override bool DetectSchemaChanges()
		{
			return false;
		}

		protected override bool DoFinalize()
		{
			return _doFinalize;
		}

		internal Db4objects.Db4o.Internal.Buffer ExpectedByteResponse(Msg expectedMessage
			)
		{
			Msg msg = ExpectedResponse(expectedMessage);
			if (msg == null)
			{
				return null;
			}
			return msg.GetByteLoad();
		}

		public Msg ExpectedResponse(Msg expectedMessage)
		{
			Msg message = GetResponse();
			if (expectedMessage.Equals(message))
			{
				return message;
			}
			CheckDb4oExceptionMessage(message);
			throw new InvalidOperationException("Unexpected Message:" + message + "  Expected:"
				 + expectedMessage);
		}

		private void CheckDb4oExceptionMessage(Msg msg)
		{
			if (msg is MDb4oException)
			{
				throw ((MDb4oException)msg).Exception();
			}
		}

		public override AbstractQueryResult GetAll(Transaction trans)
		{
			int mode = Config().QueryEvaluationMode().AsInt();
			MsgD msg = Msg.GET_ALL.GetWriterForInt(trans, mode);
			WriteMsg(msg, true);
			return ReadQueryResult(trans);
		}

		/// <summary>may return null, if no message is returned.</summary>
		/// <remarks>
		/// may return null, if no message is returned. Error handling is weak and
		/// should ideally be able to trigger some sort of state listener (connection
		/// dead) on the client.
		/// </remarks>
		public virtual Msg GetResponse()
		{
			return _singleThreaded ? GetResponseSingleThreaded() : GetResponseMultiThreaded();
		}

		private Msg GetResponseMultiThreaded()
		{
			Msg msg = (Msg)_messageQueue.Next();
			if (msg == Msg.ERROR)
			{
				OnMsgError();
			}
			return msg;
		}

		private void OnMsgError()
		{
			Close();
			throw new Db4oException(Db4objects.Db4o.Internal.Messages.Get(Db4objects.Db4o.Internal.Messages
				.CLOSED_OR_OPEN_FAILED));
		}

		private Msg GetResponseSingleThreaded()
		{
			while (IsMessageDispatcherAlive())
			{
				try
				{
					Msg message = Msg.ReadMessage(this, i_trans, i_socket);
					if (message is IClientSideMessage)
					{
						if (((IClientSideMessage)message).ProcessAtClient())
						{
							continue;
						}
					}
					return message;
				}
				catch (Exception)
				{
				}
			}
			return null;
		}

		public virtual bool IsMessageDispatcherAlive()
		{
			return i_socket != null;
		}

		public override ClassMetadata ClassMetadataForId(int a_id)
		{
			if (a_id == 0)
			{
				return null;
			}
			ClassMetadata yc = base.ClassMetadataForId(a_id);
			if (yc != null)
			{
				return yc;
			}
			MsgD msg = Msg.CLASS_NAME_FOR_ID.GetWriterForInt(SystemTransaction(), a_id);
			WriteMsg(msg, true);
			MsgD message = (MsgD)ExpectedResponse(Msg.CLASS_NAME_FOR_ID);
			string className = message.ReadString();
			if (className != null && className.Length > 0)
			{
				IReflectClass claxx = Reflector().ForName(className);
				if (claxx != null)
				{
					return ProduceClassMetadata(claxx);
				}
			}
			return null;
		}

		public override bool NeedsLockFileThread()
		{
			return false;
		}

		protected override bool HasShutDownHook()
		{
			return false;
		}

		public override Db4oDatabase Identity()
		{
			if (i_db == null)
			{
				WriteMsg(Msg.IDENTITY, true);
				Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Msg.ID_LIST);
				ShowInternalClasses(true);
				try
				{
					i_db = (Db4oDatabase)GetByID(reader.ReadInt());
					Activate1(SystemTransaction(), i_db, 3);
				}
				finally
				{
					ShowInternalClasses(false);
				}
			}
			return i_db;
		}

		public override bool IsClient()
		{
			return true;
		}

		private void LoginToServer(ISocket4 a_socket)
		{
			UnicodeStringIO stringWriter = new UnicodeStringIO();
			int length = stringWriter.Length(userName) + stringWriter.Length(password);
			MsgD message = Msg.LOGIN.GetWriterForLength(SystemTransaction(), length);
			message.WriteString(userName);
			message.WriteString(password);
			message.Write(this, a_socket);
			try
			{
				Msg msg = Msg.ReadMessage(this, SystemTransaction(), a_socket);
				if (!Msg.LOGIN_OK.Equals(msg))
				{
					throw new InvalidPasswordException();
				}
				Db4objects.Db4o.Internal.Buffer payLoad = msg.PayLoad();
				_blockSize = payLoad.ReadInt();
				int doEncrypt = payLoad.ReadInt();
				if (doEncrypt == 0)
				{
					i_handlers.OldEncryptionOff();
				}
			}
			catch (IOException e)
			{
				throw new OpenDatabaseException(e);
			}
		}

		public override bool MaintainsIndices()
		{
			return false;
		}

		public sealed override int NewUserObject()
		{
			int prefetchIDCount = Config().PrefetchIDCount();
			EnsureIDCacheAllocated(prefetchIDCount);
			Db4objects.Db4o.Internal.Buffer reader = null;
			if (remainingIDs < 1)
			{
				MsgD msg = Msg.PREFETCH_IDS.GetWriterForInt(i_trans, prefetchIDCount);
				WriteMsg(msg, true);
				reader = ExpectedByteResponse(Msg.ID_LIST);
				for (int i = prefetchIDCount - 1; i >= 0; i--)
				{
					_prefetchedIDs[i] = reader.ReadInt();
				}
				remainingIDs = prefetchIDCount;
			}
			remainingIDs--;
			return _prefetchedIDs[remainingIDs];
		}

		internal virtual void ProcessBlobMessage(MsgBlob msg)
		{
			lock (blobLock)
			{
				bool needStart = blobThread == null || blobThread.IsTerminated();
				if (needStart)
				{
					blobThread = new BlobProcessor(this);
				}
				blobThread.Add(msg);
				if (needStart)
				{
					blobThread.Start();
				}
			}
		}

		public override void RaiseVersion(long a_minimumVersion)
		{
			WriteMsg(Msg.RAISE_VERSION.GetWriterForLong(i_trans, a_minimumVersion), true);
		}

		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			throw Exceptions4.VirtualException();
		}

		public override void ReadBytes(byte[] a_bytes, int a_address, int a_length)
		{
			MsgD msg = Msg.READ_BYTES.GetWriterForInts(i_trans, new int[] { a_address, a_length
				 });
			WriteMsg(msg, true);
			Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Msg.READ_BYTES);
			System.Array.Copy(reader._buffer, 0, a_bytes, 0, a_length);
		}

		protected override bool Rename1(Config4Impl config)
		{
			LogMsg(58, null);
			return false;
		}

		public sealed override StatefulBuffer ReadWriterByID(Transaction a_ta, int a_id)
		{
			MsgD msg = Msg.READ_OBJECT.GetWriterForInt(a_ta, a_id);
			WriteMsg(msg, true);
			StatefulBuffer bytes = ((MsgObject)ExpectedResponse(Msg.OBJECT_TO_CLIENT)).Unmarshall
				();
			bytes.SetTransaction(a_ta);
			return bytes;
		}

		public sealed override StatefulBuffer[] ReadWritersByIDs(Transaction a_ta, int[] 
			ids)
		{
			try
			{
				MsgD msg = Msg.READ_MULTIPLE_OBJECTS.GetWriterForIntArray(a_ta, ids, ids.Length);
				WriteMsg(msg, true);
				MsgD response = (MsgD)ExpectedResponse(Msg.READ_MULTIPLE_OBJECTS);
				int count = response.ReadInt();
				StatefulBuffer[] yapWriters = new StatefulBuffer[count];
				for (int i = 0; i < count; i++)
				{
					MsgObject mso = (MsgObject)Msg.OBJECT_TO_CLIENT.PublicClone();
					mso.SetTransaction(GetTransaction());
					mso.PayLoad(response.PayLoad().ReadYapBytes());
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Const4.MESSAGE_LENGTH);
						yapWriters[i] = mso.Unmarshall(Const4.MESSAGE_LENGTH);
						yapWriters[i].SetTransaction(a_ta);
					}
				}
				return yapWriters;
			}
			catch (Exception e)
			{
			}
			return null;
		}

		public sealed override Db4objects.Db4o.Internal.Buffer ReadReaderByID(Transaction
			 a_ta, int a_id)
		{
			return ReadWriterByID(a_ta, a_id);
		}

		private AbstractQueryResult ReadQueryResult(Transaction trans)
		{
			AbstractQueryResult queryResult = null;
			Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Msg.QUERY_RESULT);
			int queryResultID = reader.ReadInt();
			if (queryResultID > 0)
			{
				queryResult = new LazyClientQueryResult(trans, this, queryResultID);
			}
			else
			{
				queryResult = new ClientQueryResult(trans);
			}
			queryResult.LoadFromIdReader(reader);
			return queryResult;
		}

		internal virtual void ReadThis()
		{
			WriteMsg(Msg.GET_CLASSES.GetWriter(SystemTransaction()), true);
			Db4objects.Db4o.Internal.Buffer bytes = ExpectedByteResponse(Msg.GET_CLASSES);
			ClassCollection().SetID(bytes.ReadInt());
			CreateStringIO(bytes.ReadByte());
			ClassCollection().Read(SystemTransaction());
			ClassCollection().RefreshClasses();
		}

		public override void ReleaseSemaphore(string name)
		{
			lock (i_lock)
			{
				CheckClosed();
				if (name == null)
				{
					throw new ArgumentNullException();
				}
				WriteMsg(Msg.RELEASE_SEMAPHORE.GetWriterForString(i_trans, name), true);
			}
		}

		public override void ReleaseSemaphores(Transaction ta)
		{
		}

		private void ReReadAll(IConfiguration config)
		{
			remainingIDs = 0;
			Initialize1(config);
			InitializeTransactions();
			ReadThis();
		}

		public sealed override void Rollback1()
		{
			if (i_config.BatchMessages())
			{
				ClearBatchedObjects();
			}
			WriteMsg(Msg.ROLLBACK, true);
			i_trans.Rollback();
		}

		public override void Send(object obj)
		{
			lock (i_lock)
			{
				if (obj != null)
				{
					WriteMsg(Msg.USER_MESSAGE.GetWriter(Serializer.Marshall(i_trans, obj)), true);
				}
			}
		}

		public sealed override void SetDirtyInSystemTransaction(PersistentBase a_object)
		{
		}

		public override bool SetSemaphore(string name, int timeout)
		{
			lock (i_lock)
			{
				CheckClosed();
				if (name == null)
				{
					throw new ArgumentNullException();
				}
				MsgD msg = Msg.SET_SEMAPHORE.GetWriterForIntString(i_trans, timeout, name);
				WriteMsg(msg, true);
				Msg message = GetResponse();
				return (message.Equals(Msg.SUCCESS));
			}
		}

		public virtual void SwitchToFile(string fileName)
		{
			lock (i_lock)
			{
				Commit();
				MsgD msg = Msg.SWITCH_TO_FILE.GetWriterForString(i_trans, fileName);
				WriteMsg(msg, true);
				ExpectedResponse(Msg.OK);
				ReReadAll(Db4oFactory.CloneConfiguration());
				switchedToFile = fileName;
			}
		}

		public virtual void SwitchToMainFile()
		{
			lock (i_lock)
			{
				Commit();
				WriteMsg(Msg.SWITCH_TO_MAIN_FILE, true);
				ExpectedResponse(Msg.OK);
				ReReadAll(Db4oFactory.CloneConfiguration());
				switchedToFile = null;
			}
		}

		public virtual string Name()
		{
			return ToString();
		}

		public override string ToString()
		{
			return "Client Connection " + userName;
		}

		public override void Shutdown()
		{
		}

		public sealed override void WriteDirty()
		{
		}

		public sealed override void WriteEmbedded(StatefulBuffer a_parent, StatefulBuffer
			 a_child)
		{
			a_parent.AddEmbedded(a_child);
		}

		public void Write(Msg msg)
		{
			msg.Write(this, i_socket);
		}

		public void WriteMsg(Msg a_message, bool flush)
		{
			if (i_config.BatchMessages())
			{
				if (flush && _batchedMessages.IsEmpty())
				{
					Write(a_message);
				}
				else
				{
					AddToBatch(a_message);
					if (flush || _batchedQueueLength > i_config.MaxBatchQueueSize())
					{
						WriteBatchedMessages();
					}
				}
			}
			else
			{
				Write(a_message);
			}
		}

		public sealed override void WriteNew(ClassMetadata a_yapClass, StatefulBuffer aWriter
			)
		{
			MsgD msg = Msg.WRITE_NEW.GetWriter(a_yapClass, aWriter);
			WriteMsg(msg, false);
		}

		public sealed override void WriteTransactionPointer(int a_address)
		{
		}

		public sealed override void WriteUpdate(ClassMetadata a_yapClass, StatefulBuffer 
			a_bytes)
		{
			MsgD msg = Msg.WRITE_UPDATE.GetWriter(a_yapClass, a_bytes);
			WriteMsg(msg, false);
		}

		public virtual bool IsAlive()
		{
			try
			{
				WriteMsg(Msg.PING, true);
				return ExpectedResponse(Msg.OK) != null;
			}
			catch (Db4oException)
			{
				return false;
			}
		}

		public virtual ISocket4 Socket()
		{
			return i_socket;
		}

		private void EnsureIDCacheAllocated(int prefetchIDCount)
		{
			if (_prefetchedIDs == null)
			{
				_prefetchedIDs = new int[prefetchIDCount];
				return;
			}
			if (prefetchIDCount > _prefetchedIDs.Length)
			{
				int[] newPrefetchedIDs = new int[prefetchIDCount];
				System.Array.Copy(_prefetchedIDs, 0, newPrefetchedIDs, 0, _prefetchedIDs.Length);
				_prefetchedIDs = newPrefetchedIDs;
			}
		}

		public override ISystemInfo SystemInfo()
		{
			throw new NotImplementedException("Functionality not availble on clients.");
		}

		public virtual void WriteBlobTo(Transaction trans, BlobImpl blob, Sharpen.IO.File
			 file)
		{
			MsgBlob msg = (MsgBlob)Msg.READ_BLOB.GetWriterForInt(trans, (int)GetID(blob));
			msg._blob = blob;
			ProcessBlobMessage(msg);
		}

		public virtual void ReadBlobFrom(Transaction trans, BlobImpl blob, Sharpen.IO.File
			 file)
		{
			MsgBlob msg = null;
			lock (Lock())
			{
				Set(blob);
				int id = (int)GetID(blob);
				msg = (MsgBlob)Msg.WRITE_BLOB.GetWriterForInt(trans, id);
				msg._blob = blob;
				blob.SetStatus(Status.QUEUED);
			}
			ProcessBlobMessage(msg);
		}

		public override long[] GetIDsForClass(Transaction trans, ClassMetadata clazz)
		{
			MsgD msg = Msg.GET_INTERNAL_IDS.GetWriterForInt(trans, clazz.GetID());
			WriteMsg(msg, true);
			Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Msg.ID_LIST);
			int size = reader.ReadInt();
			long[] ids = new long[size];
			for (int i = 0; i < size; i++)
			{
				ids[i] = reader.ReadInt();
			}
			return ids;
		}

		public override IQueryResult ClassOnlyQuery(Transaction trans, ClassMetadata clazz
			)
		{
			long[] ids = clazz.GetIDs(trans);
			ClientQueryResult resClient = new ClientQueryResult(trans, ids.Length);
			for (int i = 0; i < ids.Length; i++)
			{
				resClient.Add((int)ids[i]);
			}
			return resClient;
		}

		public override IQueryResult ExecuteQuery(QQuery query)
		{
			Transaction trans = query.GetTransaction();
			query.EvaluationMode(Config().QueryEvaluationMode());
			query.Marshall();
			MsgD msg = Msg.QUERY_EXECUTE.GetWriter(Serializer.Marshall(trans, query));
			WriteMsg(msg, true);
			return ReadQueryResult(trans);
		}

		public void WriteBatchedMessages()
		{
			if (_batchedMessages.IsEmpty())
			{
				return;
			}
			Msg msg;
			MsgD multibytes = Msg.WRITE_BATCHED_MESSAGES.GetWriterForLength(GetTransaction(), 
				_batchedQueueLength);
			multibytes.WriteInt(_batchedMessages.Size());
			IEnumerator iter = _batchedMessages.GetEnumerator();
			while (iter.MoveNext())
			{
				msg = (Msg)iter.Current;
				if (msg == null)
				{
					multibytes.WriteInt(0);
				}
				else
				{
					multibytes.WriteInt(msg.PayLoad().GetLength());
					multibytes.PayLoad().Append(msg.PayLoad()._buffer);
				}
			}
			Write(multibytes);
			ClearBatchedObjects();
		}

		public void AddToBatch(Msg msg)
		{
			_batchedMessages.Add(msg);
			_batchedQueueLength += Const4.INT_LENGTH + msg.PayLoad().GetLength();
		}

		private void ClearBatchedObjects()
		{
			_batchedMessages.Clear();
			_batchedQueueLength = Const4.INT_LENGTH;
		}

		internal virtual int Timeout()
		{
			return IsEmbeddedClient() ? Const4.CLIENT_EMBEDDED_TIMEOUT : ConfigImpl().TimeoutClientSocket
				();
		}

		private bool IsEmbeddedClient()
		{
			return i_socket is LoopbackSocket;
		}

		protected override void ShutdownDataStorage()
		{
		}

		public virtual void SetDispatcherName(string name)
		{
		}

		public virtual void StartDispatcher()
		{
		}

		public virtual IClientMessageDispatcher MessageDispatcher()
		{
			return _singleThreaded ? this : _messageDispatcher;
		}
	}
}
