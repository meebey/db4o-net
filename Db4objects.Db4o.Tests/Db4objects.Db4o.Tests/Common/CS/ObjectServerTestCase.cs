/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ObjectServerTestCase : TestWithTempFile
	{
		private IExtObjectServer server;

		private string fileName;

		public virtual void TestClientCount()
		{
			AssertClientCount(0);
			IObjectContainer client1 = OpenClient();
			try
			{
				AssertClientCount(1);
				IObjectContainer client2 = OpenClient();
				try
				{
					AssertClientCount(2);
				}
				finally
				{
					client2.Close();
				}
			}
			finally
			{
				client1.Close();
			}
		}

		// closing is asynchronous, relying on completion is hard
		// That's why there is no test here. 
		// ClientProcessesTestCase tests closing.
		public virtual void TestClientConnectedEvent()
		{
			ArrayList connections = new ArrayList();
			IObjectServerEvents events = (IObjectServerEvents)server;
			events.ClientConnected += new System.EventHandler<ClientConnectionEventArgs>(new 
				_IEventListener4_46(connections).OnEvent);
			IObjectContainer client = OpenClient();
			try
			{
				Assert.AreEqual(1, connections.Count);
				Iterator4Assert.AreEqual(ServerMessageDispatchers(), Iterators.Iterator(connections
					));
			}
			finally
			{
				client.Close();
			}
		}

		private sealed class _IEventListener4_46
		{
			public _IEventListener4_46(ArrayList connections)
			{
				this.connections = connections;
			}

			public void OnEvent(object sender, ClientConnectionEventArgs args)
			{
				connections.Add(((ClientConnectionEventArgs)args).Connection);
			}

			private readonly ArrayList connections;
		}

		public virtual void TestServerClosedEvent()
		{
			BooleanByRef receivedEvent = new BooleanByRef(false);
			IObjectServerEvents events = (IObjectServerEvents)server;
			events.Closed += new System.EventHandler<ServerClosedEventArgs>(new _IEventListener4_64
				(receivedEvent).OnEvent);
			server.Close();
			Assert.IsTrue(receivedEvent.value);
		}

		private sealed class _IEventListener4_64
		{
			public _IEventListener4_64(BooleanByRef receivedEvent)
			{
				this.receivedEvent = receivedEvent;
			}

			public void OnEvent(object sender, ServerClosedEventArgs args)
			{
				receivedEvent.value = true;
			}

			private readonly BooleanByRef receivedEvent;
		}

		private IEnumerator ServerMessageDispatchers()
		{
			return ((ObjectServerImpl)server).IterateDispatchers();
		}

		/// <exception cref="System.Exception"></exception>
		public override void SetUp()
		{
			fileName = TempFile();
			server = Db4oFactory.OpenServer(Db4oFactory.NewConfiguration(), fileName, -1).Ext
				();
			server.GrantAccess(Credentials(), Credentials());
		}

		/// <exception cref="System.Exception"></exception>
		public override void TearDown()
		{
			server.Close();
			base.TearDown();
		}

		private IObjectContainer OpenClient()
		{
			return Db4oFactory.OpenClient(Db4oFactory.NewConfiguration(), "localhost", Port()
				, Credentials(), Credentials());
		}

		private void AssertClientCount(int count)
		{
			Assert.AreEqual(count, server.ClientCount());
		}

		private int Port()
		{
			return server.Port();
		}

		private string Credentials()
		{
			return "DB4O";
		}
	}
}
#endif // !SILVERLIGHT
