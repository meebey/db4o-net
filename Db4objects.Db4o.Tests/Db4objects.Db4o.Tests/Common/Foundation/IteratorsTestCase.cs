/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions.Foundation;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class IteratorsTestCase : ITestCase
	{
		public virtual void TestFilter()
		{
			AssertFilter(new string[] { "bar", "baz" }, new string[] { "foo", "bar", "baz", "zong"
				 }, new _IPredicate4_19(this));
			AssertFilter(new string[] { "foo", "bar" }, new string[] { "foo", "bar" }, new _IPredicate4_27
				(this));
			AssertFilter(new string[0], new string[] { "foo", "bar" }, new _IPredicate4_36(this
				));
		}

		private sealed class _IPredicate4_19 : IPredicate4
		{
			public _IPredicate4_19(IteratorsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return ((string)candidate).StartsWith("b");
			}

			private readonly IteratorsTestCase _enclosing;
		}

		private sealed class _IPredicate4_27 : IPredicate4
		{
			public _IPredicate4_27(IteratorsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return true;
			}

			private readonly IteratorsTestCase _enclosing;
		}

		private sealed class _IPredicate4_36 : IPredicate4
		{
			public _IPredicate4_36(IteratorsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return false;
			}

			private readonly IteratorsTestCase _enclosing;
		}

		private void AssertFilter(string[] expected, string[] actual, IPredicate4 filter)
		{
			Iterator4Assert.AreEqual(expected, Iterators.Filter(actual, filter));
		}

		public virtual void TestMap()
		{
			int[] array = new int[] { 1, 2, 3 };
			Collection4 args = new Collection4();
			IEnumerator iterator = Iterators.Map(IntArrays4.NewIterator(array), new _IFunction4_52
				(this, args));
			Assert.IsNotNull(iterator);
			Assert.AreEqual(0, args.Size());
			for (int i = 0; i < array.Length; ++i)
			{
				Assert.IsTrue(iterator.MoveNext());
				Assert.AreEqual(i + 1, args.Size());
				Assert.AreEqual(array[i] * 2, iterator.Current);
			}
		}

		private sealed class _IFunction4_52 : IFunction4
		{
			public _IFunction4_52(IteratorsTestCase _enclosing, Collection4 args)
			{
				this._enclosing = _enclosing;
				this.args = args;
			}

			public object Apply(object arg)
			{
				args.Add(arg);
				return ((int)arg) * 2;
			}

			private readonly IteratorsTestCase _enclosing;

			private readonly Collection4 args;
		}
	}
}
