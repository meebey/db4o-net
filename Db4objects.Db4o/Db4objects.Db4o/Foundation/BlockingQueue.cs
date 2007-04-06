using System;
using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class BlockingQueue : IQueue4
	{
		private NonblockingQueue _queue = new NonblockingQueue();

		private Lock4 _lock = new Lock4();

		public virtual void Add(object obj)
		{
			_lock.Run(new _AnonymousInnerClass15(this, obj));
		}

		private sealed class _AnonymousInnerClass15 : ISafeClosure4
		{
			public _AnonymousInnerClass15(BlockingQueue _enclosing, object obj)
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
			bool hasNext = (bool)_lock.Run(new _AnonymousInnerClass25(this));
			return hasNext;
		}

		private sealed class _AnonymousInnerClass25 : ISafeClosure4
		{
			public _AnonymousInnerClass25(BlockingQueue _enclosing)
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
			return (IEnumerator)_lock.Run(new _AnonymousInnerClass34(this));
		}

		private sealed class _AnonymousInnerClass34 : ISafeClosure4
		{
			public _AnonymousInnerClass34(BlockingQueue _enclosing)
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
			try
			{
				return _lock.Run(new _AnonymousInnerClass43(this));
			}
			catch (Exception e)
			{
				throw new Db4oUnexpectedException(e);
			}
		}

		private sealed class _AnonymousInnerClass43 : IClosure4
		{
			public _AnonymousInnerClass43(BlockingQueue _enclosing)
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
				return this._enclosing._queue.Next();
			}

			private readonly BlockingQueue _enclosing;
		}
	}
}
