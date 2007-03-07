namespace Db4objects.Db4o.Internal.CS
{
	public class ObjectServerImpl : Db4objects.Db4o.IObjectServer, Db4objects.Db4o.Ext.IExtObjectServer
		, Sharpen.Lang.IRunnable, Db4objects.Db4o.Foundation.Network.ILoopbackSocketServer
	{
		private readonly string _name;

		private Db4objects.Db4o.Foundation.Network.ServerSocket4 _serverSocket;

		private readonly int _port;

		private int i_threadIDGen = 1;

		private readonly Db4objects.Db4o.Foundation.Collection4 _threads = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Internal.LocalObjectContainer _container;

		private readonly object _startupLock = new object();

		private Db4objects.Db4o.Internal.Config4Impl _config;

		public ObjectServerImpl(Db4objects.Db4o.Internal.LocalObjectContainer container, 
			int port)
		{
			_container = container;
			_port = port;
			_config = _container.ConfigImpl();
			_name = "db4o ServerSocket FILE: " + container.ToString() + "  PORT:" + _port;
			_container.SetServer(true);
			ConfigureObjectServer();
			EnsureLoadStaticClass();
			EnsureLoadConfiguredClasses();
			StartServer();
		}

		private void StartServer()
		{
			if (IsEmbeddedServer())
			{
				return;
			}
			StartServerSocket();
			StartServerThread();
			WaitForThreadStart();
		}

		private void StartServerThread()
		{
			Sharpen.Lang.Thread thread = new Sharpen.Lang.Thread(this);
			thread.SetDaemon(true);
			thread.Start();
		}

		private void StartServerSocket()
		{
			try
			{
				_serverSocket = new Db4objects.Db4o.Foundation.Network.ServerSocket4(_port);
				_serverSocket.SetSoTimeout(_config.TimeoutServerSocket());
			}
			catch (System.IO.IOException)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages
					.COULD_NOT_OPEN_PORT, string.Empty + _port);
			}
		}

		private bool IsEmbeddedServer()
		{
			return _port <= 0;
		}

		private void WaitForThreadStart()
		{
			lock (_startupLock)
			{
				try
				{
					Sharpen.Runtime.Wait(_startupLock, 1000);
				}
				catch
				{
				}
			}
		}

		private void EnsureLoadStaticClass()
		{
			_container.ProduceYapClass(_container.i_handlers.ICLASS_STATICCLASS);
		}

		private void EnsureLoadConfiguredClasses()
		{
			_config.ExceptionalClasses().ForEachValue(new _AnonymousInnerClass95(this));
		}

		private sealed class _AnonymousInnerClass95 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass95(ObjectServerImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing._container.ProduceYapClass(this._enclosing._container.Reflector()
					.ForName(((Db4objects.Db4o.Internal.Config4Class)a_object).GetName()));
			}

			private readonly ObjectServerImpl _enclosing;
		}

		private void ConfigureObjectServer()
		{
			_config.Callbacks(false);
			_config.IsServer(true);
			_config.ObjectClass(typeof(Db4objects.Db4o.User)).MinimumActivationDepth(1);
		}

		public virtual void Backup(string path)
		{
			_container.Backup(path);
		}

		internal void CheckClosed()
		{
			if (_container == null)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages
					.CLOSED_OR_OPEN_FAILED, _name);
			}
			_container.CheckClosed();
		}

		public virtual bool Close()
		{
			lock (this)
			{
				CloseServerSocket();
				bool isClosed = CloseFile();
				CloseMessageDispatchers();
				return isClosed;
			}
		}

		private bool CloseFile()
		{
			if (_container == null)
			{
				return true;
			}
			bool isClosed = _container.Close();
			_container = null;
			return isClosed;
		}

		private void CloseMessageDispatchers()
		{
			System.Collections.IEnumerator i = IterateThreads();
			while (i.MoveNext())
			{
				try
				{
					((Db4objects.Db4o.Internal.CS.ServerMessageDispatcher)i.Current).Close();
				}
				catch (System.Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		private System.Collections.IEnumerator IterateThreads()
		{
			lock (_threads)
			{
				return new Db4objects.Db4o.Foundation.Collection4(_threads).GetEnumerator();
			}
		}

		private void CloseServerSocket()
		{
			try
			{
				if (_serverSocket != null)
				{
					_serverSocket.Close();
				}
			}
			catch
			{
			}
			_serverSocket = null;
		}

		public virtual Db4objects.Db4o.Config.IConfiguration Configure()
		{
			return _config;
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectServer Ext()
		{
			return this;
		}

		internal virtual Db4objects.Db4o.Internal.CS.ServerMessageDispatcher FindThread(int
			 a_threadID)
		{
			lock (_threads)
			{
				System.Collections.IEnumerator i = _threads.GetEnumerator();
				while (i.MoveNext())
				{
					Db4objects.Db4o.Internal.CS.ServerMessageDispatcher serverThread = (Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
						)i.Current;
					if (serverThread.i_threadID == a_threadID)
					{
						return serverThread;
					}
				}
			}
			return null;
		}

		public virtual void GrantAccess(string userName, string password)
		{
			lock (this)
			{
				CheckClosed();
				lock (_container.i_lock)
				{
					Db4objects.Db4o.User existing = GetUser(userName);
					if (existing != null)
					{
						SetPassword(existing, password);
					}
					else
					{
						AddUser(userName, password);
					}
					_container.Commit();
				}
			}
		}

		private void AddUser(string userName, string password)
		{
			_container.Set(new Db4objects.Db4o.User(userName, password));
		}

		private void SetPassword(Db4objects.Db4o.User existing, string password)
		{
			existing.password = password;
			_container.Set(existing);
		}

		internal virtual Db4objects.Db4o.User GetUser(string userName)
		{
			Db4objects.Db4o.IObjectSet result = QueryUsers(userName);
			if (!result.HasNext())
			{
				return null;
			}
			return (Db4objects.Db4o.User)result.Next();
		}

		private Db4objects.Db4o.IObjectSet QueryUsers(string userName)
		{
			_container.ShowInternalClasses(true);
			try
			{
				return _container.Get(new Db4objects.Db4o.User(userName, null));
			}
			finally
			{
				_container.ShowInternalClasses(false);
			}
		}

		public virtual Db4objects.Db4o.IObjectContainer ObjectContainer()
		{
			return _container;
		}

		public virtual Db4objects.Db4o.IObjectContainer OpenClient()
		{
			return OpenClient(Db4objects.Db4o.Db4oFactory.CloneConfiguration());
		}

		public virtual Db4objects.Db4o.IObjectContainer OpenClient(Db4objects.Db4o.Config.IConfiguration
			 config)
		{
			lock (this)
			{
				CheckClosed();
				try
				{
					Db4objects.Db4o.Internal.CS.ClientObjectContainer client = new Db4objects.Db4o.Internal.CS.ClientObjectContainer
						(config, OpenClientSocket(), Db4objects.Db4o.Internal.Const4.EMBEDDED_CLIENT_USER
						 + (i_threadIDGen - 1), string.Empty, false);
					client.BlockSize(_container.BlockSize());
					return client;
				}
				catch (System.IO.IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
				return null;
			}
		}

		public virtual Db4objects.Db4o.Foundation.Network.LoopbackSocket OpenClientSocket
			()
		{
			int timeout = _config.TimeoutClientSocket();
			Db4objects.Db4o.Foundation.Network.LoopbackSocket clientFake = new Db4objects.Db4o.Foundation.Network.LoopbackSocket
				(this, timeout);
			Db4objects.Db4o.Foundation.Network.LoopbackSocket serverFake = new Db4objects.Db4o.Foundation.Network.LoopbackSocket
				(this, timeout, clientFake);
			try
			{
				Db4objects.Db4o.Internal.CS.ServerMessageDispatcher thread = new Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
					(this, _container, serverFake, NewThreadId(), true);
				AddThread(thread);
				thread.Start();
				return clientFake;
			}
			catch (System.Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return null;
		}

		internal virtual void RemoveThread(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 aThread)
		{
			lock (_threads)
			{
				_threads.Remove(aThread);
			}
		}

		public virtual void RevokeAccess(string userName)
		{
			lock (this)
			{
				CheckClosed();
				lock (_container.i_lock)
				{
					DeleteUsers(userName);
					_container.Commit();
				}
			}
		}

		private void DeleteUsers(string userName)
		{
			Db4objects.Db4o.IObjectSet set = QueryUsers(userName);
			while (set.HasNext())
			{
				_container.Delete(set.Next());
			}
		}

		public virtual void Run()
		{
			SetThreadName();
			LogListeningOnPort();
			NotifyThreadStarted();
			SocketServerLoop();
		}

		private void SetThreadName()
		{
			Sharpen.Lang.Thread.CurrentThread().SetName(_name);
		}

		private void SocketServerLoop()
		{
			while (_serverSocket != null)
			{
				try
				{
					Db4objects.Db4o.Internal.CS.ServerMessageDispatcher thread = new Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
						(this, _container, _serverSocket.Accept(), NewThreadId(), false);
					AddThread(thread);
					thread.Start();
				}
				catch
				{
				}
			}
		}

		private void NotifyThreadStarted()
		{
			lock (_startupLock)
			{
				Sharpen.Runtime.NotifyAll(_startupLock);
			}
		}

		private void LogListeningOnPort()
		{
			_container.LogMsg(Db4objects.Db4o.Internal.Messages.SERVER_LISTENING_ON_PORT, string.Empty
				 + _serverSocket.GetLocalPort());
		}

		private int NewThreadId()
		{
			return i_threadIDGen++;
		}

		private void AddThread(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher thread
			)
		{
			lock (_threads)
			{
				_threads.Add(thread);
			}
		}
	}
}
