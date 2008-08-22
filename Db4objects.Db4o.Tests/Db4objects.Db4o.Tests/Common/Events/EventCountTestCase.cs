/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class EventCountTestCase : AbstractDb4oTestCase
	{
		private int _activated;

		private int _updated;

		private int _deleted;

		private int _created;

		private int _committed;

		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			new EventCountTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestEventRegistryStress()
		{
			RegisterEventHandlers();
			for (int i = 0; i < 1000; i++)
			{
				EventCountTestCase.Item item = new EventCountTestCase.Item(i);
				Db().Store(item);
				Assert.IsTrue(Db().IsStored(item));
				if ((i % 100) == 9)
				{
					Db().Commit();
				}
			}
			Assert.AreEqual(1000, _created, "The counted number of created objects is not correct"
				);
			Assert.AreEqual(10, _committed, "The counted number of committed objects is not correct"
				);
			ReopenAndRegister();
			IObjectSet items = NewQuery(typeof(EventCountTestCase.Item)).Execute();
			Assert.AreEqual(1000, items.Size(), "Wrong number of objects retrieved");
			while (items.HasNext())
			{
				EventCountTestCase.Item item = (EventCountTestCase.Item)items.Next();
				item._value++;
				Store(item);
			}
			Assert.AreEqual(1000, _activated, "The counted number of activated objects is not correct"
				);
			Assert.AreEqual(1000, _updated, "The counted number of updated objects is not correct"
				);
			items.Reset();
			while (items.HasNext())
			{
				object item = items.Next();
				Db().Delete(item);
				Assert.IsFalse(Db().IsStored(item));
			}
			Assert.AreEqual(1000, _deleted, "The counted number of deleted objects is not correct"
				);
		}

		/// <exception cref="Exception"></exception>
		private void ReopenAndRegister()
		{
			Reopen();
			RegisterEventHandlers();
		}

		private void RegisterEventHandlers()
		{
			IObjectContainer deletionEventSource = Db();
			if (Fixture() is IDb4oClientServerFixture)
			{
				IDb4oClientServerFixture clientServerFixture = (IDb4oClientServerFixture)Fixture(
					);
				deletionEventSource = clientServerFixture.Server().Ext().ObjectContainer();
			}
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(Db());
			IEventRegistry deletionEventRegistry = EventRegistryFactory.ForObjectContainer(deletionEventSource
				);
			deletionEventRegistry.Deleted += new Db4objects.Db4o.Events.ObjectEventHandler(new 
				_IEventListener4_87(this).OnEvent);
			eventRegistry.Activated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_93
				(this).OnEvent);
			eventRegistry.Committed += new Db4objects.Db4o.Events.CommitEventHandler(new _IEventListener4_99
				(this).OnEvent);
			eventRegistry.Created += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_105
				(this).OnEvent);
			eventRegistry.Updated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_111
				(this).OnEvent);
		}

		private sealed class _IEventListener4_87
		{
			public _IEventListener4_87(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing._deleted++;
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_93
		{
			public _IEventListener4_93(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing._activated++;
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_99
		{
			public _IEventListener4_99(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CommitEventArgs args)
			{
				this._enclosing._committed++;
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_105
		{
			public _IEventListener4_105(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing._created++;
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_111
		{
			public _IEventListener4_111(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing._updated++;
			}

			private readonly EventCountTestCase _enclosing;
		}

		public class Item
		{
			public Item(int i)
			{
				_value = i;
			}

			public int _value;
		}
	}
}
