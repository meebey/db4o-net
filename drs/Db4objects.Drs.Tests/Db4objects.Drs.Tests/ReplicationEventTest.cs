/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class ReplicationEventTest : Db4objects.Drs.Tests.DrsTestCase
	{
		private static readonly string InA = "in A";

		private static readonly string ModifiedInA = "modified in A";

		private static readonly string ModifiedInB = "modified in B";

		public virtual void Test()
		{
			TstNoAction();
			Clean();
			TstNewObject();
			Clean();
			TstOverrideWhenNoConflicts();
			Clean();
			TstOverrideWhenConflicts();
			Clean();
			TstStopTraversal();
		}

		//		tstDeletionDefaultPrevail();
		//		clean();
		//
		//		tstDeletionOverrideToPrevail();
		//		clean();
		//
		//		tstDeletionNotPrevail();
		//		clean();
		private void DeleteInProviderA()
		{
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Tests.SPCParent));
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Tests.SPCChild));
			A().Provider().Commit();
			EnsureNotExist(A().Provider(), typeof(Db4objects.Drs.Tests.SPCChild));
			EnsureNotExist(A().Provider(), typeof(Db4objects.Drs.Tests.SPCParent));
		}

		private void EnsureNames(Db4objects.Drs.Tests.IDrsFixture fixture, string parentName
			, string childName)
		{
			EnsureOneInstanceOfParentAndChild(fixture);
			Db4objects.Drs.Tests.SPCParent parent = (Db4objects.Drs.Tests.SPCParent)GetOneInstance
				(fixture, typeof(Db4objects.Drs.Tests.SPCParent));
			if (!parent.GetName().Equals(parentName))
			{
				Sharpen.Runtime.Out.WriteLine("expected = " + parentName);
				Sharpen.Runtime.Out.WriteLine("actual = " + parent.GetName());
			}
			Db4oUnit.Assert.AreEqual(parent.GetName(), parentName);
			Db4oUnit.Assert.AreEqual(childName, parent.GetChild().GetName());
		}

		private void EnsureNotExist(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 provider, System.Type type)
		{
			Db4oUnit.Assert.IsTrue(!provider.GetStoredObjects(type).GetEnumerator().MoveNext(
				));
		}

		private void EnsureOneInstanceOfParentAndChild(Db4objects.Drs.Tests.IDrsFixture fixture
			)
		{
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Tests.SPCParent));
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Tests.SPCChild));
		}

		private void ModifyInProviderA()
		{
			Db4objects.Drs.Tests.SPCParent parent = (Db4objects.Drs.Tests.SPCParent)GetOneInstance
				(A(), typeof(Db4objects.Drs.Tests.SPCParent));
			parent.SetName(ModifiedInA);
			Db4objects.Drs.Tests.SPCChild child = parent.GetChild();
			child.SetName(ModifiedInA);
			A().Provider().Update(parent);
			A().Provider().Update(child);
			A().Provider().Commit();
			EnsureNames(A(), ModifiedInA, ModifiedInA);
		}

		private void ModifyInProviderB()
		{
			Db4objects.Drs.Tests.SPCParent parent = (Db4objects.Drs.Tests.SPCParent)GetOneInstance
				(B(), typeof(Db4objects.Drs.Tests.SPCParent));
			parent.SetName(ModifiedInB);
			Db4objects.Drs.Tests.SPCChild child = parent.GetChild();
			child.SetName(ModifiedInB);
			B().Provider().Update(parent);
			B().Provider().Update(child);
			B().Provider().Commit();
			EnsureNames(B(), ModifiedInB, ModifiedInB);
		}

		private void ReplicateAllToProviderBFirstTime()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureNames(A(), InA, InA);
			EnsureNames(B(), InA, InA);
		}

		private void StoreParentAndChildToProviderA()
		{
			Db4objects.Drs.Tests.SPCChild child = new Db4objects.Drs.Tests.SPCChild(InA);
			Db4objects.Drs.Tests.SPCParent parent = new Db4objects.Drs.Tests.SPCParent(child, 
				InA);
			A().Provider().StoreNew(parent);
			A().Provider().Commit();
			EnsureNames(A(), InA, InA);
		}

		private void TstNewObject()
		{
			StoreParentAndChildToProviderA();
			Db4objects.Drs.Tests.ReplicationEventTest.BooleanClosure invoked = new Db4objects.Drs.Tests.ReplicationEventTest.BooleanClosure
				(false);
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_221
				(invoked);
			ReplicateAll(A().Provider(), B().Provider(), listener);
			Db4oUnit.Assert.IsTrue(invoked.GetValue());
			EnsureNames(A(), InA, InA);
			EnsureNotExist(B().Provider(), typeof(Db4objects.Drs.Tests.SPCParent));
			EnsureNotExist(B().Provider(), typeof(Db4objects.Drs.Tests.SPCChild));
		}

		private sealed class _IReplicationEventListener_221 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_221(Db4objects.Drs.Tests.ReplicationEventTest.BooleanClosure
				 invoked)
			{
				this.invoked = invoked;
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
				invoked.SetValue(true);
				Db4objects.Drs.IObjectState stateA = @event.StateInProviderA();
				Db4objects.Drs.IObjectState stateB = @event.StateInProviderB();
				Db4oUnit.Assert.IsTrue(stateA.IsNew());
				Db4oUnit.Assert.IsTrue(!stateB.IsNew());
				Db4oUnit.Assert.IsNotNull(stateA.GetObject());
				Db4oUnit.Assert.IsNull(stateB.GetObject());
				@event.OverrideWith(null);
			}

			private readonly Db4objects.Drs.Tests.ReplicationEventTest.BooleanClosure invoked;
		}

		private void TstNoAction()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_252
				();
			//do nothing
			ReplicateAll(B().Provider(), A().Provider(), listener);
			EnsureNames(A(), ModifiedInB, ModifiedInB);
			EnsureNames(B(), ModifiedInB, ModifiedInB);
		}

		private sealed class _IReplicationEventListener_252 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_252()
			{
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
			}
		}

		private void TstOverrideWhenConflicts()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			//introduce conflicts
			ModifyInProviderA();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_272
				();
			ReplicateAll(A().Provider(), B().Provider(), listener);
			EnsureNames(A(), ModifiedInB, ModifiedInB);
			EnsureNames(B(), ModifiedInB, ModifiedInB);
		}

		private sealed class _IReplicationEventListener_272 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_272()
			{
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
				Db4oUnit.Assert.IsTrue(@event.IsConflict());
				if (@event.IsConflict())
				{
					@event.OverrideWith(@event.StateInProviderB());
				}
			}
		}

		private void TstOverrideWhenNoConflicts()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_292
				();
			ReplicateAll(B().Provider(), A().Provider(), listener);
			EnsureNames(A(), InA, InA);
			EnsureNames(B(), InA, InA);
		}

		private sealed class _IReplicationEventListener_292 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_292()
			{
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
				Db4oUnit.Assert.IsTrue(!@event.IsConflict());
				@event.OverrideWith(@event.StateInProviderB());
			}
		}

		private void TstStopTraversal()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			//introduce conflicts
			ModifyInProviderA();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_313
				();
			ReplicateAll(A().Provider(), B().Provider(), listener);
			EnsureNames(A(), ModifiedInA, ModifiedInA);
			EnsureNames(B(), ModifiedInB, ModifiedInB);
		}

		private sealed class _IReplicationEventListener_313 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_313()
			{
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
				Db4oUnit.Assert.IsTrue(@event.IsConflict());
				@event.OverrideWith(null);
			}
		}

		internal class BooleanClosure
		{
			private bool value;

			public BooleanClosure(bool value)
			{
				this.value = value;
			}

			internal virtual void SetValue(bool v)
			{
				value = v;
			}

			public virtual bool GetValue()
			{
				return value;
			}
		}
	}
}
