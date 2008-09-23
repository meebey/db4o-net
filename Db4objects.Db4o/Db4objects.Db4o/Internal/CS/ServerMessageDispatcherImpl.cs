/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	public sealed class ServerMessageDispatcherImpl : Thread, IServerMessageDispatcher
	{
		private string _clientName;

		private bool _loggedin;

		private bool _closeMessageSent;

		private readonly ObjectServerImpl _server;

		private ISocket4 _socket;

		private ClientTransactionHandle _transactionHandle;

		private Hashtable4 _queryResults;

		internal readonly int _threadID;

		private CallbackObjectInfoCollections _committedInfo;

		private bool _caresAboutCommitted;

		private bool _isClosed;

		private readonly object _lock = new object();

		private readonly object _mainLock;

		/// <exception cref="Exception"></exception>
		internal ServerMessageDispatcherImpl(ObjectServerImpl server, ClientTransactionHandle
			 transactionHandle, ISocket4 socket, int threadID, bool loggedIn, object mainLock
			)
		{
			_mainLock = mainLock;
			_transactionHandle = transactionHandle;
			SetDaemon(true);
			_loggedin = loggedIn;
			_server = server;
			_threadID = threadID;
			SetDispatcherName(string.Empty + threadID);
			_socket = socket;
			_socket.SetSoTimeout(((Config4Impl)server.Configure()).TimeoutServerSocket());
		}

		// TODO: Experiment with packetsize and noDelay
		// i_socket.setSendBufferSize(100);
		// i_socket.setTcpNoDelay(true);
		public bool Close()
		{
			lock (_lock)
			{
				if (!IsMessageDispatcherAlive())
				{
					return true;
				}
				_isClosed = true;
			}
			lock (_mainLock)
			{
				_transactionHandle.ReleaseTransaction();
				SendCloseMessage();
				_transactionHandle.Close();
				CloseSocket();
				RemoveFromServer();
				return true;
			}
		}

		public void CloseConnection()
		{
			lock (_lock)
			{
				if (!IsMessageDispatcherAlive())
				{
					return;
				}
				_isClosed = true;
			}
			lock (_mainLock)
			{
				CloseSocket();
				RemoveFromServer();
			}
		}

		public bool IsMessageDispatcherAlive()
		{
			lock (_lock)
			{
				return !_isClosed;
			}
		}

		private void SendCloseMessage()
		{
			try
			{
				if (!_closeMessageSent)
				{
					_closeMessageSent = true;
					Write(Msg.Close);
				}
			}
			catch (Exception e)
			{
			}
		}

		private void RemoveFromServer()
		{
			try
			{
				_server.RemoveThread(this);
			}
			catch (Exception e)
			{
			}
		}

		private void CloseSocket()
		{
			try
			{
				if (_socket != null)
				{
					_socket.Close();
				}
			}
			catch (Db4oIOException e)
			{
			}
		}

		public Transaction GetTransaction()
		{
			return _transactionHandle.Transaction();
		}

		public override void Run()
		{
			try
			{
				MessageLoop();
			}
			finally
			{
				Close();
			}
		}

		private void MessageLoop()
		{
			while (IsMessageDispatcherAlive())
			{
				try
				{
					if (!MessageProcessor())
					{
						return;
					}
				}
				catch (Db4oIOException e)
				{
					if (DTrace.enabled)
					{
						DTrace.AddToClassIndex.Log(e.ToString());
					}
					return;
				}
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		private bool MessageProcessor()
		{
			Msg message = Msg.ReadMessage(this, GetTransaction(), _socket);
			if (message == null)
			{
				return true;
			}
			if (!_loggedin && !Msg.Login.Equals(message))
			{
				return true;
			}
			// TODO: COR-885 - message may process against closed server
			// Checking aliveness just makes the issue less likely to occur. Naive synchronization against main lock is prohibitive.        
			if (IsMessageDispatcherAlive())
			{
				return ((IServerSideMessage)message).ProcessAtServer();
			}
			return false;
		}

		public ObjectServerImpl Server()
		{
			return _server;
		}

		public void QueryResultFinalized(int queryResultID)
		{
			_queryResults.Remove(queryResultID);
		}

		public void MapQueryResultToID(LazyClientObjectSetStub stub, int queryResultID)
		{
			if (_queryResults == null)
			{
				_queryResults = new Hashtable4();
			}
			_queryResults.Put(queryResultID, stub);
		}

		public LazyClientObjectSetStub QueryResultForID(int queryResultID)
		{
			return (LazyClientObjectSetStub)_queryResults.Get(queryResultID);
		}

		public void SwitchToFile(MSwitchToFile message)
		{
			lock (_mainLock)
			{
				string fileName = message.ReadString();
				try
				{
					_transactionHandle.ReleaseTransaction();
					_transactionHandle.AcquireTransactionForFile(fileName);
					Write(Msg.Ok);
				}
				catch (Exception e)
				{
					_transactionHandle.ReleaseTransaction();
					Write(Msg.Error);
				}
			}
		}

		public void SwitchToMainFile()
		{
			lock (_mainLock)
			{
				_transactionHandle.ReleaseTransaction();
				Write(Msg.Ok);
			}
		}

		public void UseTransaction(MUseTransaction message)
		{
			int threadID = message.ReadInt();
			Transaction transToUse = _server.FindTransaction(threadID);
			_transactionHandle.Transaction(transToUse);
		}

		public bool Write(Msg msg)
		{
			lock (_lock)
			{
				if (!IsMessageDispatcherAlive())
				{
					return false;
				}
				return msg.Write(_socket);
			}
		}

		public ISocket4 Socket()
		{
			return _socket;
		}

		public string Name()
		{
			return _clientName;
		}

		public void SetDispatcherName(string name)
		{
			_clientName = name;
			// set thread name
			SetName("db4o server message dispatcher " + name);
		}

		public int DispatcherID()
		{
			return _threadID;
		}

		public void Login()
		{
			_loggedin = true;
		}

		public void StartDispatcher()
		{
			Start();
		}

		public bool CaresAboutCommitted()
		{
			return _caresAboutCommitted;
		}

		public void CaresAboutCommitted(bool care)
		{
			_caresAboutCommitted = true;
			Server().CheckCaresAboutCommitted();
		}

		public CallbackObjectInfoCollections CommittedInfo()
		{
			return _committedInfo;
		}

		public void DispatchCommitted(CallbackObjectInfoCollections committedInfo)
		{
			_committedInfo = committedInfo;
		}

		public bool WillDispatchCommitted()
		{
			return Server().CaresAboutCommitted();
		}

		public Db4objects.Db4o.Internal.CS.ClassInfoHelper ClassInfoHelper()
		{
			return Server().ClassInfoHelper();
		}
	}
}
