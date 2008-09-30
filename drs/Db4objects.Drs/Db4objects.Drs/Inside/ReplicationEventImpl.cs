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
using System;
using Db4objects.Drs;
using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Inside
{
	internal sealed class ReplicationEventImpl : IReplicationEvent
	{
		internal readonly ObjectStateImpl _stateInProviderA = new ObjectStateImpl();

		internal readonly ObjectStateImpl _stateInProviderB = new ObjectStateImpl();

		internal bool _isConflict;

		internal IObjectState _actionChosenState;

		internal bool _actionWasChosen;

		internal bool _actionShouldStopTraversal;

		internal long _creationDate;

		public IObjectState StateInProviderA()
		{
			return _stateInProviderA;
		}

		public IObjectState StateInProviderB()
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

		public void OverrideWith(IObjectState chosen)
		{
			if (_actionWasChosen)
			{
				throw new Exception();
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
