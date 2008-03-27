/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class CollectionHandlerImplTest : Db4objects.Drs.Tests.DrsTestCase
	{
		private readonly Db4objects.Db4o.Reflect.IReflector _reflector = Db4objects.Drs.Inside.ReplicationReflector
			.GetInstance().Reflector();

		private readonly Db4objects.Drs.Inside.CollectionHandlerImpl _collectionHandler = 
			new Db4objects.Drs.Inside.CollectionHandlerImpl();

		public virtual void TestVector()
		{
			System.Collections.ArrayList vector = new System.Collections.ArrayList();
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(vector));
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(vector))
				);
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(typeof(System.Collections.ArrayList
				)));
		}

		public virtual void TestList()
		{
			System.Collections.IList list = new System.Collections.ArrayList();
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(list));
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(list)));
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(typeof(System.Collections.IList
				)));
		}

		public virtual void TestSet()
		{
			Sharpen.Util.ISet set = new Sharpen.Util.HashSet();
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(set));
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(set)));
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(typeof(Sharpen.Util.ISet)));
		}

		public virtual void TestMap()
		{
			System.Collections.IDictionary map = new System.Collections.Hashtable();
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(map));
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(_reflector.ForObject(map)));
			Db4oUnit.Assert.IsTrue(_collectionHandler.CanHandle(typeof(System.Collections.IDictionary
				)));
		}

		public virtual void TestString()
		{
			string str = "abc";
			Db4oUnit.Assert.IsTrue(!_collectionHandler.CanHandle(str));
			Db4oUnit.Assert.IsTrue(!_collectionHandler.CanHandle(_reflector.ForObject(str)));
			Db4oUnit.Assert.IsTrue(!_collectionHandler.CanHandle(typeof(string)));
		}
	}
}
