using System;
using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public abstract class MappingIterator : IEnumerator
	{
		private readonly IEnumerator _iterator;

		private object _current;

		public static readonly object SKIP = new object();

		public MappingIterator(IEnumerator iterator)
		{
			if (null == iterator)
			{
				throw new ArgumentNullException();
			}
			_iterator = iterator;
			_current = Iterators.NO_ELEMENT;
		}

		protected abstract object Map(object current);

		public virtual bool MoveNext()
		{
			do
			{
				if (!_iterator.MoveNext())
				{
					_current = Iterators.NO_ELEMENT;
					return false;
				}
				_current = Map(_iterator.Current);
			}
			while (_current == SKIP);
			return true;
		}

		public virtual void Reset()
		{
			_current = Iterators.NO_ELEMENT;
			_iterator.Reset();
		}

		public virtual object Current
		{
			get
			{
				if (Iterators.NO_ELEMENT == _current)
				{
					throw new InvalidOperationException();
				}
				return _current;
			}
		}
	}
}
