namespace Db4objects.Db4o.Internal.Btree
{
	public abstract class AbstractBTreeRangeIterator : System.Collections.IEnumerator
	{
		private readonly Db4objects.Db4o.Internal.Btree.BTreeRangeSingle _range;

		private Db4objects.Db4o.Internal.Btree.BTreePointer _cursor;

		private Db4objects.Db4o.Internal.Btree.BTreePointer _current;

		public AbstractBTreeRangeIterator(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 range)
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

		protected virtual Db4objects.Db4o.Internal.Btree.BTreePointer CurrentPointer()
		{
			if (null == _current)
			{
				throw new System.InvalidOperationException();
			}
			return _current;
		}

		private bool ReachedEnd(Db4objects.Db4o.Internal.Btree.BTreePointer cursor)
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
