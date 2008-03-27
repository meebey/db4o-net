/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class FirstClassObjectHandler : ITypeHandler4, ICompositeTypeHandler
	{
		private const int HashcodeForNull = 72483944;

		private Db4objects.Db4o.Internal.ClassMetadata _classMetadata;

		public FirstClassObjectHandler(Db4objects.Db4o.Internal.ClassMetadata classMetadata
			)
		{
			_classMetadata = classMetadata;
		}

		public FirstClassObjectHandler()
		{
		}

		// required for reflection cloning
		public virtual void Defragment(IDefragmentContext context)
		{
			if (_classMetadata.HasClassIndex())
			{
				context.CopyID();
			}
			else
			{
				context.CopyUnindexedID();
			}
			int restLength = (_classMetadata.LinkLength() - Const4.IntLength);
			context.IncrementOffset(restLength);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			((ObjectContainerBase)context.ObjectContainer()).DeleteByID(context.Transaction()
				, context.ReadInt(), context.CascadeDeleteDepth());
		}

		public void InstantiateFields(UnmarshallingContext context)
		{
			BooleanByRef updateFieldFound = new BooleanByRef();
			int savedOffset = context.Offset();
			FirstClassObjectHandler.TraverseFieldCommand command = new _TraverseFieldCommand_52
				(updateFieldFound, context);
			TraverseFields(context, command);
			if (updateFieldFound.value)
			{
				context.Seek(savedOffset);
				command = new _TraverseFieldCommand_76(context);
				TraverseFields(context, command);
			}
		}

		private sealed class _TraverseFieldCommand_52 : FirstClassObjectHandler.TraverseFieldCommand
		{
			public _TraverseFieldCommand_52(BooleanByRef updateFieldFound, UnmarshallingContext
				 context)
			{
				this.updateFieldFound = updateFieldFound;
				this.context = context;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, Db4objects.Db4o.Internal.ClassMetadata
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

		private sealed class _TraverseFieldCommand_76 : FirstClassObjectHandler.TraverseFieldCommand
		{
			public _TraverseFieldCommand_76(UnmarshallingContext context)
			{
				this.context = context;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, Db4objects.Db4o.Internal.ClassMetadata
				 containingClass)
			{
				field.AttemptUpdate(context);
			}

			private readonly UnmarshallingContext context;
		}

		public virtual object Read(IReadContext context)
		{
			UnmarshallingContext unmarshallingContext = (UnmarshallingContext)context;
			// FIXME: Commented out code below is the implementation plan to let
			//        FirstClassObjectHandler take responsibility of fieldcount
			//        and null Bitmap.        
			//        BitMap4 nullBitMap = unmarshallingContext.readBitMap(fieldCount);
			//        int fieldCount = context.readInt();
			InstantiateFields(unmarshallingContext);
			return unmarshallingContext.PersistentObject();
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			//        int fieldCount = _classMetadata.fieldCount();
			//        context.writeInt(fieldCount);
			//        final BitMap4 nullBitMap = new BitMap4(fieldCount);
			//        ReservedBuffer bitMapBuffer = context.reserve(nullBitMap.marshalledLength());
			Marshall(obj, (MarshallingContext)context);
		}

		//        bitMapBuffer.writeBytes(nullBitMap.bytes());
		public virtual void Marshall(object obj, MarshallingContext context)
		{
			Transaction trans = context.Transaction();
			FirstClassObjectHandler.TraverseFieldCommand command = new _TraverseFieldCommand_116
				(context, trans, obj);
			TraverseFields(context, command);
		}

		private sealed class _TraverseFieldCommand_116 : FirstClassObjectHandler.TraverseFieldCommand
		{
			public _TraverseFieldCommand_116(MarshallingContext context, Transaction trans, object
				 obj)
			{
				this.context = context;
				this.trans = trans;
				this.obj = obj;
				this.fieldIndex = -1;
			}

			private int fieldIndex;

			public override int FieldCount(Db4objects.Db4o.Internal.ClassMetadata classMetadata
				, ByteArrayBuffer buffer)
			{
				int fieldCount = classMetadata.i_fields.Length;
				context.FieldCount(fieldCount);
				return fieldCount;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, Db4objects.Db4o.Internal.ClassMetadata
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

		public virtual IPreparedComparison PrepareComparison(IContext context, object source
			)
		{
			if (source == null)
			{
				return new _IPreparedComparison_145();
			}
			int id = 0;
			IReflectClass claxx = null;
			if (source is int)
			{
				id = ((int)source);
			}
			else
			{
				if (source is TransactionContext)
				{
					TransactionContext tc = (TransactionContext)source;
					object obj = tc._object;
					id = _classMetadata.Stream().GetID(tc._transaction, obj);
					claxx = _classMetadata.Reflector().ForObject(obj);
				}
				else
				{
					throw new IllegalComparisonException();
				}
			}
			return new ClassMetadata.PreparedComparisonImpl(id, claxx);
		}

		private sealed class _IPreparedComparison_145 : IPreparedComparison
		{
			public _IPreparedComparison_145()
			{
			}

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return -1;
			}
		}

		protected abstract class TraverseFieldCommand
		{
			private bool _cancelled = false;

			public virtual int FieldCount(ClassMetadata classMetadata, ByteArrayBuffer reader
				)
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

		protected void TraverseFields(IMarshallingInfo context, FirstClassObjectHandler.TraverseFieldCommand
			 command)
		{
			TraverseFields(context.ClassMetadata(), (ByteArrayBuffer)context.Buffer(), context
				, command);
		}

		protected void TraverseFields(ClassMetadata classMetadata, ByteArrayBuffer buffer
			, IFieldListInfo fieldList, FirstClassObjectHandler.TraverseFieldCommand command
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

		protected virtual bool IsNull(IFieldListInfo fieldList, int fieldIndex)
		{
			return fieldList.IsNull(fieldIndex);
		}

		public virtual ClassMetadata ClassMetadata()
		{
			return _classMetadata;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FirstClassObjectHandler))
			{
				return false;
			}
			FirstClassObjectHandler other = (FirstClassObjectHandler)obj;
			if (_classMetadata == null)
			{
				return other._classMetadata == null;
			}
			return _classMetadata.Equals(other._classMetadata);
		}

		public override int GetHashCode()
		{
			if (_classMetadata != null)
			{
				return _classMetadata.GetHashCode();
			}
			return HashcodeForNull;
		}

		public virtual ITypeHandler4 GenericTemplate()
		{
			return new FirstClassObjectHandler(null);
		}

		public virtual object DeepClone(object context)
		{
			TypeHandlerCloneContext typeHandlerCloneContext = (TypeHandlerCloneContext)context;
			FirstClassObjectHandler cloned = (FirstClassObjectHandler)Reflection4.NewInstance
				(this);
			FirstClassObjectHandler original = (FirstClassObjectHandler)typeHandlerCloneContext
				.original;
			cloned._classMetadata = original._classMetadata;
			return cloned;
		}
	}
}
