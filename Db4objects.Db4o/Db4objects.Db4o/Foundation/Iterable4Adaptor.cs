using System;
using System.Collections;

namespace Db4objects.Db4o.Foundation
{
	/// <summary>
	/// Adapts Iterable4/Iterator4 iteration model (moveNext, current) to the old db4o
	/// and jdk model (hasNext, next).
	/// </summary>
	/// <remarks>
	/// Adapts Iterable4/Iterator4 iteration model (moveNext, current) to the old db4o
	/// and jdk model (hasNext, next).
	/// </remarks>
	/// <exclude></exclude>
	public class Iterable4Adaptor
	{
		private static readonly object EOF = new object();

		private static readonly object MOVE_NEXT = new object();

		private readonly IEnumerable _delegate;

		private IEnumerator _iterator;

		private object _current = MOVE_NEXT;

		public Iterable4Adaptor(IEnumerable delegate_)
		{
			_delegate = delegate_;
		}

		public virtual bool HasNext()
		{
			if (_current == MOVE_NEXT)
			{
				return MoveNext();
			}
			return _current != EOF;
		}

		public virtual object Next()
		{
			if (!HasNext())
			{
				throw new InvalidOperationException();
			}
			object returnValue = _current;
			_current = MOVE_NEXT;
			return returnValue;
		}

		private bool MoveNext()
		{
			if (null == _iterator)
			{
				_iterator = _delegate.GetEnumerator();
			}
			if (_iterator.MoveNext())
			{
				_current = _iterator.Current;
				return true;
			}
			_current = EOF;
			return false;
		}

		public virtual void Reset()
		{
			_iterator = null;
			_current = MOVE_NEXT;
		}
	}
}
