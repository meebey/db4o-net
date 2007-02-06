namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class GetByUUIDTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.GetByUUIDTestCase().RunSolo();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem)).GenerateUUIDs
				(true);
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem("one"));
			Db().Set(new Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem("two"));
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Foundation.Hashtable4 uuidCache = new Db4objects.Db4o.Foundation.Hashtable4
				();
			AssertItemsCanBeRetrievedByUUID(uuidCache);
			Reopen();
			AssertItemsCanBeRetrievedByUUID(uuidCache);
		}

		private void AssertItemsCanBeRetrievedByUUID(Db4objects.Db4o.Foundation.Hashtable4
			 uuidCache)
		{
			Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem.AssertItemsCanBeRetrievedByUUID
				(Db(), uuidCache);
		}
	}
}
