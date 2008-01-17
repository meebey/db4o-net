/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class ObjectVersionTest : AbstractDb4oTestCase
	{
		protected override void Configure(IConfiguration config)
		{
			config.GenerateUUIDs(ConfigScope.Globally);
			config.GenerateVersionNumbers(ConfigScope.Globally);
		}

		public virtual void Test()
		{
			IExtObjectContainer oc = this.Db();
			ObjectVersionTest.Item @object = new ObjectVersionTest.Item("c1");
			oc.Store(@object);
			IObjectInfo objectInfo1 = oc.GetObjectInfo(@object);
			long oldVer = objectInfo1.GetVersion();
			//Update
			@object.SetName("c3");
			oc.Store(@object);
			IObjectInfo objectInfo2 = oc.GetObjectInfo(@object);
			long newVer = objectInfo2.GetVersion();
			Assert.IsNotNull(objectInfo1.GetUUID());
			Assert.IsNotNull(objectInfo2.GetUUID());
			Assert.IsTrue(oldVer > 0);
			Assert.IsTrue(newVer > 0);
			Assert.AreEqual(objectInfo1.GetUUID(), objectInfo2.GetUUID());
			Assert.IsTrue(newVer > oldVer);
		}

		public class Item
		{
			public string name;

			public Item()
			{
			}

			public Item(string name_)
			{
				this.name = name_;
			}

			public virtual string GetName()
			{
				return name;
			}

			public virtual void SetName(string name_)
			{
				name = name_;
			}
		}
	}
}
