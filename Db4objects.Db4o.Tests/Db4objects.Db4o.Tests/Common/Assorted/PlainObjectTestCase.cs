/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class PlainObjectTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new PlainObjectTestCase().RunAll();
		}

		public class Item
		{
			public string _name;

			public object _plainObject;

			public Item(string name, object plainObject)
			{
				_name = name;
				_plainObject = plainObject;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(PlainObjectTestCase.Item)).CascadeOnDelete(true);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			object plainObject = new object();
			PlainObjectTestCase.Item item = new PlainObjectTestCase.Item("one", plainObject);
			Store(item);
			RetrieveItem("one");
			Assert.IsTrue(Db().IsStored(item._plainObject));
			Store(new PlainObjectTestCase.Item("two", plainObject));
		}

		public virtual void TestRetrieve()
		{
			PlainObjectTestCase.Item itemOne = RetrieveItem("one");
			Assert.IsNotNull(itemOne._plainObject);
			Assert.IsTrue(Db().IsStored(itemOne._plainObject));
			PlainObjectTestCase.Item itemTwo = RetrieveItem("two");
			Assert.AreSame(itemOne._plainObject, itemTwo._plainObject);
		}

		public virtual void TestDelete()
		{
			PlainObjectTestCase.Item itemOne = RetrieveItem("one");
			Db().Delete(itemOne);
		}

		public virtual void _testEvaluationQuery()
		{
			// The evaluation doesn't work in C/S mode
			// because TransportObjectContainer#storeInteral  
			// never gets a chance to intercept.
			PlainObjectTestCase.Item itemOne = RetrieveItem("one");
			object plainObject = itemOne._plainObject;
			IQuery q = NewQuery(typeof(PlainObjectTestCase.Item));
			q.Constrain(new _IEvaluation_65(this, plainObject));
			IObjectSet objectSet = q.Execute();
			Assert.AreEqual(2, objectSet.Size());
		}

		private sealed class _IEvaluation_65 : IEvaluation
		{
			public _IEvaluation_65(PlainObjectTestCase _enclosing, object plainObject)
			{
				this._enclosing = _enclosing;
				this.plainObject = plainObject;
			}

			public void Evaluate(ICandidate candidate)
			{
				PlainObjectTestCase.Item item = (PlainObjectTestCase.Item)candidate.GetObject();
				candidate.Include(item._plainObject == plainObject);
			}

			private readonly PlainObjectTestCase _enclosing;

			private readonly object plainObject;
		}

		public virtual void TestIdentityQuery()
		{
			PlainObjectTestCase.Item itemOne = RetrieveItem("one");
			object plainObject = itemOne._plainObject;
			IQuery q = NewQuery(typeof(PlainObjectTestCase.Item));
			q.Descend("_plainObject").Constrain(plainObject).Identity();
			IObjectSet objectSet = q.Execute();
			Assert.AreEqual(2, objectSet.Size());
		}

		private PlainObjectTestCase.Item RetrieveItem(string name)
		{
			IQuery query = NewQuery(typeof(PlainObjectTestCase.Item));
			query.Descend("_name").Constrain(name);
			IObjectSet objectSet = query.Execute();
			Assert.AreEqual(1, objectSet.Size());
			return (PlainObjectTestCase.Item)objectSet.Next();
		}
	}
}
