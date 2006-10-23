namespace Db4objects.Db4o.Inside.Marshall
{
	internal class ArrayMarshaller0 : Db4objects.Db4o.Inside.Marshall.ArrayMarshaller
	{
		public override void DeleteEmbedded(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.YapWriter
			 reader)
		{
			int address = reader.ReadInt();
			int length = reader.ReadInt();
			if (address <= 0)
			{
				return;
			}
			Db4objects.Db4o.Transaction trans = reader.GetTransaction();
			if (reader.CascadeDeletes() > 0 && arrayHandler.i_handler is Db4objects.Db4o.YapClass
				)
			{
				Db4objects.Db4o.YapWriter bytes = reader.GetStream().ReadWriterByAddress(trans, address
					, length);
				if (bytes != null)
				{
					bytes.SetCascadeDeletes(reader.CascadeDeletes());
					for (int i = arrayHandler.ElementCount(trans, bytes); i > 0; i--)
					{
						arrayHandler.i_handler.DeleteEmbedded(_family, bytes);
					}
				}
			}
			trans.SlotFreeOnCommit(address, address, length);
		}

		public override void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, Db4objects.Db4o.YapArray handler, object obj, bool topLevel)
		{
		}

		public override object WriteNew(Db4objects.Db4o.YapArray arrayHandler, object a_object
			, bool topLevel, Db4objects.Db4o.YapWriter a_bytes)
		{
			if (a_object == null)
			{
				a_bytes.WriteEmbeddedNull();
				return null;
			}
			int length = arrayHandler.ObjectLength(a_object);
			Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(a_bytes.GetTransaction
				(), length);
			bytes.SetUpdateDepth(a_bytes.GetUpdateDepth());
			arrayHandler.WriteNew1(a_object, bytes);
			bytes.SetID(a_bytes._offset);
			a_bytes.GetStream().WriteEmbedded(a_bytes, bytes);
			a_bytes.IncrementOffset(Db4objects.Db4o.YapConst.ID_LENGTH);
			a_bytes.WriteInt(length);
			return a_object;
		}

		public override object Read(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.YapWriter
			 a_bytes)
		{
			Db4objects.Db4o.YapWriter bytes = a_bytes.ReadEmbeddedObject();
			if (bytes == null)
			{
				return null;
			}
			return arrayHandler.Read1(_family, bytes);
		}

		public override void ReadCandidates(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.YapReader
			 reader, Db4objects.Db4o.QCandidates candidates)
		{
			Db4objects.Db4o.YapReader bytes = reader.ReadEmbeddedObject(candidates.i_trans);
			if (bytes == null)
			{
				return;
			}
			int count = arrayHandler.ElementCount(candidates.i_trans, bytes);
			for (int i = 0; i < count; i++)
			{
				candidates.AddByIdentity(new Db4objects.Db4o.QCandidate(candidates, null, bytes.ReadInt
					(), true));
			}
		}

		public sealed override object ReadQuery(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapReader reader)
		{
			Db4objects.Db4o.YapReader bytes = reader.ReadEmbeddedObject(trans);
			if (bytes == null)
			{
				return null;
			}
			object array = arrayHandler.Read1Query(trans, _family, bytes);
			return array;
		}

		protected override Db4objects.Db4o.YapReader PrepareIDReader(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapReader reader)
		{
			return reader.ReadEmbeddedObject(trans);
		}

		public override void DefragIDs(Db4objects.Db4o.YapArray arrayHandler, Db4objects.Db4o.ReaderPair
			 readers)
		{
		}
	}
}
