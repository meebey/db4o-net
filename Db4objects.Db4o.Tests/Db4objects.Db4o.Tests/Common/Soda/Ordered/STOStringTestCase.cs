/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Soda.Util;

namespace Db4objects.Db4o.Tests.Common.Soda.Ordered
{
	public class STOStringTestCase : SodaBaseTestCase
	{
		public string foo;

		public STOStringTestCase()
		{
		}

		public STOStringTestCase(string str)
		{
			this.foo = str;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase
				(null), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase("bbb"), 
				new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase("bbb"), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase
				("dod"), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase("aaa"), 
				new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase("Xbb"), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase
				("bbq") };
		}

		public virtual void TestAscending()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase));
			q.Descend("foo").OrderAscending();
			ExpectOrdered(q, new int[] { 5, 4, 1, 2, 6, 3, 0 });
		}

		public virtual void TestDescending()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase));
			q.Descend("foo").OrderDescending();
			ExpectOrdered(q, new int[] { 3, 6, 2, 1, 4, 5, 0 });
		}

		public virtual void TestAscendingLike()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase));
			IQuery qStr = q.Descend("foo");
			qStr.Constrain("b").Like();
			qStr.OrderAscending();
			ExpectOrdered(q, new int[] { 5, 1, 2, 6 });
		}

		public virtual void TestDescendingContains()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOStringTestCase));
			IQuery qStr = q.Descend("foo");
			qStr.Constrain("b").Contains();
			qStr.OrderDescending();
			ExpectOrdered(q, new int[] { 6, 2, 1, 5 });
		}
	}
}
