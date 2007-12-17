/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

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
			return Slot.MARSHALLED_LENGTH;
		}

		public virtual object ReadIndexEntry(BufferImpl reader)
		{
			return new Slot(reader.ReadInt(), reader.ReadInt());
		}

		public virtual void WriteIndexEntry(BufferImpl writer, object obj)
		{
			Slot slot = (Slot)obj;
			writer.WriteInt(slot.Address());
			writer.WriteInt(slot.Length());
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			_current = (Slot)obj;
			return this;
		}

		public abstract int CompareTo(object arg1);
	}
}
