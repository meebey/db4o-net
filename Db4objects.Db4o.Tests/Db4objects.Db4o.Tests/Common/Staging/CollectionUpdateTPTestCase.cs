/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.Staging;
using Sharpen.Util;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class CollectionUpdateTPTestCase : AbstractDb4oTestCase
	{
		private const int Id1 = 1;

		private const int Id2 = 2;

		public class Item : IActivatable
		{
			[System.NonSerialized]
			public IActivator _activator;

			public int _id;

			public Item(int id)
			{
				_id = id;
			}

			public virtual void Id(int id)
			{
				_activator.Activate(ActivationPurpose.Write);
				_id = id;
			}

			public virtual int Id()
			{
				_activator.Activate(ActivationPurpose.Read);
				return _id;
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
				_activator.Activate(ActivationPurpose.Read);
				return "Item #" + _id;
			}
		}

		public class Holder
		{
			public IList _list;

			public Holder(CollectionUpdateTPTestCase.Item[] items)
			{
				_list = new ArrayList();
				Sharpen.Collections.AddAll(_list, Arrays.AsList(items));
			}

			public virtual CollectionUpdateTPTestCase.Item Item(int idx)
			{
				return ((CollectionUpdateTPTestCase.Item)_list[idx]);
			}

			public virtual void Add(CollectionUpdateTPTestCase.Item item)
			{
				_list.Add(item);
			}
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentPersistenceSupport());
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			CollectionUpdateTPTestCase.Holder holder = new CollectionUpdateTPTestCase.Holder(
				new CollectionUpdateTPTestCase.Item[] { new CollectionUpdateTPTestCase.Item(1), 
				new CollectionUpdateTPTestCase.Item(2) });
			Store(holder);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestStructureUpdate()
		{
			AssertItemUpdates(1, new _IProcedure4_81(this));
			Reopen();
			AssertHolderContent(new int[] { Id1, Id2, 3 });
		}

		private sealed class _IProcedure4_81 : IProcedure4
		{
			public _IProcedure4_81(CollectionUpdateTPTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object holder)
			{
				CollectionUpdateTPTestCase.Item item = new CollectionUpdateTPTestCase.Item(3);
				this._enclosing.Store(item);
				((CollectionUpdateTPTestCase.Holder)holder).Add(item);
				this._enclosing.Db().Store(((CollectionUpdateTPTestCase.Holder)holder), int.MaxValue
					);
			}

			private readonly CollectionUpdateTPTestCase _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestElementUpdate()
		{
			AssertItemUpdates(1, new _IProcedure4_94());
			//db().store(holder, Integer.MAX_VALUE);
			Reopen();
			AssertHolderContent(new int[] { 42, Id2 });
		}

		private sealed class _IProcedure4_94 : IProcedure4
		{
			public _IProcedure4_94()
			{
			}

			public void Apply(object holder)
			{
				((CollectionUpdateTPTestCase.Holder)holder).Item(0).Id(42);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestElementUpdateAndActivation()
		{
			AssertItemUpdates(1, new _IProcedure4_105());
			//db().store(holder, Integer.MAX_VALUE);
			Reopen();
			AssertHolderContent(new int[] { 42, Id2 });
		}

		private sealed class _IProcedure4_105 : IProcedure4
		{
			public _IProcedure4_105()
			{
			}

			public void Apply(object holder)
			{
				((CollectionUpdateTPTestCase.Holder)holder).Item(0).Id(42);
				((CollectionUpdateTPTestCase.Holder)holder).Item(1).Id();
			}
		}

		private void AssertItemUpdates(int expectedCount, IProcedure4 block)
		{
			IntByRef itemCount = new IntByRef(0);
			EventRegistry().Updated += new System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
				(new _IEventListener4_118(itemCount).OnEvent);
			CollectionUpdateTPTestCase.Holder holder = ((CollectionUpdateTPTestCase.Holder)RetrieveOnlyInstance
				(typeof(CollectionUpdateTPTestCase.Holder)));
			block.Apply(holder);
			Commit();
			Assert.AreEqual(expectedCount, itemCount.value);
		}

		private sealed class _IEventListener4_118
		{
			public _IEventListener4_118(IntByRef itemCount)
			{
				this.itemCount = itemCount;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectInfoEventArgs args
				)
			{
				if (((ObjectInfoEventArgs)args).Object is CollectionUpdateTPTestCase.Item)
				{
					Sharpen.Runtime.Out.WriteLine(((ObjectInfoEventArgs)args).Object);
					itemCount.value = itemCount.value + 1;
				}
			}

			private readonly IntByRef itemCount;
		}

		private void AssertHolderContent(int[] ids)
		{
			CollectionUpdateTPTestCase.Holder holder = ((CollectionUpdateTPTestCase.Holder)RetrieveOnlyInstance
				(typeof(CollectionUpdateTPTestCase.Holder)));
			for (int idx = 0; idx < ids.Length; idx++)
			{
				Assert.AreEqual(ids[idx], holder.Item(idx).Id());
			}
		}
	}
}
