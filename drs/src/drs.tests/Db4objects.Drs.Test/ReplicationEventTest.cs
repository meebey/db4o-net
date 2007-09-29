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
	public class ReplicationEventTest : Db4objects.Drs.Test.DrsTestCase
	{
		private static readonly string IN_A = "in A";

		private static readonly string MODIFIED_IN_A = "modified in A";

		private static readonly string MODIFIED_IN_B = "modified in B";

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

		private void DeleteInProviderA()
		{
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Test.SPCParent));
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Test.SPCChild));
			A().Provider().Commit();
			EnsureNotExist(A().Provider(), typeof(Db4objects.Drs.Test.SPCChild));
			EnsureNotExist(A().Provider(), typeof(Db4objects.Drs.Test.SPCParent));
		}

		private void EnsureNames(Db4objects.Drs.Test.IDrsFixture fixture, string parentName
			, string childName)
		{
			EnsureOneInstanceOfParentAndChild(fixture);
			Db4objects.Drs.Test.SPCParent parent = (Db4objects.Drs.Test.SPCParent)GetOneInstance
				(fixture, typeof(Db4objects.Drs.Test.SPCParent));
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

		private void EnsureOneInstanceOfParentAndChild(Db4objects.Drs.Test.IDrsFixture fixture
			)
		{
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Test.SPCParent));
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Test.SPCChild));
		}

		private void ModifyInProviderA()
		{
			Db4objects.Drs.Test.SPCParent parent = (Db4objects.Drs.Test.SPCParent)GetOneInstance
				(A(), typeof(Db4objects.Drs.Test.SPCParent));
			parent.SetName(MODIFIED_IN_A);
			Db4objects.Drs.Test.SPCChild child = parent.GetChild();
			child.SetName(MODIFIED_IN_A);
			A().Provider().Update(parent);
			A().Provider().Update(child);
			A().Provider().Commit();
			EnsureNames(A(), MODIFIED_IN_A, MODIFIED_IN_A);
		}

		private void ModifyInProviderB()
		{
			Db4objects.Drs.Test.SPCParent parent = (Db4objects.Drs.Test.SPCParent)GetOneInstance
				(B(), typeof(Db4objects.Drs.Test.SPCParent));
			parent.SetName(MODIFIED_IN_B);
			Db4objects.Drs.Test.SPCChild child = parent.GetChild();
			child.SetName(MODIFIED_IN_B);
			B().Provider().Update(parent);
			B().Provider().Update(child);
			B().Provider().Commit();
			EnsureNames(B(), MODIFIED_IN_B, MODIFIED_IN_B);
		}

		private void ReplicateAllToProviderBFirstTime()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureNames(A(), IN_A, IN_A);
			EnsureNames(B(), IN_A, IN_A);
		}

		private void StoreParentAndChildToProviderA()
		{
			Db4objects.Drs.Test.SPCChild child = new Db4objects.Drs.Test.SPCChild(IN_A);
			Db4objects.Drs.Test.SPCParent parent = new Db4objects.Drs.Test.SPCParent(child, IN_A
				);
			A().Provider().StoreNew(parent);
			A().Provider().Commit();
			EnsureNames(A(), IN_A, IN_A);
		}

		private void TstNewObject()
		{
			StoreParentAndChildToProviderA();
			Db4objects.Drs.Test.ReplicationEventTest.BooleanClosure invoked = new Db4objects.Drs.Test.ReplicationEventTest.BooleanClosure
				(false);
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_221
				(this, invoked);
			ReplicateAll(A().Provider(), B().Provider(), listener);
			Db4oUnit.Assert.IsTrue(invoked.GetValue());
			EnsureNames(A(), IN_A, IN_A);
			EnsureNotExist(B().Provider(), typeof(Db4objects.Drs.Test.SPCParent));
			EnsureNotExist(B().Provider(), typeof(Db4objects.Drs.Test.SPCChild));
		}

		private sealed class _IReplicationEventListener_221 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_221(ReplicationEventTest _enclosing, Db4objects.Drs.Test.ReplicationEventTest.BooleanClosure
				 invoked)
			{
				this._enclosing = _enclosing;
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

			private readonly ReplicationEventTest _enclosing;

			private readonly Db4objects.Drs.Test.ReplicationEventTest.BooleanClosure invoked;
		}

		private void TstNoAction()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_252
				(this);
			ReplicateAll(B().Provider(), A().Provider(), listener);
			EnsureNames(A(), MODIFIED_IN_B, MODIFIED_IN_B);
			EnsureNames(B(), MODIFIED_IN_B, MODIFIED_IN_B);
		}

		private sealed class _IReplicationEventListener_252 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_252(ReplicationEventTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
			}

			private readonly ReplicationEventTest _enclosing;
		}

		private void TstOverrideWhenConflicts()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyInProviderA();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_272
				(this);
			ReplicateAll(A().Provider(), B().Provider(), listener);
			EnsureNames(A(), MODIFIED_IN_B, MODIFIED_IN_B);
			EnsureNames(B(), MODIFIED_IN_B, MODIFIED_IN_B);
		}

		private sealed class _IReplicationEventListener_272 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_272(ReplicationEventTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
				Db4oUnit.Assert.IsTrue(@event.IsConflict());
				if (@event.IsConflict())
				{
					@event.OverrideWith(@event.StateInProviderB());
				}
			}

			private readonly ReplicationEventTest _enclosing;
		}

		private void TstOverrideWhenNoConflicts()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_292
				(this);
			ReplicateAll(B().Provider(), A().Provider(), listener);
			EnsureNames(A(), IN_A, IN_A);
			EnsureNames(B(), IN_A, IN_A);
		}

		private sealed class _IReplicationEventListener_292 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_292(ReplicationEventTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
				Db4oUnit.Assert.IsTrue(!@event.IsConflict());
				@event.OverrideWith(@event.StateInProviderB());
			}

			private readonly ReplicationEventTest _enclosing;
		}

		private void TstStopTraversal()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyInProviderA();
			ModifyInProviderB();
			Db4objects.Drs.IReplicationEventListener listener = new _IReplicationEventListener_313
				(this);
			ReplicateAll(A().Provider(), B().Provider(), listener);
			EnsureNames(A(), MODIFIED_IN_A, MODIFIED_IN_A);
			EnsureNames(B(), MODIFIED_IN_B, MODIFIED_IN_B);
		}

		private sealed class _IReplicationEventListener_313 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_313(ReplicationEventTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent @event)
			{
				Db4oUnit.Assert.IsTrue(@event.IsConflict());
				@event.OverrideWith(null);
			}

			private readonly ReplicationEventTest _enclosing;
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
