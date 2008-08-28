/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
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

		public virtual void TestBaseClass()
		{
			RunTest(new CallbackTestCase.Item());
		}

		public virtual void TestDerived()
		{
			RunTest(new CallbackTestCase.DerivedItem());
		}

		private void RunTest(CallbackTestCase.Item item)
		{
			Store(item);
			Db().Commit();
			Assert.IsTrue(item.IsStored());
			Assert.IsTrue(Db().Ext().IsStored(item));
		}

		public class Item
		{
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
		}

		public class DerivedItem : CallbackTestCase.Item
		{
		}
	}
}
