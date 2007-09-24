/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller0 : ArrayMarshaller
	{
		public override void DeleteEmbedded(ArrayHandler arrayHandler, StatefulBuffer reader
			)
		{
			Slot slot = reader.ReadSlot();
			if (slot.Address() <= 0)
			{
				return;
			}
			Transaction trans = reader.GetTransaction();
			if (reader.CascadeDeletes() > 0 && arrayHandler._handler is ClassMetadata)
			{
				StatefulBuffer bytes = reader.GetStream().ReadWriterByAddress(trans, slot.Address
					(), slot.Length());
				bytes.SetCascadeDeletes(reader.CascadeDeletes());
				for (int i = arrayHandler.ElementCount(trans, bytes); i > 0; i--)
				{
					arrayHandler._handler.DeleteEmbedded(_family, bytes);
				}
			}
			trans.SlotFreeOnCommit(slot.Address(), slot);
		}

		public override object Read(ArrayHandler arrayHandler, StatefulBuffer a_bytes)
		{
			StatefulBuffer bytes = a_bytes.ReadEmbeddedObject();
			if (bytes == null)
			{
				return null;
			}
			return arrayHandler.Read1(_family, bytes);
		}

		protected override Db4objects.Db4o.Internal.Buffer PrepareIDReader(Transaction trans
			, Db4objects.Db4o.Internal.Buffer reader)
		{
			return reader.ReadEmbeddedObject(trans);
		}

		public override void DefragIDs(ArrayHandler arrayHandler, BufferPair readers)
		{
		}
	}
}
