namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class ConjunctiveQbETestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Sup
		{
			public bool _flag;

			public Sup(bool flag)
			{
				this._flag = flag;
			}

			public virtual Db4objects.Db4o.IObjectSet Query(Db4objects.Db4o.IObjectContainer 
				db)
			{
				Db4objects.Db4o.Query.IQuery query = db.Query();
				query.Constrain(this);
				query.Descend("_flag").Constrain(true).Not();
				return query.Execute();
			}
		}

		public class Sub1 : Db4objects.Db4o.Tests.Common.Querying.ConjunctiveQbETestCase.Sup
		{
			public Sub1(bool flag) : base(flag)
			{
			}
		}

		public class Sub2 : Db4objects.Db4o.Tests.Common.Querying.ConjunctiveQbETestCase.Sup
		{
			public Sub2(bool flag) : base(flag)
			{
			}
		}

		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Querying.ConjunctiveQbETestCase.Sub1(false
				));
			Store(new Db4objects.Db4o.Tests.Common.Querying.ConjunctiveQbETestCase.Sub1(true)
				);
			Store(new Db4objects.Db4o.Tests.Common.Querying.ConjunctiveQbETestCase.Sub2(false
				));
			Store(new Db4objects.Db4o.Tests.Common.Querying.ConjunctiveQbETestCase.Sub2(true)
				);
		}

		public virtual void TestAndedQbE()
		{
			Db4oUnit.Assert.AreEqual(1, new Db4objects.Db4o.Tests.Common.Querying.ConjunctiveQbETestCase.Sub1
				(false).Query(Db()).Size());
		}
	}
}
