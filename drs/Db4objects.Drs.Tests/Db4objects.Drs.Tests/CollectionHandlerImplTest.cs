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
using System.Collections;
using Db4oUnit;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class CollectionHandlerImplTest : DrsTestCase
	{
		private CollectionHandlerImpl _collectionHandler;

		public virtual void TestVector()
		{
			ArrayList vector = new ArrayList();
			Assert.IsTrue(CollectionHandler().CanHandle(vector));
			Assert.IsTrue(CollectionHandler().CanHandleClass(ReplicationReflector().ForObject
				(vector)));
			Assert.IsTrue(CollectionHandler().CanHandleClass(typeof(ArrayList)));
		}

		public virtual void TestMap()
		{
			IDictionary map = new Hashtable();
			Assert.IsTrue(CollectionHandler().CanHandle(map));
			Assert.IsTrue(CollectionHandler().CanHandleClass(ReplicationReflector().ForObject
				(map)));
			Assert.IsTrue(CollectionHandler().CanHandleClass(typeof(IDictionary)));
		}

		public virtual void TestString()
		{
			string str = "abc";
			Assert.IsTrue(!CollectionHandler().CanHandle(str));
			Assert.IsTrue(!CollectionHandler().CanHandleClass(ReplicationReflector().ForObject
				(str)));
			Assert.IsTrue(!CollectionHandler().CanHandleClass(typeof(string)));
		}

		private Db4objects.Drs.Inside.ICollectionHandler CollectionHandler()
		{
			if (_collectionHandler == null)
			{
				_collectionHandler = new CollectionHandlerImpl(ReplicationReflector());
			}
			return _collectionHandler;
		}
	}
}
