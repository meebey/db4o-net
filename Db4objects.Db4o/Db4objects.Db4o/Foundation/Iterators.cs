/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
		/// <summary>
		/// Constant indicating that the current element in a
		/// <see cref="Iterators.Map">Iterators.Map</see>
		/// operation
		/// should be skipped.
		/// </summary>
		public static readonly object Skip = new object();

		private sealed class _IEnumerator_18 : IEnumerator
		{
			public _IEnumerator_18()
			{
			}

			public object Current
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public bool MoveNext()
			{
				return false;
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}
		}

		public static readonly IEnumerator EmptyIterator = new _IEnumerator_18();

		private sealed class _IEnumerable_32 : IEnumerable
		{
			public _IEnumerable_32()
			{
			}

			public IEnumerator GetEnumerator()
			{
				return Iterators.EmptyIterator;
			}
		}

		public static readonly IEnumerable EmptyIterable = new _IEnumerable_32();

		internal static readonly object NoElement = new object();

		/// <summary>
		/// Generates
		/// <see cref="EnumerateIterator.Tuple">EnumerateIterator.Tuple</see>
		/// items with indexes starting at 0.
		/// </summary>
		/// <param name="iterable">the iterable to be enumerated</param>
		public static IEnumerable Enumerate(IEnumerable iterable)
		{
			return new _IEnumerable_46(iterable);
		}

		private sealed class _IEnumerable_46 : IEnumerable
		{
			public _IEnumerable_46(IEnumerable iterable)
			{
				this.iterable = iterable;
			}

			public IEnumerator GetEnumerator()
			{
				return new EnumerateIterator(iterable.GetEnumerator());
			}

			private readonly IEnumerable iterable;
		}

		public static IEnumerator Concat(IEnumerator[] array)
		{
			return Concat(Iterate(array));
		}

		public static IEnumerator Concat(IEnumerator iterators)
		{
			return new CompositeIterator4(iterators);
		}

		public static IEnumerable Concat(IEnumerable[] iterables)
		{
			return Concat(Iterable(iterables));
		}

		public static IEnumerable Concat(IEnumerable iterables)
		{
			return new CompositeIterable4(iterables);
		}

		public static IEnumerator Concat(IEnumerator first, IEnumerator second)
		{
			return Concat(new IEnumerator[] { first, second });
		}

		public static IEnumerable ConcatMap(IEnumerable iterable, IFunction4 function)
		{
			return Concat(Map(iterable, function));
		}

		/// <summary>
		/// Returns a new iterator which yields the result of applying the function
		/// to every element in the original iterator.
		/// </summary>
		/// <remarks>
		/// Returns a new iterator which yields the result of applying the function
		/// to every element in the original iterator.
		/// <see cref="Iterators.Skip">Iterators.Skip</see>
		/// can be returned from function to indicate the current
		/// element should be skipped.
		/// </remarks>
		/// <param name="iterator"></param>
		/// <param name="function"></param>
		/// <returns></returns>
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

		public static IEnumerable Cons(object element)
		{
			return new _IEnumerable_105(element);
		}

		private sealed class _IEnumerable_105 : IEnumerable
		{
			public _IEnumerable_105(object element)
			{
				this.element = element;
			}

			public IEnumerator GetEnumerator()
			{
				return Iterators.IterateSingle(element);
			}

			private readonly object element;
		}

		public static IEnumerable Cons(IEnumerable front, object last)
		{
			return Concat(Iterable(new object[] { front, Cons(last) }));
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

		public static int Size(IEnumerator iterator)
		{
			int count = 0;
			while (iterator.MoveNext())
			{
				++count;
			}
			return count;
		}

		public static string ToString(IEnumerable i)
		{
			return ToString(i.GetEnumerator());
		}

		public static string ToString(IEnumerator i)
		{
			return Join(i, "[", "]", ", ");
		}

		public static string Join(IEnumerator i, string separator)
		{
			return Join(i, string.Empty, string.Empty, separator);
		}

		public static string Join(IEnumerator i, string prefix, string suffix, string separator
			)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(prefix);
			if (i.MoveNext())
			{
				sb.Append(i.Current);
				while (i.MoveNext())
				{
					sb.Append(separator);
					sb.Append(i.Current);
				}
			}
			sb.Append(suffix);
			return sb.ToString();
		}

		public static object[] ToArray(IEnumerator tests)
		{
			return ToArray(tests, new _IArrayFactory_167());
		}

		private sealed class _IArrayFactory_167 : IArrayFactory
		{
			public _IArrayFactory_167()
			{
			}

			public object[] NewArray(int size)
			{
				return new object[size];
			}
		}

		public static object[] ToArray(IEnumerator tests, IArrayFactory factory)
		{
			Collection4 elements = new Collection4(tests);
			return elements.ToArray(factory.NewArray(elements.Size()));
		}

		/// <summary>Yields a flat sequence of elements.</summary>
		/// <remarks>
		/// Yields a flat sequence of elements. Any
		/// <see cref="IEnumerable">IEnumerable</see>
		/// or
		/// <see cref="IEnumerator">IEnumerator</see>
		/// found in the original sequence is recursively flattened.
		/// </remarks>
		/// <param name="iterable">original sequence</param>
		/// <returns></returns>
		public static IEnumerator Flatten(IEnumerator iterator)
		{
			return new FlatteningIterator(iterator);
		}

		public static IEnumerable Map(IEnumerable iterable, IFunction4 function)
		{
			return new _IEnumerable_191(iterable, function);
		}

		private sealed class _IEnumerable_191 : IEnumerable
		{
			public _IEnumerable_191(IEnumerable iterable, IFunction4 function)
			{
				this.iterable = iterable;
				this.function = function;
			}

			public IEnumerator GetEnumerator()
			{
				return Iterators.Map(iterable.GetEnumerator(), function);
			}

			private readonly IEnumerable iterable;

			private readonly IFunction4 function;
		}

		public static IEnumerable CrossProduct(IEnumerable iterables)
		{
			return CrossProduct((IEnumerable[])ToArray(iterables.GetEnumerator(), new _IArrayFactory_199
				()));
		}

		private sealed class _IArrayFactory_199 : IArrayFactory
		{
			public _IArrayFactory_199()
			{
			}

			public object[] NewArray(int size)
			{
				return new IEnumerable[size];
			}
		}

		public static IEnumerable CrossProduct(IEnumerable[] iterables)
		{
			return CrossProduct(iterables, 0, Iterators.EmptyIterable);
		}

		private static IEnumerable CrossProduct(IEnumerable[] iterables, int level, IEnumerable
			 row)
		{
			if (level == iterables.Length - 1)
			{
				return Map(iterables[level], new _IFunction4_214(row));
			}
			return ConcatMap(iterables[level], new _IFunction4_222(iterables, level, row));
		}

		private sealed class _IFunction4_214 : IFunction4
		{
			public _IFunction4_214(IEnumerable row)
			{
				this.row = row;
			}

			public object Apply(object arg)
			{
				return Iterators.Cons(row, arg);
			}

			private readonly IEnumerable row;
		}

		private sealed class _IFunction4_222 : IFunction4
		{
			public _IFunction4_222(IEnumerable[] iterables, int level, IEnumerable row)
			{
				this.iterables = iterables;
				this.level = level;
				this.row = row;
			}

			public object Apply(object arg)
			{
				return Iterators.CrossProduct(iterables, level + 1, Iterators.Cons(row, arg));
			}

			private readonly IEnumerable[] iterables;

			private readonly int level;

			private readonly IEnumerable row;
		}

		public static IEnumerable Iterable(object[] objects)
		{
			return new _IEnumerable_230(objects);
		}

		private sealed class _IEnumerable_230 : IEnumerable
		{
			public _IEnumerable_230(object[] objects)
			{
				this.objects = objects;
			}

			public IEnumerator GetEnumerator()
			{
				return Iterators.Iterate(objects);
			}

			private readonly object[] objects;
		}

		public static IEnumerator IterateSingle(object element)
		{
			return new SingleValueIterator(element);
		}

		public static IEnumerable Iterable(IEnumerator iterator)
		{
			return new _IEnumerable_242(iterator);
		}

		private sealed class _IEnumerable_242 : IEnumerable
		{
			public _IEnumerable_242(IEnumerator iterator)
			{
				this.iterator = iterator;
			}

			public IEnumerator GetEnumerator()
			{
				return iterator;
			}

			private readonly IEnumerator iterator;
		}
	}
}
