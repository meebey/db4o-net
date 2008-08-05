/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Migration;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	/// <exclude></exclude>
	public class ListTypeHandlerMigrationSimulationTestCase : ITestLifeCycle
	{
		public class Item
		{
			public IList list;
		}

		private string _fileName;

		internal bool _useListTypeHandler;

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			_fileName = Path.GetTempFileName();
			File4.Delete(_fileName);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			File4.Delete(_fileName);
		}

		public virtual void TestMigration()
		{
			if (TypeHandlerConfiguration.Enabled())
			{
				// Then we always have a list Typehandler installed.
				// This test no longer makes sense.
				return;
			}
			_useListTypeHandler = false;
			StoreItemWithListElement("one");
			StoreItemWithListElement("two");
			StoreItemWithListElement("three");
			StoreItemWithListElement(42);
			AssertSingleItemElementQuery("one");
			AssertNoItemFoundByElement("four");
			_useListTypeHandler = true;
			AssertSingleItemElementQuery("one");
			AssertSingleItemElementQuery(42);
			UpdateItemByListElement("one", "newOne");
			AssertNoItemFoundByElement("one");
			AssertSingleItemElementQuery("two");
		}

		private void AssertSingleItemElementQuery(object element)
		{
			IObjectContainer db = OpenContainer();
			try
			{
				ListTypeHandlerMigrationSimulationTestCase.Item item = RetrieveItemByElement(element
					, db);
				object listElement = item.list[0];
				Assert.AreEqual(element, listElement);
			}
			finally
			{
				db.Close();
			}
		}

		private ListTypeHandlerMigrationSimulationTestCase.Item RetrieveItemByElement(object
			 element, IObjectContainer db)
		{
			IQuery q = db.Query();
			q.Constrain(typeof(ListTypeHandlerMigrationSimulationTestCase.Item));
			q.Descend("list").Constrain(element);
			IObjectSet objectSet = q.Execute();
			Assert.AreEqual(1, objectSet.Size());
			ListTypeHandlerMigrationSimulationTestCase.Item item = (ListTypeHandlerMigrationSimulationTestCase.Item
				)objectSet.Next();
			return item;
		}

		private void AssertNoItemFoundByElement(object element)
		{
			IObjectContainer db = OpenContainer();
			try
			{
				IQuery q = db.Query();
				q.Constrain(typeof(ListTypeHandlerMigrationSimulationTestCase.Item));
				q.Descend("list").Constrain(element);
				IObjectSet objectSet = q.Execute();
				Assert.AreEqual(0, objectSet.Size());
			}
			finally
			{
				db.Close();
			}
		}

		private void UpdateItemByListElement(object oldElement, object newElement)
		{
			IObjectContainer db = OpenContainer();
			try
			{
				ListTypeHandlerMigrationSimulationTestCase.Item item = RetrieveItemByElement(oldElement
					, db);
				item.list.Clear();
				item.list.Add(newElement);
				db.Store(item.list);
				db.Store(item);
			}
			finally
			{
				db.Close();
			}
		}

		private void StoreItemWithListElement(object element)
		{
			ListTypeHandlerMigrationSimulationTestCase.Item item = new ListTypeHandlerMigrationSimulationTestCase.Item
				();
			item.list = new ArrayList();
			item.list.Add(element);
			IObjectContainer db = OpenContainer();
			try
			{
				db.Store(item);
			}
			finally
			{
				db.Close();
			}
		}

		private void Store(ListTypeHandlerMigrationSimulationTestCase.Item item)
		{
			IObjectContainer db = OpenContainer();
			try
			{
				db.Store(item);
			}
			finally
			{
				db.Close();
			}
		}

		private void UpdateItem()
		{
			IObjectContainer db = OpenContainer();
			try
			{
				IObjectSet objectSet = db.Query(typeof(ListTypeHandlerMigrationSimulationTestCase.Item
					));
				db.Store(objectSet.Next());
			}
			finally
			{
				db.Close();
			}
		}

		private IObjectContainer OpenContainer()
		{
			IConfiguration configuration = Db4oFactory.NewConfiguration();
			if (_useListTypeHandler)
			{
				configuration.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(typeof(ArrayList
					)), new ListTypeHandler());
			}
			IObjectContainer db = Db4oFactory.OpenFile(configuration, _fileName);
			return db;
		}
	}
}
