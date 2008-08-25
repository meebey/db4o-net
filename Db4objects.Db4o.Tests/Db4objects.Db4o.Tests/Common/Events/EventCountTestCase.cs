/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class EventCountTestCase : AbstractDb4oTestCase
	{
		private const int MaxChecks = 10;

		private const long WaitTime = 10;

		private IntByRef _activated = new IntByRef(0);

		private IntByRef _updated = new IntByRef(0);

		private IntByRef _deleted = new IntByRef(0);

		private IntByRef _created = new IntByRef(0);

		private IntByRef _committed = new IntByRef(0);

		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			new EventCountTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestEventRegistryCounts()
		{
			RegisterEventHandlers();
			for (int i = 0; i < 1000; i++)
			{
				EventCountTestCase.Item item = new EventCountTestCase.Item(i);
				Db().Store(item);
				Assert.IsTrue(Db().IsStored(item));
				if (((i + 1) % 100) == 0)
				{
					Db().Commit();
				}
			}
			AssertCount(_created, 1000, "created");
			AssertCount(_committed, 10, "commit");
			ReopenAndRegister();
			IObjectSet items = NewQuery(typeof(EventCountTestCase.Item)).Execute();
			Assert.AreEqual(1000, items.Size(), "Wrong number of objects retrieved");
			while (items.HasNext())
			{
				EventCountTestCase.Item item = (EventCountTestCase.Item)items.Next();
				item._value++;
				Store(item);
			}
			AssertCount(_activated, 1000, "activated");
			AssertCount(_updated, 1000, "updated");
			items.Reset();
			while (items.HasNext())
			{
				object item = items.Next();
				Db().Delete(item);
				Assert.IsFalse(Db().IsStored(item));
			}
			AssertCount(_deleted, 1000, "deleted");
		}

		/// <exception cref="Exception"></exception>
		private void AssertCount(IntByRef @ref, int expected, string name)
		{
			for (int checkCount = 0; checkCount < MaxChecks; checkCount++)
			{
				lock (@ref)
				{
					if (@ref.value == expected)
					{
						break;
					}
					Sharpen.Runtime.Wait(@ref, WaitTime);
				}
			}
			Assert.AreEqual(expected, @ref.value, "Incorrect count for " + name);
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
			// No dedicated IncrementListener class due to sharpen event semantics
			deletionEventRegistry.Deleted += new Db4objects.Db4o.Events.ObjectEventHandler(new 
				_IEventListener4_104(this).OnEvent);
			eventRegistry.Activated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_109
				(this).OnEvent);
			eventRegistry.Committed += new Db4objects.Db4o.Events.CommitEventHandler(new _IEventListener4_114
				(this).OnEvent);
			eventRegistry.Created += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_119
				(this).OnEvent);
			eventRegistry.Updated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_124
				(this).OnEvent);
		}

		private sealed class _IEventListener4_104
		{
			public _IEventListener4_104(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				EventCountTestCase.Increment(this._enclosing._deleted);
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_109
		{
			public _IEventListener4_109(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				EventCountTestCase.Increment(this._enclosing._activated);
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_114
		{
			public _IEventListener4_114(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CommitEventArgs args)
			{
				EventCountTestCase.Increment(this._enclosing._committed);
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_119
		{
			public _IEventListener4_119(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				EventCountTestCase.Increment(this._enclosing._created);
			}

			private readonly EventCountTestCase _enclosing;
		}

		private sealed class _IEventListener4_124
		{
			public _IEventListener4_124(EventCountTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				EventCountTestCase.Increment(this._enclosing._updated);
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

		internal static void Increment(IntByRef @ref)
		{
			lock (@ref)
			{
				@ref.value++;
				Sharpen.Runtime.NotifyAll(@ref);
			}
		}
	}
}
