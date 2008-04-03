/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs.Tests.Regression
{
	public class DRS42Test : Db4objects.Drs.Tests.DrsTestCase
	{
		internal Db4objects.Drs.Tests.Regression.NewPilot andrew = new Db4objects.Drs.Tests.Regression.NewPilot
			("Andrew", 100, new int[] { 100, 200, 300 });

		public virtual void Test()
		{
			StoreToProviderA();
			ReplicateAllToProviderB();
		}

		internal virtual void StoreToProviderA()
		{
			Db4objects.Drs.Inside.ITestableReplicationProviderInside provider = A().Provider(
				);
			provider.StoreNew(andrew);
			provider.Commit();
			EnsureContent(andrew, provider);
		}

		internal virtual void ReplicateAllToProviderB()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureContent(andrew, B().Provider());
		}

		private void EnsureContent(Db4objects.Drs.Tests.Regression.NewPilot newPilot, Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 provider)
		{
			Db4objects.Db4o.IObjectSet result = provider.GetStoredObjects(typeof(Db4objects.Drs.Tests.Regression.NewPilot)
				);
			Db4oUnit.Assert.AreEqual(1, result.Count);
			Db4objects.Drs.Tests.Regression.NewPilot p = (Db4objects.Drs.Tests.Regression.NewPilot
				)result.Next();
			Db4oUnit.Assert.AreEqual(newPilot.GetName(), p.GetName());
			Db4oUnit.Assert.AreEqual(newPilot.GetPoints(), p.GetPoints());
			for (int j = 0; j < newPilot.GetArr().Length; j++)
			{
				Db4oUnit.Assert.AreEqual(newPilot.GetArr()[j], p.GetArr()[j]);
			}
		}
	}
}
