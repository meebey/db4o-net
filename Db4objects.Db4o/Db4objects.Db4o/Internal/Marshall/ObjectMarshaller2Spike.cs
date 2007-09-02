/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ObjectMarshaller2Spike : ObjectMarshaller1
	{
		public ObjectMarshaller2Spike()
		{
			throw new InvalidOperationException();
		}

		public override StatefulBuffer MarshallNew(Transaction trans, ObjectReference @ref
			, int updateDepth)
		{
			MarshallingContext context = new MarshallingContext(trans, @ref, updateDepth, true
				);
			Marshall(@ref.GetObject(), context);
			return context.ToWriteBuffer();
		}

		protected virtual void Marshall(object obj, MarshallingContext context)
		{
			Transaction trans = context.Transaction();
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_32(this
				, context, trans, obj);
			TraverseFields(context.ClassMetadata(), null, context, command);
		}

		private sealed class _TraverseFieldCommand_32 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_32(ObjectMarshaller2Spike _enclosing, MarshallingContext
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

			private readonly ObjectMarshaller2Spike _enclosing;

			private readonly MarshallingContext context;

			private readonly Transaction trans;

			private readonly object obj;
		}
	}
}
