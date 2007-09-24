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

		protected void TraverseFields(IMarshallingInfo context, ObjectMarshaller.TraverseFieldCommand
			 command)
		{
			TraverseFields(context.ClassMetadata(), context.Buffer(), context, command);
		}

		protected void TraverseFields(ClassMetadata classMetadata, Db4objects.Db4o.Internal.Buffer
			 buffer, IFieldListInfo fieldList, ObjectMarshaller.TraverseFieldCommand command
			)
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

		public abstract bool FindOffset(ClassMetadata classMetadata, IFieldListInfo fieldListInfo
			, Db4objects.Db4o.Internal.Buffer buffer, FieldMetadata field);

		public abstract void InstantiateFields(ClassMetadata yc, ObjectHeaderAttributes attributes
			, ObjectReference yo, object obj, StatefulBuffer reader);

		public void MarshallUpdateWrite(Transaction trans, Pointer4 pointer, ObjectReference
			 @ref, object obj, Db4objects.Db4o.Internal.Buffer buffer)
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

		public void InstantiateFields(UnmarshallingContext context)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_161(this
				, context);
			TraverseFields(context, command);
		}

		private sealed class _TraverseFieldCommand_161 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_161(ObjectMarshaller _enclosing, UnmarshallingContext
				 context)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
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

			private readonly ObjectMarshaller _enclosing;

			private readonly UnmarshallingContext context;
		}

		public virtual void Marshall(object obj, MarshallingContext context)
		{
			Transaction trans = context.Transaction();
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_183(this
				, context, trans, obj);
			TraverseFields(context, command);
		}

		private sealed class _TraverseFieldCommand_183 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_183(ObjectMarshaller _enclosing, MarshallingContext 
				context, Transaction trans, object obj)
			{
				this._enclosing = _enclosing;
				this.context = context;
				this.trans = trans;
				this.obj = obj;
			}

			private int fieldIndex = -1;

			public override int FieldCount(ClassMetadata classMetadata, Db4objects.Db4o.Internal.Buffer
				 buffer)
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

			private readonly ObjectMarshaller _enclosing;

			private readonly MarshallingContext context;

			private readonly Transaction trans;

			private readonly object obj;
		}
	}
}
