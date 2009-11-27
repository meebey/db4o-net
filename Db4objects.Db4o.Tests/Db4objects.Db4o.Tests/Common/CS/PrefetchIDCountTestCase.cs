/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.CS;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Api;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class PrefetchIDCountTestCase : TestWithTempFile
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(PrefetchIDCountTestCase)).Run();
		}

		private const int PrefetchIdCount = 100;

		private static readonly string User = "db4o";

		private static readonly string Password = "db4o";

		public class Item
		{
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			ObjectServerImpl server = (ObjectServerImpl)Db4oClientServer.OpenServer(TempFile(
				), Db4oClientServer.ArbitraryPort);
			Lock4 Lock = new Lock4();
			server.ClientDisconnected += new System.EventHandler<Db4objects.Db4o.Events.StringEventArgs>
				(new _IEventListener4_33(Lock).OnEvent);
			server.GrantAccess(User, Password);
			IObjectContainer client = OpenClient(server.Port());
			client.Store(new PrefetchIDCountTestCase.Item());
			ServerMessageDispatcherImpl msgDispatcher = FirstMessageDispatcherFor(server);
			Lock.Run(new _IClosure4_47(client, Lock, server, msgDispatcher));
			server.Close();
		}

		private sealed class _IEventListener4_33
		{
			public _IEventListener4_33(Lock4 Lock)
			{
				this.Lock = Lock;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.StringEventArgs args)
			{
				Lock.Run(new _IClosure4_34(Lock));
			}

			private sealed class _IClosure4_34 : IClosure4
			{
				public _IClosure4_34(Lock4 Lock)
				{
					this.Lock = Lock;
				}

				public object Run()
				{
					Lock.Awake();
					return null;
				}

				private readonly Lock4 Lock;
			}

			private readonly Lock4 Lock;
		}

		private sealed class _IClosure4_47 : IClosure4
		{
			public _IClosure4_47(IObjectContainer client, Lock4 Lock, ObjectServerImpl server
				, ServerMessageDispatcherImpl msgDispatcher)
			{
				this.client = client;
				this.Lock = Lock;
				this.server = server;
				this.msgDispatcher = msgDispatcher;
			}

			public object Run()
			{
				client.Close();
				try
				{
					Lock.Snooze(100000);
					IReflectClass reflector = server.ObjectContainer().Ext().Reflector().ForClass(typeof(
						ServerMessageDispatcherImpl));
					IReflectField prefetchedIdsField = reflector.GetDeclaredField("_prefetchedIDs");
					Assert.IsNull(prefetchedIdsField.Get(msgDispatcher));
				}
				catch (Exception)
				{
				}
				return null;
			}

			private readonly IObjectContainer client;

			private readonly Lock4 Lock;

			private readonly ObjectServerImpl server;

			private readonly ServerMessageDispatcherImpl msgDispatcher;
		}

		private ServerMessageDispatcherImpl FirstMessageDispatcherFor(ObjectServerImpl server
			)
		{
			IEnumerator dispatchers = server.IterateDispatchers();
			Assert.IsTrue(dispatchers.MoveNext());
			ServerMessageDispatcherImpl msgDispatcher = (ServerMessageDispatcherImpl)dispatchers
				.Current;
			return msgDispatcher;
		}

		private IObjectContainer OpenClient(int port)
		{
			IClientConfiguration config = Db4oClientServer.NewClientConfiguration();
			config.PrefetchIDCount = PrefetchIdCount;
			return Db4oClientServer.OpenClient(config, "localhost", port, User, Password);
		}
	}
}
#endif // !SILVERLIGHT
