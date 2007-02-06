namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class IndexCreateDropTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class IndexCreateDropItem
		{
			public int _int;

			public string _string;

			public Sharpen.Util.Date _date;

			public IndexCreateDropItem(int int_, string string_, Sharpen.Util.Date date_)
			{
				_int = int_;
				_string = string_;
				_date = date_;
			}

			public IndexCreateDropItem(int int_) : this(int_, int_ == 0 ? null : string.Empty
				 + int_, int_ == 0 ? null : new Sharpen.Util.Date(int_))
			{
			}
		}

		private readonly int[] VALUES = new int[] { 4, 7, 6, 6, 5, 4, 0, 0 };

		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.IndexCreateDropTestCase().RunSolo();
		}

		protected override void Store()
		{
			for (int i = 0; i < VALUES.Length; i++)
			{
				Db().Set(new Db4objects.Db4o.Tests.Common.Assorted.IndexCreateDropTestCase.IndexCreateDropItem
					(VALUES[i]));
			}
		}

		public virtual void Test()
		{
			AssertQueryResults();
			AssertQueryResults(true);
			AssertQueryResults(false);
			AssertQueryResults(true);
		}

		private void AssertQueryResults(bool indexed)
		{
			Indexed(indexed);
			Reopen();
			AssertQueryResults();
		}

		private void Indexed(bool flag)
		{
			Db4objects.Db4o.Config.IObjectClass oc = Fixture().Config().ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.IndexCreateDropTestCase.IndexCreateDropItem)
				);
			oc.ObjectField("_int").Indexed(flag);
			oc.ObjectField("_string").Indexed(flag);
			oc.ObjectField("_date").Indexed(flag);
		}

		protected override Db4objects.Db4o.Query.IQuery NewQuery()
		{
			Db4objects.Db4o.Query.IQuery q = base.NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.IndexCreateDropTestCase.IndexCreateDropItem)
				);
			return q;
		}

		private void AssertQueryResults()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Descend("_int").Constrain(6);
			AssertQuerySize(2, q);
			q = NewQuery();
			q.Descend("_int").Constrain(4).Greater();
			AssertQuerySize(4, q);
			q = NewQuery();
			q.Descend("_int").Constrain(4).Greater().Equal();
			AssertQuerySize(6, q);
			q = NewQuery();
			q.Descend("_int").Constrain(7).Smaller().Equal();
			AssertQuerySize(8, q);
			q = NewQuery();
			q.Descend("_int").Constrain(7).Smaller();
			AssertQuerySize(7, q);
			q = NewQuery();
			q.Descend("_string").Constrain("6");
			AssertQuerySize(2, q);
			q = NewQuery();
			q.Descend("_string").Constrain("7");
			AssertQuerySize(1, q);
			q = NewQuery();
			q.Descend("_string").Constrain("4");
			AssertQuerySize(2, q);
			q = NewQuery();
			q.Descend("_string").Constrain(null);
			AssertQuerySize(2, q);
			q = NewQuery();
			q.Descend("_date").Constrain(new Sharpen.Util.Date(4)).Greater();
			AssertQuerySize(4, q);
			q = NewQuery();
			q.Descend("_date").Constrain(new Sharpen.Util.Date(4)).Greater().Equal();
			AssertQuerySize(6, q);
			q = NewQuery();
			q.Descend("_date").Constrain(new Sharpen.Util.Date(7)).Smaller().Equal();
			AssertQuerySize(6, q);
			q = NewQuery();
			q.Descend("_date").Constrain(new Sharpen.Util.Date(7)).Smaller();
			AssertQuerySize(5, q);
			q = NewQuery();
			q.Descend("_date").Constrain(null);
			AssertQuerySize(2, q);
		}

		private void AssertQuerySize(int size, Db4objects.Db4o.Query.IQuery q)
		{
			Db4oUnit.Assert.AreEqual(size, q.Execute().Size());
		}
	}
}
