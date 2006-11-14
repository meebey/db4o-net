namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeDeleteFalse : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class CascadeDeleteFalseHelper
		{
		}

		public Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse.CascadeDeleteFalseHelper
			 h1;

		public Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse.CascadeDeleteFalseHelper
			 h2;

		public Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse.CascadeDeleteFalseHelper
			 h3;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration conf)
		{
			conf.ObjectClass(this).CascadeOnDelete(true);
			conf.ObjectClass(this).ObjectField("h3").CascadeOnDelete(false);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse cdf = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse
				();
			cdf.h1 = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse.CascadeDeleteFalseHelper
				();
			cdf.h2 = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse.CascadeDeleteFalseHelper
				();
			cdf.h3 = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse.CascadeDeleteFalseHelper
				();
			Db().Set(cdf);
		}

		public virtual void Test()
		{
			CheckHelperCount(3);
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse cdf = (Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse
				)RetrieveOnlyInstance(GetType());
			Db().Delete(cdf);
			CheckHelperCount(1);
		}

		private void CheckHelperCount(int count)
		{
			Db4oUnit.Assert.AreEqual(count, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteFalse.CascadeDeleteFalseHelper)
				));
		}
	}
}
