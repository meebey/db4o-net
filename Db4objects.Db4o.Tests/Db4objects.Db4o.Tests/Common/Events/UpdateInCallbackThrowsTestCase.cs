/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

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

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new UpdateInCallbackThrowsTestCase.Item("foo", new UpdateInCallbackThrowsTestCase.Item
				("bar")));
		}

		public virtual void TestReentrantUpdateAfterActivationThrows()
		{
			UpdateInCallbackThrowsTestCase.Item foo = ((UpdateInCallbackThrowsTestCase.Item)QueryItemsByName
				("foo").Next());
			Db().Deactivate(foo);
			IEventRegistry registry = EventRegistryFactory.ForObjectContainer(Db());
			registry.Activated += new System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
				(new _IEventListener4_44(this).OnEvent);
			Db().Activate(foo, 1);
		}

		private sealed class _IEventListener4_44
		{
			public _IEventListener4_44(UpdateInCallbackThrowsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectInfoEventArgs args
				)
			{
				object obj = ((ObjectInfoEventArgs)args).Object;
				if (!(obj is UpdateInCallbackThrowsTestCase.Item))
				{
					return;
				}
				UpdateInCallbackThrowsTestCase.Item item = (UpdateInCallbackThrowsTestCase.Item)obj;
				if (!item._name.Equals("foo"))
				{
					return;
				}
				Assert.Expect(typeof(InvalidOperationException), new _ICodeBlock_56(this, item));
			}

			private sealed class _ICodeBlock_56 : ICodeBlock
			{
				public _ICodeBlock_56(_IEventListener4_44 _enclosing, UpdateInCallbackThrowsTestCase.Item
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

				private readonly _IEventListener4_44 _enclosing;

				private readonly UpdateInCallbackThrowsTestCase.Item item;
			}

			private readonly UpdateInCallbackThrowsTestCase _enclosing;
		}

		public virtual void TestReentrantUpdateThrows()
		{
			ByRef activateRaised = new ByRef();
			activateRaised.value = false;
			IEventRegistry registry = EventRegistryFactory.ForObjectContainer(Db());
			registry.Activated += new System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
				(new _IEventListener4_71(this, activateRaised).OnEvent);
			IObjectSet items = QueryItemsByName("foo");
			Assert.AreEqual(1, items.Count);
			Assert.IsFalse((((bool)activateRaised.value)));
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
			Assert.IsTrue((((bool)activateRaised.value)));
		}

		private sealed class _IEventListener4_71
		{
			public _IEventListener4_71(UpdateInCallbackThrowsTestCase _enclosing, ByRef activateRaised
				)
			{
				this._enclosing = _enclosing;
				this.activateRaised = activateRaised;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectInfoEventArgs args
				)
			{
				object obj = ((ObjectInfoEventArgs)args).Object;
				if (!(obj is UpdateInCallbackThrowsTestCase.Item))
				{
					return;
				}
				UpdateInCallbackThrowsTestCase.Item item = (UpdateInCallbackThrowsTestCase.Item)obj;
				if (!item._name.Equals("foo"))
				{
					return;
				}
				activateRaised.value = true;
				Assert.Expect(typeof(InvalidOperationException), new _ICodeBlock_85(this, item));
			}

			private sealed class _ICodeBlock_85 : ICodeBlock
			{
				public _ICodeBlock_85(_IEventListener4_71 _enclosing, UpdateInCallbackThrowsTestCase.Item
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

				private readonly _IEventListener4_71 _enclosing;

				private readonly UpdateInCallbackThrowsTestCase.Item item;
			}

			private readonly UpdateInCallbackThrowsTestCase _enclosing;

			private readonly ByRef activateRaised;
		}

		private IObjectSet QueryItemsByName(string name)
		{
			IQuery query = NewQuery(typeof(UpdateInCallbackThrowsTestCase.Item));
			query.Descend("_name").Constrain(name);
			return query.Execute();
		}
	}
}
