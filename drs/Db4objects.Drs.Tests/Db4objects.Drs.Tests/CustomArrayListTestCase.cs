/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class CustomArrayListTestCase : DrsTestCase
	{
		public class NamedList : DelegatingList
		{
			private string _name;

			public NamedList(string name) : base(new ArrayList())
			{
				_name = name;
			}

			public virtual string Name()
			{
				return _name;
			}
		}

		public virtual void Test()
		{
			CustomArrayListTestCase.NamedList original = new CustomArrayListTestCase.NamedList
				("foo");
			original.Add("bar");
			A().Provider().StoreNew(original);
			A().Provider().Commit();
			ReplicateAll(A().Provider(), B().Provider());
			CustomArrayListTestCase.NamedList replicated = (CustomArrayListTestCase.NamedList
				)B().Provider().GetStoredObjects(typeof(CustomArrayListTestCase.NamedList))[0];
			Assert.AreEqual(original.Name(), replicated.Name());
			CollectionAssert.AreEqual(original, replicated);
		}
	}
}
