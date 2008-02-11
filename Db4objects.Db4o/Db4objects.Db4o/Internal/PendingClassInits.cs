/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	internal class PendingClassInits
	{
		private readonly Transaction _systemTransaction;

		private Collection4 _pending = new Collection4();

		private IQueue4 _members = new NonblockingQueue();

		private IQueue4 _statics = new NonblockingQueue();

		private IQueue4 _writes = new NonblockingQueue();

		private IQueue4 _inits = new NonblockingQueue();

		private bool _running = false;

		internal PendingClassInits(Transaction systemTransaction)
		{
			_systemTransaction = systemTransaction;
		}

		internal virtual void Process(ClassMetadata newYapClass)
		{
			if (_pending.Contains(newYapClass))
			{
				return;
			}
			ClassMetadata ancestor = newYapClass.GetAncestor();
			if (ancestor != null)
			{
				Process(ancestor);
			}
			_pending.Add(newYapClass);
			_members.Add(newYapClass);
			if (_running)
			{
				return;
			}
			_running = true;
			CheckInits();
			_pending = new Collection4();
			_running = false;
		}

		private void CheckMembers()
		{
			while (_members.HasNext())
			{
				ClassMetadata yc = (ClassMetadata)_members.Next();
				yc.AddMembers(Stream());
				_statics.Add(yc);
			}
		}

		private ObjectContainerBase Stream()
		{
			return _systemTransaction.Container();
		}

		private void CheckStatics()
		{
			CheckMembers();
			while (_statics.HasNext())
			{
				ClassMetadata yc = (ClassMetadata)_statics.Next();
				yc.StoreStaticFieldValues(_systemTransaction, true);
				_writes.Add(yc);
				CheckMembers();
			}
		}

		private void CheckWrites()
		{
			CheckStatics();
			while (_writes.HasNext())
			{
				ClassMetadata yc = (ClassMetadata)_writes.Next();
				yc.SetStateDirty();
				yc.Write(_systemTransaction);
				_inits.Add(yc);
				CheckStatics();
			}
		}

		private void CheckInits()
		{
			CheckWrites();
			while (_inits.HasNext())
			{
				ClassMetadata yc = (ClassMetadata)_inits.Next();
				yc.InitConfigOnUp(_systemTransaction);
				CheckWrites();
			}
		}
	}
}
