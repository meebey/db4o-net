/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class ObjectMarshaller
	{
		public MarshallerFamily _family;

		protected abstract class TraverseFieldCommand
		{
			private bool _cancelled = false;

			public virtual int FieldCount(ClassMetadata classMetadata, Db4objects.Db4o.Internal.Buffer
				 reader)
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

		protected void TraverseFields(ClassMetadata classMetadata, Db4objects.Db4o.Internal.Buffer
			 reader, IFieldListInfo fieldList, ObjectMarshaller.TraverseFieldCommand command
			)
		{
			int fieldIndex = 0;
			while (classMetadata != null && !command.Cancelled())
			{
				int fieldCount = command.FieldCount(classMetadata, reader);
				for (int i = 0; i < fieldCount && !command.Cancelled(); i++)
				{
					command.ProcessField(classMetadata.i_fields[i], IsNull(fieldList, fieldIndex), classMetadata
						);
					fieldIndex++;
				}
				classMetadata = classMetadata.i_ancestor;
			}
		}

		protected abstract bool IsNull(IFieldListInfo fieldList, int fieldIndex);

		public abstract void AddFieldIndices(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, Slot oldSlot);

		public abstract TreeInt CollectFieldIDs(TreeInt tree, ClassMetadata yc, ObjectHeaderAttributes
			 attributes, StatefulBuffer reader, string name);

		protected virtual StatefulBuffer CreateWriterForNew(Transaction trans, ObjectReference
			 yo, int updateDepth, int length)
		{
			int id = yo.GetID();
			Slot slot = new Slot(-1, length);
			if (trans is LocalTransaction)
			{
				slot = ((LocalTransaction)trans).File().GetSlot(length);
				trans.SlotFreeOnRollback(id, slot);
			}
			trans.SetPointer(id, slot);
			return CreateWriterForUpdate(trans, updateDepth, id, slot.Address(), slot.Length(
				));
		}

		protected virtual StatefulBuffer CreateWriterForUpdate(Transaction a_trans, int updateDepth
			, int id, int address, int length)
		{
			length = a_trans.Container().BlockAlignedBytes(length);
			StatefulBuffer writer = new StatefulBuffer(a_trans, length);
			writer.UseSlot(id, address, length);
			writer.SetUpdateDepth(updateDepth);
			return writer;
		}

		public abstract void DeleteMembers(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, int a_type, bool isUpdate);

		public abstract bool FindOffset(ClassMetadata yc, ObjectHeaderAttributes attributes
			, Db4objects.Db4o.Internal.Buffer reader, FieldMetadata field);

		public abstract void InstantiateFields(ClassMetadata yc, ObjectHeaderAttributes attributes
			, ObjectReference yo, object obj, StatefulBuffer reader);

		public abstract StatefulBuffer MarshallNew(Transaction a_trans, ObjectReference yo
			, int a_updateDepth);

		public abstract void MarshallUpdate(Transaction a_trans, int a_updateDepth, ObjectReference
			 a_yapObject, object a_object);

		protected virtual void MarshallUpdateWrite(Transaction trans, ObjectReference yo, 
			object obj, StatefulBuffer writer)
		{
			ClassMetadata yc = yo.ClassMetadata();
			ObjectContainerBase stream = trans.Container();
			stream.WriteUpdate(yc, writer);
			if (yo.IsActive())
			{
				yo.SetStateClean();
			}
			yo.EndProcessing();
			ObjectOnUpdate(trans, yc, obj);
		}

		private void ObjectOnUpdate(Transaction transaction, ClassMetadata yc, object obj
			)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnUpdate(transaction, obj);
			yc.DispatchEvent(container, obj, EventDispatcher.UPDATE);
		}

		public abstract object ReadIndexEntry(ClassMetadata yc, ObjectHeaderAttributes attributes
			, FieldMetadata yf, StatefulBuffer reader);

		public abstract ObjectHeaderAttributes ReadHeaderAttributes(Db4objects.Db4o.Internal.Buffer
			 reader);

		public abstract void ReadVirtualAttributes(Transaction trans, ClassMetadata yc, ObjectReference
			 yo, ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.Buffer reader);

		public abstract void DefragFields(ClassMetadata yapClass, ObjectHeader header, BufferPair
			 readers);

		public abstract void WriteObjectClassID(Db4objects.Db4o.Internal.Buffer reader, int
			 id);

		public abstract void SkipMarshallerInfo(Db4objects.Db4o.Internal.Buffer reader);
	}
}
