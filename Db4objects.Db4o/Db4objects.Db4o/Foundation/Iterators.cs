namespace Db4objects.Db4o.Foundation
{
	/// <summary>Iterator primitives (concat, map, reduce, filter, etc...).</summary>
	/// <remarks>Iterator primitives (concat, map, reduce, filter, etc...).</remarks>
	/// <exclude></exclude>
	public class Iterators
	{
		public static readonly System.Collections.IEnumerator EMPTY_ITERATOR = new Db4objects.Db4o.Foundation.Iterator4Impl
			(null);

		private sealed class _AnonymousInnerClass15 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass15()
			{
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return Db4objects.Db4o.Foundation.Iterators.EMPTY_ITERATOR;
			}
		}

		public static readonly System.Collections.IEnumerable EMPTY_ITERABLE = new _AnonymousInnerClass15
			();

		internal static readonly object NO_ELEMENT = new object();

		public static System.Collections.IEnumerator Concat(System.Collections.IEnumerator
			 iterators)
		{
			return new Db4objects.Db4o.Foundation.CompositeIterator4(iterators);
		}

		public static System.Collections.IEnumerator Map(System.Collections.IEnumerator iterator
			, Db4objects.Db4o.Foundation.IFunction4 function)
		{
			return new Db4objects.Db4o.Foundation.FunctionApplicationIterator(iterator, function
				);
		}

		public static System.Collections.IEnumerator Map(object[] array, Db4objects.Db4o.Foundation.IFunction4
			 function)
		{
			return Map(new Db4objects.Db4o.Foundation.ArrayIterator4(array), function);
		}

		public static System.Collections.IEnumerator Filter(object[] array, Db4objects.Db4o.Foundation.IPredicate4
			 predicate)
		{
			return Filter(new Db4objects.Db4o.Foundation.ArrayIterator4(array), predicate);
		}

		public static System.Collections.IEnumerator Filter(System.Collections.IEnumerator
			 iterator, Db4objects.Db4o.Foundation.IPredicate4 predicate)
		{
			return new Db4objects.Db4o.Foundation.FilteredIterator(iterator, predicate);
		}

		public static int Size(System.Collections.IEnumerable iterable)
		{
			return Size(iterable.GetEnumerator());
		}

		private static int Size(System.Collections.IEnumerator iterator)
		{
			int count = 0;
			while (iterator.MoveNext())
			{
				++count;
			}
			return count;
		}
	}
}
