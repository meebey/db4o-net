/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.CS.Caching;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.CS.Caching;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.CS.Caching
{
	public class ClientSlotCacheTestCase : AbstractDb4oTestCase, IOptOutAllButNetworkingCS
	{
		public virtual void TestSlotCacheIsTransactionBased()
		{
			Container().WithEnvironment(new _IRunnable_16(this));
		}

		private sealed class _IRunnable_16 : IRunnable
		{
			public _IRunnable_16(ClientSlotCacheTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Transaction t1 = this._enclosing.NewTransaction();
				Transaction t2 = this._enclosing.NewTransaction();
				IClientSlotCache subject = ((IClientSlotCache)Environments.My(typeof(IClientSlotCache
					)));
				ByteArrayBuffer slot = new ByteArrayBuffer(0);
				subject.Add(t1, 42, slot);
				Assert.AreSame(slot, subject.Get(t1, 42));
				Assert.IsNull(subject.Get(t2, 42));
				t1.Commit();
				Assert.IsNull(subject.Get(t1, 42));
			}

			private readonly ClientSlotCacheTestCase _enclosing;
		}

		public virtual void TestCacheIsCleanUponTransactionCommit()
		{
			AssertCacheIsCleanAfterTransactionOperation(new _IProcedure4_37());
		}

		private sealed class _IProcedure4_37 : IProcedure4
		{
			public _IProcedure4_37()
			{
			}

			public void Apply(object value)
			{
				((Transaction)value).Commit();
			}
		}

		public virtual void TestCacheIsCleanUponTransactionRollback()
		{
			AssertCacheIsCleanAfterTransactionOperation(new _IProcedure4_46());
		}

		private sealed class _IProcedure4_46 : IProcedure4
		{
			public _IProcedure4_46()
			{
			}

			public void Apply(object value)
			{
				((Transaction)value).Rollback();
			}
		}

		private void AssertCacheIsCleanAfterTransactionOperation(IProcedure4 operation)
		{
			Container().WithEnvironment(new _IRunnable_54(this, operation));
		}

		private sealed class _IRunnable_54 : IRunnable
		{
			public _IRunnable_54(ClientSlotCacheTestCase _enclosing, IProcedure4 operation)
			{
				this._enclosing = _enclosing;
				this.operation = operation;
			}

			public void Run()
			{
				IClientSlotCache subject = ((IClientSlotCache)Environments.My(typeof(IClientSlotCache
					)));
				ByteArrayBuffer slot = new ByteArrayBuffer(0);
				subject.Add(this._enclosing.Trans(), 42, slot);
				operation.Apply(this._enclosing.Trans());
				Assert.IsNull(subject.Get(this._enclosing.Trans(), 42));
			}

			private readonly ClientSlotCacheTestCase _enclosing;

			private readonly IProcedure4 operation;
		}

		public virtual void TestSlotCacheEntryIsPurgedUponActivation()
		{
			ClientSlotCacheTestCase.Item item = new ClientSlotCacheTestCase.Item();
			Db().Store(item);
			int id = (int)Db().GetID(item);
			Db().Purge(item);
			Db().Configure().ClientServer().PrefetchDepth(1);
			Container().WithEnvironment(new _IRunnable_77(this, id));
		}

		private sealed class _IRunnable_77 : IRunnable
		{
			public _IRunnable_77(ClientSlotCacheTestCase _enclosing, int id)
			{
				this._enclosing = _enclosing;
				this.id = id;
			}

			public void Run()
			{
				IClientSlotCache subject = ((IClientSlotCache)Environments.My(typeof(IClientSlotCache
					)));
				IObjectSet items = this._enclosing.NewQuery(typeof(ClientSlotCacheTestCase.Item))
					.Execute();
				Assert.IsNotNull(subject.Get(this._enclosing.Trans(), id));
				Assert.IsNotNull(((ClientSlotCacheTestCase.Item)items.Next()));
				Assert.IsNull(subject.Get(this._enclosing.Trans(), id), "activation should have purged slot from cache"
					);
			}

			private readonly ClientSlotCacheTestCase _enclosing;

			private readonly int id;
		}

		public class Item
		{
		}
	}
}
#endif // !SILVERLIGHT
