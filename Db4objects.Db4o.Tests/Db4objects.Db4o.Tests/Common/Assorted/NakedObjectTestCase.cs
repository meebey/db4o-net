/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class NakedObjectTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public object field = new object();
		}

		public virtual void TestStoreNakedObjects()
		{
			try
			{
				Db().Set(new NakedObjectTestCase.Item());
				Assert.Fail("Naked objects can't be stored");
			}
			catch (ObjectNotStorableException)
			{
			}
		}
	}
}
