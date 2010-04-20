/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class IdSlotMapping
	{
		public readonly int _id;

		public readonly int _address;

		public readonly int _length;

		public IdSlotMapping(int id, int address, int length)
		{
			_id = id;
			_address = address;
			_length = length;
		}

		public IdSlotMapping(int id, Db4objects.Db4o.Internal.Slots.Slot slot) : this(id, 
			slot.Address(), slot.Length())
		{
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot Slot()
		{
			return new Db4objects.Db4o.Internal.Slots.Slot(_address, _length);
		}

		public virtual void Write(ByteArrayBuffer buffer)
		{
			buffer.WriteInt(_id);
			buffer.WriteInt(_address);
			buffer.WriteInt(_length);
		}

		public static Db4objects.Db4o.Internal.Ids.IdSlotMapping Read(ByteArrayBuffer buffer
			)
		{
			return new Db4objects.Db4o.Internal.Ids.IdSlotMapping(buffer.ReadInt(), buffer.ReadInt
				(), buffer.ReadInt());
		}

		public override string ToString()
		{
			return string.Empty + _id + ":" + _address + "," + _length;
		}
	}
}
