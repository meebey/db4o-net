/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class NestedArraysTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new NestedArraysTestCase().RunConcurrency();
		}

		public object _object;

		public object[] _objectArray;

		private const int Depth = 5;

		private const int Elements = 3;

		public NestedArraysTestCase()
		{
		}

		protected override void Store()
		{
			_object = new object[Elements];
			Fill((object[])_object, Depth);
			_objectArray = new object[Elements];
			Fill(_objectArray, Depth);
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
			for (int i = 0; i < Elements; i++)
			{
				arr[i] = new object[Elements];
				Fill((object[])arr[i], depth);
			}
		}

		public virtual void Conc(IExtObjectContainer oc)
		{
			NestedArraysTestCase nr = (NestedArraysTestCase)RetrieveOnlyInstance(oc, typeof(NestedArraysTestCase
				));
			Check((object[])nr._object, Depth);
			Check((object[])nr._objectArray, Depth);
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
			for (int i = 0; i < Elements; i++)
			{
				Check((object[])arr[i], depth);
			}
		}
	}
}
