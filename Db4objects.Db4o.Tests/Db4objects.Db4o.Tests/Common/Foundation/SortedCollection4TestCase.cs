namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class SortedCollection4TestCase : Db4oUnit.ITestCase
	{
		private sealed class _AnonymousInnerClass14 : Db4objects.Db4o.Foundation.IComparison4
		{
			public _AnonymousInnerClass14()
			{
			}

			public int Compare(object x, object y)
			{
				return ((int)x) - ((int)y);
			}
		}

		internal static readonly Db4objects.Db4o.Foundation.IComparison4 INTEGER_COMPARISON
			 = new _AnonymousInnerClass14();

		public virtual void TestAddAllAndToArray()
		{
			object[] array = Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.ToObjectArray
				(new int[] { 6, 4, 1, 2, 7, 3 });
			Db4objects.Db4o.Foundation.SortedCollection4 collection = NewSortedCollection();
			Db4oUnit.Assert.AreEqual(0, collection.Size());
			collection.AddAll(new Db4objects.Db4o.Foundation.ArrayIterator4(array));
			AssertCollection(new int[] { 1, 2, 3, 4, 6, 7 }, collection);
		}

		public virtual void TestToArrayOnEmptyCollection()
		{
			object[] array = new object[0];
			Db4oUnit.Assert.AreSame(array, NewSortedCollection().ToArray(array));
		}

		public virtual void TestAddRemove()
		{
			Db4objects.Db4o.Foundation.SortedCollection4 collection = NewSortedCollection();
			collection.Add(3);
			collection.Add(1);
			collection.Add(5);
			AssertCollection(new int[] { 1, 3, 5 }, collection);
			collection.Remove(3);
			AssertCollection(new int[] { 1, 5 }, collection);
			collection.Remove(1);
			AssertCollection(new int[] { 5 }, collection);
		}

		private void AssertCollection(int[] expected, Db4objects.Db4o.Foundation.SortedCollection4
			 collection)
		{
			Db4oUnit.Assert.AreEqual(expected.Length, collection.Size());
			Db4oUnit.ArrayAssert.AreEqual(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.
				ToObjectArray(expected), collection.ToArray(new object[collection.Size()]));
		}

		private Db4objects.Db4o.Foundation.SortedCollection4 NewSortedCollection()
		{
			return new Db4objects.Db4o.Foundation.SortedCollection4(INTEGER_COMPARISON);
		}
	}
}
