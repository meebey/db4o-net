namespace Db4objects.Db4o.Tests.Common.Soda.Util
{
	public class SodaTestUtil
	{
		public static void ExpectOne(Db4objects.Db4o.Query.IQuery query, object @object)
		{
			Expect(query, new object[] { @object });
		}

		public static void ExpectNone(Db4objects.Db4o.Query.IQuery query)
		{
			Expect(query, null);
		}

		public static void Expect(Db4objects.Db4o.Query.IQuery query, object[] results)
		{
			Expect(query, results, false);
		}

		public static void ExpectOrdered(Db4objects.Db4o.Query.IQuery query, object[] results
			)
		{
			Expect(query, results, true);
		}

		public static void Expect(Db4objects.Db4o.Query.IQuery query, object[] results, bool
			 ordered)
		{
			Db4objects.Db4o.IObjectSet set = query.Execute();
			if (results == null || results.Length == 0)
			{
				if (set.Size() > 0)
				{
					Db4oUnit.Assert.Fail("No content expected.");
				}
				return;
			}
			int j = 0;
			if (set.Size() == results.Length)
			{
				while (set.HasNext())
				{
					object obj = set.Next();
					bool found = false;
					if (ordered)
					{
						if (Db4objects.Db4o.Tests.Common.Soda.Util.TCompare.IsEqual(results[j], obj))
						{
							results[j] = null;
							found = true;
						}
						j++;
					}
					else
					{
						for (int i = 0; i < results.Length; i++)
						{
							if (results[i] != null)
							{
								if (Db4objects.Db4o.Tests.Common.Soda.Util.TCompare.IsEqual(results[i], obj))
								{
									results[i] = null;
									found = true;
									break;
								}
							}
						}
					}
					if (!found)
					{
						Db4oUnit.Assert.Fail("Object not expected: " + obj);
					}
				}
				for (int i = 0; i < results.Length; i++)
				{
					if (results[i] != null)
					{
						Db4oUnit.Assert.Fail("Expected object not returned: " + results[i]);
					}
				}
			}
			else
			{
				Db4oUnit.Assert.Fail("Unexpected size returned.\nExpected: " + results.Length + " Returned: "
					 + set.Size());
			}
		}

		private SodaTestUtil()
		{
		}
	}
}
