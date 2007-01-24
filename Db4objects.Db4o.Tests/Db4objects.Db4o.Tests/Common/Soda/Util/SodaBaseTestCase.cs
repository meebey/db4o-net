namespace Db4objects.Db4o.Tests.Common.Soda.Util
{
	public abstract class SodaBaseTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		[System.NonSerialized]
		protected object[] _array;

		protected override void Db4oSetupBeforeStore()
		{
			_array = CreateData();
		}

		protected override void Store()
		{
			object[] data = CreateData();
			for (int idx = 0; idx < data.Length; idx++)
			{
				Db().Set(data[idx]);
			}
		}

		public abstract object[] CreateData();

		protected virtual void Expect(Db4objects.Db4o.Query.IQuery query, int[] indices)
		{
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(query, CollectCandidates
				(indices), false);
		}

		protected virtual void ExpectOrdered(Db4objects.Db4o.Query.IQuery query, int[] indices
			)
		{
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOrdered(query, CollectCandidates
				(indices));
		}

		private object[] CollectCandidates(int[] indices)
		{
			object[] data = new object[indices.Length];
			for (int idx = 0; idx < indices.Length; idx++)
			{
				data[idx] = _array[indices[idx]];
			}
			return data;
		}
	}
}
