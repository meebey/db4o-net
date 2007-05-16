/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class FieldMarshaller1 : FieldMarshaller0
	{
		private bool HasBTreeIndex(FieldMetadata field)
		{
			return !field.IsVirtual();
		}

		public override void Write(Transaction trans, ClassMetadata clazz, FieldMetadata 
			field, Db4objects.Db4o.Internal.Buffer writer)
		{
			base.Write(trans, clazz, field, writer);
			if (!HasBTreeIndex(field))
			{
				return;
			}
			writer.WriteIDOf(trans, field.GetIndex(trans));
		}

		public override RawFieldSpec ReadSpec(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			RawFieldSpec spec = base.ReadSpec(stream, reader);
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

		protected override FieldMetadata FromSpec(RawFieldSpec spec, ObjectContainerBase 
			stream, FieldMetadata field)
		{
			FieldMetadata actualField = base.FromSpec(spec, stream, field);
			if (spec == null)
			{
				return field;
			}
			if (spec.IndexID() != 0)
			{
				actualField.InitIndex(stream.SystemTransaction(), spec.IndexID());
			}
			return actualField;
		}

		public override int MarshalledLength(ObjectContainerBase stream, FieldMetadata field
			)
		{
			int len = base.MarshalledLength(stream, field);
			if (!HasBTreeIndex(field))
			{
				return len;
			}
			int BTREE_ID = Const4.ID_LENGTH;
			return len + BTREE_ID;
		}

		public override void Defrag(ClassMetadata yapClass, FieldMetadata yapField, LatinStringIO
			 sio, ReaderPair readers)
		{
			base.Defrag(yapClass, yapField, sio, readers);
			if (yapField.IsVirtual())
			{
				return;
			}
			if (yapField.HasIndex())
			{
				BTree index = yapField.GetIndex(readers.SystemTrans());
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
	}
}
