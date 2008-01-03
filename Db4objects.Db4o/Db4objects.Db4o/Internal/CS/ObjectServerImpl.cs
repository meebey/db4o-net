/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
	public class ObjectServerImpl : IObjectServer, IExtObjectServer, IRunnable
	{
		private const int StartThreadWaitTimeout = 5000;

		private readonly string _name;

		private ServerSocket4 _serverSocket;

		private int _port;

		private int i_threadIDGen = 1;

		private readonly Collection4 _dispatchers = new Collection4();

		internal LocalObjectContainer _container;

		internal ClientTransactionPool _transactionPool;

		private readonly object _startupLock = new object();

		private Config4Impl _config;

		private BlockingQueue _committedInfosQueue = new BlockingQueue();

		private CommittedCallbacksDispatcher _committedCallbacksDispatcher;

		private bool _caresAboutCommitted;

		private readonly INativeSocketFactory _socketFactory;

		private readonly bool _isEmbeddedServer;

		public ObjectServerImpl(LocalObjectContainer container, int port, INativeSocketFactory
			 socketFactory) : this(container, (port < 0 ? 0 : port), port == 0, socketFactory
			)
		{
		}

		public ObjectServerImpl(LocalObjectContainer container, int port, bool isEmbeddedServer
			, INativeSocketFactory socketFactory)
		{
			_isEmbeddedServer = isEmbeddedServer;
			_socketFactory = socketFactory;
			_container = container;
			_transactionPool = new ClientTransactionPool(container);
			_port = port;
			_config = _container.ConfigImpl();
			_name = "db4o ServerSocket FILE: " + container.ToString() + "  PORT:" + _port;
			_container.SetServer(true);
			ConfigureObjectServer();
			bool ok = false;
			try
			{
				EnsureLoadStaticClass();
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
						Sharpen.Runtime.Wait(_startupLock, StartThreadWaitTimeout);
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
				_serverSocket = new ServerSocket4(_socketFactory, _port);
				_port = _serverSocket.GetLocalPort();
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
			_serverSocket.SetSoTimeout(_config.TimeoutServerSocket());
		}

		private bool IsEmbeddedServer()
		{
			return _isEmbeddedServer;
		}

		private void EnsureLoadStaticClass()
		{
			_container.ProduceClassMetadata(_container._handlers.IclassStaticclass);
		}

		private void ConfigureObjectServer()
		{
			_config.Callbacks(false);
			_config.IsServer(true);
			_config.ObjectClass(typeof(User)).MinimumActivationDepth(1);
		}

		/// <exception cref="IOException"></exception>
		public virtual void Backup(string path)
		{
			_container.Backup(path);
		}

		internal void CheckClosed()
		{
			if (_container == null)
			{
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.ClosedOrOpenFailed
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
				_transactionPool.Close();
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
			i = IterateDispatchers();
			while (i.MoveNext())
			{
				try
				{
					((Thread)i.Current).Join();
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

		private ServerMessageDispatcherImpl FindThread(int a_threadID)
		{
			lock (_dispatchers)
			{
				IEnumerator i = _dispatchers.GetEnumerator();
				while (i.MoveNext())
				{
					ServerMessageDispatcherImpl serverThread = (ServerMessageDispatcherImpl)i.Current;
					if (serverThread._threadID == a_threadID)
					{
						return serverThread;
					}
				}
			}
			return null;
		}

		internal virtual Transaction FindTransaction(int threadID)
		{
			ServerMessageDispatcherImpl dispatcher = FindThread(threadID);
			return (dispatcher == null ? null : dispatcher.GetTransaction());
		}

		public virtual void GrantAccess(string userName, string password)
		{
			lock (this)
			{
				CheckClosed();
				lock (_container._lock)
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
			_container.Store(new User(userName, password));
		}

		private void SetPassword(User existing, string password)
		{
			existing.password = password;
			_container.Store(existing);
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
				return _container.QueryByExample(new User(userName, null));
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
				lock (_container._lock)
				{
					return new EmbeddedClientObjectContainer(_container);
				}
			}
		}

		internal virtual void RemoveThread(ServerMessageDispatcherImpl dispatcher)
		{
			lock (_dispatchers)
			{
				_dispatchers.Remove(dispatcher);
				CheckCaresAboutCommitted();
			}
		}

		public virtual void RevokeAccess(string userName)
		{
			lock (this)
			{
				CheckClosed();
				lock (_container._lock)
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
			thread.SetName("committed callback thread");
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
						, new ClientTransactionHandle(_transactionPool), _serverSocket.Accept(), NewThreadId
						(), false, _container.Lock());
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
			_container.LogMsg(Db4objects.Db4o.Internal.Messages.ServerListeningOnPort, string.Empty
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
				CheckCaresAboutCommitted();
			}
		}

		public virtual void AddCommittedInfoMsg(MCommittedInfo message)
		{
			_committedInfosQueue.Add(message);
		}

		public virtual void BroadcastMsg(Msg message, IBroadcastFilter filter)
		{
			IEnumerator i = IterateDispatchers();
			while (i.MoveNext())
			{
				IServerMessageDispatcher dispatcher = (IServerMessageDispatcher)i.Current;
				if (filter.Accept(dispatcher))
				{
					dispatcher.Write(message);
				}
			}
		}

		public virtual bool CaresAboutCommitted()
		{
			return _caresAboutCommitted;
		}

		public virtual void CheckCaresAboutCommitted()
		{
			_caresAboutCommitted = AnyDispatcherCaresAboutCommitted();
		}

		private bool AnyDispatcherCaresAboutCommitted()
		{
			IEnumerator i = IterateDispatchers();
			while (i.MoveNext())
			{
				IServerMessageDispatcher dispatcher = (IServerMessageDispatcher)i.Current;
				if (dispatcher.CaresAboutCommitted())
				{
					return true;
				}
			}
			return false;
		}

		public virtual int Port()
		{
			return _port;
		}

		public virtual int ClientCount()
		{
			lock (_dispatchers)
			{
				return _dispatchers.Size();
			}
		}
	}
}
