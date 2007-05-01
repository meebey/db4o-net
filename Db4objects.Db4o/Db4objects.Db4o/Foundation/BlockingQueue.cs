using System.Collections;
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
			_lock.Run(new _AnonymousInnerClass13(this, obj));
		}

		private sealed class _AnonymousInnerClass13 : ISafeClosure4
		{
			public _AnonymousInnerClass13(BlockingQueue _enclosing, object obj)
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
			bool hasNext = (bool)_lock.Run(new _AnonymousInnerClass23(this));
			return hasNext;
		}

		private sealed class _AnonymousInnerClass23 : ISafeClosure4
		{
			public _AnonymousInnerClass23(BlockingQueue _enclosing)
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
			return (IEnumerator)_lock.Run(new _AnonymousInnerClass32(this));
		}

		private sealed class _AnonymousInnerClass32 : ISafeClosure4
		{
			public _AnonymousInnerClass32(BlockingQueue _enclosing)
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
			return _lock.Run(new _AnonymousInnerClass40(this));
		}

		private sealed class _AnonymousInnerClass40 : ISafeClosure4
		{
			public _AnonymousInnerClass40(BlockingQueue _enclosing)
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
			_lock.Run(new _AnonymousInnerClass56(this));
		}

		private sealed class _AnonymousInnerClass56 : ISafeClosure4
		{
			public _AnonymousInnerClass56(BlockingQueue _enclosing)
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
