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
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS
{
	public class ObjectServerImpl : IObjectServer, IExtObjectServer, IRunnable, ILoopbackSocketServer
	{
		private const int START_THREAD_WAIT_TIMEOUT = 5000;

		private readonly string _name;

		private ServerSocket4 _serverSocket;

		private readonly int _port;

		private int i_threadIDGen = 1;

		private readonly Collection4 _dispatchers = new Collection4();

		internal LocalObjectContainer _container;

		private readonly object _startupLock = new object();

		private Config4Impl _config;

		private BlockingQueue _committedInfosQueue = new BlockingQueue();

		private CommittedCallbacksDispatcher _committedCallbacksDispatcher;

		public ObjectServerImpl(LocalObjectContainer container, int port)
		{
			_container = container;
			_port = port;
			_config = _container.ConfigImpl();
			_name = "db4o ServerSocket FILE: " + container.ToString() + "  PORT:" + _port;
			_container.SetServer(true);
			ConfigureObjectServer();
			bool ok = false;
			try
			{
				EnsureLoadStaticClass();
				EnsureLoadConfiguredClasses();
				StartCommittedCallbackThread(_committedInfosQueue);
				StartServer();
				ok = true;
			}
			finally
			{
				if (!ok)
				{
					Close();
				}
			}
		}

		private void StartServer()
		{
			if (IsEmbeddedServer())
			{
				return;
			}
			lock (_startupLock)
			{
				StartServerSocket();
				StartServerThread();
				bool started = false;
				while (!started)
				{
					try
					{
						Sharpen.Runtime.Wait(_startupLock, START_THREAD_WAIT_TIMEOUT);
						started = true;
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private void StartServerThread()
		{
			lock (_startupLock)
			{
				Thread thread = new Thread(this);
				thread.SetDaemon(true);
				thread.Start();
			}
		}

		private void StartServerSocket()
		{
			try
			{
				_serverSocket = new ServerSocket4(_port);
				_serverSocket.SetSoTimeout(_config.TimeoutServerSocket());
			}
			catch (IOException)
			{
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.COULD_NOT_OPEN_PORT
					, string.Empty + _port);
			}
		}

		private bool IsEmbeddedServer()
		{
			return _port <= 0;
		}

		private void EnsureLoadStaticClass()
		{
			_container.ProduceClassMetadata(_container.i_handlers.ICLASS_STATICCLASS);
		}

		private void EnsureLoadConfiguredClasses()
		{
			IEnumerator i = _config.ExceptionalClasses().Iterator();
			while (i.MoveNext())
			{
				IEntry4 entry = (IEntry4)i.Current;
				_container.ProduceClassMetadata(_container.Reflector().ForName(((Config4Class)entry
					.Value()).GetName()));
			}
		}

		private void ConfigureObjectServer()
		{
			_config.Callbacks(false);
			_config.IsServer(true);
			_config.ObjectClass(typeof(User)).MinimumActivationDepth(1);
		}

		public virtual void Backup(string path)
		{
			_container.Backup(path);
		}

		internal void CheckClosed()
		{
			if (_container == null)
			{
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.CLOSED_OR_OPEN_FAILED
					, _name);
			}
			_container.CheckClosed();
		}

		public virtual bool Close()
		{
			lock (this)
			{
				CloseServerSocket();
				StopCommittedCallbacksDispatcher();
				CloseMessageDispatchers();
				return CloseFile();
			}
		}

		private void StopCommittedCallbacksDispatcher()
		{
			if (_committedCallbacksDispatcher != null)
			{
				_committedCallbacksDispatcher.Stop();
			}
		}

		private bool CloseFile()
		{
			if (_container != null)
			{
				_container.Close();
				_container = null;
			}
			return true;
		}

		private void CloseMessageDispatchers()
		{
			IEnumerator i = IterateDispatchers();
			while (i.MoveNext())
			{
				try
				{
					((IServerMessageDispatcher)i.Current).Close();
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		public virtual IEnumerator IterateDispatchers()
		{
			lock (_dispatchers)
			{
				return new Collection4(_dispatchers).GetEnumerator();
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
			catch (Exception)
			{
			}
			_serverSocket = null;
		}

		public virtual IConfiguration Configure()
		{
			return _config;
		}

		public virtual IExtObjectServer Ext()
		{
			return this;
		}

		internal virtual ServerMessageDispatcherImpl FindThread(int a_threadID)
		{
			lock (_dispatchers)
			{
				IEnumerator i = _dispatchers.GetEnumerator();
				while (i.MoveNext())
				{
					ServerMessageDispatcherImpl serverThread = (ServerMessageDispatcherImpl)i.Current;
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
					User existing = GetUser(userName);
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
			_container.Set(new User(userName, password));
		}

		private void SetPassword(User existing, string password)
		{
			existing.password = password;
			_container.Set(existing);
		}

		public virtual User GetUser(string userName)
		{
			IObjectSet result = QueryUsers(userName);
			if (!result.HasNext())
			{
				return null;
			}
			return (User)result.Next();
		}

		private IObjectSet QueryUsers(string userName)
		{
			_container.ShowInternalClasses(true);
			try
			{
				return _container.Get(new User(userName, null));
			}
			finally
			{
				_container.ShowInternalClasses(false);
			}
		}

		public virtual IObjectContainer ObjectContainer()
		{
			return _container;
		}

		public virtual IObjectContainer OpenClient()
		{
			return OpenClient(Db4oFactory.CloneConfiguration());
		}

		public virtual IObjectContainer OpenClient(IConfiguration config)
		{
			lock (this)
			{
				CheckClosed();
				ClientObjectContainer client = new ClientObjectContainer(config, OpenClientSocket
					(), Const4.EMBEDDED_CLIENT_USER + (i_threadIDGen - 1), string.Empty, false);
				client.BlockSize(_container.BlockSize());
				return client;
			}
		}

		public virtual LoopbackSocket OpenClientSocket()
		{
			int timeout = _config.TimeoutClientSocket();
			LoopbackSocket clientFake = new LoopbackSocket(this, timeout);
			LoopbackSocket serverFake = new LoopbackSocket(this, timeout, clientFake);
			try
			{
				IServerMessageDispatcher messageDispatcher = new ServerMessageDispatcherImpl(this
					, _container, serverFake, NewThreadId(), true);
				AddServerMessageDispatcher(messageDispatcher);
				messageDispatcher.StartDispatcher();
				return clientFake;
			}
			catch (Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return null;
		}

		internal virtual void RemoveThread(ServerMessageDispatcherImpl aThread)
		{
			lock (_dispatchers)
			{
				_dispatchers.Remove(aThread);
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
			IObjectSet set = QueryUsers(userName);
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
			Listen();
		}

		private void StartCommittedCallbackThread(BlockingQueue committedInfosQueue)
		{
			_committedCallbacksDispatcher = new CommittedCallbacksDispatcher(this, committedInfosQueue
				);
			Thread thread = new Thread(_committedCallbacksDispatcher);
			thread.SetDaemon(true);
			thread.Start();
		}

		private void SetThreadName()
		{
			Thread.CurrentThread().SetName(_name);
		}

		private void Listen()
		{
			while (_serverSocket != null)
			{
				try
				{
					IServerMessageDispatcher messageDispatcher = new ServerMessageDispatcherImpl(this
						, _container, _serverSocket.Accept(), NewThreadId(), false);
					AddServerMessageDispatcher(messageDispatcher);
					messageDispatcher.StartDispatcher();
				}
				catch (Exception)
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

		private void AddServerMessageDispatcher(IServerMessageDispatcher thread)
		{
			lock (_dispatchers)
			{
				_dispatchers.Add(thread);
			}
		}

		public virtual void AddCommittedInfoMsg(MCommittedInfo message)
		{
			_committedInfosQueue.Add(message);
		}

		public virtual void SendCommittedInfoMsg(MCommittedInfo message)
		{
			IEnumerator i = IterateDispatchers();
			while (i.MoveNext())
			{
				IServerMessageDispatcher dispatcher = (IServerMessageDispatcher)i.Current;
				if (dispatcher.CaresAboutCommitted())
				{
					dispatcher.WriteIfAlive(message);
				}
			}
		}
	}
}
