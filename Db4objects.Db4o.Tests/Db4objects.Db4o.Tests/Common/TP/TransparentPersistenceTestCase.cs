/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TP;

namespace Db4objects.Db4o.Tests.Common.TP
{
	public class TransparentPersistenceTestCase : AbstractDb4oTestCase
	{
		public class Item : ActivatableImpl
		{
			public string name;

			public Item()
			{
			}

			public Item(string initialName)
			{
				name = initialName;
			}

			public virtual string GetName()
			{
				Activate(ActivationPurpose.Read);
				return name;
			}

			public virtual void SetName(string newName)
			{
				Activate(ActivationPurpose.Write);
				name = newName;
			}

			public override string ToString()
			{
				return "Item(" + GetName() + ")";
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentPersistenceSupport());
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new TransparentPersistenceTestCase.Item("Foo"));
			Store(new TransparentPersistenceTestCase.Item("Bar"));
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestActivateOnWrite()
		{
			TransparentPersistenceTestCase.Item foo = ItemByName("Foo");
			foo.SetName("Foo*");
			Assert.AreEqual("Foo*", foo.GetName());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestConcurrentClientModification()
		{
			if (!IsClientServer())
			{
				return;
			}
			IExtObjectContainer client1 = Db();
			IExtObjectContainer client2 = OpenNewClient();
			try
			{
				TransparentPersistenceTestCase.Item foo1 = ItemByName(client1, "Foo");
				foo1.SetName("Foo*");
				TransparentPersistenceTestCase.Item foo2 = ItemByName(client2, "Foo");
				foo2.SetName("Foo**");
				AssertUpdatedObjects(client1, foo1);
				AssertUpdatedObjects(client2, foo2);
				client1.Refresh(foo1, 1);
				Assert.AreEqual(foo2.GetName(), foo1.GetName());
			}
			finally
			{
				client2.Close();
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestTransparentUpdate()
		{
			TransparentPersistenceTestCase.Item foo = ItemByName("Foo");
			foo.SetName("Bar");
			// changing more than once shouldn't be a problem
			foo.SetName("Foo*");
			TransparentPersistenceTestCase.Item bar = ItemByName("Bar");
			Assert.AreEqual("Bar", bar.GetName());
			// accessed but not changed
			AssertUpdatedObjects(foo);
			Reopen();
			Assert.IsNotNull(ItemByName("Foo*"));
			Assert.IsNull(ItemByName("Foo"));
			Assert.IsNotNull(ItemByName("Bar"));
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestChangedAfterCommit()
		{
			TransparentPersistenceTestCase.Item item = ItemByName("Foo");
			item.SetName("Bar");
			AssertUpdatedObjects(item);
			item.SetName("Foo");
			AssertUpdatedObjects(item);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestUpdateAfterActivation()
		{
			TransparentPersistenceTestCase.Item foo = ItemByName("Foo");
			Assert.AreEqual("Foo", foo.GetName());
			foo.SetName("Foo*");
			AssertUpdatedObjects(foo);
		}

		private void AssertUpdatedObjects(TransparentPersistenceTestCase.Item expected)
		{
			AssertUpdatedObjects(Db(), expected);
		}

		private void AssertUpdatedObjects(IExtObjectContainer container, TransparentPersistenceTestCase.Item
			 expected)
		{
			Collection4 updated = CommitCapturingUpdatedObjects(container);
			Assert.AreEqual(1, updated.Size(), updated.ToString());
			Assert.AreSame(expected, updated.SingleElement());
		}

		private Collection4 CommitCapturingUpdatedObjects(IExtObjectContainer container)
		{
			Collection4 updated = new Collection4();
			EventRegistryFor(container).Updated += new Db4objects.Db4o.Events.ObjectEventHandler
				(new _IEventListener4_134(this, updated).OnEvent);
			container.Commit();
			return updated;
		}

		private sealed class _IEventListener4_134
		{
			public _IEventListener4_134(TransparentPersistenceTestCase _enclosing, Collection4
				 updated)
			{
				this._enclosing = _enclosing;
				this.updated = updated;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				ObjectEventArgs objectArgs = (ObjectEventArgs)args;
				updated.Add(objectArgs.Object);
			}

			private readonly TransparentPersistenceTestCase _enclosing;

			private readonly Collection4 updated;
		}

		private TransparentPersistenceTestCase.Item ItemByName(string name)
		{
			return ItemByName(Db(), name);
		}

		private TransparentPersistenceTestCase.Item ItemByName(IExtObjectContainer container
			, string name)
		{
			IQuery q = NewQuery(container, typeof(TransparentPersistenceTestCase.Item));
			q.Descend("name").Constrain(name);
			IObjectSet result = q.Execute();
			if (result.HasNext())
			{
				return (TransparentPersistenceTestCase.Item)result.Next();
			}
			return null;
		}

		private IExtObjectContainer OpenNewClient()
		{
			return ((IDb4oClientServerFixture)this.Fixture()).OpenNewClient();
		}
	}
}
