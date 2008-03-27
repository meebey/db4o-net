/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class Db4oListTest : Db4objects.Drs.Tests.ListTest
	{
		public override void Test()
		{
			if (!(A().Provider() is Db4objects.Drs.Db4o.IDb4oReplicationProvider))
			{
				return;
			}
			base.ActualTest();
		}

		protected override Db4objects.Drs.Tests.ListHolder CreateHolder()
		{
			Db4objects.Drs.Tests.ListHolder lh = new Db4objects.Drs.Tests.ListHolder("h1");
			Db4objects.Db4o.Types.IDb4oList list = ((Db4objects.Drs.Db4o.IDb4oReplicationProvider
				)A().Provider()).GetObjectContainer().Collections().NewLinkedList();
			lh.SetList(list);
			return lh;
		}
	}
}
