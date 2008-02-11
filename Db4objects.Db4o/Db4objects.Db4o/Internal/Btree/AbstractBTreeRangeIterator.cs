/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Internal.Btree
{
	public abstract class AbstractBTreeRangeIterator : IEnumerator
	{
		private readonly BTreeRangeSingle _range;

		private BTreePointer _cursor;

		private BTreePointer _current;

		public AbstractBTreeRangeIterator(BTreeRangeSingle range)
		{
			_range = range;
			_cursor = range.First();
		}

		public virtual bool MoveNext()
		{
			if (ReachedEnd(_cursor))
			{
				_current = null;
				return false;
			}
			_current = _cursor;
			_cursor = _cursor.Next();
			return true;
		}

		public virtual void Reset()
		{
			_cursor = _range.First();
		}

		protected virtual BTreePointer CurrentPointer()
		{
			if (null == _current)
			{
				throw new InvalidOperationException();
			}
			return _current;
		}

		private bool ReachedEnd(BTreePointer cursor)
		{
			if (cursor == null)
			{
				return true;
			}
			if (_range.End() == null)
			{
				return false;
			}
			return _range.End().Equals(cursor);
		}

		public abstract object Current
		{
			get;
		}
	}
}
