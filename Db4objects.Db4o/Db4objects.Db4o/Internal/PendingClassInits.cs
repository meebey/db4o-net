namespace Db4objects.Db4o.Internal
{
	internal class PendingClassInits
	{
		private readonly Db4objects.Db4o.Internal.Transaction _systemTransaction;

		private Db4objects.Db4o.Foundation.Collection4 _pending = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Foundation.Queue4 _members = new Db4objects.Db4o.Foundation.Queue4
			();

		private Db4objects.Db4o.Foundation.Queue4 _statics = new Db4objects.Db4o.Foundation.Queue4
			();

		private Db4objects.Db4o.Foundation.Queue4 _writes = new Db4objects.Db4o.Foundation.Queue4
			();

		private Db4objects.Db4o.Foundation.Queue4 _inits = new Db4objects.Db4o.Foundation.Queue4
			();

		private bool _running = false;

		internal PendingClassInits(Db4objects.Db4o.Internal.Transaction systemTransaction
			)
		{
			_systemTransaction = systemTransaction;
		}

		internal virtual void Process(Db4objects.Db4o.Internal.ClassMetadata newYapClass)
		{
			if (_pending.Contains(newYapClass))
			{
				return;
			}
			Db4objects.Db4o.Internal.ClassMetadata ancestor = newYapClass.GetAncestor();
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
			_pending = new Db4objects.Db4o.Foundation.Collection4();
			_running = false;
		}

		private void CheckMembers()
		{
			while (_members.HasNext())
			{
				Db4objects.Db4o.Internal.ClassMetadata yc = (Db4objects.Db4o.Internal.ClassMetadata
					)_members.Next();
				yc.AddMembers(Stream());
				_statics.Add(yc);
			}
		}

		private Db4objects.Db4o.Internal.ObjectContainerBase Stream()
		{
			return _systemTransaction.Stream();
		}

		private void CheckStatics()
		{
			CheckMembers();
			while (_statics.HasNext())
			{
				Db4objects.Db4o.Internal.ClassMetadata yc = (Db4objects.Db4o.Internal.ClassMetadata
					)_statics.Next();
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
				Db4objects.Db4o.Internal.ClassMetadata yc = (Db4objects.Db4o.Internal.ClassMetadata
					)_writes.Next();
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
				Db4objects.Db4o.Internal.ClassMetadata yc = (Db4objects.Db4o.Internal.ClassMetadata
					)_inits.Next();
				yc.InitConfigOnUp(_systemTransaction);
				CheckWrites();
			}
		}
	}
}
