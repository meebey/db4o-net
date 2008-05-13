/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
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
				classMetadata = classMetadata.i_ancestor;
			}
		}

		protected abstract bool IsNull(IFieldListInfo fieldList, int fieldIndex);

		public abstract void AddFieldIndices(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, Slot oldSlot);

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

		public abstract bool FindOffset(ClassMetadata classMetadata, IFieldListInfo fieldListInfo
			, ByteArrayBuffer buffer, FieldMetadata field);

		public void MarshallUpdateWrite(Transaction trans, Pointer4 pointer, ObjectReference
			 @ref, object obj, ByteArrayBuffer buffer)
		{
			ClassMetadata classMetadata = @ref.ClassMetadata();
			ObjectContainerBase container = trans.Container();
			container.WriteUpdate(trans, pointer, classMetadata, buffer);
			if (@ref.IsActive())
			{
				@ref.SetStateClean();
			}
			@ref.EndProcessing();
			ObjectOnUpdate(trans, classMetadata, obj);
		}

		private void ObjectOnUpdate(Transaction transaction, ClassMetadata yc, object obj
			)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnUpdate(transaction, obj);
			yc.DispatchEvent(transaction, obj, EventDispatcher.Update);
		}

		public abstract object ReadIndexEntry(ClassMetadata yc, ObjectHeaderAttributes attributes
			, FieldMetadata yf, StatefulBuffer reader);

		public abstract ObjectHeaderAttributes ReadHeaderAttributes(ByteArrayBuffer reader
			);

		public abstract void ReadVirtualAttributes(Transaction trans, ClassMetadata yc, ObjectReference
			 yo, ObjectHeaderAttributes attributes, ByteArrayBuffer reader);

		public abstract void DefragFields(ClassMetadata yapClass, ObjectHeader header, DefragmentContextImpl
			 context);

		public abstract void WriteObjectClassID(ByteArrayBuffer reader, int id);

		public abstract void SkipMarshallerInfo(ByteArrayBuffer reader);

		public void InstantiateFields(UnmarshallingContext context)
		{
			BooleanByRef updateFieldFound = new BooleanByRef();
			int savedOffset = context.Offset();
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_155(updateFieldFound
				, context);
			TraverseFields(context, command);
			if (updateFieldFound.value)
			{
				context.Seek(savedOffset);
				command = new _TraverseFieldCommand_179(context);
				TraverseFields(context, command);
			}
		}

		private sealed class _TraverseFieldCommand_155 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_155(BooleanByRef updateFieldFound, UnmarshallingContext
				 context)
			{
				this.updateFieldFound = updateFieldFound;
				this.context = context;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (field.Updating())
				{
					updateFieldFound.value = true;
				}
				if (isNull)
				{
					field.Set(context.PersistentObject(), null);
					return;
				}
				bool ok = false;
				try
				{
					field.Instantiate(context);
					ok = true;
				}
				finally
				{
					if (!ok)
					{
						this.Cancel();
					}
				}
			}

			private readonly BooleanByRef updateFieldFound;

			private readonly UnmarshallingContext context;
		}

		private sealed class _TraverseFieldCommand_179 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_179(UnmarshallingContext context)
			{
				this.context = context;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				field.AttemptUpdate(context);
			}

			private readonly UnmarshallingContext context;
		}

		public virtual void Marshall(object obj, MarshallingContext context)
		{
			Transaction trans = context.Transaction();
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_191(context
				, trans, obj);
			TraverseFields(context, command);
		}

		private sealed class _TraverseFieldCommand_191 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_191(MarshallingContext context, Transaction trans, object
				 obj)
			{
				this.context = context;
				this.trans = trans;
				this.obj = obj;
				this.fieldIndex = -1;
			}

			private int fieldIndex;

			public override int FieldCount(ClassMetadata classMetadata, IReadBuffer buffer)
			{
				int fieldCount = classMetadata.i_fields.Length;
				context.FieldCount(fieldCount);
				return fieldCount;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				context.NextField();
				this.fieldIndex++;
				object child = field.GetOrCreate(trans, obj);
				if (child == null)
				{
					context.IsNull(this.fieldIndex, true);
					field.AddIndexEntry(trans, context.ObjectID(), null);
					return;
				}
				if (child is IDb4oTypeImpl)
				{
					child = ((IDb4oTypeImpl)child).StoredTo(trans);
				}
				field.Marshall(context, child);
			}

			private readonly MarshallingContext context;

			private readonly Transaction trans;

			private readonly object obj;
		}
	}
}
