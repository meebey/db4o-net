namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class Collection4TestCase : Db4oUnit.ITestCase
	{
		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(typeof(Db4objects.Db4o.Tests.Common.Foundation.Collection4TestCase)
				).Run();
		}

		public virtual void TestNulls()
		{
			Db4objects.Db4o.Foundation.Collection4 c = new Db4objects.Db4o.Foundation.Collection4
				();
			c.Add("one");
			AssertNotContainsNull(c);
			c.Add(null);
			AssertContainsNull(c);
			AssertCollection(new string[] { "one", null }, c);
			c.Prepend(null);
			AssertCollection(new string[] { null, "one", null }, c);
			c.Prepend("zero");
			c.Add("two");
			AssertCollection(new string[] { "zero", null, "one", null, "two" }, c);
			AssertContainsNull(c);
			c.Remove(null);
			AssertCollection(new string[] { "zero", "one", null, "two" }, c);
			c.Remove(null);
			AssertNotContainsNull(c);
			AssertCollection(new string[] { "zero", "one", "two" }, c);
			c.Remove(null);
			AssertCollection(new string[] { "zero", "one", "two" }, c);
		}

		public virtual void TestPrepend()
		{
			Db4objects.Db4o.Foundation.Collection4 c = new Db4objects.Db4o.Foundation.Collection4
				();
			c.Prepend("foo");
			AssertCollection(new string[] { "foo" }, c);
			c.Add("bar");
			AssertCollection(new string[] { "foo", "bar" }, c);
			c.Prepend("baz");
			AssertCollection(new string[] { "baz", "foo", "bar" }, c);
			c.Prepend("gazonk");
			AssertCollection(new string[] { "gazonk", "baz", "foo", "bar" }, c);
		}

		public virtual void TestCopyConstructor()
		{
			string[] expected = new string[] { "1", "2", "3" };
			Db4objects.Db4o.Foundation.Collection4 c = NewCollection(expected);
			AssertCollection(expected, new Db4objects.Db4o.Foundation.Collection4(c));
		}

		public virtual void TestInvalidIteratorException()
		{
			Db4objects.Db4o.Foundation.Collection4 c = NewCollection(new string[] { "1", "2" }
				);
			System.Collections.IEnumerator i = c.GetEnumerator();
			Db4oUnit.Assert.IsTrue(i.MoveNext());
			c.Add("3");
			Db4oUnit.Assert.Expect(typeof(Db4objects.Db4o.Foundation.InvalidIteratorException)
				, new _AnonymousInnerClass60(this, i));
		}

		private sealed class _AnonymousInnerClass60 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass60(Collection4TestCase _enclosing, System.Collections.IEnumerator
				 i)
			{
				this._enclosing = _enclosing;
				this.i = i;
			}

			public void Run()
			{
				Sharpen.Runtime.Out.WriteLine(i.Current);
			}

			private readonly Collection4TestCase _enclosing;

			private readonly System.Collections.IEnumerator i;
		}

		public virtual void TestRemove()
		{
			Db4objects.Db4o.Foundation.Collection4 c = NewCollection(new string[] { "1", "2", 
				"3", "4" });
			c.Remove("3");
			AssertCollection(new string[] { "1", "2", "4" }, c);
			c.Remove("4");
			AssertCollection(new string[] { "1", "2" }, c);
			c.Add("5");
			AssertCollection(new string[] { "1", "2", "5" }, c);
			c.Remove("1");
			AssertCollection(new string[] { "2", "5" }, c);
			c.Remove("2");
			c.Remove("5");
			AssertCollection(new string[] {  }, c);
			c.Add("6");
			AssertCollection(new string[] { "6" }, c);
		}

		private void AssertCollection(string[] expected, Db4objects.Db4o.Foundation.Collection4
			 c)
		{
			Db4oUnit.Assert.AreEqual(expected.Length, c.Size());
			AssertIterator(expected, c.GetEnumerator());
		}

		private void AssertContainsNull(Db4objects.Db4o.Foundation.Collection4 c)
		{
			Db4oUnit.Assert.IsTrue(c.Contains(null));
			Db4oUnit.Assert.IsNull(c.Get(null));
			int size = c.Size();
			c.Ensure(null);
			Db4oUnit.Assert.AreEqual(size, c.Size());
		}

		private void AssertNotContainsNull(Db4objects.Db4o.Foundation.Collection4 c)
		{
			Db4oUnit.Assert.IsFalse(c.Contains(null));
			Db4oUnit.Assert.IsNull(c.Get(null));
			int size = c.Size();
			c.Ensure(null);
			Db4oUnit.Assert.AreEqual(size + 1, c.Size());
			c.Remove(null);
			Db4oUnit.Assert.AreEqual(size, c.Size());
		}

		public virtual void TestIterator()
		{
			string[] expected = new string[] { "1", "2", "3" };
			Db4objects.Db4o.Foundation.Collection4 c = NewCollection(expected);
			AssertIterator(expected, c.GetEnumerator());
		}

		private Db4objects.Db4o.Foundation.Collection4 NewCollection(string[] expected)
		{
			Db4objects.Db4o.Foundation.Collection4 c = new Db4objects.Db4o.Foundation.Collection4
				();
			c.AddAll(expected);
			return c;
		}

		private void AssertIterator(string[] expected, System.Collections.IEnumerator iterator
			)
		{
			Db4oUnit.Assert.IsNotNull(iterator);
			for (int i = 0; i < expected.Length; ++i)
			{
				Db4oUnit.Assert.IsTrue(iterator.MoveNext());
				Db4oUnit.Assert.AreEqual(expected[i], iterator.Current);
			}
			Db4oUnit.Assert.IsFalse(iterator.MoveNext());
		}

		public virtual void TestToString()
		{
			Db4objects.Db4o.Foundation.Collection4 c = new Db4objects.Db4o.Foundation.Collection4
				();
			Db4oUnit.Assert.AreEqual("[]", c.ToString());
			c.Add("foo");
			Db4oUnit.Assert.AreEqual("[foo]", c.ToString());
			c.Add("bar");
			Db4oUnit.Assert.AreEqual("[foo, bar]", c.ToString());
		}
	}
}
