/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class COR756TestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new COR756TestCase().RunClientServer();
		}

		public class A
		{
			public virtual bool IsReferencedBy(COR756TestCase.B b)
			{
				return this.Equals(b.a);
			}
		}

		public class B
		{
			public COR756TestCase.A a;
		}

		[System.Serializable]
		public class BReferencedFromAPredicate : Predicate
		{
			public COR756TestCase.A _a;

			public BReferencedFromAPredicate(COR756TestCase.A a)
			{
				_a = a;
			}

			public virtual bool Match(object b)
			{
				return _a.IsReferencedBy((COR756TestCase.B)b);
			}
		}

		public virtual void _test()
		{
			COR756TestCase.A a = new COR756TestCase.A();
			COR756TestCase.B b = new COR756TestCase.B();
			b.a = a;
			IObjectContainer oc = Db();
			oc.Store(b);
			oc.Commit();
			Assert.AreEqual(1, oc.Query(new COR756TestCase.BReferencedFromAPredicate(a)).Size
				());
		}
	}
}
