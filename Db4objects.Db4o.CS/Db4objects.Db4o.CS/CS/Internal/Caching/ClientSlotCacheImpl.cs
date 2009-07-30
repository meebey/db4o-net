/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.CS.Caching;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.CS.Internal.Caching
{
	public class ClientSlotCacheImpl : IClientSlotCache
	{
		private sealed class _TransactionLocal_14 : TransactionLocal
		{
			public _TransactionLocal_14()
			{
			}

			public override object InitialValueFor(Transaction transaction)
			{
				return new Hashtable();
			}
		}

		private readonly TransactionLocal _cache = new _TransactionLocal_14();

		public ClientSlotCacheImpl()
		{
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(((IObjectContainer
				)Environments.My(typeof(IObjectContainer))));
			eventRegistry.Activated += new System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
				(new _IEventListener4_22(this).OnEvent);
		}

		private sealed class _IEventListener4_22
		{
			public _IEventListener4_22(ClientSlotCacheImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectInfoEventArgs args
				)
			{
				this._enclosing.Purge((Transaction)((ObjectInfoEventArgs)args).Transaction(), (int
					)((ObjectInfoEventArgs)args).Info.GetInternalID());
			}

			private readonly ClientSlotCacheImpl _enclosing;
		}

		public virtual void Add(Transaction provider, int id, ByteArrayBuffer slot)
		{
			CacheOn(provider)[id] = slot;
		}

		public virtual ByteArrayBuffer Get(Transaction provider, int id)
		{
			ByteArrayBuffer buffer = ((ByteArrayBuffer)CacheOn(provider)[id]);
			if (null == buffer)
			{
				return null;
			}
			buffer.Seek(0);
			return buffer;
		}

		private void Purge(Transaction provider, int id)
		{
			Sharpen.Collections.Remove(CacheOn(provider), id);
		}

		private IDictionary CacheOn(Transaction provider)
		{
			return ((IDictionary)provider.Get(_cache).value);
		}
	}
}
