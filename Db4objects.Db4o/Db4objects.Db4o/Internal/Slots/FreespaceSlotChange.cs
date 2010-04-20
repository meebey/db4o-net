/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Slots
{
	/// <exclude></exclude>
	public class FreespaceSlotChange : SystemSlotChange
	{
		private Collection4 _freed;

		public FreespaceSlotChange(int id) : base(id)
		{
		}

		protected override void Free(IFreespaceManager freespaceManager, Slot slot)
		{
			if (slot.IsNull())
			{
				return;
			}
			if (_freed == null)
			{
				_freed = new Collection4();
			}
			_freed.Add(slot);
		}

		protected override bool IsForFreespace()
		{
			return true;
		}

		public override void AccumulateFreeSlot(TransactionalIdSystemImpl idSystem, FreespaceCommitter
			 freespaceCommitter, bool forFreespace)
		{
			base.AccumulateFreeSlot(idSystem, freespaceCommitter, forFreespace);
			if (_freed == null)
			{
				return;
			}
			if (IsForFreespace() != forFreespace)
			{
				return;
			}
			IEnumerator iterator = _freed.GetEnumerator();
			while (iterator.MoveNext())
			{
				freespaceCommitter.DelayedFree((Slot)iterator.Current);
			}
		}
	}
}
