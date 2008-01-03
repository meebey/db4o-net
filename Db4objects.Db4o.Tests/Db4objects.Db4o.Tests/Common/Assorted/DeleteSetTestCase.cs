/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class DeleteSetTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new DeleteSetTestCase().RunAll();
		}

		public class Item
		{
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new DeleteSetTestCase.Item());
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			object item = RetrieveOnlyInstance(typeof(DeleteSetTestCase.Item));
			Db().Delete(item);
			Db().Store(item);
			Db().Commit();
			AssertOccurrences(typeof(DeleteSetTestCase.Item), 1);
		}
	}
}
