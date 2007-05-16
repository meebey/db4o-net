/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Types.Arrays;

namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class SimpleStringArrayTestCase : AbstractDb4oTestCase
	{
		private static readonly string[] ARRAY = new string[] { "hi", "babe" };

		public class Data
		{
			public string[] _arr;

			public Data(string[] _arr)
			{
				this._arr = _arr;
			}
		}

		protected override void Store()
		{
			Db().Set(new SimpleStringArrayTestCase.Data(ARRAY));
		}

		public virtual void TestRetrieve()
		{
			SimpleStringArrayTestCase.Data data = (SimpleStringArrayTestCase.Data)RetrieveOnlyInstance
				(typeof(SimpleStringArrayTestCase.Data));
			ArrayAssert.AreEqual(ARRAY, data._arr);
		}
	}
}
