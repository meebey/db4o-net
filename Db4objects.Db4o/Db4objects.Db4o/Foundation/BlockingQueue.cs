/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class BlockingQueue : IQueue4
	{
		protected NonblockingQueue _queue = new NonblockingQueue();

		protected Lock4 _lock = new Lock4();

		protected bool _stopped;

		public virtual void Add(object obj)
		{
			_lock.Run(new _ISafeClosure4_16(this, obj));
		}

		private sealed class _ISafeClosure4_16 : ISafeClosure4
		{
			public _ISafeClosure4_16(BlockingQueue _enclosing, object obj)
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
			bool hasNext = (bool)_lock.Run(new _ISafeClosure4_26(this));
			return hasNext;
		}

		private sealed class _ISafeClosure4_26 : ISafeClosure4
		{
			public _ISafeClosure4_26(BlockingQueue _enclosing)
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
			return (IEnumerator)_lock.Run(new _ISafeClosure4_35(this));
		}

		private sealed class _ISafeClosure4_35 : ISafeClosure4
		{
			public _ISafeClosure4_35(BlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				return this._enclosing._queue.Iterator();
			}

			private readonly BlockingQueue _enclosing;
		}

		/// <exception cref="BlockingQueueStoppedException"></exception>
		public virtual object Next()
		{
			return _lock.Run(new _ISafeClosure4_43(this));
		}

		private sealed class _ISafeClosure4_43 : ISafeClosure4
		{
			public _ISafeClosure4_43(BlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				if (this._enclosing._queue.HasNext())
				{
					return this._enclosing._queue.Next();
				}
				if (this._enclosing._stopped)
				{
					throw new BlockingQueueStoppedException();
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
			_lock.Run(new _ISafeClosure4_62(this));
		}

		private sealed class _ISafeClosure4_62 : ISafeClosure4
		{
			public _ISafeClosure4_62(BlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				this._enclosing._stopped = true;
				this._enclosing._lock.Awake();
				return null;
			}

			private readonly BlockingQueue _enclosing;
		}

		public virtual object NextMatching(IPredicate4 condition)
		{
			return _lock.Run(new _ISafeClosure4_72(this, condition));
		}

		private sealed class _ISafeClosure4_72 : ISafeClosure4
		{
			public _ISafeClosure4_72(BlockingQueue _enclosing, IPredicate4 condition)
			{
				this._enclosing = _enclosing;
				this.condition = condition;
			}

			public object Run()
			{
				return this._enclosing._queue.NextMatching(condition);
			}

			private readonly BlockingQueue _enclosing;

			private readonly IPredicate4 condition;
		}
	}
}
