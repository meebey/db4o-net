/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class ReplicationAfterDeletionTest : DrsTestCase
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
			Delete(new Type[] { typeof(SPCChild), typeof(SPCParent) });
		}

		private void Replicate()
		{
			SPCChild child = new SPCChild("c1");
			SPCParent parent = new SPCParent(child, "p1");
			A().Provider().StoreNew(parent);
			A().Provider().Commit();
			ReplicateAll(A().Provider(), B().Provider());
		}
	}
}
