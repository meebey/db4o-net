/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	/// <exclude></exclude>
	public class ObjectSetTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ObjectSetTestCase().RunSoloAndClientServer();
		}

		public class Item
		{
			public string name;

			public Item()
			{
			}

			public Item(string name_)
			{
				name = name_;
			}

			public override string ToString()
			{
				return "Item(\"" + name + "\")";
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Db().Store(new ObjectSetTestCase.Item("foo"));
			Db().Store(new ObjectSetTestCase.Item("bar"));
			Db().Store(new ObjectSetTestCase.Item("baz"));
		}

		public virtual void TestObjectsCantBeSeenAfterDelete()
		{
			Transaction trans1 = NewTransaction();
			Transaction trans2 = NewTransaction();
			IObjectSet os = QueryItems(trans1);
			DeleteItemAndCommit(trans2, "foo");
			AssertItems(new string[] { "bar", "baz" }, os);
		}

		public virtual void TestAccessOrder()
		{
			IObjectSet result = NewQuery(typeof(ObjectSetTestCase.Item)).Execute();
			for (int i = 0; i < result.Size(); ++i)
			{
				Assert.IsTrue(result.HasNext());
				Assert.AreSame(result.Ext().Get(i), result.Next());
			}
			Assert.IsFalse(result.HasNext());
		}

		private void AssertItems(string[] expectedNames, IObjectSet actual)
		{
			for (int i = 0; i < expectedNames.Length; i++)
			{
				Assert.IsTrue(actual.HasNext());
				Assert.AreEqual(expectedNames[i], ((ObjectSetTestCase.Item)actual.Next()).name);
			}
			Assert.IsFalse(actual.HasNext());
		}

		private void DeleteItemAndCommit(Transaction trans, string name)
		{
			Stream().Delete(trans, QueryItem(trans, name));
			trans.Commit();
		}

		private ObjectSetTestCase.Item QueryItem(Transaction trans, string name)
		{
			IQuery q = NewQuery(trans, typeof(ObjectSetTestCase.Item));
			q.Descend("name").Constrain(name);
			return (ObjectSetTestCase.Item)q.Execute().Next();
		}

		private IObjectSet QueryItems(Transaction trans)
		{
			IQuery q = NewQuery(trans, typeof(ObjectSetTestCase.Item));
			q.Descend("name").OrderAscending();
			return q.Execute();
		}
	}
}
