/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class IteratorsTestCase : ITestCase
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(IteratorsTestCase)).Run();
		}

		public virtual void TestIterateSingle()
		{
			IEnumerator i = Iterators.IterateSingle("foo");
			Assert.IsTrue(i.MoveNext());
			Assert.AreEqual("foo", i.Current);
			Assert.IsFalse(i.MoveNext());
			Assert.Expect(typeof(InvalidOperationException), new _ICodeBlock_24(i));
		}

		private sealed class _ICodeBlock_24 : ICodeBlock
		{
			public _ICodeBlock_24(IEnumerator i)
			{
				this.i = i;
			}

			public void Run()
			{
				Assert.IsNotNull(i.Current);
			}

			private readonly IEnumerator i;
		}

		public virtual void TestEnumerate()
		{
			IEnumerable e = Iterators.Enumerate(Iterators.Iterable(new object[] { "1", "2" })
				);
			IEnumerator iterator = e.GetEnumerator();
			EnumerateIterator.Tuple first = (EnumerateIterator.Tuple)Iterators.Next(iterator);
			EnumerateIterator.Tuple second = (EnumerateIterator.Tuple)Iterators.Next(iterator
				);
			Assert.AreEqual(0, first.index);
			Assert.AreEqual("1", first.value);
			Assert.AreEqual(1, second.index);
			Assert.AreEqual("2", second.value);
			Assert.IsFalse(iterator.MoveNext());
		}

		public virtual void TestCrossProduct()
		{
			IEnumerable[] source = new IEnumerable[] { Iterable(new object[] { "1", "2" }), Iterable
				(new object[] { "3", "4" }), Iterable(new object[] { "5", "6" }) };
			string[] expected = new string[] { "[1, 3, 5]", "[1, 3, 6]", "[1, 4, 5]", "[1, 4, 6]"
				, "[2, 3, 5]", "[2, 3, 6]", "[2, 4, 5]", "[2, 4, 6]" };
			IEnumerator iterator = Iterators.CrossProduct(source).GetEnumerator();
			Iterator4Assert.AreEqual(expected, Iterators.Map(iterator, new _IFunction4_62()));
		}

		private sealed class _IFunction4_62 : IFunction4
		{
			public _IFunction4_62()
			{
			}

			public object Apply(object arg)
			{
				return Iterators.ToString((IEnumerable)arg);
			}
		}

		private IEnumerable Iterable(object[] objects)
		{
			return Iterators.Iterable(objects);
		}

		public virtual void TestFlatten()
		{
			IEnumerator iterator = Iterate(new object[] { "1", "2", Iterate(new object[] { Iterate
				(new object[] { "3", "4" }), Iterators.EmptyIterator, Iterators.EmptyIterator, "5"
				 }), Iterators.EmptyIterator, "6" });
			Iterator4Assert.AreEqual(new object[] { "1", "2", "3", "4", "5", "6" }, Iterators
				.Flatten(iterator));
		}

		internal virtual IEnumerator Iterate(object[] values)
		{
			return Iterators.Iterate(values);
		}

		public virtual void TestFilter()
		{
			AssertFilter(new string[] { "bar", "baz" }, new string[] { "foo", "bar", "baz", "zong"
				 }, new _IPredicate4_103());
			AssertFilter(new string[] { "foo", "bar" }, new string[] { "foo", "bar" }, new _IPredicate4_111
				());
			AssertFilter(new string[0], new string[] { "foo", "bar" }, new _IPredicate4_120()
				);
		}

		private sealed class _IPredicate4_103 : IPredicate4
		{
			public _IPredicate4_103()
			{
			}

			public bool Match(object candidate)
			{
				return ((string)candidate).StartsWith("b");
			}
		}

		private sealed class _IPredicate4_111 : IPredicate4
		{
			public _IPredicate4_111()
			{
			}

			public bool Match(object candidate)
			{
				return true;
			}
		}

		private sealed class _IPredicate4_120 : IPredicate4
		{
			public _IPredicate4_120()
			{
			}

			public bool Match(object candidate)
			{
				return false;
			}
		}

		private void AssertFilter(string[] expected, string[] actual, IPredicate4 filter)
		{
			Iterator4Assert.AreEqual(expected, Iterators.Filter(actual, filter));
		}

		public virtual void TestMap()
		{
			int[] array = new int[] { 1, 2, 3 };
			Collection4 args = new Collection4();
			IEnumerator iterator = Iterators.Map(IntArrays4.NewIterator(array), new _IFunction4_136
				(args));
			Assert.IsNotNull(iterator);
			Assert.AreEqual(0, args.Size());
			for (int i = 0; i < array.Length; ++i)
			{
				Assert.IsTrue(iterator.MoveNext());
				Assert.AreEqual(i + 1, args.Size());
				Assert.AreEqual(array[i] * 2, iterator.Current);
			}
		}

		private sealed class _IFunction4_136 : IFunction4
		{
			public _IFunction4_136(Collection4 args)
			{
				this.args = args;
			}

			public object Apply(object arg)
			{
				args.Add(arg);
				return ((int)arg) * 2;
			}

			private readonly Collection4 args;
		}
	}
}
