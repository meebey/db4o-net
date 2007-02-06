namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller1 : Db4objects.Db4o.Internal.Marshall.ArrayMarshaller
	{
		public override void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 arrayHandler, object obj, bool topLevel)
		{
			Db4objects.Db4o.Internal.ITypeHandler4 typeHandler = arrayHandler.i_handler;
			if (topLevel)
			{
				header.AddBaseLength(arrayHandler.LinkLength());
			}
			else
			{
				header.AddPayLoadLength(arrayHandler.LinkLength());
			}
			if (typeHandler.HasFixedLength())
			{
				header.AddPayLoadLength(arrayHandler.ObjectLength(obj));
			}
			else
			{
				header.AddPayLoadLength(arrayHandler.OwnLength(obj));
				object[] all = arrayHandler.AllElements(obj);
				for (int i = 0; i < all.Length; i++)
				{
					typeHandler.CalculateLengths(trans, header, false, all[i], true);
				}
			}
		}

		public override void DeleteEmbedded(Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 arrayHandler, Db4objects.Db4o.Internal.StatefulBuffer reader)
		{
			int address = reader.ReadInt();
			reader.ReadInt();
			if (address <= 0)
			{
				return;
			}
			int linkOffSet = reader._offset;
			Db4objects.Db4o.Internal.Transaction trans = reader.GetTransaction();
			Db4objects.Db4o.Internal.ITypeHandler4 typeHandler = arrayHandler.i_handler;
			if (reader.CascadeDeletes() > 0 && typeHandler is Db4objects.Db4o.Internal.ClassMetadata
				)
			{
				reader._offset = address;
				reader.SetCascadeDeletes(reader.CascadeDeletes());
				for (int i = arrayHandler.ElementCount(trans, reader); i > 0; i--)
				{
					arrayHandler.i_handler.DeleteEmbedded(_family, reader);
				}
			}
			if (linkOffSet > 0)
			{
				reader._offset = linkOffSet;
			}
		}

		public override object Read(Db4objects.Db4o.Internal.Handlers.ArrayHandler arrayHandler
			, Db4objects.Db4o.Internal.StatefulBuffer reader)
		{
			int linkOffSet = reader.PreparePayloadRead();
			object array = arrayHandler.Read1(_family, reader);
			reader._offset = linkOffSet;
			return array;
		}

		public override void ReadCandidates(Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 arrayHandler, Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates)
		{
			reader._offset = reader.ReadInt();
			arrayHandler.Read1Candidates(_family, reader, candidates);
		}

		public sealed override object ReadQuery(Db4objects.Db4o.Internal.Handlers.ArrayHandler
			 arrayHandler, Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			reader._offset = reader.ReadInt();
			return arrayHandler.Read1Query(trans, _family, reader);
		}

		public override object WriteNew(Db4objects.Db4o.Internal.Handlers.ArrayHandler arrayHandler
			, object obj, bool restoreLinkOffset, Db4objects.Db4o.Internal.StatefulBuffer writer
			)
		{
			if (obj == null)
			{
				writer.WriteEmbeddedNull();
				return null;
			}
			int length = arrayHandler.ObjectLength(obj);
			int linkOffset = writer.ReserveAndPointToPayLoadSlot(length);
			arrayHandler.WriteNew1(obj, writer);
			if (restoreLinkOffset)
			{
				writer._offset = linkOffset;
			}
			return obj;
		}

		protected override Db4objects.Db4o.Internal.Buffer PrepareIDReader(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Buffer reader)
		{
			reader._offset = reader.ReadInt();
			return reader;
		}

		public override void DefragIDs(Db4objects.Db4o.Internal.Handlers.ArrayHandler arrayHandler
			, Db4objects.Db4o.Internal.ReaderPair readers)
		{
			int offset = readers.PreparePayloadRead();
			arrayHandler.Defrag1(_family, readers);
			readers.Offset(offset);
		}
	}
}
