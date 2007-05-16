/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public class AddressKeySlotHandler : SlotHandler
	{
		public override int CompareTo(object obj)
		{
			return _current.CompareByAddress((Slot)obj);
		}
	}
}
