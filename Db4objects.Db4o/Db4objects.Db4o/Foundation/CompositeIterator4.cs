namespace Db4objects.Db4o.Foundation
{
	public class CompositeIterator4 : System.Collections.IEnumerator
	{
		private readonly System.Collections.IEnumerator _iterators;

		private System.Collections.IEnumerator _currentIterator;

		public CompositeIterator4(System.Collections.IEnumerator[] iterators) : this(new 
			Db4objects.Db4o.Foundation.ArrayIterator4(iterators))
		{
		}

		public CompositeIterator4(System.Collections.IEnumerator iterators)
		{
			if (null == iterators)
			{
				throw new System.ArgumentNullException();
			}
			_iterators = iterators;
		}

		public virtual bool MoveNext()
		{
			if (null == _currentIterator)
			{
				if (!_iterators.MoveNext())
				{
					return false;
				}
				_currentIterator = NextIterator(_iterators.Current);
			}
			if (!_currentIterator.MoveNext())
			{
				_currentIterator = null;
				return MoveNext();
			}
			return true;
		}

		public virtual void Reset()
		{
			ResetIterators();
			_currentIterator = null;
			_iterators.Reset();
		}

		private void ResetIterators()
		{
			_iterators.Reset();
			while (_iterators.MoveNext())
			{
				NextIterator(_iterators.Current).Reset();
			}
		}

		public virtual System.Collections.IEnumerator CurrentIterator()
		{
			return _currentIterator;
		}

		public virtual object Current
		{
			get
			{
				return _currentIterator.Current;
			}
		}

		protected virtual System.Collections.IEnumerator NextIterator(object current)
		{
			return (System.Collections.IEnumerator)current;
		}
	}
}
