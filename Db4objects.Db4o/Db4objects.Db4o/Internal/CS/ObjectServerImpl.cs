namespace Db4objects.Db4o.Internal.CS
{
	public class ObjectServerImpl : Db4objects.Db4o.IObjectServer, Db4objects.Db4o.Ext.IExtObjectServer
		, Sharpen.Lang.IRunnable, Db4objects.Db4o.Foundation.Network.ILoopbackSocketServer
	{
		private string i_name;

		private Db4objects.Db4o.Foundation.Network.ServerSocket4 i_serverSocket;

		private int i_threadIDGen = 1;

		private Db4objects.Db4o.Foundation.Collection4 i_threads = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Internal.LocalObjectContainer i_yapFile;

		private readonly object _lock = new object();

		public ObjectServerImpl(Db4objects.Db4o.Internal.LocalObjectContainer a_yapFile, 
			int a_port)
		{
			a_yapFile.SetServer(true);
			i_name = "db4o ServerSocket  FILE: " + a_yapFile.ToString() + "  PORT:" + a_port;
			i_yapFile = a_yapFile;
			Db4objects.Db4o.Internal.Config4Impl config = (Db4objects.Db4o.Internal.Config4Impl
				)i_yapFile.Configure();
			config.Callbacks(false);
			config.IsServer(true);
			a_yapFile.ProduceYapClass(a_yapFile.i_handlers.ICLASS_STATICCLASS);
			config.ExceptionalClasses().ForEachValue(new _AnonymousInnerClass41(this, a_yapFile
				));
			if (a_port > 0)
			{
				try
				{
					i_serverSocket = new Db4objects.Db4o.Foundation.Network.ServerSocket4(a_port);
					i_serverSocket.SetSoTimeout(config.TimeoutServerSocket());
				}
				catch (System.IO.IOException)
				{
					Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(30, string.Empty + a_port
						);
				}
				new Sharpen.Lang.Thread(this).Start();
				lock (_lock)
				{
					try
					{
						Sharpen.Runtime.Wait(_lock, 1000);
					}
					catch
					{
					}
				}
			}
		}

		private sealed class _AnonymousInnerClass41 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass41(ObjectServerImpl _enclosing, Db4objects.Db4o.Internal.LocalObjectContainer
				 a_yapFile)
			{
				this._enclosing = _enclosing;
				this.a_yapFile = a_yapFile;
			}

			public void Visit(object a_object)
			{
				a_yapFile.ProduceYapClass(a_yapFile.Reflector().ForName(((Db4objects.Db4o.Internal.Config4Class
					)a_object).GetName()));
			}

			private readonly ObjectServerImpl _enclosing;

			private readonly Db4objects.Db4o.Internal.LocalObjectContainer a_yapFile;
		}

		public virtual void Backup(string path)
		{
			i_yapFile.Backup(path);
		}

		internal void CheckClosed()
		{
			if (i_yapFile == null)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(20, i_name);
			}
			i_yapFile.CheckClosed();
		}

		public virtual bool Close()
		{
			lock (Db4objects.Db4o.Internal.Global4.Lock)
			{
				Db4objects.Db4o.Foundation.Cool.SleepIgnoringInterruption(100);
				try
				{
					if (i_serverSocket != null)
					{
						i_serverSocket.Close();
					}
				}
				catch
				{
				}
				i_serverSocket = null;
				bool isClosed = i_yapFile == null ? true : i_yapFile.Close();
				lock (i_threads)
				{
					System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Collection4(i_threads
						).GetEnumerator();
					while (i.MoveNext())
					{
						((Db4objects.Db4o.Internal.CS.ServerMessageDispatcher)i.Current).Close();
					}
				}
				i_yapFile = null;
				return isClosed;
			}
		}

		public virtual Db4objects.Db4o.Config.IConfiguration Configure()
		{
			return i_yapFile.Configure();
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectServer Ext()
		{
			return this;
		}

		internal virtual Db4objects.Db4o.Internal.CS.ServerMessageDispatcher FindThread(int
			 a_threadID)
		{
			lock (i_threads)
			{
				System.Collections.IEnumerator i = i_threads.GetEnumerator();
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
			lock (i_yapFile.i_lock)
			{
				CheckClosed();
				i_yapFile.ShowInternalClasses(true);
				try
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
					i_yapFile.Commit();
				}
				finally
				{
					i_yapFile.ShowInternalClasses(false);
				}
			}
		}

		private void AddUser(string userName, string password)
		{
			i_yapFile.Set(new Db4objects.Db4o.User(userName, password));
		}

		private void SetPassword(Db4objects.Db4o.User existing, string password)
		{
			existing.password = password;
			i_yapFile.Set(existing);
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
			return i_yapFile.Get(new Db4objects.Db4o.User(userName, null));
		}

		public virtual Db4objects.Db4o.IObjectContainer ObjectContainer()
		{
			return i_yapFile;
		}

		public virtual Db4objects.Db4o.IObjectContainer OpenClient()
		{
			return OpenClient(Db4objects.Db4o.Db4oFactory.CloneConfiguration());
		}

		public virtual Db4objects.Db4o.IObjectContainer OpenClient(Db4objects.Db4o.Config.IConfiguration
			 config)
		{
			CheckClosed();
			try
			{
				Db4objects.Db4o.Internal.CS.ClientObjectContainer client = new Db4objects.Db4o.Internal.CS.ClientObjectContainer
					(config, OpenClientSocket(), Db4objects.Db4o.Internal.Const4.EMBEDDED_CLIENT_USER
					 + (i_threadIDGen - 1), string.Empty, false);
				client.BlockSize(i_yapFile.BlockSize());
				return client;
			}
			catch (System.IO.IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return null;
		}

		public virtual Db4objects.Db4o.Foundation.Network.LoopbackSocket OpenClientSocket
			()
		{
			int timeout = ((Db4objects.Db4o.Internal.Config4Impl)Configure()).TimeoutClientSocket
				();
			Db4objects.Db4o.Foundation.Network.LoopbackSocket clientFake = new Db4objects.Db4o.Foundation.Network.LoopbackSocket
				(this, timeout);
			Db4objects.Db4o.Foundation.Network.LoopbackSocket serverFake = new Db4objects.Db4o.Foundation.Network.LoopbackSocket
				(this, timeout, clientFake);
			try
			{
				Db4objects.Db4o.Internal.CS.ServerMessageDispatcher thread = new Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
					(this, i_yapFile, serverFake, i_threadIDGen++, true);
				lock (i_threads)
				{
					i_threads.Add(thread);
				}
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
			lock (i_threads)
			{
				i_threads.Remove(aThread);
			}
		}

		public virtual void RevokeAccess(string userName)
		{
			lock (i_yapFile.i_lock)
			{
				i_yapFile.ShowInternalClasses(true);
				try
				{
					CheckClosed();
					DeleteUsers(userName);
					i_yapFile.Commit();
				}
				finally
				{
					i_yapFile.ShowInternalClasses(false);
				}
			}
		}

		private void DeleteUsers(string userName)
		{
			Db4objects.Db4o.IObjectSet set = QueryUsers(userName);
			while (set.HasNext())
			{
				i_yapFile.Delete(set.Next());
			}
		}

		public virtual void Run()
		{
			Sharpen.Lang.Thread.CurrentThread().SetName(i_name);
			i_yapFile.LogMsg(31, string.Empty + i_serverSocket.GetLocalPort());
			lock (_lock)
			{
				Sharpen.Runtime.NotifyAll(_lock);
			}
			while (i_serverSocket != null)
			{
				try
				{
					Db4objects.Db4o.Internal.CS.ServerMessageDispatcher thread = new Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
						(this, i_yapFile, i_serverSocket.Accept(), i_threadIDGen++, false);
					lock (i_threads)
					{
						i_threads.Add(thread);
					}
					thread.Start();
				}
				catch
				{
				}
			}
		}
	}
}
