namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTreeRangeSingle : Db4objects.Db4o.Internal.Btree.IBTreeRange
	{
		private sealed class _AnonymousInnerClass14 : Db4objects.Db4o.Foundation.IComparison4
		{
			public _AnonymousInnerClass14()
			{
			}

			public int Compare(object x, object y)
			{
				Db4objects.Db4o.Internal.Btree.BTreeRangeSingle xRange = (Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
					)x;
				Db4objects.Db4o.Internal.Btree.BTreeRangeSingle yRange = (Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
					)y;
				return xRange.First().CompareTo(yRange.First());
			}
		}

		public static readonly Db4objects.Db4o.Foundation.IComparison4 COMPARISON = new _AnonymousInnerClass14
			();

		private readonly Db4objects.Db4o.Internal.Transaction _transaction;

		private readonly Db4objects.Db4o.Internal.Btree.BTree _btree;

		private readonly Db4objects.Db4o.Internal.Btree.BTreePointer _first;

		private readonly Db4objects.Db4o.Internal.Btree.BTreePointer _end;

		public BTreeRangeSingle(Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Internal.Btree.BTree
			 btree, Db4objects.Db4o.Internal.Btree.BTreePointer first, Db4objects.Db4o.Internal.Btree.BTreePointer
			 end)
		{
			if (transaction == null || btree == null)
			{
				throw new System.ArgumentNullException();
			}
			_transaction = transaction;
			_btree = btree;
			_first = first;
			_end = end;
		}

		public virtual void Accept(Db4objects.Db4o.Internal.Btree.IBTreeRangeVisitor visitor
			)
		{
			visitor.Visit(this);
		}

		public virtual bool IsEmpty()
		{
			return Db4objects.Db4o.Internal.Btree.BTreePointer.Equals(_first, _end);
		}

		public virtual int Size()
		{
			if (IsEmpty())
			{
				return 0;
			}
			int size = 0;
			System.Collections.IEnumerator i = Keys();
			while (i.MoveNext())
			{
				++size;
			}
			return size;
		}

		public virtual System.Collections.IEnumerator Pointers()
		{
			return new Db4objects.Db4o.Internal.Btree.BTreeRangePointerIterator(this);
		}

		public virtual System.Collections.IEnumerator Keys()
		{
			return new Db4objects.Db4o.Internal.Btree.BTreeRangeKeyIterator(this);
		}

		public Db4objects.Db4o.Internal.Btree.BTreePointer End()
		{
			return _end;
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreePointer First()
		{
			return _first;
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange Greater()
		{
			return NewBTreeRangeSingle(_end, null);
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange Union(Db4objects.Db4o.Internal.Btree.IBTreeRange
			 other)
		{
			if (null == other)
			{
				throw new System.ArgumentNullException();
			}
			return new Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeSingleUnion(this).Dispatch
				(other);
		}

		public virtual bool Adjacent(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range
			)
		{
			return Db4objects.Db4o.Internal.Btree.BTreePointer.Equals(_end, range._first) || 
				Db4objects.Db4o.Internal.Btree.BTreePointer.Equals(range._end, _first);
		}

		public virtual bool Overlaps(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range
			)
		{
			return FirstOverlaps(this, range) || FirstOverlaps(range, this);
		}

		private bool FirstOverlaps(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle x, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 y)
		{
			return Db4objects.Db4o.Internal.Btree.BTreePointer.LessThan(y._first, x._end) && 
				Db4objects.Db4o.Internal.Btree.BTreePointer.LessThan(x._first, y._end);
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange ExtendToFirst()
		{
			return NewBTreeRangeSingle(FirstBTreePointer(), _end);
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange ExtendToLast()
		{
			return NewBTreeRangeSingle(_first, null);
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange Smaller()
		{
			return NewBTreeRangeSingle(FirstBTreePointer(), _first);
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreeRangeSingle NewBTreeRangeSingle
			(Db4objects.Db4o.Internal.Btree.BTreePointer first, Db4objects.Db4o.Internal.Btree.BTreePointer
			 end)
		{
			return new Db4objects.Db4o.Internal.Btree.BTreeRangeSingle(Transaction(), _btree, 
				first, end);
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange NewEmptyRange()
		{
			return NewBTreeRangeSingle(null, null);
		}

		private Db4objects.Db4o.Internal.Btree.BTreePointer FirstBTreePointer()
		{
			return Btree().FirstPointer(Transaction());
		}

		private Db4objects.Db4o.Internal.Btree.BTree Btree()
		{
			return _btree;
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange Intersect(Db4objects.Db4o.Internal.Btree.IBTreeRange
			 range)
		{
			if (null == range)
			{
				throw new System.ArgumentNullException();
			}
			return new Db4objects.Db4o.Internal.Btree.Algebra.BTreeRangeSingleIntersect(this)
				.Dispatch(range);
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange ExtendToLastOf(Db4objects.Db4o.Internal.Btree.IBTreeRange
			 range)
		{
			Db4objects.Db4o.Internal.Btree.BTreeRangeSingle rangeImpl = CheckRangeArgument(range
				);
			return NewBTreeRangeSingle(_first, rangeImpl._end);
		}

		public override string ToString()
		{
			return "BTreeRangeSingle(first=" + _first + ", end=" + _end + ")";
		}

		private Db4objects.Db4o.Internal.Btree.BTreeRangeSingle CheckRangeArgument(Db4objects.Db4o.Internal.Btree.IBTreeRange
			 range)
		{
			if (null == range)
			{
				throw new System.ArgumentNullException();
			}
			Db4objects.Db4o.Internal.Btree.BTreeRangeSingle rangeImpl = (Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
				)range;
			if (Btree() != rangeImpl.Btree())
			{
				throw new System.ArgumentException();
			}
			return rangeImpl;
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreePointer LastPointer()
		{
			if (_end == null)
			{
				return Btree().LastPointer(Transaction());
			}
			return _end.Previous();
		}
	}
}
