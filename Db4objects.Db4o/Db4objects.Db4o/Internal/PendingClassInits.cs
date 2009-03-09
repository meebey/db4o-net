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

		internal virtual void Process(ClassMetadata newClassMetadata)
		{
			if (_pending.Contains(newClassMetadata))
			{
				return;
			}
			ClassMetadata ancestor = newClassMetadata.GetAncestor();
			if (ancestor != null)
			{
				Process(ancestor);
			}
			_pending.Add(newClassMetadata);
			_members.Add(newClassMetadata);
			if (_running)
			{
				return;
			}
			_running = true;
			try
			{
				CheckInits();
				_pending = new Collection4();
			}
			finally
			{
				_running = false;
			}
		}

		private void CheckMembers()
		{
			while (_members.HasNext())
			{
				ClassMetadata classMetadata = (ClassMetadata)_members.Next();
				classMetadata.AddMembers(Stream());
				_statics.Add(classMetadata);
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
