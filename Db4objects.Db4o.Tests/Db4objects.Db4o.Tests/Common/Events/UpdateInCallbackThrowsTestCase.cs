/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class UpdateInCallbackThrowsTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new UpdateInCallbackThrowsTestCase().RunAll();
		}

		public class Item
		{
			public string _name;

			public UpdateInCallbackThrowsTestCase.Item _child;

			public Item(string name) : this(name, null)
			{
			}

			public Item(string name, UpdateInCallbackThrowsTestCase.Item child)
			{
				_name = name;
				_child = child;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new UpdateInCallbackThrowsTestCase.Item("foo", new UpdateInCallbackThrowsTestCase.Item
				("bar")));
		}

		public virtual void TestReentrantUpdateThrows()
		{
			ByRef activateRaised = new ByRef();
			activateRaised.value = false;
			IEventRegistry registry = EventRegistryFactory.ForObjectContainer(Db());
			registry.Activated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_49
				(this, activateRaised).OnEvent);
			IQuery query = NewQuery(typeof(UpdateInCallbackThrowsTestCase.Item));
			query.Descend("_name").Constrain("foo");
			IObjectSet items = query.Execute();
			Assert.AreEqual(1, items.Count);
			try
			{
				items.Next();
			}
			catch (DatabaseClosedException dce)
			{
				if (!IsClientServer())
				{
					throw;
				}
			}
			Assert.IsTrue(((bool)activateRaised.value));
		}

		private sealed class _IEventListener4_49
		{
			public _IEventListener4_49(UpdateInCallbackThrowsTestCase _enclosing, ByRef activateRaised
				)
			{
				this._enclosing = _enclosing;
				this.activateRaised = activateRaised;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				object obj = ((ObjectEventArgs)args).Object;
				if (!(obj is UpdateInCallbackThrowsTestCase.Item))
				{
					return;
				}
				UpdateInCallbackThrowsTestCase.Item item = (UpdateInCallbackThrowsTestCase.Item)obj;
				if (item._name.Equals("foo"))
				{
					activateRaised.value = true;
					Assert.Expect(typeof(InvalidOperationException), new _ICodeBlock_61(this, item));
				}
			}

			private sealed class _ICodeBlock_61 : ICodeBlock
			{
				public _ICodeBlock_61(_IEventListener4_49 _enclosing, UpdateInCallbackThrowsTestCase.Item
					 item)
				{
					this._enclosing = _enclosing;
					this.item = item;
				}

				public void Run()
				{
					item._child = new UpdateInCallbackThrowsTestCase.Item("baz");
					this._enclosing._enclosing.Store(item);
				}

				private readonly _IEventListener4_49 _enclosing;

				private readonly UpdateInCallbackThrowsTestCase.Item item;
			}

			private readonly UpdateInCallbackThrowsTestCase _enclosing;

			private readonly ByRef activateRaised;
		}
	}
}
