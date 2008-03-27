/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class ReplicationFeaturesMain : Db4objects.Drs.Tests.DrsTestCase
	{
		private static readonly string AStuff = "A";

		private static readonly string BStuff = "B";

		private readonly Db4objects.Drs.Tests.Foundation.Set4 _setA = new Db4objects.Drs.Tests.Foundation.Set4
			(1);

		private readonly Db4objects.Drs.Tests.Foundation.Set4 _setB = new Db4objects.Drs.Tests.Foundation.Set4
			(1);

		private readonly Db4objects.Drs.Tests.Foundation.Set4 _setBoth = new Db4objects.Drs.Tests.Foundation.Set4
			(2);

		private readonly Db4objects.Drs.Tests.Foundation.Set4 None = Db4objects.Drs.Tests.Foundation.Set4
			.EmptySet;

		private Db4objects.Drs.Tests.Foundation.Set4 _direction;

		private Db4objects.Drs.Tests.Foundation.Set4 _containersToQueryFrom;

		private Db4objects.Drs.Tests.Foundation.Set4 _containersWithNewObjects;

		private Db4objects.Drs.Tests.Foundation.Set4 _containersWithChangedObjects;

		private Db4objects.Drs.Tests.Foundation.Set4 _containersWithDeletedObjects;

		private Db4objects.Drs.Tests.Foundation.Set4 _containerStateToPrevail;

		private string _intermittentErrors = string.Empty;

		private int _testCombination;

		private static void Fail(string @string)
		{
			Sharpen.Runtime.Err.WriteLine(@string);
			throw new System.Exception(@string);
		}

		private void ReplicateQueryingFrom(Db4objects.Drs.IReplicationSession replication
			, Db4objects.Drs.IReplicationProvider origin, Db4objects.Drs.IReplicationProvider
			 other)
		{
			Db4objects.Drs.ReplicationConflictException exception = null;
			System.Collections.IEnumerator changes = origin.ObjectsChangedSinceLastReplication
				().GetEnumerator();
			while (changes.MoveNext())
			{
				object changed = changes.Current;
				try
				{
					replication.Replicate(changed);
				}
				catch (Db4objects.Drs.ReplicationConflictException e)
				{
					exception = e;
				}
			}
			if (exception != null)
			{
				throw exception;
			}
		}

		private bool IsReplicationConflictExceptionExpectedReplicatingModifications()
		{
			return WasConflictReplicatingModifications() && IsDefaultReplicationBehaviorAllowed
				();
		}

		private bool IsReplicationConflictExceptionExpectedReplicatingDeletions()
		{
			return WasConflictReplicatingDeletions() && IsDefaultReplicationBehaviorAllowed();
		}

		private bool WasConflictReplicatingDeletions()
		{
			if (_containersWithDeletedObjects.Size() != 1)
			{
				return false;
			}
			string container = (string)FirstContainerWithDeletedObjects();
			if (HasChanges(Other(container)))
			{
				return true;
			}
			if (_direction.Size() != 1)
			{
				return false;
			}
			return _direction.Contains(container);
		}

		private string FirstContainerWithDeletedObjects()
		{
			return First(_containersWithDeletedObjects.GetEnumerator());
		}

		private bool IsDefaultReplicationBehaviorAllowed()
		{
			return _containerStateToPrevail != null && _containerStateToPrevail.IsEmpty();
		}

		protected virtual void ActualTest()
		{
			Clean();
			_setA.Add(AStuff);
			_setB.Add(BStuff);
			_setBoth.AddAll(_setA);
			_setBoth.AddAll(_setB);
			_testCombination = 0;
			TstWithDeletedObjectsIn(None);
			TstWithDeletedObjectsIn(_setA);
			TstWithDeletedObjectsIn(_setB);
			TstWithDeletedObjectsIn(_setBoth);
			if (_intermittentErrors.Length > 0)
			{
				Sharpen.Runtime.Err.WriteLine("Intermittent errors found in test combinations:" +
					 _intermittentErrors);
				Db4oUnit.Assert.IsTrue(false);
			}
		}

		//	protected void clean() {
		//		delete(new Class[]{Replicated.class});
		//	}
		private void ChangeObject(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 container, string name, string newName)
		{
			Db4objects.Drs.Tests.Replicated obj = Find(container, name);
			if (obj == null)
			{
				return;
			}
			obj.SetName(newName);
			container.Update(obj);
			Out("CHANGED: " + container + ": " + name + " => " + newName + " - " + obj);
		}

		private void CheckEmpty(Db4objects.Drs.Inside.ITestableReplicationProviderInside 
			provider)
		{
			if (provider.GetStoredObjects(typeof(Db4objects.Drs.Tests.Replicated)).GetEnumerator
				().MoveNext())
			{
				throw new System.Exception(provider.GetName() + " is not empty");
			}
		}

		private int checkNameCount = 0;

		private void CheckName(Db4objects.Drs.Inside.ITestableReplicationProviderInside container
			, string name, bool isExpected)
		{
			Out(string.Empty);
			Out(name + (isExpected ? " " : " NOT") + " expected in container " + ContainerName
				(container));
			Db4objects.Drs.Tests.Replicated obj = Find(container, name);
			Out(checkNameCount.ToString());
			checkNameCount++;
			if (isExpected)
			{
				Db4oUnit.Assert.IsNotNull(obj);
			}
			else
			{
				Db4oUnit.Assert.IsNull(obj);
			}
		}

		private string ContainerName(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 container)
		{
			return container == A().Provider() ? "A" : "B";
		}

		private void CheckNames()
		{
			CheckNames(AStuff, AStuff);
			CheckNames(AStuff, BStuff);
			CheckNames(BStuff, AStuff);
			CheckNames(BStuff, BStuff);
		}

		private void CheckNames(string origin, string inspected)
		{
			CheckName(Container(inspected), "oldFrom" + origin, IsOldNameExpected(inspected));
			CheckName(Container(inspected), "newFrom" + origin, IsNewNameExpected(origin, inspected
				));
			CheckName(Container(inspected), "oldFromAChangedIn" + origin, IsChangedNameExpected
				(origin, inspected));
			CheckName(Container(inspected), "oldFromBChangedIn" + origin, IsChangedNameExpected
				(origin, inspected));
		}

		//	public void configure() {
		//		Db4o.configure().generateUUIDs(Integer.MAX_VALUE);
		//		Db4o.configure().generateVersionNumbers(Integer.MAX_VALUE);
		//	}
		private Db4objects.Drs.Inside.ITestableReplicationProviderInside Container(string
			 aOrB)
		{
			return aOrB.Equals(AStuff) ? A().Provider() : B().Provider();
		}

		private void DeleteObject(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 container, string name)
		{
			Db4objects.Drs.Tests.Replicated obj = Find(container, name);
			container.Delete(obj);
		}

		private void DoIt()
		{
			InitState();
			PrintProvidersContent("before changes");
			PerformChanges();
			PrintProvidersContent("after changes");
			Db4objects.Drs.IReplicationSession replication = new Db4objects.Drs.Inside.GenericReplicationSession
				(A().Provider(), B().Provider(), new _IReplicationEventListener_174(this));
			//Default replication behaviour.
			if (_direction.Size() == 1)
			{
				if (_direction.Contains(AStuff))
				{
					replication.SetDirection(B().Provider(), A().Provider());
				}
				if (_direction.Contains(BStuff))
				{
					replication.SetDirection(A().Provider(), B().Provider());
				}
			}
			Out("DIRECTION: " + _direction);
			bool successful = TryToReplicate(replication);
			replication.Commit();
			PrintProvidersContent("after replication");
			if (successful)
			{
				CheckNames();
			}
			Clean();
		}

		private sealed class _IReplicationEventListener_174 : Db4objects.Drs.IReplicationEventListener
		{
			public _IReplicationEventListener_174(ReplicationFeaturesMain _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnReplicate(Db4objects.Drs.IReplicationEvent e)
			{
				if (this._enclosing._containerStateToPrevail == null)
				{
					e.OverrideWith(null);
					return;
				}
				if (this._enclosing._containerStateToPrevail.IsEmpty())
				{
					return;
				}
				Db4objects.Drs.IObjectState @override = this._enclosing._containerStateToPrevail.
					Contains(Db4objects.Drs.Tests.ReplicationFeaturesMain.AStuff) ? e.StateInProviderA
					() : e.StateInProviderB();
				e.OverrideWith(@override);
			}

			private readonly ReplicationFeaturesMain _enclosing;
		}

		private void PrintProvidersContent(string msg)
		{
		}

		//		System.out.println("*** "+msg);
		//		printProviderContent(a().provider());
		//		printProviderContent(b().provider());
		//	private void printProviderContent(TestableReplicationProviderInside provider) {
		//		ObjectContainer db=((Db4oReplicationProvider)provider).objectContainer();
		//		ObjectSet result=db.query(Replicated.class);
		//		System.out.println("PROVIDER: "+provider);
		//		while(result.hasNext()) {
		//			System.out.println(result.next());
		//		}
		//	}
		private bool TryToReplicate(Db4objects.Drs.IReplicationSession replication)
		{
			try
			{
				Replicate(replication, AStuff);
				Replicate(replication, BStuff);
				Db4oUnit.Assert.IsFalse(IsReplicationConflictExceptionExpectedReplicatingModifications
					());
			}
			catch (Db4objects.Drs.ReplicationConflictException)
			{
				Out("Conflict exception during modification replication.");
				Db4oUnit.Assert.IsTrue(IsReplicationConflictExceptionExpectedReplicatingModifications
					());
				return false;
			}
			try
			{
				if (IsDeletionReplicationTriggered())
				{
					replication.ReplicateDeletions(typeof(Db4objects.Drs.Tests.Replicated));
				}
				Db4oUnit.Assert.IsFalse(IsReplicationConflictExceptionExpectedReplicatingDeletions
					());
			}
			catch (Db4objects.Drs.ReplicationConflictException)
			{
				Out("Conflict exception during deletion replication.");
				Db4oUnit.Assert.IsTrue(IsReplicationConflictExceptionExpectedReplicatingDeletions
					());
				return false;
			}
			return true;
		}

		private void Replicate(Db4objects.Drs.IReplicationSession replication, string originName
			)
		{
			Db4objects.Drs.IReplicationProvider origin = Container(originName);
			Db4objects.Drs.IReplicationProvider destination = Container(Other(originName));
			if (!_containersToQueryFrom.Contains(originName))
			{
				return;
			}
			ReplicateQueryingFrom(replication, origin, destination);
		}

		private Db4objects.Drs.Tests.Replicated Find(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 container, string name)
		{
			System.Collections.IEnumerator storedObjects = container.GetStoredObjects(typeof(
				Db4objects.Drs.Tests.Replicated)).GetEnumerator();
			int resultCount = 0;
			Db4objects.Drs.Tests.Replicated result = null;
			while (storedObjects.MoveNext())
			{
				Db4objects.Drs.Tests.Replicated replicated = (Db4objects.Drs.Tests.Replicated)storedObjects
					.Current;
				if (replicated == null)
				{
					throw new System.Exception();
				}
				if (name.Equals(replicated.GetName()))
				{
					result = replicated;
					resultCount++;
				}
			}
			if (resultCount > 1)
			{
				Fail("At most one object with name " + name + " was expected.");
			}
			return result;
		}

		private bool HasChanges(string container)
		{
			return _containersWithChangedObjects.Contains(container);
		}

		private bool HasDeletions(string container)
		{
			return _containersWithDeletedObjects.Contains(container);
		}

		private void InitState()
		{
			CheckEmpty(A().Provider());
			CheckEmpty(B().Provider());
			A().Provider().StoreNew(new Db4objects.Drs.Tests.Replicated("oldFromA"));
			B().Provider().StoreNew(new Db4objects.Drs.Tests.Replicated("oldFromB"));
			A().Provider().Commit();
			B().Provider().Commit();
			Db4objects.Drs.IReplicationSession replication = new Db4objects.Drs.Inside.GenericReplicationSession
				(A().Provider(), B().Provider());
			ReplicateQueryingFrom(replication, A().Provider(), B().Provider());
			ReplicateQueryingFrom(replication, B().Provider(), A().Provider());
			replication.Commit();
		}

		private bool IsChangedNameExpected(string changedContainer, string inspectedContainer
			)
		{
			if (!HasChanges(changedContainer))
			{
				return false;
			}
			if (IsDeletionExpected(inspectedContainer))
			{
				return false;
			}
			if (IsDeletionExpected(changedContainer))
			{
				return false;
			}
			if (inspectedContainer.Equals(changedContainer))
			{
				return !DidReceiveRemoteState(inspectedContainer);
			}
			return DidReceiveRemoteState(inspectedContainer);
		}

		private bool DidReceiveRemoteState(string inspectedContainer)
		{
			string other = Other(inspectedContainer);
			if (IsDirectionTo(other))
			{
				return false;
			}
			if (_containerStateToPrevail == null)
			{
				return false;
			}
			if (_containerStateToPrevail.Contains(inspectedContainer))
			{
				return false;
			}
			if (_containerStateToPrevail.Contains(other))
			{
				if (IsModificationReplicationTriggered())
				{
					return true;
				}
				if (IsDeletionReplicationTriggered())
				{
					return true;
				}
				return false;
			}
			//No override to prevail. Default replication behavior.
			if (HasChanges(inspectedContainer))
			{
				return false;
			}
			//A conflict would have been ignored long ago.
			return IsModificationReplicationTriggered();
		}

		private bool IsDeletionReplicationTriggered()
		{
			return !_containersWithDeletedObjects.IsEmpty();
		}

		private bool IsDirectionTo(string container)
		{
			return _direction.Size() == 1 && _direction.Contains(container);
		}

		private bool WasConflictReplicatingModifications()
		{
			return WasConflictWhileReplicatingModificationsQueryingFrom(AStuff) || WasConflictWhileReplicatingModificationsQueryingFrom
				(BStuff);
		}

		private bool IsModificationReplicationTriggered()
		{
			return WasModificationReplicationTriggeredQueryingFrom(AStuff) || WasModificationReplicationTriggeredQueryingFrom
				(BStuff);
		}

		private bool IsDeletionExpected(string inspectedContainer)
		{
			if (_containerStateToPrevail == null)
			{
				return HasDeletions(inspectedContainer);
			}
			if (_containerStateToPrevail.Contains(inspectedContainer))
			{
				return HasDeletions(inspectedContainer);
			}
			string other = Other(inspectedContainer);
			if (IsDirectionTo(other))
			{
				return HasDeletions(inspectedContainer);
			}
			if (_containerStateToPrevail.Contains(other))
			{
				return HasDeletions(other);
			}
			//_containerStateToPrevail is empty (default replication behaviour)
			return IsDeletionReplicationTriggered();
		}

		private bool IsNewNameExpected(string origin, string inspected)
		{
			if (!_containersWithNewObjects.Contains(origin))
			{
				return false;
			}
			if (origin.Equals(inspected))
			{
				return true;
			}
			if (_containerStateToPrevail == null)
			{
				return false;
			}
			if (_containerStateToPrevail.Contains(inspected))
			{
				return false;
			}
			if (!_containersToQueryFrom.Contains(origin))
			{
				return false;
			}
			return _direction.Contains(inspected);
		}

		private bool IsOldNameExpected(string inspectedContainer)
		{
			if (IsDeletionExpected(inspectedContainer))
			{
				return false;
			}
			if (IsChangedNameExpected(AStuff, inspectedContainer))
			{
				return false;
			}
			if (IsChangedNameExpected(BStuff, inspectedContainer))
			{
				return false;
			}
			return true;
		}

		private string Other(string aOrB)
		{
			return aOrB.Equals(AStuff) ? BStuff : AStuff;
		}

		private void PerformChanges()
		{
			if (_containersWithNewObjects.Contains(AStuff))
			{
				A().Provider().StoreNew(new Db4objects.Drs.Tests.Replicated("newFromA"));
			}
			if (_containersWithNewObjects.Contains(BStuff))
			{
				B().Provider().StoreNew(new Db4objects.Drs.Tests.Replicated("newFromB"));
			}
			if (HasDeletions(AStuff))
			{
				DeleteObject(A().Provider(), "oldFromA");
				DeleteObject(A().Provider(), "oldFromB");
			}
			if (HasDeletions(BStuff))
			{
				DeleteObject(B().Provider(), "oldFromA");
				DeleteObject(B().Provider(), "oldFromB");
			}
			if (HasChanges(AStuff))
			{
				ChangeObject(A().Provider(), "oldFromA", "oldFromAChangedInA");
				ChangeObject(A().Provider(), "oldFromB", "oldFromBChangedInA");
			}
			if (HasChanges(BStuff))
			{
				ChangeObject(B().Provider(), "oldFromA", "oldFromAChangedInB");
				ChangeObject(B().Provider(), "oldFromB", "oldFromBChangedInB");
			}
			A().Provider().Commit();
			B().Provider().Commit();
		}

		private string Print(Db4objects.Drs.Tests.Foundation.Set4 containerSet)
		{
			if (containerSet == null)
			{
				return "null";
			}
			if (containerSet.IsEmpty())
			{
				return "NONE";
			}
			if (containerSet.Size() == 2)
			{
				return "BOTH";
			}
			return First(containerSet);
		}

		private string First(Db4objects.Drs.Tests.Foundation.Set4 containerSet)
		{
			return First(containerSet.GetEnumerator());
		}

		private string First(System.Collections.IEnumerator iterator)
		{
			return (string)Db4objects.Db4o.Foundation.Iterators.Next(iterator);
		}

		private void PrintCombination()
		{
			Out(string.Empty + _testCombination + " =================================");
			Out("New Objects In: " + Print(_containersWithNewObjects));
			Out("Changed Objects In: " + Print(_containersWithChangedObjects));
			Out("Deleted Objects In: " + Print(_containersWithDeletedObjects));
			Out("Querying From: " + Print(_containersToQueryFrom));
			Out("Direction: To " + Print(_direction));
			Out("Prevailing State: " + Print(_containerStateToPrevail));
		}

		private void RunCurrentCombination()
		{
			_testCombination++;
			Out(string.Empty + _testCombination + " =================================");
			PrintCombination();
			if (_testCombination < 0)
			{
				//Use this when debugging to skip some combinations and avoid waiting.
				return;
			}
			int _errors = 0;
			while (true)
			{
				try
				{
					DoIt();
					break;
				}
				catch (System.Exception rx)
				{
					_errors++;
					if (_errors == 1)
					{
						Sleep(100);
						PrintCombination();
						throw;
					}
				}
			}
			if (_errors > 0)
			{
				_intermittentErrors += "\n\t Combination: " + _testCombination + " (" + _errors +
					 " errors)";
			}
		}

		private static void Out(string @string)
		{
		}

		//System.out.println(string);
		public virtual void Test()
		{
			ActualTest();
		}

		private void TstDirection(Db4objects.Drs.Tests.Foundation.Set4 direction)
		{
			_direction = direction;
			TstQueryingFrom(_setA);
			TstQueryingFrom(_setB);
			TstQueryingFrom(_setBoth);
		}

		private void TstQueryingFrom(Db4objects.Drs.Tests.Foundation.Set4 containersToQueryFrom
			)
		{
			_containersToQueryFrom = containersToQueryFrom;
			TstWithNewObjectsIn(None);
			TstWithNewObjectsIn(_setA);
			TstWithNewObjectsIn(_setB);
			TstWithNewObjectsIn(_setBoth);
		}

		private void TstWithChangedObjectsIn(Db4objects.Drs.Tests.Foundation.Set4 containers
			)
		{
			_containersWithChangedObjects = containers;
			TstWithContainerStateToPrevail(None);
			TstWithContainerStateToPrevail(_setA);
			TstWithContainerStateToPrevail(_setB);
			TstWithContainerStateToPrevail(null);
		}

		private void TstWithDeletedObjectsIn(Db4objects.Drs.Tests.Foundation.Set4 containers
			)
		{
			_containersWithDeletedObjects = containers;
			TstDirection(_setA);
			TstDirection(_setB);
			TstDirection(_setBoth);
		}

		private void TstWithNewObjectsIn(Db4objects.Drs.Tests.Foundation.Set4 containersWithNewObjects
			)
		{
			_containersWithNewObjects = containersWithNewObjects;
			TstWithChangedObjectsIn(None);
			TstWithChangedObjectsIn(_setA);
			TstWithChangedObjectsIn(_setB);
			TstWithChangedObjectsIn(_setBoth);
		}

		private void TstWithContainerStateToPrevail(Db4objects.Drs.Tests.Foundation.Set4 
			containers)
		{
			_containerStateToPrevail = containers;
			RunCurrentCombination();
		}

		private bool WasConflictWhileReplicatingModificationsQueryingFrom(string container
			)
		{
			if (!WasModificationReplicationTriggeredQueryingFrom(container))
			{
				return false;
			}
			if (_containersWithChangedObjects.ContainsAll(_direction))
			{
				return true;
			}
			return HasDeletions(Other(container));
		}

		private bool WasModificationReplicationTriggeredQueryingFrom(string container)
		{
			if (!_containersToQueryFrom.Contains(container))
			{
				return false;
			}
			if (_containersWithDeletedObjects.Contains(container))
			{
				return false;
			}
			return _containersWithChangedObjects.Contains(container);
		}
	}
}
