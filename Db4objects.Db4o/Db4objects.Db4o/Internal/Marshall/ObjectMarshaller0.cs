/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	internal class ObjectMarshaller0 : ObjectMarshaller
	{
		public override void AddFieldIndices(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, Slot oldSlot)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_15(this
				, yc, writer, oldSlot);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_15 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_15(ObjectMarshaller0 _enclosing, ClassMetadata yc, StatefulBuffer
				 writer, Slot oldSlot)
			{
				this._enclosing = _enclosing;
				this.yc = yc;
				this.writer = writer;
				this.oldSlot = oldSlot;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				field.AddFieldIndex(this._enclosing._family, yc, writer, oldSlot);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly ClassMetadata yc;

			private readonly StatefulBuffer writer;

			private readonly Slot oldSlot;
		}

		public override void DeleteMembers(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, int type, bool isUpdate)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_24(this
				, writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_24 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_24(ObjectMarshaller0 _enclosing, StatefulBuffer writer
				, bool isUpdate)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.isUpdate = isUpdate;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				field.Delete(this._enclosing._family, writer, isUpdate);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly StatefulBuffer writer;

			private readonly bool isUpdate;
		}

		public override bool FindOffset(ClassMetadata yc, IFieldListInfo fieldListInfo, ByteArrayBuffer
			 buffer, FieldMetadata field)
		{
			bool[] ret = new bool[] { false };
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_34(field
				, ret, buffer);
			TraverseFields(yc, buffer, fieldListInfo, command);
			return ret[0];
		}

		private sealed class _TraverseFieldCommand_34 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_34(FieldMetadata field, bool[] ret, ByteArrayBuffer 
				buffer)
			{
				this.field = field;
				this.ret = ret;
				this.buffer = buffer;
			}

			public override void ProcessField(FieldMetadata curField, bool isNull, ClassMetadata
				 containingClass)
			{
				if (curField == field)
				{
					ret[0] = true;
					this.Cancel();
					return;
				}
				curField.IncrementOffset(buffer);
			}

			private readonly FieldMetadata field;

			private readonly bool[] ret;

			private readonly ByteArrayBuffer buffer;
		}

		protected int HeaderLength()
		{
			return Const4.ObjectLength + Const4.IdLength;
		}

		/// <param name="yf"></param>
		/// <param name="yo"></param>
		protected virtual int MarshalledLength(FieldMetadata yf, ObjectReference yo)
		{
			return 0;
		}

		public override ObjectHeaderAttributes ReadHeaderAttributes(ByteArrayBuffer reader
			)
		{
			return null;
		}

		/// <exception cref="FieldIndexException"></exception>
		public override object ReadIndexEntry(ClassMetadata clazz, ObjectHeaderAttributes
			 attributes, FieldMetadata field, StatefulBuffer reader)
		{
			if (clazz == null)
			{
				return null;
			}
			if (!FindOffset(clazz, attributes, reader, field))
			{
				return null;
			}
			try
			{
				return field.ReadIndexEntry(_family, reader);
			}
			catch (CorruptionException exc)
			{
				throw new FieldIndexException(exc, field);
			}
		}

		protected override bool IsNull(IFieldListInfo fieldList, int fieldIndex)
		{
			return false;
		}

		public override void WriteObjectClassID(ByteArrayBuffer reader, int id)
		{
			reader.WriteInt(id);
		}

		public override void SkipMarshallerInfo(ByteArrayBuffer reader)
		{
		}
	}
}
