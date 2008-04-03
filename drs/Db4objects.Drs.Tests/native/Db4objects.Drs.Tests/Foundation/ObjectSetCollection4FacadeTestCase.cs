/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs.Tests.Foundation
{
	public class ObjectSetCollection4FacadeTestCase : Db4oUnit.ITestCase
	{
		public static void Main(string[] args)
		{
			new Db4oUnit.ConsoleTestRunner(typeof(Db4objects.Drs.Tests.Foundation.ObjectSetCollection4FacadeTestCase)
				).Run();
		}

		public virtual void TestEmpty()
		{
			Db4objects.Drs.Foundation.ObjectSetCollection4Facade facade = new Db4objects.Drs.Foundation.ObjectSetCollection4Facade
				(new Db4objects.Db4o.Foundation.Collection4());
			Db4oUnit.Assert.IsFalse(facade.HasNext());
			Db4oUnit.Assert.IsFalse(facade.HasNext());
		}

		public virtual void TestIteration()
		{
			Db4objects.Db4o.Foundation.Collection4 collection = new Db4objects.Db4o.Foundation.Collection4
				();
			collection.Add("bar");
			collection.Add("foo");
			Db4objects.Drs.Foundation.ObjectSetCollection4Facade facade = new Db4objects.Drs.Foundation.ObjectSetCollection4Facade
				(collection);
			Db4oUnit.Assert.IsTrue(facade.HasNext());
			Db4oUnit.Assert.AreEqual("bar", facade.Next());
			Db4oUnit.Assert.IsTrue(facade.HasNext());
			Db4oUnit.Assert.AreEqual("foo", facade.Next());
			Db4oUnit.Assert.IsFalse(facade.HasNext());
			facade.Reset();
			Db4oUnit.Assert.AreEqual("bar", facade.Next());
			Db4oUnit.Assert.AreEqual("foo", facade.Next());
			Db4oUnit.Assert.IsFalse(facade.HasNext());
		}
	}
}
