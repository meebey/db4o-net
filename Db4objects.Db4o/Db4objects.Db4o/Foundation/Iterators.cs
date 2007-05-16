/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <summary>Iterator primitives (concat, map, reduce, filter, etc...).</summary>
	/// <remarks>Iterator primitives (concat, map, reduce, filter, etc...).</remarks>
	/// <exclude></exclude>
	public class Iterators
	{
		public static readonly IEnumerator EMPTY_ITERATOR = new Iterator4Impl(null);

		private sealed class _AnonymousInnerClass15 : IEnumerable
		{
			public _AnonymousInnerClass15()
			{
			}

			public IEnumerator GetEnumerator()
			{
				return Iterators.EMPTY_ITERATOR;
			}
		}

		public static readonly IEnumerable EMPTY_ITERABLE = new _AnonymousInnerClass15();

		internal static readonly object NO_ELEMENT = new object();

		public static IEnumerator Concat(IEnumerator iterators)
		{
			return new CompositeIterator4(iterators);
		}

		public static IEnumerator Map(IEnumerator iterator, IFunction4 function)
		{
			return new FunctionApplicationIterator(iterator, function);
		}

		public static IEnumerator Map(object[] array, IFunction4 function)
		{
			return Map(new ArrayIterator4(array), function);
		}

		public static IEnumerator Filter(object[] array, IPredicate4 predicate)
		{
			return Filter(new ArrayIterator4(array), predicate);
		}

		public static IEnumerator Filter(IEnumerator iterator, IPredicate4 predicate)
		{
			return new FilteredIterator(iterator, predicate);
		}

		public static IEnumerator Iterate(object[] array)
		{
			return new ArrayIterator4(array);
		}

		public static int Size(IEnumerable iterable)
		{
			return Size(iterable.GetEnumerator());
		}

		public static object Next(IEnumerator iterator)
		{
			if (!iterator.MoveNext())
			{
				throw new InvalidOperationException();
			}
			return iterator.Current;
		}

		private static int Size(IEnumerator iterator)
		{
			int count = 0;
			while (iterator.MoveNext())
			{
				++count;
			}
			return count;
		}

		public static string ToString(IEnumerator i)
		{
			if (!i.MoveNext())
			{
				return "[]";
			}
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			sb.Append(i.Current);
			while (i.MoveNext())
			{
				sb.Append(", ");
				sb.Append(i.Current);
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
}
