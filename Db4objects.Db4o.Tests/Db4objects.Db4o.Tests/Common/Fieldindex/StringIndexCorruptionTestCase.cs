namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	/// <summary>Jira ticket: COR-373</summary>
	/// <exclude></exclude>
	public class StringIndexCorruptionTestCase : Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexCorruptionTestCase().RunSolo
				();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			config.BTreeNodeSize(4);
			config.FlushFileBuffers(false);
		}

		public virtual void TestStressSet()
		{
			Db4objects.Db4o.Ext.IExtObjectContainer container = Db();
			int itemCount = 300;
			for (int i = 0; i < itemCount; ++i)
			{
				Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item item = new Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item
					(ItemName(i));
				container.Set(item);
				container.Set(item);
				container.Commit();
				container.Set(item);
				container.Set(item);
				container.Commit();
			}
			for (int i = 0; i < itemCount; ++i)
			{
				string itemName = ItemName(i);
				Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item found = Query
					(itemName);
				Db4oUnit.Assert.IsNotNull(found, "'" + itemName + "' not found");
				Db4oUnit.Assert.AreEqual(itemName, found.name);
			}
		}

		private string ItemName(int i)
		{
			return "item " + i;
		}
	}
}
