/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Reflect;
using Sharpen;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class ClientObjectContainer : ExternalObjectContainer, IExtClient, IBlobTransport
		, IClientMessageDispatcher
	{
		internal readonly object blobLock = new object();

		private BlobProcessor blobThread;

		private ISocket4 i_socket;

		private BlockingQueue _synchronousMessageQueue = new BlockingQueue();

		private BlockingQueue _asynchronousMessageQueue = new BlockingQueue();

		private readonly string _password;

		internal int[] _prefetchedIDs;

		internal IClientMessageDispatcher _messageDispatcher;

		internal ClientAsynchronousMessageProcessor _asynchronousMessageProcessor;

		internal int remainingIDs;

		private string switchedToFile;

		private bool _singleThreaded;

		private readonly string _userName;

		private Db4oDatabase i_db;

		protected bool _doFinalize = true;

		private int _blockSize = 1;

		private Collection4 _batchedMessages = new Collection4();

		private int _batchedQueueLength = Const4.IntLength;

		private bool _login;

		private readonly ClientHeartbeat _heartbeat;

		public ClientObjectContainer(IConfiguration config, ISocket4 socket, string user, 
			string password, bool login) : base(config, null)
		{
			// null denotes password not necessary
			// initial value of _batchedQueueLength is YapConst.INT_LENGTH, which is
			// used for to write the number of messages.
			_userName = user;
			_password = password;
			_login = login;
			_heartbeat = new ClientHeartbeat(this);
			SetAndConfigSocket(socket);
			Open();
		}

		private void SetAndConfigSocket(ISocket4 socket)
		{
			i_socket = socket;
			i_socket.SetSoTimeout(_config.TimeoutClientSocket());
		}

		protected sealed override void OpenImpl()
		{
			_singleThreaded = ConfigImpl().SingleThreadedClient();
			// TODO: Experiment with packetsize and noDelay
			// socket.setSendBufferSize(100);
			// socket.setTcpNoDelay(true);
			// System.out.println(socket.getSendBufferSize());
			if (_login)
			{
				LoginToServer(i_socket);
			}
			if (!_singleThreaded)
			{
				StartDispatcherThread(i_socket, _userName);
			}
			LogMsg(36, ToString());
			StartHeartBeat();
			ReadThis();
		}

		private void StartHeartBeat()
		{
			_heartbeat.Start();
		}

		private void StartDispatcherThread(ISocket4 socket, string user)
		{
			if (!_singleThreaded)
			{
				_asynchronousMessageProcessor = new ClientAsynchronousMessageProcessor(_asynchronousMessageQueue
					);
				_asynchronousMessageProcessor.StartProcessing();
			}
			_messageDispatcher = new ClientMessageDispatcherImpl(this, socket, _synchronousMessageQueue
				, _asynchronousMessageQueue);
			_messageDispatcher.SetDispatcherName(user);
			_messageDispatcher.StartDispatcher();
		}

		/// <exception cref="NotSupportedException"></exception>
		public override void Backup(string path)
		{
			throw new NotSupportedException();
		}

		public override void Reserve(int byteCount)
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
			if ((!_singleThreaded) && (_messageDispatcher == null || !_messageDispatcher.IsMessageDispatcherAlive
				()))
			{
				StopHeartBeat();
				ShutdownObjectContainer();
				return;
			}
			try
			{
				Commit1(_transaction);
			}
			catch (Exception e)
			{
				Exceptions4.CatchAllExceptDb4oException(e);
			}
			try
			{
				Write(Msg.Close);
			}
			catch (Exception e)
			{
				Exceptions4.CatchAllExceptDb4oException(e);
			}
			ShutDownCommunicationRessources();
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

		private void StopHeartBeat()
		{
			_heartbeat.Stop();
		}

		private void CloseMessageDispatcher()
		{
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
				if (!_singleThreaded)
				{
					_asynchronousMessageProcessor.StopProcessing();
				}
			}
			catch (Exception e)
			{
				Exceptions4.CatchAllExceptDb4oException(e);
			}
		}

		public sealed override void Commit1(Transaction trans)
		{
			trans.Commit();
		}

		public override int ConverterVersion()
		{
			return Converter.Version;
		}

		/// <exception cref="IOException"></exception>
		internal virtual ISocket4 CreateParalellSocket()
		{
			Write(Msg.GetThreadId);
			int serverThreadID = ExpectedByteResponse(Msg.IdList).ReadInt();
			ISocket4 sock = i_socket.OpenParalellSocket();
			LoginToServer(sock);
			if (switchedToFile != null)
			{
				MsgD message = Msg.SwitchToFile.GetWriterForString(SystemTransaction(), switchedToFile
					);
				message.Write(sock);
				if (!(Msg.Ok.Equals(Msg.ReadMessage(this, SystemTransaction(), sock))))
				{
					throw new IOException(Db4objects.Db4o.Internal.Messages.Get(42));
				}
			}
			Msg.UseTransaction.GetWriterForInt(_transaction, serverThreadID).Write(sock);
			return sock;
		}

		public override AbstractQueryResult NewQueryResult(Transaction trans, QueryEvaluationMode
			 mode)
		{
			throw new InvalidOperationException();
		}

		public sealed override Transaction NewTransaction(Transaction parentTransaction, 
			TransactionalReferenceSystem referenceSystem)
		{
			return new ClientTransaction(this, parentTransaction, referenceSystem);
		}

		public override bool CreateClassMetadata(ClassMetadata clazz, IReflectClass claxx
			, ClassMetadata superClazz)
		{
			Write(Msg.CreateClass.GetWriterForString(SystemTransaction(), Config().ResolveAliasRuntimeName
				(claxx.GetName())));
			Msg resp = GetResponse();
			if (resp == null)
			{
				return false;
			}
			if (resp.Equals(Msg.Failed))
			{
				// if the class can not be created on the server, send class meta to the server.
				SendClassMeta(claxx);
				resp = GetResponse();
			}
			if (resp.Equals(Msg.Failed))
			{
				if (ConfigImpl().ExceptionsOnNotStorable())
				{
					throw new ObjectNotStorableException(claxx);
				}
				return false;
			}
			if (!resp.Equals(Msg.ObjectToClient))
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
			if (!base.CreateClassMetadata(clazz, claxx, superClazz))
			{
				return false;
			}
			clazz.SetID(message.GetId());
			clazz.ReadName1(SystemTransaction(), bytes);
			ClassCollection().AddClassMetadata(clazz);
			ClassCollection().ReadClassMetadata(clazz, claxx);
			return true;
		}

		private void SendClassMeta(IReflectClass reflectClass)
		{
			ClassInfo classMeta = _classMetaHelper.GetClassMeta(reflectClass);
			Write(Msg.ClassMeta.GetWriter(Serializer.Marshall(SystemTransaction(), classMeta)
				));
		}

		public override long CurrentVersion()
		{
			Write(Msg.CurrentVersion);
			return ((MsgD)ExpectedResponse(Msg.IdList)).ReadLong();
		}

		public sealed override bool Delete4(Transaction ta, ObjectReference yo, int a_cascade
			, bool userCall)
		{
			MsgD msg = Msg.Delete.GetWriterForInts(_transaction, new int[] { yo.GetID(), userCall
				 ? 1 : 0 });
			WriteBatchedMessage(msg);
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

		internal ByteArrayBuffer ExpectedByteResponse(Msg expectedMessage)
		{
			Msg msg = ExpectedResponse(expectedMessage);
			if (msg == null)
			{
				// TODO: throw Exception to allow
				// smooth shutdown
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
			CheckExceptionMessage(message);
			throw new InvalidOperationException("Unexpected Message:" + message + "  Expected:"
				 + expectedMessage);
		}

		private void CheckExceptionMessage(Msg msg)
		{
			if (msg is MRuntimeException)
			{
				((MRuntimeException)msg).ThrowPayload();
			}
		}

		public override AbstractQueryResult QueryAllObjects(Transaction trans)
		{
			int mode = Config().QueryEvaluationMode().AsInt();
			MsgD msg = Msg.GetAll.GetWriterForInt(trans, mode);
			Write(msg);
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
			while (true)
			{
				Msg msg = _singleThreaded ? GetResponseSingleThreaded() : GetResponseMultiThreaded
					();
				if (IsClientSideMessage(msg))
				{
					if (((IClientSideMessage)msg).ProcessAtClient())
					{
						continue;
					}
				}
				return msg;
			}
		}

		private Msg GetResponseSingleThreaded()
		{
			while (IsMessageDispatcherAlive())
			{
				try
				{
					Msg message = Msg.ReadMessage(this, _transaction, i_socket);
					if (IsClientSideMessage(message))
					{
						if (((IClientSideMessage)message).ProcessAtClient())
						{
							continue;
						}
					}
					return message;
				}
				catch (Db4oIOException)
				{
					OnMsgError();
				}
			}
			return null;
		}

		private Msg GetResponseMultiThreaded()
		{
			Msg msg;
			try
			{
				msg = (Msg)_synchronousMessageQueue.Next();
			}
			catch (BlockingQueueStoppedException e)
			{
				if (DTrace.enabled)
				{
					DTrace.BlockingQueueStoppedException.Log(e.ToString());
				}
				msg = Msg.Error;
			}
			if (msg is MError)
			{
				OnMsgError();
			}
			return msg;
		}

		private bool IsClientSideMessage(Msg message)
		{
			return message is IClientSideMessage;
		}

		private void OnMsgError()
		{
			Close();
			throw new DatabaseClosedException();
		}

		public virtual bool IsMessageDispatcherAlive()
		{
			return i_socket != null;
		}

		public override ClassMetadata ClassMetadataForId(int clazzId)
		{
			if (clazzId == 0)
			{
				return null;
			}
			ClassMetadata yc = base.ClassMetadataForId(clazzId);
			if (yc != null)
			{
				return yc;
			}
			MsgD msg = Msg.ClassNameForId.GetWriterForInt(SystemTransaction(), clazzId);
			Write(msg);
			MsgD message = (MsgD)ExpectedResponse(Msg.ClassNameForId);
			string className = Config().ResolveAliasStoredName(message.ReadString());
			if (className != null && className.Length > 0)
			{
				IReflectClass claxx = Reflector().ForName(className);
				if (claxx != null)
				{
					return ProduceClassMetadata(claxx);
				}
			}
			// TODO inform client class not present
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
				Write(Msg.Identity);
				ByteArrayBuffer reader = ExpectedByteResponse(Msg.IdList);
				ShowInternalClasses(true);
				try
				{
					i_db = (Db4oDatabase)GetByID(reader.ReadInt());
					Activate(SystemTransaction(), i_db, new FixedActivationDepth(3));
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

		/// <exception cref="InvalidPasswordException"></exception>
		private void LoginToServer(ISocket4 socket)
		{
			UnicodeStringIO stringWriter = new UnicodeStringIO();
			int length = stringWriter.Length(_userName) + stringWriter.Length(_password);
			MsgD message = Msg.Login.GetWriterForLength(SystemTransaction(), length);
			message.WriteString(_userName);
			message.WriteString(_password);
			message.Write(socket);
			Msg msg = ReadLoginMessage(socket);
			ByteArrayBuffer payLoad = msg.PayLoad();
			_blockSize = payLoad.ReadInt();
			int doEncrypt = payLoad.ReadInt();
			if (doEncrypt == 0)
			{
				_handlers.OldEncryptionOff();
			}
		}

		private Msg ReadLoginMessage(ISocket4 socket)
		{
			Msg msg = Msg.ReadMessage(this, SystemTransaction(), socket);
			while (Msg.Pong.Equals(msg))
			{
				msg = Msg.ReadMessage(this, SystemTransaction(), socket);
			}
			if (!Msg.LoginOk.Equals(msg))
			{
				throw new InvalidPasswordException();
			}
			return msg;
		}

		public override bool MaintainsIndices()
		{
			return false;
		}

		public sealed override int NewUserObject()
		{
			int prefetchIDCount = Config().PrefetchIDCount();
			EnsureIDCacheAllocated(prefetchIDCount);
			ByteArrayBuffer reader = null;
			if (remainingIDs < 1)
			{
				MsgD msg = Msg.PrefetchIds.GetWriterForInt(_transaction, prefetchIDCount);
				Write(msg);
				reader = ExpectedByteResponse(Msg.IdList);
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
			Write(Msg.RaiseVersion.GetWriterForLong(_transaction, a_minimumVersion));
		}

		public override void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length)
		{
			throw Exceptions4.VirtualException();
		}

		public override void ReadBytes(byte[] a_bytes, int a_address, int a_length)
		{
			MsgD msg = Msg.ReadBytes.GetWriterForInts(_transaction, new int[] { a_address, a_length
				 });
			Write(msg);
			ByteArrayBuffer reader = ExpectedByteResponse(Msg.ReadBytes);
			System.Array.Copy(reader._buffer, 0, a_bytes, 0, a_length);
		}

		protected override bool Rename1(Config4Impl config)
		{
			LogMsg(58, null);
			return false;
		}

		public sealed override StatefulBuffer ReadWriterByID(Transaction a_ta, int a_id)
		{
			return ReadWriterByID(a_ta, a_id, false);
		}

		public sealed override StatefulBuffer ReadWriterByID(Transaction a_ta, int a_id, 
			bool lastCommitted)
		{
			MsgD msg = Msg.ReadObject.GetWriterForInts(a_ta, new int[] { a_id, lastCommitted ? 
				1 : 0 });
			Write(msg);
			StatefulBuffer bytes = ((MsgObject)ExpectedResponse(Msg.ObjectToClient)).Unmarshall
				();
			if (bytes != null)
			{
				bytes.SetTransaction(a_ta);
			}
			return bytes;
		}

		public sealed override StatefulBuffer[] ReadWritersByIDs(Transaction a_ta, int[] 
			ids)
		{
			MsgD msg = Msg.ReadMultipleObjects.GetWriterForIntArray(a_ta, ids, ids.Length);
			Write(msg);
			MsgD response = (MsgD)ExpectedResponse(Msg.ReadMultipleObjects);
			int count = response.ReadInt();
			StatefulBuffer[] yapWriters = new StatefulBuffer[count];
			for (int i = 0; i < count; i++)
			{
				MsgObject mso = (MsgObject)Msg.ObjectToClient.PublicClone();
				mso.SetTransaction(a_ta);
				mso.PayLoad(response.PayLoad().ReadYapBytes());
				if (mso.PayLoad() != null)
				{
					mso.PayLoad().IncrementOffset(Const4.MessageLength);
					yapWriters[i] = mso.Unmarshall(Const4.MessageLength);
					yapWriters[i].SetTransaction(a_ta);
				}
			}
			return yapWriters;
		}

		public sealed override ByteArrayBuffer ReadReaderByID(Transaction a_ta, int a_id, 
			bool lastCommitted)
		{
			// TODO: read lightweight reader instead
			return ReadWriterByID(a_ta, a_id, lastCommitted);
		}

		public sealed override ByteArrayBuffer ReadReaderByID(Transaction a_ta, int a_id)
		{
			return ReadReaderByID(a_ta, a_id, false);
		}

		private AbstractQueryResult ReadQueryResult(Transaction trans)
		{
			AbstractQueryResult queryResult = null;
			ByteArrayBuffer reader = ExpectedByteResponse(Msg.QueryResult);
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
			Write(Msg.GetClasses.GetWriter(SystemTransaction()));
			ByteArrayBuffer bytes = ExpectedByteResponse(Msg.GetClasses);
			ClassCollection().SetID(bytes.ReadInt());
			CreateStringIO(bytes.ReadByte());
			ClassCollection().Read(SystemTransaction());
			ClassCollection().RefreshClasses();
		}

		public override void ReleaseSemaphore(string name)
		{
			lock (_lock)
			{
				CheckClosed();
				if (name == null)
				{
					throw new ArgumentNullException();
				}
				Write(Msg.ReleaseSemaphore.GetWriterForString(_transaction, name));
			}
		}

		public override void ReleaseSemaphores(Transaction ta)
		{
		}

		// do nothing
		private void ReReadAll(IConfiguration config)
		{
			remainingIDs = 0;
			Initialize1(config);
			InitializeTransactions();
			ReadThis();
		}

		public sealed override void Rollback1(Transaction trans)
		{
			if (_config.BatchMessages())
			{
				ClearBatchedObjects();
			}
			Write(Msg.Rollback);
			trans.Rollback();
		}

		public override void Send(object obj)
		{
			lock (_lock)
			{
				if (obj != null)
				{
					MUserMessage message = Msg.UserMessage;
					Write(message.MarshallUserMessage(_transaction, obj));
				}
			}
		}

		public sealed override void SetDirtyInSystemTransaction(PersistentBase a_object)
		{
		}

		// do nothing
		public override bool SetSemaphore(string name, int timeout)
		{
			lock (_lock)
			{
				CheckClosed();
				if (name == null)
				{
					throw new ArgumentNullException();
				}
				MsgD msg = Msg.SetSemaphore.GetWriterForIntString(_transaction, timeout, name);
				Write(msg);
				Msg message = GetResponse();
				return (message.Equals(Msg.Success));
			}
		}

		[System.ObsoleteAttribute]
		public virtual void SwitchToFile(string fileName)
		{
			lock (_lock)
			{
				Commit();
				MsgD msg = Msg.SwitchToFile.GetWriterForString(_transaction, fileName);
				Write(msg);
				ExpectedResponse(Msg.Ok);
				// FIXME NSC
				ReReadAll(Db4oFactory.CloneConfiguration());
				switchedToFile = fileName;
			}
		}

		[System.ObsoleteAttribute]
		public virtual void SwitchToMainFile()
		{
			lock (_lock)
			{
				Commit();
				Write(Msg.SwitchToMainFile);
				ExpectedResponse(Msg.Ok);
				// FIXME NSC
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
			// if(i_classCollection != null){
			// return i_classCollection.toString();
			// }
			return "Client Connection " + _userName;
		}

		public override void Shutdown()
		{
		}

		// do nothing
		public sealed override void WriteDirty()
		{
		}

		// do nothing
		public bool Write(Msg msg)
		{
			WriteMsg(msg, true);
			return true;
		}

		public void WriteBatchedMessage(Msg msg)
		{
			WriteMsg(msg, false);
		}

		private void WriteMsg(Msg msg, bool flush)
		{
			if (_config.BatchMessages())
			{
				if (flush && _batchedMessages.IsEmpty())
				{
					// if there's nothing batched, just send this message directly
					WriteMessageToSocket(msg);
				}
				else
				{
					AddToBatch(msg);
					if (flush || _batchedQueueLength > _config.MaxBatchQueueSize())
					{
						WriteBatchedMessages();
					}
				}
			}
			else
			{
				WriteMessageToSocket(msg);
			}
		}

		public virtual bool WriteMessageToSocket(Msg msg)
		{
			return msg.Write(i_socket);
		}

		public sealed override void WriteNew(Transaction trans, Pointer4 pointer, ClassMetadata
			 classMetadata, ByteArrayBuffer buffer)
		{
			MsgD msg = Msg.WriteNew.GetWriter(trans, pointer, classMetadata, buffer);
			WriteBatchedMessage(msg);
		}

		public sealed override void WriteTransactionPointer(int a_address)
		{
		}

		// do nothing
		public sealed override void WriteUpdate(Transaction trans, Pointer4 pointer, ClassMetadata
			 classMetadata, ByteArrayBuffer buffer)
		{
			MsgD msg = Msg.WriteUpdate.GetWriter(trans, pointer, classMetadata, buffer);
			WriteBatchedMessage(msg);
		}

		public virtual bool IsAlive()
		{
			try
			{
				Write(Msg.IsAlive);
				return ExpectedResponse(Msg.IsAlive) != null;
			}
			catch (Db4oException)
			{
				return false;
			}
		}

		// Remove, for testing purposes only
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

		/// <exception cref="IOException"></exception>
		public virtual void WriteBlobTo(Transaction trans, BlobImpl blob, Sharpen.IO.File
			 file)
		{
			MsgBlob msg = (MsgBlob)Msg.ReadBlob.GetWriterForInt(trans, (int)GetID(blob));
			msg._blob = blob;
			ProcessBlobMessage(msg);
		}

		/// <exception cref="IOException"></exception>
		public virtual void ReadBlobFrom(Transaction trans, BlobImpl blob, Sharpen.IO.File
			 file)
		{
			MsgBlob msg = null;
			lock (Lock())
			{
				Store(blob);
				int id = (int)GetID(blob);
				msg = (MsgBlob)Msg.WriteBlob.GetWriterForInt(trans, id);
				msg._blob = blob;
				blob.SetStatus(Status.Queued);
			}
			ProcessBlobMessage(msg);
		}

		public virtual void DeleteBlobFile(Transaction trans, BlobImpl blob)
		{
			MDeleteBlobFile msg = (MDeleteBlobFile)Msg.DeleteBlobFile.GetWriterForInt(trans, 
				(int)GetID(blob));
			WriteMsg(msg, false);
		}

		public override long[] GetIDsForClass(Transaction trans, ClassMetadata clazz)
		{
			MsgD msg = Msg.GetInternalIds.GetWriterForInt(trans, clazz.GetID());
			Write(msg);
			ByteArrayBuffer reader = ExpectedByteResponse(Msg.IdList);
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
			MsgD msg = Msg.QueryExecute.GetWriter(Serializer.Marshall(trans, query));
			Write(msg);
			return ReadQueryResult(trans);
		}

		public void WriteBatchedMessages()
		{
			lock (Lock())
			{
				if (_batchedMessages.IsEmpty())
				{
					return;
				}
				Msg msg;
				MsgD multibytes = Msg.WriteBatchedMessages.GetWriterForLength(Transaction(), _batchedQueueLength
					);
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
						multibytes.WriteInt(msg.PayLoad().Length());
						multibytes.PayLoad().Append(msg.PayLoad()._buffer);
					}
				}
				WriteMessageToSocket(multibytes);
				ClearBatchedObjects();
			}
		}

		public void AddToBatch(Msg msg)
		{
			lock (Lock())
			{
				_batchedMessages.Add(msg);
				// the first INT_LENGTH is for buffer.length, and then buffer content.
				_batchedQueueLength += Const4.IntLength + msg.PayLoad().Length();
			}
		}

		private void ClearBatchedObjects()
		{
			_batchedMessages.Clear();
			// initial value of _batchedQueueLength is YapConst.INT_LENGTH, which is
			// used for to write the number of messages.
			_batchedQueueLength = Const4.IntLength;
		}

		internal virtual int Timeout()
		{
			return ConfigImpl().TimeoutClientSocket();
		}

		protected override void ShutdownDataStorage()
		{
			ShutDownCommunicationRessources();
		}

		private void ShutDownCommunicationRessources()
		{
			StopHeartBeat();
			CloseMessageDispatcher();
			_synchronousMessageQueue.Stop();
			_asynchronousMessageQueue.Stop();
		}

		public virtual void SetDispatcherName(string name)
		{
		}

		// do nothing here		
		public virtual void StartDispatcher()
		{
		}

		// do nothing here for single thread, ClientObjectContainer is already running
		public virtual IClientMessageDispatcher MessageDispatcher()
		{
			return _singleThreaded ? this : _messageDispatcher;
		}

		public override void OnCommittedListener()
		{
			if (_singleThreaded)
			{
				return;
			}
			Write(Msg.CommittedCallbackRegister);
		}

		public override int ClassMetadataIdForName(string name)
		{
			MsgD msg = Msg.ClassMetadataIdForName.GetWriterForString(SystemTransaction(), name
				);
			msg.Write(i_socket);
			MsgD response = (MsgD)ExpectedResponse(Msg.ClassId);
			return response.ReadInt();
		}
	}
}
