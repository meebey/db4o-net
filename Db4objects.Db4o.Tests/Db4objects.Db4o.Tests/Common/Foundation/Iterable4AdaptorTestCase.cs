namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class Iterable4AdaptorTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestEmptyIterator()
		{
			Db4objects.Db4o.Foundation.Iterable4Adaptor adaptor = NewAdaptor(new int[] {  });
			Db4oUnit.Assert.IsFalse(adaptor.HasNext());
			Db4oUnit.Assert.IsFalse(adaptor.HasNext());
			Db4oUnit.Assert.Expect(typeof(System.InvalidOperationException), new _AnonymousInnerClass20
				(this, adaptor));
		}

		private sealed class _AnonymousInnerClass20 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass20(Iterable4AdaptorTestCase _enclosing, Db4objects.Db4o.Foundation.Iterable4Adaptor
				 adaptor)
			{
				this._enclosing = _enclosing;
				this.adaptor = adaptor;
			}

			public void Run()
			{
				adaptor.Next();
			}

			private readonly Iterable4AdaptorTestCase _enclosing;

			private readonly Db4objects.Db4o.Foundation.Iterable4Adaptor adaptor;
		}

		public virtual void TestHasNext()
		{
			int[] expected = new int[] { 1, 2, 3 };
			Db4objects.Db4o.Foundation.Iterable4Adaptor adaptor = NewAdaptor(expected);
			for (int i = 0; i < expected.Length; i++)
			{
				AssertHasNext(adaptor);
				Db4oUnit.Assert.AreEqual(expected[i], adaptor.Next());
			}
			Db4oUnit.Assert.IsFalse(adaptor.HasNext());
		}

		public virtual void TestNext()
		{
			int[] expected = new int[] { 1, 2, 3 };
			Db4objects.Db4o.Foundation.Iterable4Adaptor adaptor = NewAdaptor(expected);
			for (int i = 0; i < expected.Length; i++)
			{
				Db4oUnit.Assert.AreEqual(expected[i], adaptor.Next());
			}
			Db4oUnit.Assert.IsFalse(adaptor.HasNext());
		}

		private Db4objects.Db4o.Foundation.Iterable4Adaptor NewAdaptor(int[] expected)
		{
			return new Db4objects.Db4o.Foundation.Iterable4Adaptor(NewIterable(expected));
		}

		private void AssertHasNext(Db4objects.Db4o.Foundation.Iterable4Adaptor adaptor)
		{
			for (int i = 0; i < 10; ++i)
			{
				Db4oUnit.Assert.IsTrue(adaptor.HasNext());
			}
		}

		private System.Collections.IEnumerable NewIterable(int[] values)
		{
			Db4objects.Db4o.Foundation.Collection4 collection = new Db4objects.Db4o.Foundation.Collection4
				();
			collection.AddAll(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.ToObjectArray
				(values));
			return collection;
		}
	}
}
