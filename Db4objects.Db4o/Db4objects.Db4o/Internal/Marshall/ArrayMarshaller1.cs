/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller1 : ArrayMarshaller
	{
		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, ArrayHandler arrayHandler, object obj, bool topLevel)
		{
			ITypeHandler4 typeHandler = arrayHandler.i_handler;
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

		public override void DeleteEmbedded(ArrayHandler arrayHandler, StatefulBuffer reader
			)
		{
			int address = reader.ReadInt();
			reader.ReadInt();
			if (address <= 0)
			{
				return;
			}
			int linkOffSet = reader._offset;
			Transaction trans = reader.GetTransaction();
			ITypeHandler4 typeHandler = arrayHandler.i_handler;
			if (reader.CascadeDeletes() > 0 && typeHandler is ClassMetadata)
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

		public override object Read(ArrayHandler arrayHandler, StatefulBuffer reader)
		{
			int linkOffSet = reader.PreparePayloadRead();
			object array = arrayHandler.Read1(_family, reader);
			reader._offset = linkOffSet;
			return array;
		}

		public override void ReadCandidates(ArrayHandler arrayHandler, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			reader._offset = reader.ReadInt();
			arrayHandler.Read1Candidates(_family, reader, candidates);
		}

		public sealed override object ReadQuery(ArrayHandler arrayHandler, Transaction trans
			, Db4objects.Db4o.Internal.Buffer reader)
		{
			reader._offset = reader.ReadInt();
			return arrayHandler.Read1Query(trans, _family, reader);
		}

		public override object WriteNew(ArrayHandler arrayHandler, object obj, bool restoreLinkOffset
			, StatefulBuffer writer)
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

		protected override Db4objects.Db4o.Internal.Buffer PrepareIDReader(Transaction trans
			, Db4objects.Db4o.Internal.Buffer reader)
		{
			reader._offset = reader.ReadInt();
			return reader;
		}

		public override void DefragIDs(ArrayHandler arrayHandler, ReaderPair readers)
		{
			int offset = readers.PreparePayloadRead();
			arrayHandler.Defrag1(_family, readers);
			readers.Offset(offset);
		}
	}
}
