namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class ArrayIterator4TestCase : Db4oUnit.ITestCase
	{
		public virtual void TestEmptyArray()
		{
			AssertExhausted(new Db4objects.Db4o.Foundation.ArrayIterator4(new object[0]));
		}

		public virtual void TestArray()
		{
			Db4objects.Db4o.Foundation.ArrayIterator4 i = new Db4objects.Db4o.Foundation.ArrayIterator4
				(new object[] { "foo", "bar" });
			Db4oUnit.Assert.IsTrue(i.MoveNext());
			Db4oUnit.Assert.AreEqual("foo", i.Current);
			Db4oUnit.Assert.IsTrue(i.MoveNext());
			Db4oUnit.Assert.AreEqual("bar", i.Current);
			AssertExhausted(i);
		}

		private void AssertExhausted(Db4objects.Db4o.Foundation.ArrayIterator4 i)
		{
			Db4oUnit.Assert.IsFalse(i.MoveNext());
			Db4oUnit.Assert.Expect(typeof(System.IndexOutOfRangeException), new _AnonymousInnerClass29
				(this, i));
		}

		private sealed class _AnonymousInnerClass29 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass29(ArrayIterator4TestCase _enclosing, Db4objects.Db4o.Foundation.ArrayIterator4
				 i)
			{
				this._enclosing = _enclosing;
				this.i = i;
			}

			public void Run()
			{
				Sharpen.Runtime.Out.WriteLine(i.Current);
			}

			private readonly ArrayIterator4TestCase _enclosing;

			private readonly Db4objects.Db4o.Foundation.ArrayIterator4 i;
		}
	}
}
