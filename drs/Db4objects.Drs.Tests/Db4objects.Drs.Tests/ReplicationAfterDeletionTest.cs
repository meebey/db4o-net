/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class ReplicationAfterDeletionTest : Db4objects.Drs.Tests.DrsTestCase
	{
		public virtual void Test()
		{
			Replicate();
			Clean();
			Replicate();
			Clean();
		}

		protected override void Clean()
		{
			Delete(new System.Type[] { typeof(Db4objects.Drs.Tests.SPCChild), typeof(Db4objects.Drs.Tests.SPCParent
				) });
		}

		private void Replicate()
		{
			Db4objects.Drs.Tests.SPCChild child = new Db4objects.Drs.Tests.SPCChild("c1");
			Db4objects.Drs.Tests.SPCParent parent = new Db4objects.Drs.Tests.SPCParent(child, 
				"p1");
			A().Provider().StoreNew(parent);
			A().Provider().Commit();
			ReplicateAll(A().Provider(), B().Provider());
		}
	}
}
