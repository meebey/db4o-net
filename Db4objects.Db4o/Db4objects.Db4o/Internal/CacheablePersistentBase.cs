/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Caching;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class CacheablePersistentBase : PersistentBase
	{
		protected override ByteArrayBuffer ProduceReadBuffer(Transaction trans)
		{
			ByteArrayBuffer buffer = ((ByteArrayBuffer)Cache(trans).Produce(GetID(), ReadProducer
				(trans), null));
			buffer.Seek(0);
			return buffer;
		}

		private IFunction4 ReadProducer(Transaction trans)
		{
			return new _IFunction4_21(this, trans);
		}

		private sealed class _IFunction4_21 : IFunction4
		{
			public _IFunction4_21(CacheablePersistentBase _enclosing, Transaction trans)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
			}

			public object Apply(object id)
			{
				return this._enclosing.ReadBufferById(trans);
			}

			private readonly CacheablePersistentBase _enclosing;

			private readonly Transaction trans;
		}

		protected override ByteArrayBuffer ProduceWriteBuffer(Transaction trans, int length
			)
		{
			ByteArrayBuffer buffer = ((ByteArrayBuffer)Cache(trans).Produce(GetID(), WriterProducer
				(length), null));
			buffer.EnsureSize(length);
			buffer.Seek(0);
			return buffer;
		}

		private IFunction4 WriterProducer(int length)
		{
			return new _IFunction4_36(this, length);
		}

		private sealed class _IFunction4_36 : IFunction4
		{
			public _IFunction4_36(CacheablePersistentBase _enclosing, int length)
			{
				this._enclosing = _enclosing;
				this.length = length;
			}

			public object Apply(object id)
			{
				return this._enclosing.NewWriteBuffer(length);
			}

			private readonly CacheablePersistentBase _enclosing;

			private readonly int length;
		}

		private ICache4 Cache(Transaction trans)
		{
			return ((LocalTransaction)trans).SlotCache();
		}
	}
}
