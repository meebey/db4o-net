/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Data;

namespace Db4objects.Drs.Tests
{
	public class CustomArrayListTestCase : DrsTestCase
	{
		public virtual void Test()
		{
			NamedList original = new NamedList("foo");
			original.Add("bar");
			A().Provider().StoreNew(original);
			A().Provider().Commit();
			ReplicateAll(A().Provider(), B().Provider());
			NamedList replicated = (NamedList)B().Provider().GetStoredObjects(typeof(NamedList
				)).GetEnumerator().Current;
			Assert.AreEqual(original.Name(), replicated.Name());
			CollectionAssert.AreEqual(original, replicated);
		}
	}
}
