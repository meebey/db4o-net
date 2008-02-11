/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Tests.Common.Assorted;
using Db4objects.Db4o.Tests.Common.Regression;

namespace Db4objects.Db4o.Tests.Common.Regression
{
	public class Case1207TestCase : Db4oClientServerTestCase
	{
		/// <exception cref="Exception"></exception>
		public static void Main(string[] args)
		{
			new Case1207TestCase().RunClientServer();
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			IObjectContainer oc1 = OpenNewClient();
			IObjectContainer oc2 = OpenNewClient();
			IObjectContainer oc3 = OpenNewClient();
			try
			{
				for (int i = 0; i < 1000; i++)
				{
					SimpleObject obj1 = new SimpleObject("oc " + i, i);
					SimpleObject obj2 = new SimpleObject("oc2 " + i, i);
					oc1.Store(obj1);
					oc2.Store(obj2);
					oc2.Rollback();
					obj2 = new SimpleObject("oc2.2 " + i, i);
					oc2.Store(obj2);
				}
				oc1.Commit();
				oc2.Rollback();
				Assert.AreEqual(1000, oc1.Query(typeof(SimpleObject)).Size());
				Assert.AreEqual(1000, oc2.Query(typeof(SimpleObject)).Size());
				Assert.AreEqual(1000, oc3.Query(typeof(SimpleObject)).Size());
			}
			finally
			{
				oc1.Close();
				oc2.Close();
				oc3.Close();
			}
		}
	}
}
