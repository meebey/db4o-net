/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class ObjectMarshaller
	{
		public MarshallerFamily _family;

		protected abstract class TraverseFieldCommand
		{
			private bool _cancelled = false;

			public virtual int FieldCount(ClassMetadata classMetadata, IReadBuffer reader)
			{
				return classMetadata.ReadFieldCount(reader);
			}

			public virtual bool Cancelled()
			{
				return _cancelled;
			}

			protected virtual void Cancel()
			{
				_cancelled = true;
			}

			public abstract void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass);
		}

		protected void TraverseFields(IMarshallingInfo context, ObjectMarshaller.TraverseFieldCommand
			 command)
		{
			TraverseFields(context.ClassMetadata(), (ByteArrayBuffer)context.Buffer(), context
				, command);
		}

		protected void TraverseFields(ClassMetadata classMetadata, ByteArrayBuffer buffer
			, IFieldListInfo fieldList, ObjectMarshaller.TraverseFieldCommand command)
		{
			int fieldIndex = 0;
			while (classMetadata != null && !command.Cancelled())
			{
				int fieldCount = command.FieldCount(classMetadata, buffer);
				for (int i = 0; i < fieldCount && !command.Cancelled(); i++)
				{
					command.ProcessField(classMetadata.i_fields[i], IsNull(fieldList, fieldIndex), classMetadata
						);
					fieldIndex++;
				}
				// FIXME:  If ClassMetadata doesn't use the default _typeHandler
				//         we can't traverse it's fields. 
				//         We should stop processing ancestors if  
				//         ClassMetadata#defaultObjectHandlerIsUsed() returns false
				//         on the ancestor
				classMetadata = classMetadata.i_ancestor;
			}
		}

		protected abstract bool IsNull(IFieldListInfo fieldList, int fieldIndex);

		public abstract void AddFieldIndices(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, Slot oldSlot);

		public abstract void DeleteMembers(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, int a_type, bool isUpdate);

		public abstract bool FindOffset(ClassMetadata classMetadata, IFieldListInfo fieldListInfo
			, ByteArrayBuffer buffer, FieldMetadata field);

		public abstract object ReadIndexEntry(ClassMetadata yc, ObjectHeaderAttributes attributes
			, FieldMetadata yf, StatefulBuffer reader);

		public abstract ObjectHeaderAttributes ReadHeaderAttributes(ByteArrayBuffer reader
			);

		public abstract void WriteObjectClassID(ByteArrayBuffer reader, int id);

		public abstract void SkipMarshallerInfo(ByteArrayBuffer reader);
	}
}
