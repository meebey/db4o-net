namespace Db4objects.Db4o.Tests.Common.Header
{
	public class SimpleTimeStampIdTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase().RunSolo();
		}

		public class STSItem
		{
			public string _name;

			public STSItem()
			{
			}

			public STSItem(string name)
			{
				_name = name;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			Db4objects.Db4o.Config.IObjectClass objectClass = config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase.STSItem)
				);
			objectClass.GenerateUUIDs(true);
			objectClass.GenerateVersionNumbers(true);
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase.STSItem
				("one"));
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase.STSItem item = (Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase.STSItem
				)Db().Get(typeof(Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase.STSItem)
				).Next();
			long version = Db().GetObjectInfo(item).GetVersion();
			Db4oUnit.Assert.IsGreater(0, version);
			Db4oUnit.Assert.IsGreaterOrEqual(version, CurrentVersion());
			Reopen();
			Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase.STSItem item2 = new 
				Db4objects.Db4o.Tests.Common.Header.SimpleTimeStampIdTestCase.STSItem("two");
			Db().Set(item2);
			long secondVersion = Db().GetObjectInfo(item2).GetVersion();
			Db4oUnit.Assert.IsGreater(version, secondVersion);
			Db4oUnit.Assert.IsGreaterOrEqual(secondVersion, CurrentVersion());
		}

		private long CurrentVersion()
		{
			return ((Db4objects.Db4o.YapFile)Db()).CurrentVersion();
		}
	}
}
