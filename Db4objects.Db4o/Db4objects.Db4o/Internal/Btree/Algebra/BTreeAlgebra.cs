namespace Db4objects.Db4o.Internal.Btree.Algebra
{
	/// <exclude></exclude>
	internal class BTreeAlgebra
	{
		public static Db4objects.Db4o.Internal.Btree.IBTreeRange Intersect(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion
			 union, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle single)
		{
			Db4objects.Db4o.Foundation.SortedCollection4 collection = NewBTreeRangeSingleCollection
				();
			CollectIntersections(collection, union, single);
			return ToRange(collection);
		}

		public static Db4objects.Db4o.Internal.Btree.IBTreeRange Intersect(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion
			 union1, Db4objects.Db4o.Internal.Btree.BTreeRangeUnion union2)
		{
			Db4objects.Db4o.Foundation.SortedCollection4 collection = NewBTreeRangeSingleCollection
				();
			System.Collections.IEnumerator ranges = union1.Ranges();
			while (ranges.MoveNext())
			{
				Db4objects.Db4o.Internal.Btree.BTreeRangeSingle current = (Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
					)ranges.Current;
				CollectIntersections(collection, union2, current);
			}
			return ToRange(collection);
		}

		private static void CollectIntersections(Db4objects.Db4o.Foundation.SortedCollection4
			 collection, Db4objects.Db4o.Internal.Btree.BTreeRangeUnion union, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 single)
		{
			System.Collections.IEnumerator ranges = union.Ranges();
			while (ranges.MoveNext())
			{
				Db4objects.Db4o.Internal.Btree.BTreeRangeSingle current = (Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
					)ranges.Current;
				if (single.Overlaps(current))
				{
					collection.Add(single.Intersect(current));
				}
			}
		}

		public static Db4objects.Db4o.Internal.Btree.IBTreeRange Intersect(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 single1, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle single2)
		{
			Db4objects.Db4o.Internal.Btree.BTreePointer first = Db4objects.Db4o.Internal.Btree.BTreePointer
				.Max(single1.First(), single2.First());
			Db4objects.Db4o.Internal.Btree.BTreePointer end = Db4objects.Db4o.Internal.Btree.BTreePointer
				.Min(single1.End(), single2.End());
			return single1.NewBTreeRangeSingle(first, end);
		}

		public static Db4objects.Db4o.Internal.Btree.IBTreeRange Union(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion
			 union1, Db4objects.Db4o.Internal.Btree.BTreeRangeUnion union2)
		{
			System.Collections.IEnumerator ranges = union1.Ranges();
			Db4objects.Db4o.Internal.Btree.IBTreeRange merged = union2;
			while (ranges.MoveNext())
			{
				merged = merged.Union((Db4objects.Db4o.Internal.Btree.IBTreeRange)ranges.Current);
			}
			return merged;
		}

		public static Db4objects.Db4o.Internal.Btree.IBTreeRange Union(Db4objects.Db4o.Internal.Btree.BTreeRangeUnion
			 union, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle single)
		{
			if (single.IsEmpty())
			{
				return union;
			}
			Db4objects.Db4o.Foundation.SortedCollection4 sorted = NewBTreeRangeSingleCollection
				();
			sorted.Add(single);
			Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range = single;
			System.Collections.IEnumerator ranges = union.Ranges();
			while (ranges.MoveNext())
			{
				Db4objects.Db4o.Internal.Btree.BTreeRangeSingle current = (Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
					)ranges.Current;
				if (CanBeMerged(current, range))
				{
					sorted.Remove(range);
					range = Merge(current, range);
					sorted.Add(range);
				}
				else
				{
					sorted.Add(current);
				}
			}
			return ToRange(sorted);
		}

		private static Db4objects.Db4o.Internal.Btree.IBTreeRange ToRange(Db4objects.Db4o.Foundation.SortedCollection4
			 sorted)
		{
			if (1 == sorted.Size())
			{
				return (Db4objects.Db4o.Internal.Btree.IBTreeRange)sorted.SingleElement();
			}
			return new Db4objects.Db4o.Internal.Btree.BTreeRangeUnion(sorted);
		}

		private static Db4objects.Db4o.Foundation.SortedCollection4 NewBTreeRangeSingleCollection
			()
		{
			return new Db4objects.Db4o.Foundation.SortedCollection4(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
				.COMPARISON);
		}

		public static Db4objects.Db4o.Internal.Btree.IBTreeRange Union(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 single1, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle single2)
		{
			if (single1.IsEmpty())
			{
				return single2;
			}
			if (single2.IsEmpty())
			{
				return single1;
			}
			if (CanBeMerged(single1, single2))
			{
				return Merge(single1, single2);
			}
			return new Db4objects.Db4o.Internal.Btree.BTreeRangeUnion(new Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
				[] { single1, single2 });
		}

		private static Db4objects.Db4o.Internal.Btree.BTreeRangeSingle Merge(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle
			 range1, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range2)
		{
			return range1.NewBTreeRangeSingle(Db4objects.Db4o.Internal.Btree.BTreePointer.Min
				(range1.First(), range2.First()), Db4objects.Db4o.Internal.Btree.BTreePointer.Max
				(range1.End(), range2.End()));
		}

		private static bool CanBeMerged(Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range1
			, Db4objects.Db4o.Internal.Btree.BTreeRangeSingle range2)
		{
			return range1.Overlaps(range2) || range1.Adjacent(range2);
		}
	}
}
