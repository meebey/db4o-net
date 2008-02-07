/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	/// <summary>Regression test case for COR-1117</summary>
	public class CallbackTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new CallbackTestCase().RunAll();
		}

		public virtual void Test()
		{
			CallbackTestCase.Item item = new CallbackTestCase.Item();
			Store(item);
			Db().Commit();
			Assert.IsTrue(item.IsStored());
			Assert.IsTrue(Db().Ext().IsStored(item));
			IObjectSet result = RetrieveItems();
			Assert.AreEqual(1, result.Size());
			CallbackTestCase.Item retrievedItem = (CallbackTestCase.Item)result.Next();
			retrievedItem.Save();
			result = RetrieveItems();
			Assert.AreEqual(1, result.Size());
		}

		internal virtual IObjectSet RetrieveItems()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(CallbackTestCase.Item));
			return q.Execute();
		}

		public class Item
		{
			public string test;

			[System.NonSerialized]
			public IObjectContainer _objectContainer;

			public virtual void ObjectOnNew(IObjectContainer container)
			{
				_objectContainer = container;
			}

			public virtual bool IsStored()
			{
				return _objectContainer.Ext().IsStored(this);
			}

			public virtual void Save()
			{
				_objectContainer.Store(this);
				_objectContainer.Commit();
			}
		}
	}
}
