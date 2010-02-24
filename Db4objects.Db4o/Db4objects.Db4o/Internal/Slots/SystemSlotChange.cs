/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Slots
{
	/// <exclude></exclude>
	public class SystemSlotChange : SlotChange
	{
		public SystemSlotChange(int id) : base(id)
		{
		}

		public override void FreeDuringCommit(TransactionalIdSystem idSystem, IFreespaceManager
			 freespaceManager, bool forFreespace)
		{
			base.FreeDuringCommit(idSystem, freespaceManager, forFreespace);
		}

		// FIXME: If we are doing a delete, we should also free our pointer here.
		protected override Slot ModifiedSlotInUnderlyingIdSystem(TransactionalIdSystem idSystem
			)
		{
			return null;
		}
	}
}
