/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Acid;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class ReadCommittedIsolationTestCase : AbstractDb4oTestCase, IOptOutSolo
	{
		private readonly object _updatesMonitor = new object();

		private static readonly string Original = "original";

		private static readonly string Modified = "modified";

		private IExtObjectContainer _client2;

		// We introduce this variable to be able to wait for completion.
		// For a real usecase it is not necessary.
		public static void Main(string[] arguments)
		{
			new ReadCommittedIsolationTestCase().RunAll();
		}

		public class Item
		{
			public string name;

			public Item(string name_)
			{
				name = name_;
			}

			public override string ToString()
			{
				return "Item: " + name;
			}
		}

		public virtual void TestRefresh()
		{
			ReadCommittedIsolationTestCase.Item item2 = RetrieveOnlyInstance(Client2());
			Assert.AreEqual(Original, item2.name);
			ReadCommittedIsolationTestCase.Item item1 = RetrieveOnlyInstance(Client1());
			Assert.AreEqual(Original, item1.name);
			item1.name = Modified;
			Client1().Store(item1);
			Client1().Commit();
			Assert.AreEqual(Original, item2.name);
			Client2().Refresh(item2, 2);
			Assert.AreEqual(Modified, item2.name);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestPushedUpdates()
		{
			RegisterPushedUpdates(Client2());
			ReadCommittedIsolationTestCase.Item item2 = RetrieveOnlyInstance(Client2());
			Assert.AreEqual(Original, item2.name);
			ReadCommittedIsolationTestCase.Item item1 = RetrieveOnlyInstance(Client1());
			Assert.AreNotSame(item2, item1);
			Assert.AreEqual(Original, item1.name);
			item1.name = Modified;
			Client1().Store(item1);
			lock (_updatesMonitor)
			{
				Client1().Commit();
				if (IsNetworkingCS())
				{
					Sharpen.Runtime.Wait(_updatesMonitor, 1000);
				}
			}
			Assert.AreEqual(Modified, item2.name);
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			IDb4oClientServerFixture fixture = (IDb4oClientServerFixture)Fixture();
			_client2 = fixture.OpenNewClient();
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oTearDownBeforeClean()
		{
			_client2.Close();
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new ReadCommittedIsolationTestCase.Item(Original));
		}

		private IExtObjectContainer Client1()
		{
			return Db();
		}

		private IExtObjectContainer Client2()
		{
			return _client2;
		}

		private ReadCommittedIsolationTestCase.Item RetrieveOnlyInstance(IExtObjectContainer
			 container)
		{
			IQuery q = container.Query();
			q.Constrain(typeof(ReadCommittedIsolationTestCase.Item));
			IObjectSet objectSet = q.Execute();
			Assert.AreEqual(1, objectSet.Count);
			return (ReadCommittedIsolationTestCase.Item)objectSet.Next();
		}

		private bool IsNetworkingCS()
		{
			return Client2() is ClientObjectContainer;
		}

		private void RegisterPushedUpdates(IExtObjectContainer client)
		{
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(client);
			eventRegistry.Committed += new Db4objects.Db4o.Events.CommitEventHandler(new _IEventListener4_117
				(this, client).OnEvent);
		}

		private sealed class _IEventListener4_117
		{
			public _IEventListener4_117(ReadCommittedIsolationTestCase _enclosing, IExtObjectContainer
				 client)
			{
				this._enclosing = _enclosing;
				this.client = client;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CommitEventArgs args)
			{
				lock (this._enclosing._updatesMonitor)
				{
					Transaction trans = ((IInternalObjectContainer)client).Transaction();
					IObjectInfoCollection updated = ((CommitEventArgs)args).Updated;
					IEnumerator infos = updated.GetEnumerator();
					while (infos.MoveNext())
					{
						IObjectInfo info = (IObjectInfo)infos.Current;
						object obj = trans.ObjectForIdFromCache((int)info.GetInternalID());
						if (obj == null)
						{
							continue;
						}
						// DEPTH may need to be 2 for member collections
						// to be updated also.
						client.Refresh(obj, 1);
					}
					if (this._enclosing.IsNetworkingCS())
					{
						Sharpen.Runtime.NotifyAll(this._enclosing._updatesMonitor);
					}
				}
			}

			private readonly ReadCommittedIsolationTestCase _enclosing;

			private readonly IExtObjectContainer client;
		}
	}
}
