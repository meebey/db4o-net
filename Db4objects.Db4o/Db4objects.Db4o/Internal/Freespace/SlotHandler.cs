/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public abstract class SlotHandler : IIndexable4
	{
		protected Slot _current;

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
			throw new NotImplementedException();
		}

		public virtual int LinkLength()
		{
			return Slot.MarshalledLength;
		}

		public virtual object ReadIndexEntry(ByteArrayBuffer reader)
		{
			return new Slot(reader.ReadInt(), reader.ReadInt());
		}

		public virtual void WriteIndexEntry(ByteArrayBuffer writer, object obj)
		{
			Slot slot = (Slot)obj;
			writer.WriteInt(slot.Address());
			writer.WriteInt(slot.Length());
		}

		public abstract IPreparedComparison PrepareComparison(IContext arg1, object arg2);
	}
}
