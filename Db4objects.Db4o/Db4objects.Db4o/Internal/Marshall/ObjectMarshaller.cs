/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
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

			public virtual int FieldCount(ClassMetadata yapClass, Db4objects.Db4o.Internal.Buffer
				 reader)
			{
				return (Debug.atHome ? yapClass.ReadFieldCountSodaAtHome(reader) : yapClass.ReadFieldCount
					(reader));
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

		protected virtual void TraverseFields(ClassMetadata yc, Db4objects.Db4o.Internal.Buffer
			 reader, ObjectHeaderAttributes attributes, ObjectMarshaller.TraverseFieldCommand
			 command)
		{
			int fieldIndex = 0;
			while (yc != null && !command.Cancelled())
			{
				int fieldCount = command.FieldCount(yc, reader);
				for (int i = 0; i < fieldCount && !command.Cancelled(); i++)
				{
					command.ProcessField(yc.i_fields[i], IsNull(attributes, fieldIndex), yc);
					fieldIndex++;
				}
				yc = yc.i_ancestor;
			}
		}

		protected abstract bool IsNull(ObjectHeaderAttributes attributes, int fieldIndex);

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
			ClassMetadata yc = yo.GetYapClass();
			ObjectContainerBase stream = trans.Stream();
			stream.WriteUpdate(yc, writer);
			if (yo.IsActive())
			{
				yo.SetStateClean();
			}
			yo.EndProcessing();
			ObjectOnUpdate(yc, stream, obj);
		}

		private void ObjectOnUpdate(ClassMetadata yc, ObjectContainerBase stream, object 
			obj)
		{
			stream.Callbacks().ObjectOnUpdate(obj);
			yc.DispatchEvent(stream, obj, EventDispatcher.UPDATE);
		}

		public abstract object ReadIndexEntry(ClassMetadata yc, ObjectHeaderAttributes attributes
			, FieldMetadata yf, StatefulBuffer reader);

		public abstract ObjectHeaderAttributes ReadHeaderAttributes(Db4objects.Db4o.Internal.Buffer
			 reader);

		public abstract void ReadVirtualAttributes(Transaction trans, ClassMetadata yc, ObjectReference
			 yo, ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.Buffer reader);

		public abstract void DefragFields(ClassMetadata yapClass, ObjectHeader header, ReaderPair
			 readers);

		public abstract void WriteObjectClassID(Db4objects.Db4o.Internal.Buffer reader, int
			 id);

		public abstract void SkipMarshallerInfo(Db4objects.Db4o.Internal.Buffer reader);
	}
}
