/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class UntypedMarshaller1 : UntypedMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void DeleteEmbedded(StatefulBuffer reader)
		{
			int payLoadOffset = reader.ReadInt();
			if (payLoadOffset > 0)
			{
				int linkOffset = reader._offset;
				reader._offset = payLoadOffset;
				int yapClassID = reader.ReadInt();
				ClassMetadata yc = reader.GetStream().ClassMetadataForId(yapClassID);
				if (yc != null)
				{
					yc.DeleteEmbedded(_family, reader);
				}
				reader._offset = linkOffset;
			}
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction trans, Db4objects.Db4o.Internal.Buffer[]
			 reader)
		{
			int payLoadOffSet = reader[0].ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			ITypeHandler4 ret = null;
			reader[0]._offset = payLoadOffSet;
			int yapClassID = reader[0].ReadInt();
			ClassMetadata yc = trans.Container().ClassMetadataForId(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadArrayHandler(trans, _family, reader);
			}
			return ret;
		}

		public override void Defrag(BufferPair readers)
		{
			int payLoadOffSet = readers.ReadInt();
			if (payLoadOffSet == 0)
			{
				return;
			}
			int linkOffSet = readers.Offset();
			readers.Offset(payLoadOffSet);
			int yapClassID = readers.CopyIDAndRetrieveMapping().Orig();
			ClassMetadata yc = readers.Context().YapClass(yapClassID);
			if (yc != null)
			{
				yc.Defrag(_family, readers, false);
			}
			readers.Offset(linkOffSet);
		}
	}
}
