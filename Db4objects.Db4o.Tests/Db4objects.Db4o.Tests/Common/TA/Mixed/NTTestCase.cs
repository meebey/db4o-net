/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.Mixed;

namespace Db4objects.Db4o.Tests.Common.TA.Mixed
{
	/// <exclude></exclude>
	public class NTTestCase : ItemTestCaseBase
	{
		public static void Main(string[] args)
		{
			new NTTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		protected override object CreateItem()
		{
			return new NTItem(42);
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertRetrievedItem(object obj)
		{
			NTItem item = (NTItem)obj;
			Assert.IsNotNull(item.tItem);
			Assert.AreEqual(0, item.tItem.value);
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertItemValue(object obj)
		{
			NTItem item = (NTItem)obj;
			Assert.AreEqual(42, item.tItem.Value());
		}
	}
}
