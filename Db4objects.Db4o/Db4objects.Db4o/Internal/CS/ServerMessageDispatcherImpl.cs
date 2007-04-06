using System;
using Db4objects.Db4o;
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

		private long _lastClientMessageTime;

		private readonly LocalObjectContainer i_mainStream;

		private Transaction i_mainTrans;

		private int i_pingAttempts = 0;

		private bool i_rollbackOnClose = true;

		private bool i_sendCloseMessage = true;

		private readonly ObjectServerImpl i_server;

		private ISocket4 i_socket;

		private LocalObjectContainer i_substituteStream;

		private Transaction i_substituteTrans;

		private Hashtable4 _queryResults;

		private Config4Impl i_config;

		internal readonly int i_threadID;

		internal ServerMessageDispatcherImpl(ObjectServerImpl aServer, LocalObjectContainer
			 aStream, ISocket4 aSocket, int aThreadID, bool loggedIn)
		{
			SetDaemon(true);
			i_loggedin = loggedIn;
			_lastClientMessageTime = Runtime.CurrentTimeMillis();
			i_server = aServer;
			i_config = (Config4Impl)i_server.Configure();
			i_mainStream = aStream;
			i_threadID = aThreadID;
			SetDispatcherName("db4o message server " + aThreadID);
			i_mainTrans = aStream.NewTransaction();
			try
			{
				i_socket = aSocket;
				i_socket.SetSoTimeout(((Config4Impl)aServer.Configure()).TimeoutServerSocket());
			}
			catch (Exception e)
			{
				i_socket.Close();
				throw (e);
			}
		}

		public bool Close()
		{
			lock (this)
			{
				if (!IsMessageDispatcherAlive())
				{
					return true;
				}
				CloseSubstituteStream();
				SendCloseMessage();
				RollbackMainTransaction();
				CloseSocket();
				RemoveFromServer();
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

		private void RollbackMainTransaction()
		{
			if (i_mainStream != null && i_mainTrans != null)
			{
				i_mainTrans.Close(i_rollbackOnClose);
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
				i_socket.Close();
			}
			catch (Exception e)
			{
			}
			i_socket = null;
		}

		public bool IsMessageDispatcherAlive()
		{
			return i_socket != null;
		}

		private void CloseSubstituteStream()
		{
			if (i_substituteStream != null)
			{
				if (i_substituteTrans != null)
				{
					i_substituteTrans.Close(i_rollbackOnClose);
					i_substituteTrans = null;
				}
				try
				{
					i_substituteStream.Close();
				}
				catch (Exception e)
				{
				}
				i_substituteStream = null;
			}
		}

		private LocalObjectContainer GetStream()
		{
			if (i_substituteStream != null)
			{
				return i_substituteStream;
			}
			return i_mainStream;
		}

		public Transaction GetTransaction()
		{
			if (i_substituteTrans != null)
			{
				return i_substituteTrans;
			}
			return i_mainTrans;
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
				catch (Exception e)
				{
					if (i_mainStream == null || i_mainStream.IsClosed())
					{
						break;
					}
					if (!i_socket.IsConnected())
					{
						break;
					}
				}
				if (PingClientTimeoutReached())
				{
					if (i_pingAttempts > 5)
					{
						GetStream().LogMsg(33, i_clientName);
						break;
					}
					if (IsMessageDispatcherAlive())
					{
						Write(Msg.PING);
						i_pingAttempts++;
					}
				}
			}
			Close();
		}

		private bool PingClientTimeoutReached()
		{
			return (Runtime.CurrentTimeMillis() - _lastClientMessageTime > i_config.TimeoutPingClients
				());
		}

		private bool MessageProcessor()
		{
			Msg message = Msg.ReadMessage(this, GetTransaction(), i_socket);
			if (message == null)
			{
				return true;
			}
			_lastClientMessageTime = Runtime.CurrentTimeMillis();
			i_pingAttempts = 0;
			if (!i_loggedin && !Msg.LOGIN.Equals(message))
			{
				return true;
			}
			return ((IServerSideMessage)message).ProcessAtServer();
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
			lock (i_mainStream.i_lock)
			{
				string fileName = message.ReadString();
				try
				{
					CloseSubstituteStream();
					i_substituteStream = (LocalObjectContainer)Db4oFactory.OpenFile(fileName);
					i_substituteTrans = i_substituteStream.NewTransaction();
					i_substituteStream.ConfigImpl().SetMessageRecipient(i_mainStream.ConfigImpl().MessageRecipient
						());
					Write(Msg.OK);
				}
				catch (Exception e)
				{
					CloseSubstituteStream();
					Write(Msg.ERROR);
				}
			}
		}

		public void SwitchToMainFile()
		{
			lock (i_mainStream.i_lock)
			{
				CloseSubstituteStream();
				Write(Msg.OK);
			}
		}

		public void UseTransaction(MUseTransaction message)
		{
			int threadID = message.ReadInt();
			Db4objects.Db4o.Internal.CS.ServerMessageDispatcherImpl transactionThread = i_server
				.FindThread(threadID);
			if (transactionThread != null)
			{
				Transaction transToUse = transactionThread.GetTransaction();
				if (i_substituteTrans != null)
				{
					i_substituteTrans = transToUse;
				}
				else
				{
					i_mainTrans = transToUse;
				}
				i_rollbackOnClose = false;
			}
		}

		public void Write(Msg msg)
		{
			msg.Write(GetStream(), i_socket);
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
			SetName("db4o server socket for client " + name);
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
	}
}
