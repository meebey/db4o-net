/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public class LengthKeySlotHandler : SlotHandler
	{
		public override int CompareTo(object obj)
		{
			return _current.CompareByLength((Slot)obj);
		}

		public override IPreparedComparison NewPrepareCompare(object obj)
		{
			Slot sourceSlot = (Slot)obj;
			return new _IPreparedComparison_20(this, sourceSlot);
		}

		private sealed class _IPreparedComparison_20 : IPreparedComparison
		{
			public _IPreparedComparison_20(LengthKeySlotHandler _enclosing, Slot sourceSlot)
			{
				this._enclosing = _enclosing;
				this.sourceSlot = sourceSlot;
			}

			public int CompareTo(object obj)
			{
				Slot targetSlot = (Slot)obj;
				return -sourceSlot.CompareByLength(targetSlot);
			}

			private readonly LengthKeySlotHandler _enclosing;

			private readonly Slot sourceSlot;
		}
	}
}
