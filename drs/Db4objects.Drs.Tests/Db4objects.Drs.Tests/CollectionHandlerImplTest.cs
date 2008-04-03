/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Reflect;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;
using Sharpen.Util;

namespace Db4objects.Drs.Tests
{
	public class CollectionHandlerImplTest : DrsTestCase
	{
		private readonly IReflector _reflector = ReplicationReflector.GetInstance().Reflector
			();

		private readonly CollectionHandlerImpl _collectionHandler = new CollectionHandlerImpl
			();

		public virtual void TestVector()
		{
			ArrayList vector = new ArrayList();
			Assert.IsTrue(_collectionHandler.CanHandle(vector));
			Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(vector)));
			Assert.IsTrue(_collectionHandler.CanHandle(typeof(ArrayList)));
		}

		public virtual void TestList()
		{
			IList list = new ArrayList();
			Assert.IsTrue(_collectionHandler.CanHandle(list));
			Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(list)));
			Assert.IsTrue(_collectionHandler.CanHandle(typeof(IList)));
		}

		public virtual void TestSet()
		{
			ISet set = new HashSet();
			Assert.IsTrue(_collectionHandler.CanHandle(set));
			Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(set)));
			Assert.IsTrue(_collectionHandler.CanHandle(typeof(ISet)));
		}

		public virtual void TestMap()
		{
			IDictionary map = new Hashtable();
			Assert.IsTrue(_collectionHandler.CanHandle(map));
			Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(map)));
			Assert.IsTrue(_collectionHandler.CanHandle(typeof(IDictionary)));
		}

		public virtual void TestString()
		{
			string str = "abc";
			Assert.IsTrue(!_collectionHandler.CanHandle(str));
			Assert.IsTrue(!_collectionHandler.CanHandle(_reflector.ForObject(str)));
			Assert.IsTrue(!_collectionHandler.CanHandle(typeof(string)));
		}
	}
}
