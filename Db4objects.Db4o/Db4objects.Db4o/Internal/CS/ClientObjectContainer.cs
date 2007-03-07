namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class ClientObjectContainer : Db4objects.Db4o.Internal.ObjectContainerBase
		, Db4objects.Db4o.Ext.IExtClient, Db4objects.Db4o.IBlobTransport
	{
		internal readonly object blobLock = new object();

		private Db4objects.Db4o.Internal.CS.BlobProcessor blobThread;

		private Db4objects.Db4o.Foundation.Network.ISocket4 i_socket;

		internal Db4objects.Db4o.Foundation.Queue4 messageQueue = new Db4objects.Db4o.Foundation.Queue4
			();

		internal readonly Db4objects.Db4o.Foundation.Lock4 messageQueueLock = new Db4objects.Db4o.Foundation.Lock4
			();

		private string password;

		internal int[] _prefetchedIDs;

		private Db4objects.Db4o.Internal.CS.ClientMessageDispatcher _readerThread;

		internal int remainingIDs;

		private string switchedToFile;

		private bool _singleThreaded;

		private string userName;

		private Db4objects.Db4o.Ext.Db4oDatabase i_db;

		protected bool _doFinalize = true;

		private int _blockSize = 1;

		private Db4objects.Db4o.Foundation.Collection4 _batchedMessages = new Db4objects.Db4o.Foundation.Collection4
			();

		private int _batchedQueueLength = Db4objects.Db4o.Internal.Const4.INT_LENGTH;

		private ClientObjectContainer(Db4objects.Db4o.Config.IConfiguration config) : base
			(config, null)
		{
		}

		public ClientObjectContainer(string fakeServerFile) : this(Db4objects.Db4o.Db4oFactory
			.CloneConfiguration())
		{
			lock (Lock())
			{
				_singleThreaded = ConfigImpl().SingleThreadedClient();
				throw new System.Exception("This constructor is for Debug.fakeServer use only.");
				Initialize3();
				Db4objects.Db4o.Internal.Platform4.PostOpen(this);
			}
		}

		public ClientObjectContainer(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Foundation.Network.ISocket4
			 socket, string user, string password_, bool login) : this(config)
		{
			lock (Lock())
			{
				_singleThreaded = ConfigImpl().SingleThreadedClient();
				if (password_ == null)
				{
					throw new System.ArgumentNullException(Db4objects.Db4o.Internal.Messages.Get(56));
				}
				if (!login)
				{
					password_ = null;
				}
				userName = user;
				password = password_;
				i_socket = socket;
				try
				{
					LoginToServer(socket);
				}
				catch (System.IO.IOException e)
				{
					StopSession();
					throw;
				}
				if (!_singleThreaded)
				{
					StartReaderThread(socket, user);
				}
				LogMsg(36, ToString());
				ReadThis();
				Initialize3();
				Db4objects.Db4o.Internal.Platform4.PostOpen(this);
			}
		}

		private void StartReaderThread(Db4objects.Db4o.Foundation.Network.ISocket4 socket
			, string user)
		{
			_readerThread = new Db4objects.Db4o.Internal.CS.ClientMessageDispatcher(this, socket
				, messageQueue, messageQueueLock);
			_readerThread.SetName("db4o message client for user " + user);
			_readerThread.Start();
		}

		public override void Backup(string path)
		{
			Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(60);
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
			if (_readerThread == null || _readerThread.IsClosed())
			{
				base.Close2();
				return;
			}
			try
			{
				WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.COMMIT_OK, true);
				ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.OK);
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Internal.Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.CLOSE, true);
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Internal.Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				if (!_singleThreaded)
				{
					_readerThread.Close();
				}
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Internal.Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				i_socket.Close();
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Internal.Exceptions4.CatchAllExceptDb4oException(e);
			}
			base.Close2();
		}

		public sealed override void Commit1()
		{
			i_trans.Commit();
		}

		public override int ConverterVersion()
		{
			return Db4objects.Db4o.Internal.Convert.Converter.VERSION;
		}

		internal virtual Db4objects.Db4o.Foundation.Network.ISocket4 CreateParalellSocket
			()
		{
			WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.GET_THREAD_ID, true);
			int serverThreadID = ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.ID_LIST).ReadInt();
			Db4objects.Db4o.Foundation.Network.ISocket4 sock = i_socket.OpenParalellSocket();
			if (!(i_socket is Db4objects.Db4o.Foundation.Network.LoopbackSocket))
			{
				LoginToServer(sock);
			}
			if (switchedToFile != null)
			{
				Db4objects.Db4o.Internal.CS.Messages.MsgD message = Db4objects.Db4o.Internal.CS.Messages.Msg
					.SWITCH_TO_FILE.GetWriterForString(i_systemTrans, switchedToFile);
				message.Write(this, sock);
				if (!(Db4objects.Db4o.Internal.CS.Messages.Msg.OK.Equals(Db4objects.Db4o.Internal.CS.Messages.Msg
					.ReadMessage(i_systemTrans, sock))))
				{
					throw new System.IO.IOException(Db4objects.Db4o.Internal.Messages.Get(42));
				}
			}
			Db4objects.Db4o.Internal.CS.Messages.Msg.USE_TRANSACTION.GetWriterForInt(i_trans, 
				serverThreadID).Write(this, sock);
			return sock;
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult NewQueryResult
			(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Config.QueryEvaluationMode
			 mode)
		{
			throw new System.InvalidOperationException();
		}

		public sealed override Db4objects.Db4o.Internal.Transaction NewTransaction(Db4objects.Db4o.Internal.Transaction
			 parentTransaction)
		{
			return new Db4objects.Db4o.Internal.CS.ClientTransaction(this, parentTransaction);
		}

		public override bool CreateYapClass(Db4objects.Db4o.Internal.ClassMetadata a_yapClass
			, Db4objects.Db4o.Reflect.IReflectClass a_class, Db4objects.Db4o.Internal.ClassMetadata
			 a_superYapClass)
		{
			WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.CREATE_CLASS.GetWriterForString
				(i_systemTrans, a_class.GetName()), true);
			Db4objects.Db4o.Internal.CS.Messages.Msg resp = GetResponse();
			if (resp == null)
			{
				return false;
			}
			if (resp.Equals(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED))
			{
				SendClassMeta(a_class);
				resp = GetResponse();
			}
			if (resp.Equals(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED))
			{
				if (ConfigImpl().ExceptionsOnNotStorable())
				{
					throw new Db4objects.Db4o.Ext.ObjectNotStorableException(a_class);
				}
				return false;
			}
			if (!resp.Equals(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT))
			{
				return false;
			}
			Db4objects.Db4o.Internal.CS.Messages.MsgObject message = (Db4objects.Db4o.Internal.CS.Messages.MsgObject
				)resp;
			Db4objects.Db4o.Internal.StatefulBuffer bytes = message.Unmarshall();
			if (bytes == null)
			{
				return false;
			}
			bytes.SetTransaction(GetSystemTransaction());
			if (!base.CreateYapClass(a_yapClass, a_class, a_superYapClass))
			{
				return false;
			}
			a_yapClass.SetID(message.GetId());
			a_yapClass.ReadName1(GetSystemTransaction(), bytes);
			ClassCollection().AddYapClass(a_yapClass);
			ClassCollection().ReadYapClass(a_yapClass, a_class);
			return true;
		}

		private void SendClassMeta(Db4objects.Db4o.Reflect.IReflectClass reflectClass)
		{
			Db4objects.Db4o.Internal.CS.ClassInfo classMeta = _classMetaHelper.GetClassMeta(reflectClass
				);
			WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.CLASS_META.GetWriter(Marshall(i_systemTrans
				, classMeta)), true);
		}

		public override long CurrentVersion()
		{
			WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.CURRENT_VERSION, true);
			return ((Db4objects.Db4o.Internal.CS.Messages.MsgD)ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.ID_LIST)).ReadLong();
		}

		public sealed override bool Delete4(Db4objects.Db4o.Internal.Transaction ta, Db4objects.Db4o.Internal.ObjectReference
			 yo, int a_cascade, bool userCall)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.DELETE.GetWriterForInts(i_trans, new int[] { yo.GetID(), userCall ? 1 : 0 });
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

		internal Db4objects.Db4o.Internal.Buffer ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
			 expectedMessage)
		{
			Db4objects.Db4o.Internal.CS.Messages.Msg msg = ExpectedResponse(expectedMessage);
			if (msg == null)
			{
				return null;
			}
			return msg.GetByteLoad();
		}

		public Db4objects.Db4o.Internal.CS.Messages.Msg ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
			 expectedMessage)
		{
			Db4objects.Db4o.Internal.CS.Messages.Msg message = GetResponse();
			if (expectedMessage.Equals(message))
			{
				return message;
			}
			return null;
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult GetAll(
			Db4objects.Db4o.Internal.Transaction trans)
		{
			int mode = Config().QueryEvaluationMode().AsInt();
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.GET_ALL.GetWriterForInt(trans, mode);
			WriteMsg(msg, true);
			return ReadQueryResult(trans);
		}

		/// <summary>may return null, if no message is returned.</summary>
		/// <remarks>
		/// may return null, if no message is returned. Error handling is weak and
		/// should ideally be able to trigger some sort of state listener (connection
		/// dead) on the client.
		/// </remarks>
		internal virtual Db4objects.Db4o.Internal.CS.Messages.Msg GetResponse()
		{
			return _singleThreaded ? GetResponseSingleThreaded() : GetResponseMultiThreaded();
		}

		private Db4objects.Db4o.Internal.CS.Messages.Msg GetResponseMultiThreaded()
		{
			try
			{
				return (Db4objects.Db4o.Internal.CS.Messages.Msg)messageQueueLock.Run(new _AnonymousInnerClass334
					(this));
			}
			catch (System.Exception ex)
			{
				Db4objects.Db4o.Internal.Exceptions4.CatchAllExceptDb4oException(ex);
				return null;
			}
		}

		private sealed class _AnonymousInnerClass334 : Db4objects.Db4o.Foundation.IClosure4
		{
			public _AnonymousInnerClass334(ClientObjectContainer _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				Db4objects.Db4o.Internal.CS.Messages.Msg message = this.RetrieveMessage();
				if (message != null)
				{
					return message;
				}
				this.ThrowOnClosed();
				this._enclosing.messageQueueLock.Snooze(this._enclosing.Timeout());
				this.ThrowOnClosed();
				return this.RetrieveMessage();
			}

			private void ThrowOnClosed()
			{
				if (this._enclosing._readerThread.IsClosed())
				{
					this._enclosing._doFinalize = false;
					throw new Db4objects.Db4o.Ext.Db4oException(Db4objects.Db4o.Internal.Messages.Get
						(Db4objects.Db4o.Internal.Messages.CLOSED_OR_OPEN_FAILED));
				}
			}

			private Db4objects.Db4o.Internal.CS.Messages.Msg RetrieveMessage()
			{
				Db4objects.Db4o.Internal.CS.Messages.Msg message = null;
				message = (Db4objects.Db4o.Internal.CS.Messages.Msg)this._enclosing.messageQueue.
					Next();
				if (message != null)
				{
					if (Db4objects.Db4o.Internal.CS.Messages.Msg.ERROR.Equals(message))
					{
						throw new Db4objects.Db4o.Ext.Db4oException("Client connection error");
					}
				}
				return message;
			}

			private readonly ClientObjectContainer _enclosing;
		}

		private Db4objects.Db4o.Internal.CS.Messages.Msg GetResponseSingleThreaded()
		{
			while (i_socket != null)
			{
				try
				{
					Db4objects.Db4o.Internal.CS.Messages.Msg message = Db4objects.Db4o.Internal.CS.Messages.Msg
						.ReadMessage(i_trans, i_socket);
					if (Db4objects.Db4o.Internal.CS.Messages.Msg.PING.Equals(message))
					{
						WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.OK, true);
					}
					else
					{
						if (Db4objects.Db4o.Internal.CS.Messages.Msg.CLOSE.Equals(message))
						{
							LogMsg(35, ToString());
							Close();
							return null;
						}
						else
						{
							if (message != null)
							{
								return message;
							}
						}
					}
				}
				catch
				{
				}
			}
			return null;
		}

		public override Db4objects.Db4o.Internal.ClassMetadata GetYapClass(int a_id)
		{
			if (a_id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Internal.ClassMetadata yc = base.GetYapClass(a_id);
			if (yc != null)
			{
				return yc;
			}
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.CLASS_NAME_FOR_ID.GetWriterForInt(i_systemTrans, a_id);
			WriteMsg(msg, true);
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = (Db4objects.Db4o.Internal.CS.Messages.MsgD
				)ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.CLASS_NAME_FOR_ID);
			string className = message.ReadString();
			if (className != null && className.Length > 0)
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = Reflector().ForName(className);
				if (claxx != null)
				{
					return ProduceYapClass(claxx);
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

		public override Db4objects.Db4o.Ext.Db4oDatabase Identity()
		{
			if (i_db == null)
			{
				WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.IDENTITY, true);
				Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
					.ID_LIST);
				ShowInternalClasses(true);
				try
				{
					i_db = (Db4objects.Db4o.Ext.Db4oDatabase)GetByID(reader.ReadInt());
					Activate1(i_systemTrans, i_db, 3);
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

		internal virtual void LoginToServer(Db4objects.Db4o.Foundation.Network.ISocket4 a_socket
			)
		{
			if (password != null)
			{
				Db4objects.Db4o.Internal.UnicodeStringIO stringWriter = new Db4objects.Db4o.Internal.UnicodeStringIO
					();
				int length = stringWriter.Length(userName) + stringWriter.Length(password);
				Db4objects.Db4o.Internal.CS.Messages.MsgD message = Db4objects.Db4o.Internal.CS.Messages.Msg
					.LOGIN.GetWriterForLength(i_systemTrans, length);
				message.WriteString(userName);
				message.WriteString(password);
				message.Write(this, a_socket);
				Db4objects.Db4o.Internal.CS.Messages.Msg msg = Db4objects.Db4o.Internal.CS.Messages.Msg
					.ReadMessage(i_systemTrans, a_socket);
				if (!Db4objects.Db4o.Internal.CS.Messages.Msg.LOGIN_OK.Equals(msg))
				{
					throw new System.IO.IOException(Db4objects.Db4o.Internal.Messages.Get(42));
				}
				Db4objects.Db4o.Internal.Buffer payLoad = msg.PayLoad();
				_blockSize = payLoad.ReadInt();
				int doEncrypt = payLoad.ReadInt();
				if (doEncrypt == 0)
				{
					i_handlers.OldEncryptionOff();
				}
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
				Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
					.PREFETCH_IDS.GetWriterForInt(i_trans, prefetchIDCount);
				WriteMsg(msg, true);
				reader = ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.ID_LIST);
				for (int i = prefetchIDCount - 1; i >= 0; i--)
				{
					_prefetchedIDs[i] = reader.ReadInt();
				}
				remainingIDs = prefetchIDCount;
			}
			remainingIDs--;
			return _prefetchedIDs[remainingIDs];
		}

		internal virtual void ProcessBlobMessage(Db4objects.Db4o.Internal.CS.Messages.MsgBlob
			 msg)
		{
			lock (blobLock)
			{
				bool needStart = blobThread == null || blobThread.IsTerminated();
				if (needStart)
				{
					blobThread = new Db4objects.Db4o.Internal.CS.BlobProcessor(this);
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
			WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.RAISE_VERSION.GetWriterForLong(
				i_trans, a_minimumVersion), true);
		}

		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.VirtualException();
		}

		public override void ReadBytes(byte[] a_bytes, int a_address, int a_length)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.READ_BYTES.GetWriterForInts(i_trans, new int[] { a_address, a_length });
			WriteMsg(msg, true);
			Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.READ_BYTES);
			System.Array.Copy(reader._buffer, 0, a_bytes, 0, a_length);
		}

		protected override bool Rename1(Db4objects.Db4o.Internal.Config4Impl config)
		{
			LogMsg(58, null);
			return false;
		}

		public sealed override Db4objects.Db4o.Internal.StatefulBuffer ReadWriterByID(Db4objects.Db4o.Internal.Transaction
			 a_ta, int a_id)
		{
			try
			{
				Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
					.READ_OBJECT.GetWriterForInt(a_ta, a_id);
				WriteMsg(msg, true);
				Db4objects.Db4o.Internal.StatefulBuffer bytes = ((Db4objects.Db4o.Internal.CS.Messages.MsgObject
					)ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT)).Unmarshall
					();
				if (bytes == null)
				{
					return null;
				}
				bytes.SetTransaction(a_ta);
				return bytes;
			}
			catch
			{
				return null;
			}
		}

		public sealed override Db4objects.Db4o.Internal.StatefulBuffer[] ReadWritersByIDs
			(Db4objects.Db4o.Internal.Transaction a_ta, int[] ids)
		{
			try
			{
				Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
					.READ_MULTIPLE_OBJECTS.GetWriterForIntArray(a_ta, ids, ids.Length);
				WriteMsg(msg, true);
				Db4objects.Db4o.Internal.CS.Messages.MsgD message = (Db4objects.Db4o.Internal.CS.Messages.MsgD
					)ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.READ_MULTIPLE_OBJECTS
					);
				int count = message.ReadInt();
				Db4objects.Db4o.Internal.StatefulBuffer[] yapWriters = new Db4objects.Db4o.Internal.StatefulBuffer
					[count];
				for (int i = 0; i < count; i++)
				{
					Db4objects.Db4o.Internal.CS.Messages.MsgObject mso = (Db4objects.Db4o.Internal.CS.Messages.MsgObject
						)Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_TO_CLIENT.Clone(GetTransaction(
						));
					mso.PayLoad(message.PayLoad().ReadYapBytes());
					if (mso.PayLoad() != null)
					{
						mso.PayLoad().IncrementOffset(Db4objects.Db4o.Internal.Const4.MESSAGE_LENGTH);
						yapWriters[i] = mso.Unmarshall(Db4objects.Db4o.Internal.Const4.MESSAGE_LENGTH);
						yapWriters[i].SetTransaction(a_ta);
					}
				}
				return yapWriters;
			}
			catch (System.Exception e)
			{
			}
			return null;
		}

		public sealed override Db4objects.Db4o.Internal.Buffer ReadReaderByID(Db4objects.Db4o.Internal.Transaction
			 a_ta, int a_id)
		{
			return ReadWriterByID(a_ta, a_id);
		}

		private Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult ReadQueryResult
			(Db4objects.Db4o.Internal.Transaction trans)
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = null;
			Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.QUERY_RESULT);
			int queryResultID = reader.ReadInt();
			if (queryResultID > 0)
			{
				queryResult = new Db4objects.Db4o.Internal.CS.LazyClientQueryResult(trans, this, 
					queryResultID);
			}
			else
			{
				queryResult = new Db4objects.Db4o.Internal.CS.ClientQueryResult(trans);
			}
			queryResult.LoadFromIdReader(reader);
			return queryResult;
		}

		internal virtual void ReadThis()
		{
			WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.GET_CLASSES.GetWriter(i_systemTrans
				), true);
			Db4objects.Db4o.Internal.Buffer bytes = ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.GET_CLASSES);
			ClassCollection().SetID(bytes.ReadInt());
			CreateStringIO(bytes.ReadByte());
			ClassCollection().Read(i_systemTrans);
			ClassCollection().RefreshClasses();
		}

		public override void ReleaseSemaphore(string name)
		{
			lock (i_lock)
			{
				CheckClosed();
				if (name == null)
				{
					throw new System.ArgumentNullException();
				}
				WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.RELEASE_SEMAPHORE.GetWriterForString
					(i_trans, name), true);
			}
		}

		public override void ReleaseSemaphores(Db4objects.Db4o.Internal.Transaction ta)
		{
		}

		private void ReReadAll(Db4objects.Db4o.Config.IConfiguration config)
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
			WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.ROLLBACK, true);
			i_trans.Rollback();
		}

		public override void Send(object obj)
		{
			lock (i_lock)
			{
				if (obj != null)
				{
					WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.USER_MESSAGE.GetWriter(Marshall
						(i_trans, obj)), true);
				}
			}
		}

		public sealed override void SetDirtyInSystemTransaction(Db4objects.Db4o.Internal.PersistentBase
			 a_object)
		{
		}

		public override bool SetSemaphore(string name, int timeout)
		{
			lock (i_lock)
			{
				CheckClosed();
				if (name == null)
				{
					throw new System.ArgumentNullException();
				}
				Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
					.SET_SEMAPHORE.GetWriterForIntString(i_trans, timeout, name);
				WriteMsg(msg, true);
				Db4objects.Db4o.Internal.CS.Messages.Msg message = GetResponse();
				return (message.Equals(Db4objects.Db4o.Internal.CS.Messages.Msg.SUCCESS));
			}
		}

		public virtual void SwitchToFile(string fileName)
		{
			lock (i_lock)
			{
				Commit();
				Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
					.SWITCH_TO_FILE.GetWriterForString(i_trans, fileName);
				WriteMsg(msg, true);
				ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.OK);
				ReReadAll(Db4objects.Db4o.Db4oFactory.CloneConfiguration());
				switchedToFile = fileName;
			}
		}

		public virtual void SwitchToMainFile()
		{
			lock (i_lock)
			{
				Commit();
				WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.SWITCH_TO_MAIN_FILE, true);
				ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.OK);
				ReReadAll(Db4objects.Db4o.Db4oFactory.CloneConfiguration());
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

		public override void Write(bool shuttingDown)
		{
		}

		public sealed override void WriteDirty()
		{
		}

		public sealed override void WriteEmbedded(Db4objects.Db4o.Internal.StatefulBuffer
			 a_parent, Db4objects.Db4o.Internal.StatefulBuffer a_child)
		{
			a_parent.AddEmbedded(a_child);
		}

		internal void WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg a_message)
		{
			a_message.Write(this, i_socket);
		}

		public void WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg a_message, bool flush
			)
		{
			if (i_config.BatchMessages())
			{
				if (flush && _batchedMessages.IsEmpty())
				{
					WriteMsg(a_message);
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
				WriteMsg(a_message);
			}
		}

		public sealed override void WriteNew(Db4objects.Db4o.Internal.ClassMetadata a_yapClass
			, Db4objects.Db4o.Internal.StatefulBuffer aWriter)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.WRITE_NEW.GetWriter(a_yapClass, aWriter);
			WriteMsg(msg, false);
		}

		public sealed override void WriteTransactionPointer(int a_address)
		{
		}

		public sealed override void WriteUpdate(Db4objects.Db4o.Internal.ClassMetadata a_yapClass
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.WRITE_UPDATE.GetWriter(a_yapClass, a_bytes);
			WriteMsg(msg, false);
		}

		public virtual bool IsAlive()
		{
			try
			{
				WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.PING, true);
				return ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.OK) != null;
			}
			catch (Db4objects.Db4o.Ext.Db4oException)
			{
				return false;
			}
		}

		public virtual Db4objects.Db4o.Foundation.Network.ISocket4 Socket()
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

		public override Db4objects.Db4o.Ext.ISystemInfo SystemInfo()
		{
			throw new System.NotImplementedException("Functionality not availble on clients."
				);
		}

		public virtual void WriteBlobTo(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.BlobImpl
			 blob, Sharpen.IO.File file)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgBlob msg = (Db4objects.Db4o.Internal.CS.Messages.MsgBlob
				)Db4objects.Db4o.Internal.CS.Messages.Msg.READ_BLOB.GetWriterForInt(trans, (int)
				GetID(blob));
			msg._blob = blob;
			ProcessBlobMessage(msg);
		}

		public virtual void ReadBlobFrom(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.BlobImpl
			 blob, Sharpen.IO.File file)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgBlob msg = null;
			lock (Lock())
			{
				Set(blob);
				int id = (int)GetID(blob);
				msg = (Db4objects.Db4o.Internal.CS.Messages.MsgBlob)Db4objects.Db4o.Internal.CS.Messages.Msg
					.WRITE_BLOB.GetWriterForInt(trans, id);
				msg._blob = blob;
				blob.SetStatus(Db4objects.Db4o.Ext.Status.QUEUED);
			}
			ProcessBlobMessage(msg);
		}

		public override long[] GetIDsForClass(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.ClassMetadata clazz)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.GET_INTERNAL_IDS.GetWriterForInt(trans, clazz.GetID());
			WriteMsg(msg, true);
			Db4objects.Db4o.Internal.Buffer reader = ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.ID_LIST);
			int size = reader.ReadInt();
			long[] ids = new long[size];
			for (int i = 0; i < size; i++)
			{
				ids[i] = reader.ReadInt();
			}
			return ids;
		}

		public override Db4objects.Db4o.Internal.Query.Result.IQueryResult ClassOnlyQuery
			(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.ClassMetadata
			 clazz)
		{
			long[] ids = clazz.GetIDs(trans);
			Db4objects.Db4o.Internal.CS.ClientQueryResult resClient = new Db4objects.Db4o.Internal.CS.ClientQueryResult
				(trans, ids.Length);
			for (int i = 0; i < ids.Length; i++)
			{
				resClient.Add((int)ids[i]);
			}
			return resClient;
		}

		public override Db4objects.Db4o.Internal.Query.Result.IQueryResult ExecuteQuery(Db4objects.Db4o.Internal.Query.Processor.QQuery
			 query)
		{
			Db4objects.Db4o.Internal.Transaction trans = query.GetTransaction();
			query.EvaluationMode(Config().QueryEvaluationMode());
			query.Marshall();
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.QUERY_EXECUTE.GetWriter(Marshall(trans, query));
			WriteMsg(msg, true);
			return ReadQueryResult(trans);
		}

		public void WriteBatchedMessages()
		{
			if (_batchedMessages.IsEmpty())
			{
				return;
			}
			Db4objects.Db4o.Internal.CS.Messages.Msg msg;
			Db4objects.Db4o.Internal.CS.Messages.MsgD multibytes = Db4objects.Db4o.Internal.CS.Messages.Msg
				.WRITE_BATCHED_MESSAGES.GetWriterForLength(GetTransaction(), _batchedQueueLength
				);
			multibytes.WriteInt(_batchedMessages.Size());
			System.Collections.IEnumerator iter = _batchedMessages.GetEnumerator();
			while (iter.MoveNext())
			{
				msg = (Db4objects.Db4o.Internal.CS.Messages.Msg)iter.Current;
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
			WriteMsg(multibytes);
			ClearBatchedObjects();
		}

		public void AddToBatch(Db4objects.Db4o.Internal.CS.Messages.Msg msg)
		{
			_batchedMessages.Add(msg);
			_batchedQueueLength += Db4objects.Db4o.Internal.Const4.INT_LENGTH + msg.PayLoad()
				.GetLength();
		}

		private void ClearBatchedObjects()
		{
			_batchedMessages.Clear();
			_batchedQueueLength = Db4objects.Db4o.Internal.Const4.INT_LENGTH;
		}

		private int Timeout()
		{
			return IsEmbeddedClient() ? Db4objects.Db4o.Internal.Const4.CLIENT_EMBEDDED_TIMEOUT
				 : ConfigImpl().TimeoutClientSocket();
		}

		private bool IsEmbeddedClient()
		{
			return i_socket is Db4objects.Db4o.Foundation.Network.LoopbackSocket;
		}
	}
}
