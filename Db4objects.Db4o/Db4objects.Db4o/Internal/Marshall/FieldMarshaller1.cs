/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Encoding;
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

		public override void Write(Transaction trans, ClassMetadata clazz, ClassAspect aspect
			, ByteArrayBuffer writer)
		{
			base.Write(trans, clazz, aspect, writer);
			if (!(aspect is FieldMetadata))
			{
				return;
			}
			FieldMetadata field = (FieldMetadata)aspect;
			if (!HasBTreeIndex(field))
			{
				return;
			}
			writer.WriteIDOf(trans, field.GetIndex(trans));
		}

		protected override RawFieldSpec ReadSpec(AspectType aspectType, ObjectContainerBase
			 stream, ByteArrayBuffer reader)
		{
			RawFieldSpec spec = base.ReadSpec(aspectType, stream, reader);
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

		public override int MarshalledLength(ObjectContainerBase stream, ClassAspect aspect
			)
		{
			int len = base.MarshalledLength(stream, aspect);
			if (!(aspect is FieldMetadata))
			{
				return len;
			}
			FieldMetadata field = (FieldMetadata)aspect;
			if (!HasBTreeIndex(field))
			{
				return len;
			}
			int BtreeId = Const4.IdLength;
			return len + BtreeId;
		}

		public override void Defrag(ClassMetadata classMetadata, ClassAspect aspect, LatinStringIO
			 sio, DefragmentContextImpl context)
		{
			base.Defrag(classMetadata, aspect, sio, context);
			if (!(aspect is FieldMetadata))
			{
				return;
			}
			FieldMetadata field = (FieldMetadata)aspect;
			if (field.IsVirtual())
			{
				return;
			}
			if (field.HasIndex())
			{
				BTree index = field.GetIndex(context.SystemTrans());
				int targetIndexID = context.CopyID();
				if (targetIndexID != 0)
				{
					index.DefragBTree(context.Services());
				}
			}
			else
			{
				context.WriteInt(0);
			}
		}
	}
}
