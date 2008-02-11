/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class RollbackTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public string _string;

			public Item()
			{
			}

			public Item(string str)
			{
				_string = str;
			}
		}

		public static void Main(string[] args)
		{
			new RollbackTestCase().RunClientServer();
		}

		public virtual void TestNotIsStoredOnRollback()
		{
			RollbackTestCase.Item item = new RollbackTestCase.Item();
			Store(item);
			Db().Rollback();
			Assert.IsFalse(Db().IsStored(item));
		}
	}
}
