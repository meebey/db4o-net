/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class BlockingQueue : IQueue4
	{
		protected NonblockingQueue _queue = new NonblockingQueue();

		protected Lock4 _lock = new Lock4();

		public virtual void Add(object obj)
		{
			_lock.Run(new _ISafeClosure4_14(this, obj));
		}

		private sealed class _ISafeClosure4_14 : ISafeClosure4
		{
			public _ISafeClosure4_14(BlockingQueue _enclosing, object obj)
			{
				this._enclosing = _enclosing;
				this.obj = obj;
			}

			public object Run()
			{
				this._enclosing._queue.Add(obj);
				this._enclosing._lock.Awake();
				return null;
			}

			private readonly BlockingQueue _enclosing;

			private readonly object obj;
		}

		public virtual bool HasNext()
		{
			bool hasNext = (bool)_lock.Run(new _ISafeClosure4_24(this));
			return hasNext;
		}

		private sealed class _ISafeClosure4_24 : ISafeClosure4
		{
			public _ISafeClosure4_24(BlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				return this._enclosing._queue.HasNext();
			}

			private readonly BlockingQueue _enclosing;
		}

		public virtual IEnumerator Iterator()
		{
			return (IEnumerator)_lock.Run(new _ISafeClosure4_33(this));
		}

		private sealed class _ISafeClosure4_33 : ISafeClosure4
		{
			public _ISafeClosure4_33(BlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				return this._enclosing._queue.Iterator();
			}

			private readonly BlockingQueue _enclosing;
		}

		public virtual object Next()
		{
			return _lock.Run(new _ISafeClosure4_41(this));
		}

		private sealed class _ISafeClosure4_41 : ISafeClosure4
		{
			public _ISafeClosure4_41(BlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				if (this._enclosing._queue.HasNext())
				{
					return this._enclosing._queue.Next();
				}
				this._enclosing._lock.Snooze(int.MaxValue);
				object obj = this._enclosing._queue.Next();
				if (obj == null)
				{
					throw new BlockingQueueStoppedException();
				}
				return obj;
			}

			private readonly BlockingQueue _enclosing;
		}

		public virtual void Stop()
		{
			_lock.Run(new _ISafeClosure4_57(this));
		}

		private sealed class _ISafeClosure4_57 : ISafeClosure4
		{
			public _ISafeClosure4_57(BlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				this._enclosing._lock.Awake();
				return null;
			}

			private readonly BlockingQueue _enclosing;
		}
	}
}
