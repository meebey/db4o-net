/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside
{
	internal sealed class ReplicationEventImpl : Db4objects.Drs.IReplicationEvent
	{
		internal readonly Db4objects.Drs.Inside.ObjectStateImpl _stateInProviderA = new Db4objects.Drs.Inside.ObjectStateImpl
			();

		internal readonly Db4objects.Drs.Inside.ObjectStateImpl _stateInProviderB = new Db4objects.Drs.Inside.ObjectStateImpl
			();

		internal bool _isConflict;

		internal Db4objects.Drs.IObjectState _actionChosenState;

		internal bool _actionWasChosen;

		internal bool _actionShouldStopTraversal;

		internal long _creationDate;

		public Db4objects.Drs.IObjectState StateInProviderA()
		{
			return _stateInProviderA;
		}

		public Db4objects.Drs.IObjectState StateInProviderB()
		{
			return _stateInProviderB;
		}

		public long ObjectCreationDate()
		{
			return _creationDate;
		}

		public bool IsConflict()
		{
			return _isConflict;
		}

		public void OverrideWith(Db4objects.Drs.IObjectState chosen)
		{
			if (_actionWasChosen)
			{
				throw new System.Exception();
			}
			//FIXME Use Db4o's standard exception throwing.
			_actionWasChosen = true;
			_actionChosenState = chosen;
		}

		public void StopTraversal()
		{
			_actionShouldStopTraversal = true;
		}

		internal void ResetAction()
		{
			_actionChosenState = null;
			_actionWasChosen = false;
			_actionShouldStopTraversal = false;
			_creationDate = -1;
		}
	}
}
