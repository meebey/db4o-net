/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	internal class ArrayMarshaller1 : ArrayMarshaller
	{
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
			ITypeHandler4 typeHandler = arrayHandler._handler;
			if (reader.CascadeDeletes() > 0 && typeHandler is ClassMetadata)
			{
				reader._offset = address;
				reader.SetCascadeDeletes(reader.CascadeDeletes());
				for (int i = arrayHandler.ElementCount(trans, reader); i > 0; i--)
				{
					arrayHandler._handler.DeleteEmbedded(_family, reader);
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

		protected override Db4objects.Db4o.Internal.Buffer PrepareIDReader(Transaction trans
			, Db4objects.Db4o.Internal.Buffer reader)
		{
			reader._offset = reader.ReadInt();
			return reader;
		}

		public override void DefragIDs(ArrayHandler arrayHandler, BufferPair readers)
		{
			int offset = readers.PreparePayloadRead();
			arrayHandler.Defrag1(_family, readers);
			readers.Offset(offset);
		}
	}
}
