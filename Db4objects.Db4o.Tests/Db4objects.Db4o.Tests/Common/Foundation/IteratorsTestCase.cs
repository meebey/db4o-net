namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class IteratorsTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestFilter()
		{
			AssertFilter(new string[] { "bar", "baz" }, new string[] { "foo", "bar", "baz", "zong"
				 }, new _AnonymousInnerClass18(this));
			AssertFilter(new string[] { "foo", "bar" }, new string[] { "foo", "bar" }, new _AnonymousInnerClass26
				(this));
			AssertFilter(new string[0], new string[] { "foo", "bar" }, new _AnonymousInnerClass35
				(this));
		}

		private sealed class _AnonymousInnerClass18 : Db4objects.Db4o.Foundation.IPredicate4
		{
			public _AnonymousInnerClass18(IteratorsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return ((string)candidate).StartsWith("b");
			}

			private readonly IteratorsTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass26 : Db4objects.Db4o.Foundation.IPredicate4
		{
			public _AnonymousInnerClass26(IteratorsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return true;
			}

			private readonly IteratorsTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass35 : Db4objects.Db4o.Foundation.IPredicate4
		{
			public _AnonymousInnerClass35(IteratorsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return false;
			}

			private readonly IteratorsTestCase _enclosing;
		}

		private void AssertFilter(string[] expected, string[] actual, Db4objects.Db4o.Foundation.IPredicate4
			 filter)
		{
			Db4objects.Db4o.Tests.Common.Foundation.IteratorAssert.AreEqual(expected, Db4objects.Db4o.Foundation.Iterators
				.Filter(actual, filter));
		}

		public virtual void TestMap()
		{
			int[] array = new int[] { 1, 2, 3 };
			Db4objects.Db4o.Foundation.Collection4 args = new Db4objects.Db4o.Foundation.Collection4
				();
			System.Collections.IEnumerator iterator = Db4objects.Db4o.Foundation.Iterators.Map
				(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.NewIterator(array), new _AnonymousInnerClass51
				(this, args));
			Db4oUnit.Assert.IsNotNull(iterator);
			Db4oUnit.Assert.AreEqual(0, args.Size());
			for (int i = 0; i < array.Length; ++i)
			{
				Db4oUnit.Assert.IsTrue(iterator.MoveNext());
				Db4oUnit.Assert.AreEqual(i + 1, args.Size());
				Db4oUnit.Assert.AreEqual(array[i] * 2, iterator.Current);
			}
		}

		private sealed class _AnonymousInnerClass51 : Db4objects.Db4o.Foundation.IFunction4
		{
			public _AnonymousInnerClass51(IteratorsTestCase _enclosing, Db4objects.Db4o.Foundation.Collection4
				 args)
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

			private readonly Db4objects.Db4o.Foundation.Collection4 args;
		}
	}
}
