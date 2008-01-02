/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TP;

namespace Db4objects.Db4o.Tests.Common.TP
{
	public class TransparentPersistenceTestCase : AbstractDb4oTestCase
	{
		public class Item : IActivatable
		{
			[System.NonSerialized]
			private IActivator _activator;

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
				Activate(ActivationPurpose.READ);
				return name;
			}

			public virtual void SetName(string newName)
			{
				Activate(ActivationPurpose.WRITE);
				name = newName;
			}

			public virtual void Activate(ActivationPurpose purpose)
			{
				_activator.Activate(purpose);
			}

			public virtual void Bind(IActivator activator)
			{
				_activator = activator;
			}

			public override string ToString()
			{
				return "Item(" + GetName() + ")";
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentActivationSupport());
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new TransparentPersistenceTestCase.Item("Foo"));
			Store(new TransparentPersistenceTestCase.Item("Bar"));
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestTransparentUpdate()
		{
			TransparentPersistenceTestCase.Item foo = ItemByName("Foo");
			foo.SetName("Bar");
			foo.SetName("Foo*");
			TransparentPersistenceTestCase.Item bar = ItemByName("Bar");
			Assert.AreEqual("Bar", bar.GetName());
			AssertUpdatedObjects(foo);
			Reopen();
			Assert.IsNotNull(ItemByName("Foo*"));
			Assert.IsNull(ItemByName("Foo"));
			Assert.IsNotNull(ItemByName("Bar"));
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
			Collection4 updated = CommitCapturingUpdatedObjects();
			Assert.AreEqual(1, updated.Size(), updated.ToString());
			Assert.AreSame(expected, updated.SingleElement());
		}

		private Collection4 CommitCapturingUpdatedObjects()
		{
			Collection4 updated = new Collection4();
			EventRegistry().Updated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_97
				(this, updated).OnEvent);
			Commit();
			return updated;
		}

		private sealed class _IEventListener4_97
		{
			public _IEventListener4_97(TransparentPersistenceTestCase _enclosing, Collection4
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

		private void Commit()
		{
			Db().Commit();
		}

		private TransparentPersistenceTestCase.Item ItemByName(string name)
		{
			IQuery q = NewQuery(typeof(TransparentPersistenceTestCase.Item));
			q.Descend("name").Constrain(name);
			IObjectSet result = q.Execute();
			if (result.HasNext())
			{
				return (TransparentPersistenceTestCase.Item)result.Next();
			}
			return null;
		}
	}
}
