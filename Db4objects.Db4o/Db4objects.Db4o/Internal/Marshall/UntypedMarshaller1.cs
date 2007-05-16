/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class UntypedMarshaller1 : UntypedMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

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

		public override object Read(StatefulBuffer reader)
		{
			object ret = null;
			int payLoadOffSet = reader.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			int linkOffSet = reader._offset;
			reader._offset = payLoadOffSet;
			int yapClassID = reader.ReadInt();
			ClassMetadata yc = reader.GetStream().ClassMetadataForId(yapClassID);
			if (yc != null)
			{
				ret = yc.Read(_family, reader, true);
			}
			reader._offset = linkOffSet;
			return ret;
		}

		public override object ReadQuery(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader, bool toArray)
		{
			object ret = null;
			int payLoadOffSet = reader.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			int linkOffSet = reader._offset;
			reader._offset = payLoadOffSet;
			int yapClassID = reader.ReadInt();
			ClassMetadata yc = trans.Stream().ClassMetadataForId(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadQuery(trans, _family, false, reader, toArray);
			}
			reader._offset = linkOffSet;
			return ret;
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
			ClassMetadata yc = trans.Stream().ClassMetadataForId(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadArrayHandler(trans, _family, reader);
			}
			return ret;
		}

		public override QCandidate ReadSubCandidate(Db4objects.Db4o.Internal.Buffer reader
			, QCandidates candidates, bool withIndirection)
		{
			int payLoadOffSet = reader.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			QCandidate ret = null;
			int linkOffSet = reader._offset;
			reader._offset = payLoadOffSet;
			int yapClassID = reader.ReadInt();
			ClassMetadata yc = candidates.i_trans.Stream().ClassMetadataForId(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadSubCandidate(_family, reader, candidates, false);
			}
			reader._offset = linkOffSet;
			return ret;
		}

		public override object WriteNew(object obj, bool restoreLinkOffset, StatefulBuffer
			 writer)
		{
			if (obj == null)
			{
				writer.WriteInt(0);
				return 0;
			}
			ClassMetadata yc = ClassMetadata.ForObject(writer.GetTransaction(), obj, false);
			if (yc == null)
			{
				writer.WriteInt(0);
				return 0;
			}
			writer.WriteInt(writer._payloadOffset);
			int linkOffset = writer._offset;
			writer._offset = writer._payloadOffset;
			writer.WriteInt(yc.GetID());
			yc.WriteNew(_family, obj, false, writer, false, false);
			if (writer._payloadOffset < writer._offset)
			{
				writer._payloadOffset = writer._offset;
			}
			if (restoreLinkOffset)
			{
				writer._offset = linkOffset;
			}
			return obj;
		}

		public override void Defrag(ReaderPair readers)
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
