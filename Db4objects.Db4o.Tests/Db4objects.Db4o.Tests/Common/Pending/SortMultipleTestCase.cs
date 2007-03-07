namespace Db4objects.Db4o.Tests.Common.Pending
{
	public class SortMultipleTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class IntHolder
		{
			public int _value;

			public IntHolder(int value)
			{
				this._value = value;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.IntHolder intHolder = (
					Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.IntHolder)obj;
				return _value == intHolder._value;
			}

			public override int GetHashCode()
			{
				return _value;
			}

			public override string ToString()
			{
				return _value.ToString();
			}
		}

		public class Data
		{
			public int _first;

			public int _second;

			public Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.IntHolder _third;

			public Data(int first, int second, int third)
			{
				this._first = first;
				this._second = second;
				this._third = new Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.IntHolder
					(third);
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data data = (Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data
					)obj;
				return _first == data._first && _second == data._second && _third.Equals(data._third
					);
			}

			public override int GetHashCode()
			{
				int hc = _first;
				hc *= 29 + _second;
				hc *= 29 + _third.GetHashCode();
				return hc;
			}

			public override string ToString()
			{
				return _first + "/" + _second + "/" + _third;
			}
		}

		private static readonly Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data[]
			 DATA = { new Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data(1, 
			2, 4), new Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data(1, 4, 
			3), new Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data(2, 4, 2), 
			new Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data(3, 1, 4), new 
			Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data(4, 3, 1), new Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data
			(4, 1, 3) };

		protected override void Store()
		{
			for (int dataIdx = 0; dataIdx < DATA.Length; dataIdx++)
			{
				Store(DATA[dataIdx]);
			}
		}

		public virtual void TestSortFirstThenSecond()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data)
				);
			query.Descend("_first").OrderAscending();
			query.Descend("_second").OrderAscending();
			AssertSortOrder(query, new int[] { 0, 1, 2, 3, 5, 4 });
		}

		public virtual void TestSortSecondThenFirst()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data)
				);
			query.Descend("_second").OrderAscending();
			query.Descend("_first").OrderAscending();
			AssertSortOrder(query, new int[] { 3, 5, 0, 4, 1, 2 });
		}

		public virtual void TestSortThirdThenFirst()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data)
				);
			query.Descend("_third").Descend("_value").OrderAscending();
			query.Descend("_first").OrderAscending();
			AssertSortOrder(query, new int[] { 4, 2, 1, 5, 0, 3 });
		}

		public virtual void TestSortThirdThenSecond()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase.Data)
				);
			query.Descend("_third").Descend("_value").OrderAscending();
			query.Descend("_second").OrderAscending();
			AssertSortOrder(query, new int[] { 4, 2, 5, 1, 3, 0 });
		}

		private void AssertSortOrder(Db4objects.Db4o.Query.IQuery query, int[] expectedIndexes
			)
		{
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(expectedIndexes.Length, result.Size());
			for (int i = 0; i < expectedIndexes.Length; i++)
			{
				Db4oUnit.Assert.AreEqual(DATA[expectedIndexes[i]], result.Next());
			}
		}
	}
}
