namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeOnActivate : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public string name;

		public Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate child;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration conf)
		{
			conf.ObjectClass(this).CascadeOnActivate(true);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate coa = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate
				();
			coa.name = "1";
			coa.child = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate();
			coa.child.name = "2";
			coa.child.child = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate();
			coa.child.child.name = "3";
			Db().Set(coa);
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(GetType());
			q.Descend("name").Constrain("1");
			Db4objects.Db4o.IObjectSet os = q.Execute();
			Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate coa = (Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate
				)os.Next();
			Db4objects.Db4o.Tests.Common.Querying.CascadeOnActivate coa3 = coa.child.child;
			Db4oUnit.Assert.AreEqual("3", coa3.name);
			Db().Deactivate(coa, int.MaxValue);
			Db4oUnit.Assert.IsNull(coa3.name);
			Db().Activate(coa, 1);
			Db4oUnit.Assert.AreEqual("3", coa3.name);
		}
	}
}
