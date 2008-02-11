/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class WriteContextInfo
	{
		internal readonly bool _isNew;

		private Db4objects.Db4o.Internal.Slots.Slot _slot;

		public WriteContextInfo(bool isNew, Db4objects.Db4o.Internal.Slots.Slot slot)
		{
			_isNew = isNew;
			_slot = slot;
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot Slot()
		{
			return _slot;
		}

		public virtual void Slot(Db4objects.Db4o.Internal.Slots.Slot slot)
		{
			_slot = slot;
		}
	}
}
