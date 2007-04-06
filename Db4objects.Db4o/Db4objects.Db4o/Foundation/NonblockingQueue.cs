using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <summary>
	/// Using the CollectionElement the other way around:
	/// CollectionElement.i_next points to the previous element
	/// </summary>
	/// <exclude></exclude>
	public class NonblockingQueue : IQueue4
	{
		private sealed class Queue4Iterator : IEnumerator
		{
			private bool _active = false;

			private List4 _current = null;

			public object Current
			{
				get
				{
					return this._current._element;
				}
			}

			public bool MoveNext()
			{
				if (!this._active)
				{
					this._current = this._enclosing._last;
					this._active = true;
				}
				else
				{
					if (this._current != null)
					{
						this._current = this._current._next;
					}
				}
				return this._current != null;
			}

			public void Reset()
			{
				this._current = null;
				this._active = false;
			}

			internal Queue4Iterator(NonblockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly NonblockingQueue _enclosing;
		}

		private List4 _first;

		private List4 _last;

		public void Add(object obj)
		{
			List4 ce = new List4(null, obj);
			if (_first == null)
			{
				_last = ce;
			}
			else
			{
				_first._next = ce;
			}
			_first = ce;
		}

		public object Next()
		{
			if (_last == null)
			{
				return null;
			}
			object ret = _last._element;
			_last = _last._next;
			if (_last == null)
			{
				_first = null;
			}
			return ret;
		}

		public bool HasNext()
		{
			return _last != null;
		}

		public virtual IEnumerator Iterator()
		{
			return new NonblockingQueue.Queue4Iterator(this);
		}
	}
}
