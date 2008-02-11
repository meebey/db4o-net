/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Header;

namespace Db4objects.Db4o.Tests.Common.Header
{
	public class SimpleTimeStampIdTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public static void Main(string[] arguments)
		{
			new SimpleTimeStampIdTestCase().RunSolo();
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

		protected override void Configure(IConfiguration config)
		{
			IObjectClass objectClass = config.ObjectClass(typeof(SimpleTimeStampIdTestCase.STSItem
				));
			objectClass.GenerateUUIDs(true);
			objectClass.GenerateVersionNumbers(true);
		}

		protected override void Store()
		{
			Db().Store(new SimpleTimeStampIdTestCase.STSItem("one"));
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			SimpleTimeStampIdTestCase.STSItem item = (SimpleTimeStampIdTestCase.STSItem)Db().
				QueryByExample(typeof(SimpleTimeStampIdTestCase.STSItem)).Next();
			long version = Db().GetObjectInfo(item).GetVersion();
			Assert.IsGreater(0, version);
			Assert.IsGreaterOrEqual(version, CurrentVersion());
			Reopen();
			SimpleTimeStampIdTestCase.STSItem item2 = new SimpleTimeStampIdTestCase.STSItem("two"
				);
			Db().Store(item2);
			long secondVersion = Db().GetObjectInfo(item2).GetVersion();
			Assert.IsGreater(version, secondVersion);
			Assert.IsGreaterOrEqual(secondVersion, CurrentVersion());
		}

		private long CurrentVersion()
		{
			return ((LocalObjectContainer)Db()).CurrentVersion();
		}
	}
}
