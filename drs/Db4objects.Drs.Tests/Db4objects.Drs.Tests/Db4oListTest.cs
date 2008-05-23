/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Types;
using Db4objects.Drs.Db4o;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class Db4oListTest : ListTest
	{
		public override void Test()
		{
			if (!(A().Provider() is IDb4oReplicationProvider))
			{
				return;
			}
			base.ActualTest();
		}

		protected override ListHolder CreateHolder()
		{
			ListHolder lh = new ListHolder("h1");
			IDb4oList list = ((IDb4oReplicationProvider)A().Provider()).GetObjectContainer().
				Collections().NewLinkedList();
			lh.SetList(list);
			return lh;
		}
	}
}
