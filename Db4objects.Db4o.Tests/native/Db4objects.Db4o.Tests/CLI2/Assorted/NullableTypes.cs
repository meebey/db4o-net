namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
#if NET_2_0 || CF_2_0
	using System;

	using Db4objects.Db4o;
	using Db4objects.Db4o.Query;

	using Db4oUnit;
	using Db4oUnit.Extensions;

	class NullableContainer
	{
		public int? intValue = null;
		public DateTime? dateValue = null;

		public NullableContainer(int value)
		{
			intValue = value;
		}

		public NullableContainer(DateTime value)
		{
			dateValue = value;
		}
	}

	class NullableTypes : AbstractDb4oTestCase
	{
		static readonly DateTime TheDate = new DateTime(1983, 3, 7);

		protected override void Store()
		{
			Db().Set(new NullableContainer(42));
			Db().Set(new NullableContainer(TheDate));
		}

		public void TestGlobalQuery()
		{
			IQuery query = NewQuery();
			query.Constrain(typeof(NullableContainer));

			IObjectSet os = query.Execute();
			Assert.AreEqual(2, os.Size());

			bool foundInt = false;
			bool foundDate = false;
			while (os.HasNext())
			{
				NullableContainer item = (NullableContainer)os.Next();
				if (item.intValue.HasValue)
				{
					Assert.AreEqual(42, item.intValue.Value);
					Assert.IsFalse(item.dateValue.HasValue);
					foundInt = true;
				}
				else
				{
					Assert.AreEqual(TheDate, item.dateValue.Value);
					Assert.IsFalse(item.intValue.HasValue);
					foundDate = true;
				}
			}

			Assert.IsTrue(foundInt);
			Assert.IsTrue(foundDate);
		}

		public void TestDateQuery()
		{
			IObjectSet os = Db().Get(new NullableContainer(TheDate));
			CheckDateValueQueryResult(os);
		}

		private static void CheckDateValueQueryResult(IObjectSet os)
		{
			Assert.AreEqual(1, os.Size());
			NullableContainer found = (NullableContainer)os.Next();
			Assert.AreEqual(TheDate, found.dateValue.Value);
			EnsureIsNull(found.intValue);
		}

		public void TestIntQuery()
		{
			IObjectSet os = Db().Get(new NullableContainer(42));
			CheckIntValueQueryResult(os);
		}

		public void TestSodaQuery()
		{
			IQuery q = NewQuery(typeof(NullableContainer));
			q.Descend("intValue").Constrain(42);
			CheckIntValueQueryResult(q.Execute());
		}

		public void TestSodaQueryWithNullConstrain()
		{
			IQuery q = NewQuery(typeof(NullableContainer));
			q.Descend("intValue").Constrain(null);
			CheckDateValueQueryResult(q.Execute());
		}

		private static void CheckIntValueQueryResult(IObjectSet os)
		{
			Assert.AreEqual(1, os.Size());
			NullableContainer found = (NullableContainer)os.Next();
			Assert.AreEqual(42, found.intValue.Value);
			EnsureIsNull(found.dateValue);
		}

		private static void EnsureIsNull<T>(Nullable<T> value) where T : struct
		{
			Assert.IsFalse(value.HasValue, "!nullable.HasValue");
		}
	}
#endif
}
