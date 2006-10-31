/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class CsStructs : AbstractDb4oTestCase
	{
		public static string GUID = "6a0d8033-444e-4b44-b0df-bf33dfe050f9";

		SimpleStruct simpleStruct;
		RecursiveStruct recursiveStruct;
		Guid guid;

		public CsStructs()
		{
		}

		protected override void  Store()
	    {
			simpleStruct.foo = 100;
			simpleStruct.bar = "hi";

			RecursiveStruct r = new RecursiveStruct();
			r.child = new CsStructs();

			SimpleStruct s = new SimpleStruct();
			s.foo = 22;
			s.bar = "jo";
			r.child.simpleStruct = s;

			recursiveStruct = r;

			guid = new Guid(GUID);
		    
		    Store(this);
		}

		public void Test()
		{
			IExtObjectContainer oc = Db();
            IQuery q = NewQuery(GetType());
			IQuery qd = q.Descend("simpleStruct");
			qd = qd.Descend("foo");
			qd.Constrain(100);

			IObjectSet objectSet = q.Execute();

			Assert.AreEqual(1, objectSet.Size());
			CsStructs csStructs = (CsStructs)objectSet.Next();

			Assert.AreEqual(GUID, csStructs.guid.ToString());
            Assert.AreEqual(100, csStructs.simpleStruct.foo);
            Assert.AreEqual("hi", csStructs.simpleStruct.bar);
            Assert.AreEqual(22, csStructs.recursiveStruct.child.simpleStruct.foo);
            Assert.AreEqual("jo", csStructs.recursiveStruct.child.simpleStruct.bar);
		}

	}

	public struct SimpleStruct
	{
		public int foo;
		public string bar;
	}

	public struct RecursiveStruct
	{
		public CsStructs child;
	}
}
