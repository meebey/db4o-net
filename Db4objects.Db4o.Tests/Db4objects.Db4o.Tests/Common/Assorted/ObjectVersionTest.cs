using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ObjectVersionTest : AbstractDb4oTestCase
	{
		protected override void Configure(IConfiguration config)
		{
			config.GenerateUUIDs(ConfigScope.GLOBALLY);
			config.GenerateVersionNumbers(ConfigScope.GLOBALLY);
		}

		public virtual void Test()
		{
			IExtObjectContainer oc = this.Db();
			SimplestPossibleItem @object = new SimplestPossibleItem("c1");
			oc.Set(@object);
			IObjectInfo objectInfo1 = oc.GetObjectInfo(@object);
			long oldVer = objectInfo1.GetVersion();
			@object.SetName("c3");
			oc.Set(@object);
			IObjectInfo objectInfo2 = oc.GetObjectInfo(@object);
			long newVer = objectInfo2.GetVersion();
			Assert.IsNotNull(objectInfo1.GetUUID());
			Assert.IsNotNull(objectInfo2.GetUUID());
			Assert.IsTrue(oldVer > 0);
			Assert.IsTrue(newVer > 0);
			Assert.AreEqual(objectInfo1.GetUUID(), objectInfo2.GetUUID());
			Assert.IsTrue(newVer > oldVer);
		}
	}
}
