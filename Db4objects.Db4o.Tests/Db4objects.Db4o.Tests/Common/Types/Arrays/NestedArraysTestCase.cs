/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Types.Arrays;

namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class NestedArraysTestCase : AbstractDb4oTestCase
	{
		private const int DEPTH = 5;

		private const int ELEMENTS = 3;

		public class Data
		{
			public object _obj;

			public object[] _arr;

			public Data(object obj, object[] arr)
			{
				this._obj = obj;
				_arr = arr;
			}
		}

		protected override void Store()
		{
			object[] obj = new object[ELEMENTS];
			Fill(obj, DEPTH);
			object[] arr = new object[ELEMENTS];
			Fill(arr, DEPTH);
			Db().Set(new NestedArraysTestCase.Data(obj, arr));
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

		public virtual void TestOne()
		{
			NestedArraysTestCase.Data data = (NestedArraysTestCase.Data)RetrieveOnlyInstance(
				typeof(NestedArraysTestCase.Data));
			Db().Activate(data, int.MaxValue);
			Check((object[])data._obj, DEPTH);
			Check(data._arr, DEPTH);
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
