/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Config;

namespace Db4objects.Db4o.Tests.Common.Config
{
	public class VersionNumbersTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public string _name;
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.GenerateVersionNumbers(ConfigScope.Globally);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			VersionNumbersTestCase.Item item = new VersionNumbersTestCase.Item();
			item._name = "original";
			Store(item);
		}

		public virtual void Test()
		{
			VersionNumbersTestCase.Item item = (VersionNumbersTestCase.Item)RetrieveOnlyInstance
				(typeof(VersionNumbersTestCase.Item));
			IObjectInfo objectInfo = Db().GetObjectInfo(item);
			long version1 = objectInfo.GetVersion();
			item._name = "modified";
			Db().Store(item);
			Db().Commit();
			long version2 = objectInfo.GetVersion();
			Assert.IsGreater(version1, version2);
			Db().Store(item);
			Db().Commit();
			objectInfo = Db().GetObjectInfo(item);
			long version3 = objectInfo.GetVersion();
			Assert.IsGreater(version2, version3);
		}
	}
}
