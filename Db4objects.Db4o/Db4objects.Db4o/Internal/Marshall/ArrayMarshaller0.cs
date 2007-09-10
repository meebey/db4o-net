/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
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
			return arrayHandler.Read1(_family, bytes);
		}

		public override void ReadCandidates(ArrayHandler arrayHandler, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			Db4objects.Db4o.Internal.Buffer bytes = reader.ReadEmbeddedObject(candidates.i_trans
				);
			int count = arrayHandler.ElementCount(candidates.i_trans, bytes);
			for (int i = 0; i < count; i++)
			{
				candidates.AddByIdentity(new QCandidate(candidates, null, bytes.ReadInt(), true));
			}
		}

		public sealed override object ReadQuery(ArrayHandler arrayHandler, Transaction trans
			, Db4objects.Db4o.Internal.Buffer reader)
		{
			Db4objects.Db4o.Internal.Buffer bytes = reader.ReadEmbeddedObject(trans);
			return arrayHandler.Read1Query(trans, _family, bytes);
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
