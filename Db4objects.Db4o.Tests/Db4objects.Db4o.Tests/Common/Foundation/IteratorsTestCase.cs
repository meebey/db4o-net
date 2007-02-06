namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class IteratorsTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestMap()
		{
			int[] array = new int[] { 1, 2, 3 };
			Db4objects.Db4o.Foundation.Collection4 args = new Db4objects.Db4o.Foundation.Collection4
				();
			System.Collections.IEnumerator iterator = Db4objects.Db4o.Foundation.Iterators.Map
				(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.NewIterator(array), new _AnonymousInnerClass19
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

		private sealed class _AnonymousInnerClass19 : Db4objects.Db4o.Foundation.IFunction4
		{
			public _AnonymousInnerClass19(IteratorsTestCase _enclosing, Db4objects.Db4o.Foundation.Collection4
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
