namespace Db4objects.Db4o.Inside.Btree
{
	public class BTreeRangeUnion : Db4objects.Db4o.Inside.Btree.IBTreeRange
	{
		private readonly Db4objects.Db4o.Inside.Btree.BTreeRangeSingle[] _ranges;

		public BTreeRangeUnion(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle[] ranges) : 
			this(ToSortedCollection(ranges))
		{
		}

		public BTreeRangeUnion(Db4objects.Db4o.Foundation.SortedCollection4 sorted)
		{
			if (null == sorted)
			{
				throw new System.ArgumentNullException();
			}
			_ranges = ToArray(sorted);
		}

		public virtual void Accept(Db4objects.Db4o.Inside.Btree.IBTreeRangeVisitor visitor
			)
		{
			visitor.Visit(this);
		}

		public virtual bool IsEmpty()
		{
			for (int i = 0; i < _ranges.Length; i++)
			{
				if (!_ranges[i].IsEmpty())
				{
					return false;
				}
			}
			return true;
		}

		private static Db4objects.Db4o.Foundation.SortedCollection4 ToSortedCollection(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle[]
			 ranges)
		{
			if (null == ranges)
			{
				throw new System.ArgumentNullException();
			}
			Db4objects.Db4o.Foundation.SortedCollection4 collection = new Db4objects.Db4o.Foundation.SortedCollection4
				(Db4objects.Db4o.Inside.Btree.BTreeRangeSingle.COMPARISON);
			for (int i = 0; i < ranges.Length; i++)
			{
				Db4objects.Db4o.Inside.Btree.BTreeRangeSingle range = ranges[i];
				if (!range.IsEmpty())
				{
					collection.Add(range);
				}
			}
			return collection;
		}

		private static Db4objects.Db4o.Inside.Btree.BTreeRangeSingle[] ToArray(Db4objects.Db4o.Foundation.SortedCollection4
			 collection)
		{
			return (Db4objects.Db4o.Inside.Btree.BTreeRangeSingle[])collection.ToArray(new Db4objects.Db4o.Inside.Btree.BTreeRangeSingle
				[collection.Size()]);
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange ExtendToFirst()
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange ExtendToLast()
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange ExtendToLastOf(Db4objects.Db4o.Inside.Btree.IBTreeRange
			 upperRange)
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Greater()
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Intersect(Db4objects.Db4o.Inside.Btree.IBTreeRange
			 range)
		{
			if (null == range)
			{
				throw new System.ArgumentNullException();
			}
			return new Db4objects.Db4o.Inside.Btree.Algebra.BTreeRangeUnionIntersect(this).Dispatch
				(range);
		}

		public virtual System.Collections.IEnumerator Pointers()
		{
			return Db4objects.Db4o.Foundation.Iterators.Concat(Db4objects.Db4o.Foundation.Iterators
				.Map(_ranges, new _AnonymousInnerClass76(this)));
		}

		private sealed class _AnonymousInnerClass76 : Db4objects.Db4o.Foundation.IFunction4
		{
			public _AnonymousInnerClass76(BTreeRangeUnion _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object range)
			{
				return ((Db4objects.Db4o.Inside.Btree.IBTreeRange)range).Pointers();
			}

			private readonly BTreeRangeUnion _enclosing;
		}

		public virtual System.Collections.IEnumerator Keys()
		{
			return Db4objects.Db4o.Foundation.Iterators.Concat(Db4objects.Db4o.Foundation.Iterators
				.Map(_ranges, new _AnonymousInnerClass84(this)));
		}

		private sealed class _AnonymousInnerClass84 : Db4objects.Db4o.Foundation.IFunction4
		{
			public _AnonymousInnerClass84(BTreeRangeUnion _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object range)
			{
				return ((Db4objects.Db4o.Inside.Btree.IBTreeRange)range).Keys();
			}

			private readonly BTreeRangeUnion _enclosing;
		}

		public virtual int Size()
		{
			int size = 0;
			for (int i = 0; i < _ranges.Length; i++)
			{
				size += _ranges[i].Size();
			}
			return size;
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Smaller()
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange Union(Db4objects.Db4o.Inside.Btree.IBTreeRange
			 other)
		{
			if (null == other)
			{
				throw new System.ArgumentNullException();
			}
			return new Db4objects.Db4o.Inside.Btree.Algebra.BTreeRangeUnionUnion(this).Dispatch
				(other);
		}

		public virtual System.Collections.IEnumerator Ranges()
		{
			return new Db4objects.Db4o.Foundation.ArrayIterator4(_ranges);
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTreePointer LastPointer()
		{
			throw new System.NotImplementedException();
		}
	}
}
