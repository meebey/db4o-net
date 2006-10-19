namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapClient : Db4objects.Db4o.YapStream, Db4objects.Db4o.Ext.IExtClient
	{
		internal readonly object blobLock = new object();

		private Db4objects.Db4o.YapClientBlobThread blobThread;

		private Db4objects.Db4o.Foundation.Network.IYapSocket i_socket;

		internal Db4objects.Db4o.Foundation.Queue4 messageQueue = new Db4objects.Db4o.Foundation.Queue4
			();

		internal readonly Db4objects.Db4o.Foundation.Lock4 messageQueueLock = new Db4objects.Db4o.Foundation.Lock4
			();

		private string password;

		internal int[] _prefetchedIDs;

		private Db4objects.Db4o.YapClientThread _readerThread;

		internal int remainingIDs;

		private string switchedToFile;

		private bool _singleThreaded;

		private string userName;

		private Db4objects.Db4o.Ext.Db4oDatabase i_db;

		protected bool _doFinalize = true;

		private int _blockSize = 1;

		private YapClient(Db4objects.Db4o.Config.IConfiguration config) : base(config, null
			)
		{
		}

		public YapClient(string fakeServerFile) : this(Db4objects.Db4o.Db4o.CloneConfiguration
			())
		{
			lock (Lock())
			{
				_singleThreaded = ConfigImpl().SingleThreadedClient();
				throw new System.Exception("This constructor is for Debug.fakeServer use only.");
				Initialize3();
				Db4objects.Db4o.Platform4.PostOpen(this);
			}
		}

		internal YapClient(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Foundation.Network.IYapSocket
			 socket, string user, string password_, bool login) : this(config)
		{
			lock (Lock())
			{
				_singleThreaded = ConfigImpl().SingleThreadedClient();
				if (password_ == null)
				{
					throw new System.ArgumentNullException(Db4objects.Db4o.Messages.Get(56));
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
					throw e;
				}
				if (!_singleThreaded)
				{
					StartReaderThread(socket, user);
				}
				LogMsg(36, ToString());
				ReadThis();
				Initialize3();
				Db4objects.Db4o.Platform4.PostOpen(this);
			}
		}

		private void StartReaderThread(Db4objects.Db4o.Foundation.Network.IYapSocket socket
			, string user)
		{
			_readerThread = new Db4objects.Db4o.YapClientThread(this, socket, messageQueue, messageQueueLock
				);
			_readerThread.SetName("db4o message client for user " + user);
			_readerThread.Start();
		}

		public override void Backup(string path)
		{
			Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(60);
		}

		public virtual void BlockSize(int blockSize)
		{
			_blockSize = blockSize;
		}

		public override byte BlockSize()
		{
			return (byte)_blockSize;
		}

		internal override bool Close2()
		{
			if (_readerThread.IsClosed())
			{
				return base.Close2();
			}
			try
			{
				Db4objects.Db4o.Msg.COMMIT_OK.Write(this, i_socket);
				ExpectedResponse(Db4objects.Db4o.Msg.OK);
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Inside.Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				Db4objects.Db4o.Msg.CLOSE.Write(this, i_socket);
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Inside.Exceptions4.CatchAllExceptDb4oException(e);
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
				Db4objects.Db4o.Inside.Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				i_socket.Close();
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Inside.Exceptions4.CatchAllExceptDb4oException(e);
			}
			bool ret = base.Close2();
			return ret;
		}

		internal sealed override void Commit1()
		{
			i_trans.Commit();
		}

		public override int ConverterVersion()
		{
			return Db4objects.Db4o.Inside.Convert.Converter.VERSION;
		}

		internal virtual Db4objects.Db4o.Foundation.Network.IYapSocket CreateParalellSocket
			()
		{
			Db4objects.Db4o.Msg.GET_THREAD_ID.Write(this, i_socket);
			int serverThreadID = ExpectedByteResponse(Db4objects.Db4o.Msg.ID_LIST).ReadInt();
			Db4objects.Db4o.Foundation.Network.IYapSocket sock = i_socket.OpenParalellSocket(
				);
			if (!(i_socket is Db4objects.Db4o.Foundation.Network.YapSocketFake))
			{
				LoginToServer(sock);
			}
			if (switchedToFile != null)
			{
				Db4objects.Db4o.MsgD message = Db4objects.Db4o.Msg.SWITCH_TO_FILE.GetWriterForString
					(i_systemTrans, switchedToFile);
				message.Write(this, sock);
				if (!(Db4objects.Db4o.Msg.OK.Equals(Db4objects.Db4o.Msg.ReadMessage(i_systemTrans
					, sock))))
				{
					throw new System.IO.IOException(Db4objects.Db4o.Messages.Get(42));
				}
			}
			Db4objects.Db4o.Msg.USE_TRANSACTION.GetWriterForInt(i_trans, serverThreadID).Write
				(this, sock);
			return sock;
		}

		internal sealed override Db4objects.Db4o.QueryResultImpl CreateQResult(Db4objects.Db4o.Transaction
			 a_ta)
		{
			return new Db4objects.Db4o.QResultClient(a_ta);
		}

		public sealed override Db4objects.Db4o.Transaction NewTransaction(Db4objects.Db4o.Transaction
			 parentTransaction)
		{
			return new Db4objects.Db4o.TransactionClient(this, parentTransaction);
		}

		internal override bool CreateYapClass(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.Reflect.IReflectClass
			 a_class, Db4objects.Db4o.YapClass a_superYapClass)
		{
			WriteMsg(Db4objects.Db4o.Msg.CREATE_CLASS.GetWriterForString(i_systemTrans, a_class
				.GetName()));
			Db4objects.Db4o.Msg resp = GetResponse();
			if (resp == null)
			{
				return false;
			}
			if (resp.Equals(Db4objects.Db4o.Msg.FAILED))
			{
				if (ConfigImpl().ExceptionsOnNotStorable())
				{
					throw new Db4objects.Db4o.Ext.ObjectNotStorableException(a_class);
				}
				return false;
			}
			if (!resp.Equals(Db4objects.Db4o.Msg.OBJECT_TO_CLIENT))
			{
				return false;
			}
			Db4objects.Db4o.MsgObject message = (Db4objects.Db4o.MsgObject)resp;
			Db4objects.Db4o.YapWriter bytes = message.Unmarshall();
			if (bytes == null)
			{
				return false;
			}
			bytes.SetTransaction(GetSystemTransaction());
			if (!base.CreateYapClass(a_yapClass, a_class, a_superYapClass))
			{
				return false;
			}
			a_yapClass.SetID(message._id);
			a_yapClass.ReadName1(GetSystemTransaction(), bytes);
			ClassCollection().AddYapClass(a_yapClass);
			ClassCollection().ReadYapClass(a_yapClass, a_class);
			return true;
		}

		public override long CurrentVersion()
		{
			WriteMsg(Db4objects.Db4o.Msg.CURRENT_VERSION);
			return ((Db4objects.Db4o.MsgD)ExpectedResponse(Db4objects.Db4o.Msg.ID_LIST)).ReadLong
				();
		}

		internal sealed override bool Delete5(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.YapObject
			 yo, int a_cascade, bool userCall)
		{
			WriteMsg(Db4objects.Db4o.Msg.DELETE.GetWriterForInts(i_trans, new int[] { yo.GetID
				(), userCall ? 1 : 0 }));
			return true;
		}

		internal override bool DetectSchemaChanges()
		{
			return false;
		}

		protected override bool DoFinalize()
		{
			return _doFinalize;
		}

		internal Db4objects.Db4o.YapReader ExpectedByteResponse(Db4objects.Db4o.Msg expectedMessage
			)
		{
			Db4objects.Db4o.Msg msg = ExpectedResponse(expectedMessage);
			if (msg == null)
			{
				return null;
			}
			return msg.GetByteLoad();
		}

		internal Db4objects.Db4o.Msg ExpectedResponse(Db4objects.Db4o.Msg expectedMessage
			)
		{
			Db4objects.Db4o.Msg message = GetResponse();
			if (expectedMessage.Equals(message))
			{
				return message;
			}
			return null;
		}

		internal override void GetAll(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.QueryResultImpl
			 a_res)
		{
			WriteMsg(Db4objects.Db4o.Msg.GET_ALL);
			ReadResult(a_res);
		}

		/// <summary>may return null, if no message is returned.</summary>
		/// <remarks>
		/// may return null, if no message is returned. Error handling is weak and
		/// should ideally be able to trigger some sort of state listener (connection
		/// dead) on the client.
		/// </remarks>
		internal virtual Db4objects.Db4o.Msg GetResponse()
		{
			return _singleThreaded ? GetResponseSingleThreaded() : GetResponseMultiThreaded();
		}

		private Db4objects.Db4o.Msg GetResponseMultiThreaded()
		{
			try
			{
				return (Db4objects.Db4o.Msg)messageQueueLock.Run(new _AnonymousInnerClass310(this
					));
			}
			catch (System.Exception ex)
			{
				Db4objects.Db4o.Inside.Exceptions4.CatchAllExceptDb4oException(ex);
				return null;
			}
		}

		private sealed class _AnonymousInnerClass310 : Db4objects.Db4o.Foundation.IClosure4
		{
			public _AnonymousInnerClass310(YapClient _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				Db4objects.Db4o.Msg message = this.RetrieveMessage();
				if (message != null)
				{
					return message;
				}
				this.ThrowOnClosed();
				this._enclosing.messageQueueLock.Snooze(this._enclosing.ConfigImpl().TimeoutClientSocket
					());
				this.ThrowOnClosed();
				return this.RetrieveMessage();
			}

			private void ThrowOnClosed()
			{
				if (this._enclosing._readerThread.IsClosed())
				{
					this._enclosing._doFinalize = false;
					throw new Db4objects.Db4o.Ext.Db4oException(Db4objects.Db4o.Messages.Get(Db4objects.Db4o.Messages
						.CLOSED_OR_OPEN_FAILED));
				}
			}

			private Db4objects.Db4o.Msg RetrieveMessage()
			{
				Db4objects.Db4o.Msg message = null;
				message = (Db4objects.Db4o.Msg)this._enclosing.messageQueue.Next();
				if (message != null)
				{
					if (Db4objects.Db4o.Msg.ERROR.Equals(message))
					{
						throw new Db4objects.Db4o.Ext.Db4oException("Client connection error");
					}
				}
				return message;
			}

			private readonly YapClient _enclosing;
		}

		private Db4objects.Db4o.Msg GetResponseSingleThreaded()
		{
			while (i_socket != null)
			{
				try
				{
					Db4objects.Db4o.Msg message = Db4objects.Db4o.Msg.ReadMessage(i_trans, i_socket);
					if (Db4objects.Db4o.Msg.PING.Equals(message))
					{
						WriteMsg(Db4objects.Db4o.Msg.OK);
					}
					else
					{
						if (Db4objects.Db4o.Msg.CLOSE.Equals(message))
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
				catch (System.Exception e)
				{
				}
			}
			return null;
		}

		public override Db4objects.Db4o.YapClass GetYapClass(int a_id)
		{
			Db4objects.Db4o.YapClass yc = base.GetYapClass(a_id);
			if (yc != null)
			{
				return yc;
			}
			WriteMsg(Db4objects.Db4o.Msg.CLASS_NAME_FOR_ID.GetWriterForInt(i_systemTrans, a_id
				));
			Db4objects.Db4o.MsgD message = (Db4objects.Db4o.MsgD)ExpectedResponse(Db4objects.Db4o.Msg
				.CLASS_NAME_FOR_ID);
			string className = message.ReadString();
			if (className != null && className.Length > 0)
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = Reflector().ForName(className);
				if (claxx != null)
				{
					return GetYapClass(claxx, true);
				}
			}
			return null;
		}

		public override bool NeedsLockFileThread()
		{
			return false;
		}

		internal override bool HasShutDownHook()
		{
			return false;
		}

		public override Db4objects.Db4o.Ext.Db4oDatabase Identity()
		{
			if (i_db == null)
			{
				WriteMsg(Db4objects.Db4o.Msg.IDENTITY);
				Db4objects.Db4o.YapReader reader = ExpectedByteResponse(Db4objects.Db4o.Msg.ID_LIST
					);
				ShowInternalClasses(true);
				i_db = (Db4objects.Db4o.Ext.Db4oDatabase)GetByID(reader.ReadInt());
				Activate1(i_systemTrans, i_db, 3);
				ShowInternalClasses(false);
			}
			return i_db;
		}

		public override bool IsClient()
		{
			return true;
		}

		internal virtual void LoginToServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 a_socket)
		{
			if (password != null)
			{
				Db4objects.Db4o.YapStringIOUnicode stringWriter = new Db4objects.Db4o.YapStringIOUnicode
					();
				int length = stringWriter.Length(userName) + stringWriter.Length(password);
				Db4objects.Db4o.MsgD message = Db4objects.Db4o.Msg.LOGIN.GetWriterForLength(i_systemTrans
					, length);
				message.WriteString(userName);
				message.WriteString(password);
				message.Write(this, a_socket);
				Db4objects.Db4o.Msg msg = Db4objects.Db4o.Msg.ReadMessage(i_systemTrans, a_socket
					);
				if (!Db4objects.Db4o.Msg.LOGIN_OK.Equals(msg))
				{
					throw new System.IO.IOException(Db4objects.Db4o.Messages.Get(42));
				}
				Db4objects.Db4o.YapReader payLoad = msg.GetPayLoad();
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
			Db4objects.Db4o.YapReader reader = null;
			if (remainingIDs < 1)
			{
				WriteMsg(Db4objects.Db4o.Msg.PREFETCH_IDS.GetWriterForInt(i_trans, prefetchIDCount
					));
				reader = ExpectedByteResponse(Db4objects.Db4o.Msg.ID_LIST);
				for (int i = prefetchIDCount - 1; i >= 0; i--)
				{
					_prefetchedIDs[i] = reader.ReadInt();
				}
				remainingIDs = prefetchIDCount;
			}
			remainingIDs--;
			return _prefetchedIDs[remainingIDs];
		}

		internal virtual int PrefetchObjects(Db4objects.Db4o.Foundation.IIntIterator4 ids
			, object[] prefetched, int prefetchCount)
		{
			int count = 0;
			int toGet = 0;
			int[] idsToGet = new int[prefetchCount];
			int[] position = new int[prefetchCount];
			while (count < prefetchCount)
			{
				if (!ids.MoveNext())
				{
					break;
				}
				int id = ids.CurrentInt();
				if (id > 0)
				{
					object obj = ObjectForIDFromCache(id);
					if (obj != null)
					{
						prefetched[count] = obj;
					}
					else
					{
						idsToGet[toGet] = id;
						position[toGet] = count;
						toGet++;
					}
					count++;
				}
			}
			if (toGet > 0)
			{
				WriteMsg(Db4objects.Db4o.Msg.READ_MULTIPLE_OBJECTS.GetWriterForIntArray(i_trans, 
					idsToGet, toGet));
				Db4objects.Db4o.MsgD message = (Db4objects.Db4o.MsgD)ExpectedResponse(Db4objects.Db4o.Msg
					.READ_MULTIPLE_OBJECTS);
				int embeddedMessageCount = message.ReadInt();
				for (int i = 0; i < embeddedMessageCount; i++)
				{
					Db4objects.Db4o.MsgObject mso = (Db4objects.Db4o.MsgObject)Db4objects.Db4o.Msg.OBJECT_TO_CLIENT
						.Clone(GetTransaction());
					mso._payLoad = message._payLoad.ReadYapBytes();
					if (mso._payLoad != null)
					{
						mso._payLoad.IncrementOffset(Db4objects.Db4o.YapConst.MESSAGE_LENGTH);
						Db4objects.Db4o.YapWriter reader = mso.Unmarshall(Db4objects.Db4o.YapConst.MESSAGE_LENGTH
							);
						object obj = ObjectForIDFromCache(idsToGet[i]);
						if (obj != null)
						{
							prefetched[position[i]] = obj;
						}
						else
						{
							prefetched[position[i]] = new Db4objects.Db4o.YapObject(idsToGet[i]).ReadPrefetch
								(this, reader);
						}
					}
				}
			}
			return count;
		}

		internal virtual void ProcessBlobMessage(Db4objects.Db4o.MsgBlob msg)
		{
			lock (blobLock)
			{
				bool needStart = blobThread == null || blobThread.IsTerminated();
				if (needStart)
				{
					blobThread = new Db4objects.Db4o.YapClientBlobThread(this);
				}
				blobThread.Add(msg);
				if (needStart)
				{
					blobThread.Start();
				}
			}
		}

		internal void QueryExecute(Db4objects.Db4o.QQuery a_query, Db4objects.Db4o.QueryResultImpl
			 a_res)
		{
			WriteMsg(Db4objects.Db4o.Msg.QUERY_EXECUTE.GetWriter(Marshall(a_query.GetTransaction
				(), a_query)));
			ReadResult(a_res);
		}

		public override void RaiseVersion(long a_minimumVersion)
		{
			WriteMsg(Db4objects.Db4o.Msg.RAISE_VERSION.GetWriterForLong(i_trans, a_minimumVersion
				));
		}

		internal override void ReadBytes(byte[] bytes, int address, int addressOffset, int
			 length)
		{
			throw Db4objects.Db4o.Inside.Exceptions4.VirtualException();
		}

		internal override void ReadBytes(byte[] a_bytes, int a_address, int a_length)
		{
			WriteMsg(Db4objects.Db4o.Msg.READ_BYTES.GetWriterForInts(i_trans, new int[] { a_address
				, a_length }));
			Db4objects.Db4o.YapReader reader = ExpectedByteResponse(Db4objects.Db4o.Msg.READ_BYTES
				);
			System.Array.Copy(reader._buffer, 0, a_bytes, 0, a_length);
		}

		protected override bool Rename1(Db4objects.Db4o.Config4Impl config)
		{
			LogMsg(58, null);
			return false;
		}

		public sealed override Db4objects.Db4o.YapWriter ReadWriterByID(Db4objects.Db4o.Transaction
			 a_ta, int a_id)
		{
			try
			{
				WriteMsg(Db4objects.Db4o.Msg.READ_OBJECT.GetWriterForInt(a_ta, a_id));
				Db4objects.Db4o.YapWriter bytes = ((Db4objects.Db4o.MsgObject)ExpectedResponse(Db4objects.Db4o.Msg
					.OBJECT_TO_CLIENT)).Unmarshall();
				if (bytes == null)
				{
					return null;
				}
				bytes.SetTransaction(a_ta);
				return bytes;
			}
			catch (System.Exception e)
			{
				return null;
			}
		}

		public sealed override Db4objects.Db4o.YapReader ReadReaderByID(Db4objects.Db4o.Transaction
			 a_ta, int a_id)
		{
			return ReadWriterByID(a_ta, a_id);
		}

		private void ReadResult(Db4objects.Db4o.QueryResultImpl aRes)
		{
			Db4objects.Db4o.YapReader reader = ExpectedByteResponse(Db4objects.Db4o.Msg.ID_LIST
				);
			int size = reader.ReadInt();
			for (int i = 0; i < size; i++)
			{
				aRes.Add(reader.ReadInt());
			}
		}

		internal virtual void ReadThis()
		{
			WriteMsg(Db4objects.Db4o.Msg.GET_CLASSES.GetWriter(i_systemTrans));
			Db4objects.Db4o.YapReader bytes = ExpectedByteResponse(Db4objects.Db4o.Msg.GET_CLASSES
				);
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
				WriteMsg(Db4objects.Db4o.Msg.RELEASE_SEMAPHORE.GetWriterForString(i_trans, name));
			}
		}

		internal override void ReleaseSemaphores(Db4objects.Db4o.Transaction ta)
		{
		}

		private void ReReadAll(Db4objects.Db4o.Config.IConfiguration config)
		{
			remainingIDs = 0;
			Initialize0();
			Initialize1(config);
			InitializeTransactions();
			ReadThis();
		}

		internal sealed override void Rollback1()
		{
			WriteMsg(Db4objects.Db4o.Msg.ROLLBACK);
			i_trans.Rollback();
		}

		public override void Send(object obj)
		{
			lock (i_lock)
			{
				if (obj != null)
				{
					WriteMsg(Db4objects.Db4o.Msg.USER_MESSAGE.GetWriter(Marshall(i_trans, obj)));
				}
			}
		}

		public sealed override void SetDirtyInSystemTransaction(Db4objects.Db4o.YapMeta a_object
			)
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
				WriteMsg(Db4objects.Db4o.Msg.SET_SEMAPHORE.GetWriterForIntString(i_trans, timeout
					, name));
				Db4objects.Db4o.Msg message = GetResponse();
				return (message.Equals(Db4objects.Db4o.Msg.SUCCESS));
			}
		}

		public virtual void SwitchToFile(string fileName)
		{
			lock (i_lock)
			{
				Commit();
				WriteMsg(Db4objects.Db4o.Msg.SWITCH_TO_FILE.GetWriterForString(i_trans, fileName)
					);
				ExpectedResponse(Db4objects.Db4o.Msg.OK);
				ReReadAll(Db4objects.Db4o.Db4o.CloneConfiguration());
				switchedToFile = fileName;
			}
		}

		public virtual void SwitchToMainFile()
		{
			lock (i_lock)
			{
				Commit();
				WriteMsg(Db4objects.Db4o.Msg.SWITCH_TO_MAIN_FILE);
				ExpectedResponse(Db4objects.Db4o.Msg.OK);
				ReReadAll(Db4objects.Db4o.Db4o.CloneConfiguration());
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

		internal override void Write(bool shuttingDown)
		{
		}

		internal sealed override void WriteDirty()
		{
		}

		public sealed override void WriteEmbedded(Db4objects.Db4o.YapWriter a_parent, Db4objects.Db4o.YapWriter
			 a_child)
		{
			a_parent.AddEmbedded(a_child);
		}

		internal void WriteMsg(Db4objects.Db4o.Msg a_message)
		{
			a_message.Write(this, i_socket);
		}

		public sealed override void WriteNew(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.YapWriter
			 aWriter)
		{
			WriteMsg(Db4objects.Db4o.Msg.WRITE_NEW.GetWriter(a_yapClass, aWriter));
		}

		internal sealed override void WriteTransactionPointer(int a_address)
		{
		}

		public sealed override void WriteUpdate(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.YapWriter
			 a_bytes)
		{
			WriteMsg(Db4objects.Db4o.Msg.WRITE_UPDATE.GetWriter(a_yapClass, a_bytes));
		}

		public virtual bool IsAlive()
		{
			try
			{
				WriteMsg(Db4objects.Db4o.Msg.PING);
				return ExpectedResponse(Db4objects.Db4o.Msg.OK) != null;
			}
			catch (Db4objects.Db4o.Ext.Db4oException exc)
			{
				return false;
			}
		}

		public virtual Db4objects.Db4o.Foundation.Network.IYapSocket Socket()
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
	}
}
