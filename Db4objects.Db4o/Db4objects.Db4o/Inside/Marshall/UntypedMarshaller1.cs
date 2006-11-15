namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public class UntypedMarshaller1 : Db4objects.Db4o.Inside.Marshall.UntypedMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

		public override void DeleteEmbedded(Db4objects.Db4o.YapWriter reader)
		{
			int payLoadOffset = reader.ReadInt();
			if (payLoadOffset > 0)
			{
				int linkOffset = reader._offset;
				reader._offset = payLoadOffset;
				int yapClassID = reader.ReadInt();
				Db4objects.Db4o.YapClass yc = reader.GetStream().GetYapClass(yapClassID);
				if (yc != null)
				{
					yc.DeleteEmbedded(_family, reader);
				}
				reader._offset = linkOffset;
			}
		}

		public override object Read(Db4objects.Db4o.YapWriter reader)
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
			Db4objects.Db4o.YapClass yc = reader.GetStream().GetYapClass(yapClassID);
			if (yc != null)
			{
				ret = yc.Read(_family, reader, true);
			}
			reader._offset = linkOffSet;
			return ret;
		}

		public override object ReadQuery(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
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
			Db4objects.Db4o.YapClass yc = trans.Stream().GetYapClass(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadQuery(trans, _family, false, reader, toArray);
			}
			reader._offset = linkOffSet;
			return ret;
		}

		public override Db4objects.Db4o.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapReader[] reader)
		{
			int payLoadOffSet = reader[0].ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			Db4objects.Db4o.ITypeHandler4 ret = null;
			reader[0]._offset = payLoadOffSet;
			int yapClassID = reader[0].ReadInt();
			Db4objects.Db4o.YapClass yc = trans.Stream().GetYapClass(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadArrayHandler(trans, _family, reader);
			}
			return ret;
		}

		public override Db4objects.Db4o.QCandidate ReadSubCandidate(Db4objects.Db4o.YapReader
			 reader, Db4objects.Db4o.QCandidates candidates, bool withIndirection)
		{
			int payLoadOffSet = reader.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			Db4objects.Db4o.QCandidate ret = null;
			int linkOffSet = reader._offset;
			reader._offset = payLoadOffSet;
			int yapClassID = reader.ReadInt();
			Db4objects.Db4o.YapClass yc = candidates.i_trans.Stream().GetYapClass(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadSubCandidate(_family, reader, candidates, false);
			}
			reader._offset = linkOffSet;
			return ret;
		}

		public override object WriteNew(object obj, bool restoreLinkOffset, Db4objects.Db4o.YapWriter
			 writer)
		{
			if (obj == null)
			{
				writer.WriteInt(0);
				return 0;
			}
			Db4objects.Db4o.YapClass yc = Db4objects.Db4o.YapClass.ForObject(writer.GetTransaction
				(), obj, false);
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

		public override void Defrag(Db4objects.Db4o.ReaderPair readers)
		{
			int payLoadOffSet = readers.ReadInt();
			if (payLoadOffSet == 0)
			{
				return;
			}
			int linkOffSet = readers.Offset();
			readers.Offset(payLoadOffSet);
			int yapClassID = readers.CopyIDAndRetrieveMapping().Orig();
			Db4objects.Db4o.YapClass yc = readers.Context().YapClass(yapClassID);
			if (yc != null)
			{
				yc.Defrag(_family, readers, false);
			}
			readers.Offset(linkOffSet);
		}
	}
}
