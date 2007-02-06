namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class UntypedMarshaller0 : Db4objects.Db4o.Internal.Marshall.UntypedMarshaller
	{
		public override void DeleteEmbedded(Db4objects.Db4o.Internal.StatefulBuffer parentBytes
			)
		{
			int objectID = parentBytes.ReadInt();
			if (objectID > 0)
			{
				Db4objects.Db4o.Internal.StatefulBuffer reader = parentBytes.GetStream().ReadWriterByID
					(parentBytes.GetTransaction(), objectID);
				if (reader != null)
				{
					reader.SetCascadeDeletes(parentBytes.CascadeDeletes());
					Db4objects.Db4o.Internal.Marshall.ObjectHeader oh = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
						(reader);
					if (oh.YapClass() != null)
					{
						oh.YapClass().DeleteEmbedded1(_family, reader, objectID);
					}
				}
			}
		}

		public override bool UseNormalClassRead()
		{
			return true;
		}

		public override object Read(Db4objects.Db4o.Internal.StatefulBuffer reader)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.ShouldNeverBeCalled();
		}

		public override object ReadQuery(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader, bool toArray)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.ShouldNeverBeCalled();
		}

		public override Db4objects.Db4o.Internal.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Internal.Transaction
			 a_trans, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			int id = 0;
			int offset = a_bytes[0]._offset;
			try
			{
				id = a_bytes[0].ReadInt();
			}
			catch
			{
			}
			a_bytes[0]._offset = offset;
			if (id != 0)
			{
				Db4objects.Db4o.Internal.StatefulBuffer reader = a_trans.Stream().ReadWriterByID(
					a_trans, id);
				if (reader != null)
				{
					Db4objects.Db4o.Internal.Marshall.ObjectHeader oh = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
						(reader);
					try
					{
						if (oh.YapClass() != null)
						{
							a_bytes[0] = reader;
							return oh.YapClass().ReadArrayHandler1(a_bytes);
						}
					}
					catch (System.Exception e)
					{
					}
				}
			}
			return null;
		}

		public override Db4objects.Db4o.Internal.Query.Processor.QCandidate ReadSubCandidate
			(Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates, bool withIndirection)
		{
			return null;
		}

		public override object WriteNew(object a_object, bool restoreLinkOffset, Db4objects.Db4o.Internal.StatefulBuffer
			 a_bytes)
		{
			if (a_object == null)
			{
				a_bytes.WriteInt(0);
				return 0;
			}
			int id = a_bytes.GetStream().SetInternal(a_bytes.GetTransaction(), a_object, a_bytes
				.GetUpdateDepth(), true);
			a_bytes.WriteInt(id);
			return id;
		}

		public override void Defrag(Db4objects.Db4o.Internal.ReaderPair readers)
		{
		}
	}
}
