using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller0 : ArrayMarshaller
	{
		public override void DeleteEmbedded(ArrayHandler arrayHandler, StatefulBuffer reader
			)
		{
			int address = reader.ReadInt();
			int length = reader.ReadInt();
			if (address <= 0)
			{
				return;
			}
			Transaction trans = reader.GetTransaction();
			if (reader.CascadeDeletes() > 0 && arrayHandler.i_handler is ClassMetadata)
			{
				StatefulBuffer bytes = reader.GetStream().ReadWriterByAddress(trans, address, length
					);
				bytes.SetCascadeDeletes(reader.CascadeDeletes());
				for (int i = arrayHandler.ElementCount(trans, bytes); i > 0; i--)
				{
					arrayHandler.i_handler.DeleteEmbedded(_family, bytes);
				}
			}
			trans.SlotFreeOnCommit(address, address, length);
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, ArrayHandler handler, object obj, bool topLevel)
		{
		}

		public override object WriteNew(ArrayHandler arrayHandler, object a_object, bool 
			topLevel, StatefulBuffer a_bytes)
		{
			if (a_object == null)
			{
				a_bytes.WriteEmbeddedNull();
				return null;
			}
			int length = arrayHandler.ObjectLength(a_object);
			StatefulBuffer bytes = new StatefulBuffer(a_bytes.GetTransaction(), length);
			bytes.SetUpdateDepth(a_bytes.GetUpdateDepth());
			arrayHandler.WriteNew1(a_object, bytes);
			bytes.SetID(a_bytes._offset);
			a_bytes.GetStream().WriteEmbedded(a_bytes, bytes);
			a_bytes.IncrementOffset(Const4.ID_LENGTH);
			a_bytes.WriteInt(length);
			return a_object;
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

		public override void DefragIDs(ArrayHandler arrayHandler, ReaderPair readers)
		{
		}
	}
}
