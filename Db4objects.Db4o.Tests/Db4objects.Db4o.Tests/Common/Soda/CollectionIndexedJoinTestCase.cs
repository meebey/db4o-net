namespace Db4objects.Db4o.Tests.Common.Soda
{
	public class CollectionIndexedJoinTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static readonly string COLLECTIONFIELDNAME = "_data";

		private static readonly string IDFIELDNAME = "_id";

		private const int NUMENTRIES = 3;

		public class DataHolder
		{
			public System.Collections.ArrayList _data;

			public DataHolder(int id)
			{
				_data = new System.Collections.ArrayList();
				_data.Add(new Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase.Data
					(id));
			}
		}

		public class Data
		{
			public int _id;

			public Data(int id)
			{
				this._id = id;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase.Data)
				).ObjectField(IDFIELDNAME).Indexed(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < NUMENTRIES; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase.DataHolder
					(i));
			}
		}

		public virtual void TestIndexedOrTwo()
		{
			AssertIndexedOr(new int[] { 0, 1, -1 }, 2);
		}

		private void AssertIndexedOr(int[] values, int expectedResultCount)
		{
			Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase.TestConfig config
				 = new Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase.TestConfig
				(values.Length);
			while (config.MoveNext())
			{
				AssertIndexedOr(values, expectedResultCount, config.RootIndex(), config.ConnectLeft
					());
			}
		}

		public virtual void TestIndexedOrAll()
		{
			AssertIndexedOr(new int[] { 0, 1, 2 }, 3);
		}

		public virtual void TestTwoJoinLegs()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase.DataHolder)
				).Descend(COLLECTIONFIELDNAME);
			Db4objects.Db4o.Query.IConstraint left = query.Descend(IDFIELDNAME).Constrain(0);
			left.Or(query.Descend(IDFIELDNAME).Constrain(1));
			Db4objects.Db4o.Query.IConstraint right = query.Descend(IDFIELDNAME).Constrain(2);
			right.Or(query.Descend(IDFIELDNAME).Constrain(-1));
			left.Or(right);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(3, result.Size());
		}

		public virtual void AssertIndexedOr(int[] values, int expectedResultCount, int rootIdx
			, bool connectLeft)
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.CollectionIndexedJoinTestCase.DataHolder)
				).Descend(COLLECTIONFIELDNAME);
			Db4objects.Db4o.Query.IConstraint constraint = query.Descend(IDFIELDNAME).Constrain
				(values[rootIdx]);
			for (int idx = 0; idx < values.Length; idx++)
			{
				if (idx != rootIdx)
				{
					Db4objects.Db4o.Query.IConstraint curConstraint = query.Descend(IDFIELDNAME).Constrain
						(values[idx]);
					if (connectLeft)
					{
						constraint.Or(curConstraint);
					}
					else
					{
						curConstraint.Or(constraint);
					}
				}
			}
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(expectedResultCount, result.Size());
		}

		private class TestConfig : Db4objects.Db4o.Tests.Util.PermutingTestConfig
		{
			public TestConfig(int numValues) : base(new object[][] { new object[] { 0, numValues
				 - 1 }, new object[] { false, true } })
			{
			}

			public virtual int RootIndex()
			{
				return ((int)Current(0));
			}

			public virtual bool ConnectLeft()
			{
				return ((bool)Current(1));
			}
		}
	}
}
