/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class NestedArraysTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Concurrency.NestedArraysTestCase().RunConcurrency
				();
		}

		public object _object;

		public object[] _objectArray;

		private const int DEPTH = 5;

		private const int ELEMENTS = 3;

		public NestedArraysTestCase()
		{
		}

		protected override void Store()
		{
			_object = new object[ELEMENTS];
			Fill((object[])_object, DEPTH);
			_objectArray = new object[ELEMENTS];
			Fill(_objectArray, DEPTH);
			Store(this);
		}

		private void Fill(object[] arr, int depth)
		{
			if (depth <= 0)
			{
				arr[0] = "somestring";
				arr[1] = 10;
				return;
			}
			depth--;
			for (int i = 0; i < ELEMENTS; i++)
			{
				arr[i] = new object[ELEMENTS];
				Fill((object[])arr[i], depth);
			}
		}

		public virtual void Conc(IExtObjectContainer oc)
		{
			Db4objects.Db4o.Tests.Common.Concurrency.NestedArraysTestCase nr = (Db4objects.Db4o.Tests.Common.Concurrency.NestedArraysTestCase
				)RetrieveOnlyInstance(oc, typeof(Db4objects.Db4o.Tests.Common.Concurrency.NestedArraysTestCase)
				);
			Check((object[])nr._object, DEPTH);
			Check((object[])nr._objectArray, DEPTH);
		}

		private void Check(object[] arr, int depth)
		{
			if (depth <= 0)
			{
				Assert.AreEqual("somestring", arr[0]);
				Assert.AreEqual(10, arr[1]);
				return;
			}
			depth--;
			for (int i = 0; i < ELEMENTS; i++)
			{
				Check((object[])arr[i], depth);
			}
		}
	}
}
