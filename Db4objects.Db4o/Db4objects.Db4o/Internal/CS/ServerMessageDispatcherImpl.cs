/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	public sealed class ServerMessageDispatcherImpl : Thread, IServerMessageDispatcher
	{
		private string i_clientName;

		private bool i_loggedin;

		private long _lastActiveTime;

		private bool i_sendCloseMessage = true;

		private readonly ObjectServerImpl i_server;

		private ISocket4 i_socket;

		private ClientTransactionHandle _transactionHandle;

		private Hashtable4 _queryResults;

		private Config4Impl i_config;

		internal readonly int i_threadID;

		private CallbackObjectInfoCollections _committedInfo;

		private bool _caresAboutCommitted;

		private bool _isClosed;

		internal ServerMessageDispatcherImpl(ObjectServerImpl server, ClientTransactionHandle
			 transactionHandle, ISocket4 socket, int threadID, bool loggedIn)
		{
			_transactionHandle = transactionHandle;
			SetDaemon(true);
			i_loggedin = loggedIn;
			UpdateLastActiveTime();
			i_server = server;
			i_config = (Config4Impl)i_server.Configure();
			i_threadID = threadID;
			SetDispatcherName(string.Empty + threadID);
			i_socket = socket;
			i_socket.SetSoTimeout(((Config4Impl)server.Configure()).TimeoutServerSocket());
		}

		public bool Close()
		{
			lock (this)
			{
				if (!IsMessageDispatcherAlive())
				{
					return true;
				}
				_transactionHandle.ReleaseTransaction();
				SendCloseMessage();
				_transactionHandle.Close();
				CloseSocket();
				RemoveFromServer();
				_isClosed = true;
				return true;
			}
		}

		public void SendCloseMessage()
		{
			try
			{
				if (i_sendCloseMessage)
				{
					Write(Msg.CLOSE);
				}
				i_sendCloseMessage = false;
			}
			catch (Exception e)
			{
			}
		}

		private void RemoveFromServer()
		{
			try
			{
				i_server.RemoveThread(this);
			}
			catch (Exception e)
			{
			}
		}

		private void CloseSocket()
		{
			try
			{
				if (i_socket != null)
				{
					i_socket.Close();
				}
			}
			catch (IOException e)
			{
			}
		}

		public bool IsMessageDispatcherAlive()
		{
			lock (this)
			{
				return !_isClosed;
			}
		}

		public Transaction GetTransaction()
		{
			return _transactionHandle.Transaction();
		}

		public override void Run()
		{
			while (IsMessageDispatcherAlive())
			{
				try
				{
					if (!MessageProcessor())
					{
						break;
					}
				}
				catch (IOException e)
				{
					if (_transactionHandle.IsClosed())
					{
						break;
					}
					if (i_socket == null || !i_socket.IsConnected())
					{
						break;
					}
				}
			}
			Close();
		}

		private bool MessageProcessor()
		{
			Msg message = Msg.ReadMessage(this, GetTransaction(), i_socket);
			if (message == null)
			{
				return true;
			}
			UpdateLastActiveTime();
			if (!i_loggedin && !Msg.LOGIN.Equals(message))
			{
				return true;
			}
			return ((IServerSideMessage)message).ProcessAtServer();
		}

		private void UpdateLastActiveTime()
		{
			_lastActiveTime = Runtime.CurrentTimeMillis();
		}

		public ObjectServerImpl Server()
		{
			return i_server;
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
			lock (_transactionHandle.Lock())
			{
				string fileName = message.ReadString();
				try
				{
					_transactionHandle.ReleaseTransaction();
					_transactionHandle.AcquireTransactionForFile(fileName);
					Write(Msg.OK);
				}
				catch (Exception e)
				{
					_transactionHandle.ReleaseTransaction();
					Write(Msg.ERROR);
				}
			}
		}

		public void SwitchToMainFile()
		{
			lock (_transactionHandle.Lock())
			{
				_transactionHandle.ReleaseTransaction();
				Write(Msg.OK);
			}
		}

		public void UseTransaction(MUseTransaction message)
		{
			int threadID = message.ReadInt();
			Transaction transToUse = i_server.FindTransaction(threadID);
			_transactionHandle.Transaction(transToUse);
		}

		public void Write(Msg msg)
		{
			lock (this)
			{
				_transactionHandle.Write(msg, i_socket);
				UpdateLastActiveTime();
			}
		}

		public void WriteIfAlive(Msg msg)
		{
			lock (this)
			{
				if (IsMessageDispatcherAlive())
				{
					Write(msg);
				}
			}
		}

		public ISocket4 Socket()
		{
			return i_socket;
		}

		public string Name()
		{
			return i_clientName;
		}

		public void SetDispatcherName(string name)
		{
			i_clientName = name;
			SetName("db4o server message dispatcher " + name);
		}

		public int DispatcherID()
		{
			return i_threadID;
		}

		public void Login()
		{
			i_loggedin = true;
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

		public void CommittedInfo(CallbackObjectInfoCollections committedInfo)
		{
			_committedInfo = committedInfo;
		}

		public bool IsPingTimeout()
		{
			return (Runtime.CurrentTimeMillis() - _lastActiveTime > i_config.PingInterval());
		}
	}
}
