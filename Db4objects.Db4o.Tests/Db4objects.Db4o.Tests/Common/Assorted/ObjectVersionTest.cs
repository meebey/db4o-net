namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ObjectVersionTest : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.GenerateUUIDs(int.MaxValue);
			config.GenerateVersionNumbers(int.MaxValue);
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Ext.IExtObjectContainer oc = this.Db();
			Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem @object = new Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem
				("c1");
			oc.Set(@object);
			Db4objects.Db4o.Ext.IObjectInfo objectInfo1 = oc.GetObjectInfo(@object);
			long oldVer = objectInfo1.GetVersion();
			@object.SetName("c3");
			oc.Set(@object);
			Db4objects.Db4o.Ext.IObjectInfo objectInfo2 = oc.GetObjectInfo(@object);
			long newVer = objectInfo2.GetVersion();
			Db4oUnit.Assert.IsNotNull(objectInfo1.GetUUID());
			Db4oUnit.Assert.IsNotNull(objectInfo2.GetUUID());
			Db4oUnit.Assert.IsTrue(oldVer > 0);
			Db4oUnit.Assert.IsTrue(newVer > 0);
			Db4oUnit.Assert.AreEqual(objectInfo1.GetUUID(), objectInfo2.GetUUID());
			Db4oUnit.Assert.IsTrue(newVer > oldVer);
		}
	}
}
