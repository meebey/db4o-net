/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	public class CompositeIterator4 : IEnumerator
	{
		private readonly IEnumerator _iterators;

		private IEnumerator _currentIterator;

		public CompositeIterator4(IEnumerator[] iterators) : this(new ArrayIterator4(iterators
			))
		{
		}

		public CompositeIterator4(IEnumerator iterators)
		{
			if (null == iterators)
			{
				throw new ArgumentNullException();
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

		public virtual IEnumerator CurrentIterator()
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

		protected virtual IEnumerator NextIterator(object current)
		{
			return (IEnumerator)current;
		}
	}
}
