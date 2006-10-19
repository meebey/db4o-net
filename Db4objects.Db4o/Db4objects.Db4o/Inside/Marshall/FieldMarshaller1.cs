namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public class FieldMarshaller1 : Db4objects.Db4o.Inside.Marshall.FieldMarshaller0
	{
		private bool HasBTreeIndex(Db4objects.Db4o.YapField field)
		{
			return !field.IsVirtual();
		}

		public override void Write(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass
			 clazz, Db4objects.Db4o.YapField field, Db4objects.Db4o.YapReader writer)
		{
			base.Write(trans, clazz, field, writer);
			if (!HasBTreeIndex(field))
			{
				return;
			}
			writer.WriteIDOf(trans, field.GetIndex(trans));
		}

		public override Db4objects.Db4o.Inside.Marshall.RawFieldSpec ReadSpec(Db4objects.Db4o.YapStream
			 stream, Db4objects.Db4o.YapReader reader)
		{
			Db4objects.Db4o.Inside.Marshall.RawFieldSpec spec = base.ReadSpec(stream, reader);
			if (spec == null)
			{
				return null;
			}
			if (spec.IsVirtual())
			{
				return spec;
			}
			int indexID = reader.ReadInt();
			spec.IndexID(indexID);
			return spec;
		}

		protected override Db4objects.Db4o.YapField FromSpec(Db4objects.Db4o.Inside.Marshall.RawFieldSpec
			 spec, Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapField field)
		{
			Db4objects.Db4o.YapField actualField = base.FromSpec(spec, stream, field);
			if (spec == null)
			{
				return field;
			}
			if (spec.IndexID() != 0)
			{
				actualField.InitIndex(stream.GetSystemTransaction(), spec.IndexID());
			}
			return actualField;
		}

		public override int MarshalledLength(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapField
			 field)
		{
			int len = base.MarshalledLength(stream, field);
			if (!HasBTreeIndex(field))
			{
				return len;
			}
			int BTREE_ID = Db4objects.Db4o.YapConst.ID_LENGTH;
			return len + BTREE_ID;
		}

		public override void Defrag(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapField
			 yapField, Db4objects.Db4o.YapStringIO sio, Db4objects.Db4o.ReaderPair readers)
		{
			base.Defrag(yapClass, yapField, sio, readers);
			if (yapField.IsVirtual())
			{
				return;
			}
			if (yapField.HasIndex() && CanProcessIndex(yapField))
			{
				Db4objects.Db4o.Inside.Btree.BTree index = yapField.GetIndex(readers.SystemTrans(
					));
				int targetIndexID = readers.CopyID();
				if (targetIndexID != 0)
				{
					index.DefragBTree(readers.Context());
				}
			}
			else
			{
				readers.WriteInt(0);
			}
		}

		private bool CanProcessIndex(Db4objects.Db4o.YapField yapField)
		{
			return !(yapField.GetHandler() is Db4objects.Db4o.YapString) && !(yapField.GetHandler
				() is Db4objects.Db4o.YapArray);
		}
	}
}
