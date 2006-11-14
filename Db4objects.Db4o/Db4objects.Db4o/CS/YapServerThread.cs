namespace Db4objects.Db4o.CS
{
	public sealed class YapServerThread : Sharpen.Lang.Thread
	{
		private string i_clientName;

		private bool i_loggedin;

		private long i_lastClientMessage;

		private readonly Db4objects.Db4o.YapFile i_mainStream;

		private Db4objects.Db4o.Transaction i_mainTrans;

		private int i_pingAttempts = 0;

		private int i_nullMessages;

		private bool i_rollbackOnClose = true;

		private bool i_sendCloseMessage = true;

		private readonly Db4objects.Db4o.CS.YapServer i_server;

		private Db4objects.Db4o.Foundation.Network.IYapSocket i_socket;

		private Db4objects.Db4o.YapFile i_substituteStream;

		private Db4objects.Db4o.Transaction i_substituteTrans;

		private Db4objects.Db4o.Foundation.Hashtable4 _queryResults;

		private Db4objects.Db4o.Config4Impl i_config;

		internal readonly int i_threadID;

		internal YapServerThread(Db4objects.Db4o.CS.YapServer aServer, Db4objects.Db4o.YapFile
			 aStream, Db4objects.Db4o.Foundation.Network.IYapSocket aSocket, int aThreadID, 
			bool loggedIn)
		{
			i_loggedin = loggedIn;
			i_lastClientMessage = Sharpen.Runtime.CurrentTimeMillis();
			i_server = aServer;
			i_config = (Db4objects.Db4o.Config4Impl)i_server.Configure();
			i_mainStream = aStream;
			i_threadID = aThreadID;
			SetName("db4o message server " + aThreadID);
			i_mainTrans = aStream.NewTransaction();
			try
			{
				i_socket = aSocket;
				i_socket.SetSoTimeout(((Db4objects.Db4o.Config4Impl)aServer.Configure()).TimeoutServerSocket
					());
			}
			catch (System.Exception e)
			{
				i_socket.Close();
				throw (e);
			}
		}

		public void Close()
		{
			CloseSubstituteStream();
			try
			{
				if (i_sendCloseMessage)
				{
					Write(Db4objects.Db4o.CS.Messages.Msg.CLOSE);
				}
			}
			catch (System.Exception e)
			{
			}
			if (i_mainStream != null && i_mainTrans != null)
			{
				i_mainTrans.Close(i_rollbackOnClose);
			}
			try
			{
				i_socket.Close();
			}
			catch (System.Exception e)
			{
			}
			i_socket = null;
			try
			{
				i_server.RemoveThread(this);
			}
			catch (System.Exception e)
			{
			}
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
				catch (System.Exception e)
				{
				}
				i_substituteStream = null;
			}
		}

		private Db4objects.Db4o.YapFile GetStream()
		{
			if (i_substituteStream != null)
			{
				return i_substituteStream;
			}
			return i_mainStream;
		}

		internal Db4objects.Db4o.Transaction GetTransaction()
		{
			if (i_substituteTrans != null)
			{
				return i_substituteTrans;
			}
			return i_mainTrans;
		}

		public override void Run()
		{
			while (i_socket != null)
			{
				try
				{
					if (!MessageProcessor())
					{
						break;
					}
				}
				catch (System.Exception e)
				{
					if (i_mainStream == null || i_mainStream.IsClosed())
					{
						break;
					}
					if (!i_socket.IsConnected())
					{
						break;
					}
					i_nullMessages++;
				}
				if (i_nullMessages > 20 || PingClientTimeoutReached())
				{
					if (i_pingAttempts > 5)
					{
						GetStream().LogMsg(33, i_clientName);
						break;
					}
					if (null == i_socket)
					{
						break;
					}
					Write(Db4objects.Db4o.CS.Messages.Msg.PING);
					i_pingAttempts++;
				}
			}
			Close();
		}

		private bool PingClientTimeoutReached()
		{
			return (Sharpen.Runtime.CurrentTimeMillis() - i_lastClientMessage > i_config.TimeoutPingClients
				());
		}

		private bool MessageProcessor()
		{
			Db4objects.Db4o.CS.Messages.Msg message = Db4objects.Db4o.CS.Messages.Msg.ReadMessage
				(GetTransaction(), i_socket);
			if (message == null)
			{
				i_nullMessages++;
				return true;
			}
			i_lastClientMessage = Sharpen.Runtime.CurrentTimeMillis();
			i_nullMessages = 0;
			i_pingAttempts = 0;
			if (!i_loggedin)
			{
				if (Db4objects.Db4o.CS.Messages.Msg.LOGIN.Equals(message))
				{
					string userName = ((Db4objects.Db4o.CS.Messages.MsgD)message).ReadString();
					string password = ((Db4objects.Db4o.CS.Messages.MsgD)message).ReadString();
					i_mainStream.ShowInternalClasses(true);
					Db4objects.Db4o.User found = i_server.GetUser(userName);
					i_mainStream.ShowInternalClasses(false);
					if (found != null)
					{
						if (found.password.Equals(password))
						{
							i_clientName = userName;
							i_mainStream.LogMsg(32, i_clientName);
							int blockSize = i_mainStream.BlockSize();
							int encrypt = i_mainStream.i_handlers.i_encrypt ? 1 : 0;
							Write(Db4objects.Db4o.CS.Messages.Msg.LOGIN_OK.GetWriterForInts(GetTransaction(), 
								new int[] { blockSize, encrypt }));
							i_loggedin = true;
							SetName("db4o server socket for client " + i_clientName);
						}
						else
						{
							Write(Db4objects.Db4o.CS.Messages.Msg.FAILED);
							return false;
						}
					}
					else
					{
						Write(Db4objects.Db4o.CS.Messages.Msg.FAILED);
						return false;
					}
				}
				return true;
			}
			if (message.ProcessAtServer(this))
			{
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.PING.Equals(message))
			{
				WriteOK();
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_FINALIZED.Equals(message))
			{
				int queryResultID = ((Db4objects.Db4o.CS.Messages.MsgD)message).ReadInt();
				QueryResultFinalized(queryResultID);
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.CLOSE.Equals(message))
			{
				Write(Db4objects.Db4o.CS.Messages.Msg.CLOSE);
				GetTransaction().Commit();
				i_sendCloseMessage = false;
				GetStream().LogMsg(34, i_clientName);
				return false;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.IDENTITY.Equals(message))
			{
				RespondInt((int)GetStream().GetID(GetStream().Identity()));
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.CURRENT_VERSION.Equals(message))
			{
				long ver = 0;
				lock (GetStream())
				{
					ver = GetStream().CurrentVersion();
				}
				Write(Db4objects.Db4o.CS.Messages.Msg.ID_LIST.GetWriterForLong(GetTransaction(), 
					ver));
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.RAISE_VERSION.Equals(message))
			{
				long minimumVersion = ((Db4objects.Db4o.CS.Messages.MsgD)message).ReadLong();
				Db4objects.Db4o.YapStream stream = GetStream();
				lock (stream)
				{
					stream.RaiseVersion(minimumVersion);
				}
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.GET_THREAD_ID.Equals(message))
			{
				RespondInt(i_threadID);
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.SWITCH_TO_FILE.Equals(message))
			{
				SwitchToFile(message);
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.SWITCH_TO_MAIN_FILE.Equals(message))
			{
				SwitchToMainFile();
				return true;
			}
			if (Db4objects.Db4o.CS.Messages.Msg.USE_TRANSACTION.Equals(message))
			{
				UseTransaction(message);
				return true;
			}
			return true;
		}

		private void WriteOK()
		{
			Write(Db4objects.Db4o.CS.Messages.Msg.OK);
		}

		private void QueryResultFinalized(int queryResultID)
		{
			_queryResults.Remove(queryResultID);
		}

		public void MapQueryResultToID(Db4objects.Db4o.CS.LazyClientObjectSetStub stub, int
			 queryResultID)
		{
			if (_queryResults == null)
			{
				_queryResults = new Db4objects.Db4o.Foundation.Hashtable4();
			}
			_queryResults.Put(queryResultID, stub);
		}

		public Db4objects.Db4o.CS.LazyClientObjectSetStub QueryResultForID(int queryResultID
			)
		{
			return (Db4objects.Db4o.CS.LazyClientObjectSetStub)_queryResults.Get(queryResultID
				);
		}

		private void SwitchToFile(Db4objects.Db4o.CS.Messages.Msg message)
		{
			lock (i_mainStream.i_lock)
			{
				string fileName = ((Db4objects.Db4o.CS.Messages.MsgD)message).ReadString();
				try
				{
					CloseSubstituteStream();
					i_substituteStream = (Db4objects.Db4o.YapFile)Db4objects.Db4o.Db4oFactory.OpenFile
						(fileName);
					i_substituteTrans = i_substituteStream.NewTransaction();
					i_substituteStream.ConfigImpl().SetMessageRecipient(i_mainStream.ConfigImpl().MessageRecipient
						());
					WriteOK();
				}
				catch (System.Exception e)
				{
					CloseSubstituteStream();
					Write(Db4objects.Db4o.CS.Messages.Msg.ERROR);
				}
			}
		}

		private void SwitchToMainFile()
		{
			lock (i_mainStream.i_lock)
			{
				CloseSubstituteStream();
				WriteOK();
			}
		}

		private void UseTransaction(Db4objects.Db4o.CS.Messages.Msg message)
		{
			int threadID = ((Db4objects.Db4o.CS.Messages.MsgD)message).ReadInt();
			Db4objects.Db4o.CS.YapServerThread transactionThread = i_server.FindThread(threadID
				);
			if (transactionThread != null)
			{
				Db4objects.Db4o.Transaction transToUse = transactionThread.GetTransaction();
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

		private void RespondInt(int response)
		{
			Write(Db4objects.Db4o.CS.Messages.Msg.ID_LIST.GetWriterForInt(GetTransaction(), response
				));
		}

		public void Write(Db4objects.Db4o.CS.Messages.Msg msg)
		{
			msg.Write(GetStream(), i_socket);
		}

		public Db4objects.Db4o.Foundation.Network.IYapSocket Socket()
		{
			return i_socket;
		}
	}
}
