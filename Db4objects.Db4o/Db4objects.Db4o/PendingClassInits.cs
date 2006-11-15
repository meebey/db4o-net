namespace Db4objects.Db4o
{
	internal class PendingClassInits
	{
		private readonly Db4objects.Db4o.Transaction _systemTransaction;

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

		internal PendingClassInits(Db4objects.Db4o.Transaction systemTransaction)
		{
			_systemTransaction = systemTransaction;
		}

		internal virtual void Process(Db4objects.Db4o.YapClass newYapClass)
		{
			if (_pending.Contains(newYapClass))
			{
				return;
			}
			Db4objects.Db4o.YapClass ancestor = newYapClass.GetAncestor();
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
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)_members.Next();
				yc.AddMembers(Stream());
				_statics.Add(yc);
			}
		}

		private Db4objects.Db4o.YapStream Stream()
		{
			return _systemTransaction.Stream();
		}

		private void CheckStatics()
		{
			CheckMembers();
			while (_statics.HasNext())
			{
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)_statics.Next();
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
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)_writes.Next();
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
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)_inits.Next();
				yc.InitConfigOnUp(_systemTransaction);
				CheckWrites();
			}
		}
	}
}
