/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Api;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class InvalidOffsetInDeleteTestCase : TestWithTempFile, IDiagnosticListener
	{
		public class Item : InvalidOffsetInDeleteTestCase.Parent
		{
			public string _itemName;
		}

		public class Parent
		{
			public string _parentName;
		}

		public virtual void Test()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			Configure(config);
			IObjectContainer objectContainer = Db4oFactory.OpenFile(config, TempFile());
			InvalidOffsetInDeleteTestCase.Item item = new InvalidOffsetInDeleteTestCase.Item(
				);
			item._itemName = "item";
			item._parentName = "parent";
			objectContainer.Store(item);
			objectContainer.Close();
			config = Db4oFactory.NewConfiguration();
			Configure(config);
			objectContainer = Db4oFactory.OpenFile(config, TempFile());
			IQuery query = objectContainer.Query();
			query.Constrain(typeof(InvalidOffsetInDeleteTestCase.Item));
			IObjectSet objectSet = query.Execute();
			item = (InvalidOffsetInDeleteTestCase.Item)objectSet.Next();
			objectContainer.Store(item);
			objectContainer.Close();
		}

		private void Configure(IConfiguration config)
		{
			config.Diagnostic().AddListener(this);
			config.GenerateVersionNumbers(ConfigScope.Globally);
			config.GenerateUUIDs(ConfigScope.Globally);
			config.ObjectClass(typeof(InvalidOffsetInDeleteTestCase.Item)).ObjectField("_itemName"
				).Indexed(true);
			config.ObjectClass(typeof(InvalidOffsetInDeleteTestCase.Parent)).ObjectField("_parentName"
				).Indexed(true);
		}

		public virtual void OnDiagnostic(IDiagnostic d)
		{
			if (d is DeletionFailed)
			{
				Assert.Fail("No deletion failed diagnostic message expected.");
			}
		}
	}
}
