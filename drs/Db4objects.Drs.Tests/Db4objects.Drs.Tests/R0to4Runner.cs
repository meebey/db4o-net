/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Test
{
	public class R0to4Runner : Db4objects.Drs.Test.DrsTestCase
	{
		private const int LINKERS = 4;

		public R0to4Runner() : base()
		{
		}

		protected override void Clean()
		{
			Delete(A().Provider());
			Delete(B().Provider());
		}

		protected virtual void Delete(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 provider)
		{
			System.Collections.ArrayList toDelete = new System.Collections.ArrayList();
			System.Collections.IEnumerator rr = provider.GetStoredObjects(typeof(Db4objects.Drs.Test.R0)
				).GetEnumerator();
			while (rr.MoveNext())
			{
				object o = rr.Current;
				Db4objects.Db4o.Reflect.IReflectClass claxx = Db4objects.Drs.Inside.ReplicationReflector
					.GetInstance().Reflector().ForObject(o);
				SetFieldsToNull(o, claxx);
				toDelete.Add(o);
			}
			for (System.Collections.IEnumerator iterator = toDelete.GetEnumerator(); iterator
				.MoveNext(); )
			{
				object o = iterator.Current;
				provider.Delete(o);
			}
			provider.Commit();
		}

		private void CompareR4(Db4objects.Drs.Inside.ITestableReplicationProviderInside a
			, Db4objects.Drs.Inside.ITestableReplicationProviderInside b, bool isSameExpected
			)
		{
			System.Collections.IEnumerator it = a.GetStoredObjects(typeof(Db4objects.Drs.Test.R4)
				).GetEnumerator();
			while (it.MoveNext())
			{
				string name = ((Db4objects.Drs.Test.R4)it.Current).name;
				System.Collections.IEnumerator it2 = b.GetStoredObjects(typeof(Db4objects.Drs.Test.R4)
					).GetEnumerator();
				bool found = false;
				while (it2.MoveNext())
				{
					string name2 = ((Db4objects.Drs.Test.R4)it2.Current).name;
					if (name.Equals(name2))
					{
						found = true;
					}
				}
				Db4oUnit.Assert.IsTrue(found == isSameExpected);
			}
		}

		private void CopyAllToB(Db4objects.Drs.Inside.ITestableReplicationProviderInside 
			peerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside peerB)
		{
			Db4oUnit.Assert.IsTrue(ReplicateAll(peerA, peerB, false) == LINKERS * 5);
		}

		private void EnsureCount(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 provider, int linkers)
		{
			EnsureCount(provider, typeof(Db4objects.Drs.Test.R0), linkers * 5);
			EnsureCount(provider, typeof(Db4objects.Drs.Test.R1), linkers * 4);
			EnsureCount(provider, typeof(Db4objects.Drs.Test.R2), linkers * 3);
			EnsureCount(provider, typeof(Db4objects.Drs.Test.R3), linkers * 2);
			EnsureCount(provider, typeof(Db4objects.Drs.Test.R4), linkers * 1);
		}

		private void EnsureCount(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 provider, System.Type clazz, int count)
		{
			System.Collections.IEnumerator instances = provider.GetStoredObjects(clazz).GetEnumerator
				();
			int i = count;
			while (instances.MoveNext())
			{
				object o = instances.Current;
				i--;
			}
			Db4oUnit.Assert.IsTrue(i == 0);
		}

		private void EnsureR4Different(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 peerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside peerB)
		{
			CompareR4(peerB, peerA, false);
		}

		private void EnsureR4Same(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 peerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside peerB)
		{
			CompareR4(peerB, peerA, true);
			CompareR4(peerA, peerB, true);
		}

		private void Init(Db4objects.Drs.Inside.ITestableReplicationProviderInside peerA)
		{
			Db4objects.Drs.Test.R0Linker lCircles = new Db4objects.Drs.Test.R0Linker();
			lCircles.SetNames("circles");
			lCircles.LinkCircles();
			lCircles.Store(peerA);
			Db4objects.Drs.Test.R0Linker lList = new Db4objects.Drs.Test.R0Linker();
			lList.SetNames("list");
			lList.LinkList();
			lList.Store(peerA);
			Db4objects.Drs.Test.R0Linker lThis = new Db4objects.Drs.Test.R0Linker();
			lThis.SetNames("this");
			lThis.LinkThis();
			lThis.Store(peerA);
			Db4objects.Drs.Test.R0Linker lBack = new Db4objects.Drs.Test.R0Linker();
			lBack.SetNames("back");
			lBack.LinkBack();
			lBack.Store(peerA);
			peerA.Commit();
		}

		private void ModifyR4(Db4objects.Drs.Inside.ITestableReplicationProviderInside provider
			)
		{
			System.Collections.IEnumerator it = provider.GetStoredObjects(typeof(Db4objects.Drs.Test.R4)
				).GetEnumerator();
			while (it.MoveNext())
			{
				Db4objects.Drs.Test.R4 r4 = (Db4objects.Drs.Test.R4)it.Current;
				r4.name = r4.name + "_";
				provider.Update(r4);
			}
			provider.Commit();
		}

		private int Replicate(Db4objects.Drs.Inside.ITestableReplicationProviderInside peerA
			, Db4objects.Drs.Inside.ITestableReplicationProviderInside peerB)
		{
			return ReplicateAll(peerA, peerB, true);
		}

		private int ReplicateAll(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 peerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside peerB, bool modifiedOnly
			)
		{
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(peerA, peerB);
			System.Collections.IEnumerator it = modifiedOnly ? peerA.ObjectsChangedSinceLastReplication
				(typeof(Db4objects.Drs.Test.R0)).GetEnumerator() : peerA.GetStoredObjects(typeof(Db4objects.Drs.Test.R0)
				).GetEnumerator();
			int replicated = 0;
			while (it.MoveNext())
			{
				Db4objects.Drs.Test.R0 r0 = (Db4objects.Drs.Test.R0)it.Current;
				replication.Replicate(r0);
				replicated++;
			}
			replication.Commit();
			EnsureCount(peerA, LINKERS);
			EnsureCount(peerB, LINKERS);
			return replicated;
		}

		private void ReplicateNoneModified(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 peerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside peerB)
		{
			Db4oUnit.Assert.IsTrue(Replicate(peerA, peerB) == 0);
		}

		private void ReplicateR4(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 peerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside peerB)
		{
			int replicatedObjectsCount = ReplicateAll(peerA, peerB, true);
			Db4oUnit.Assert.IsTrue(replicatedObjectsCount == LINKERS);
		}

		private void SetFieldsToNull(object @object, Db4objects.Db4o.Reflect.IReflectClass
			 claxx)
		{
			Db4objects.Db4o.Reflect.IReflectField[] fields;
			fields = claxx.GetDeclaredFields();
			for (int i = 0; i < fields.Length; i++)
			{
				Db4objects.Db4o.Reflect.IReflectField field = fields[i];
				if (field.IsStatic())
				{
					continue;
				}
				if (field.IsTransient())
				{
					continue;
				}
				field.SetAccessible();
				field.Set(@object, null);
			}
			Db4objects.Db4o.Reflect.IReflectClass superclass = claxx.GetSuperclass();
			if (superclass == null)
			{
				return;
			}
			SetFieldsToNull(@object, superclass);
		}

		public virtual void Test()
		{
			ActualTest();
		}

		protected virtual void ActualTest()
		{
			Init(A().Provider());
			EnsureCount(A().Provider(), LINKERS);
			CopyAllToB(A().Provider(), B().Provider());
			ReplicateNoneModified(A().Provider(), B().Provider());
			ModifyR4(A().Provider());
			EnsureR4Different(A().Provider(), B().Provider());
			ReplicateR4(A().Provider(), B().Provider());
			EnsureR4Same(A().Provider(), B().Provider());
		}
	}
}
