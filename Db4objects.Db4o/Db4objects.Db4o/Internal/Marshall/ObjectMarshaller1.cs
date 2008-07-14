/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ObjectMarshaller1 : ObjectMarshaller
	{
		public override void AddFieldIndices(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, Slot oldSlot)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_16(this
				, writer, yc, oldSlot);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_16 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_16(ObjectMarshaller1 _enclosing, StatefulBuffer writer
				, ClassMetadata yc, Slot oldSlot)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.yc = yc;
				this.oldSlot = oldSlot;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (isNull)
				{
					field.AddIndexEntry(writer.Transaction(), writer.GetID(), null);
				}
				else
				{
					field.AddFieldIndex(this._enclosing._family, yc, writer, oldSlot);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly StatefulBuffer writer;

			private readonly ClassMetadata yc;

			private readonly Slot oldSlot;
		}

		public override void DeleteMembers(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, int type, bool isUpdate)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_30(this
				, writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_30 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_30(ObjectMarshaller1 _enclosing, StatefulBuffer writer
				, bool isUpdate)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.isUpdate = isUpdate;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (isNull)
				{
					field.RemoveIndexEntry(writer.Transaction(), writer.GetID(), null);
				}
				else
				{
					field.Delete(this._enclosing._family, writer, isUpdate);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly StatefulBuffer writer;

			private readonly bool isUpdate;
		}

		public override bool FindOffset(ClassMetadata yc, IFieldListInfo fieldListInfo, ByteArrayBuffer
			 reader, FieldMetadata field)
		{
			BooleanByRef found = new BooleanByRef(false);
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_44(field
				, found, reader);
			TraverseFields(yc, reader, fieldListInfo, command);
			return found.value;
		}

		private sealed class _TraverseFieldCommand_44 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_44(FieldMetadata field, BooleanByRef found, ByteArrayBuffer
				 reader)
			{
				this.field = field;
				this.found = found;
				this.reader = reader;
			}

			public override void ProcessField(FieldMetadata curField, bool isNull, ClassMetadata
				 containingClass)
			{
				if (curField == field)
				{
					found.value = !isNull;
					this.Cancel();
					return;
				}
				if (!isNull)
				{
					curField.IncrementOffset(reader);
				}
			}

			private readonly FieldMetadata field;

			private readonly BooleanByRef found;

			private readonly ByteArrayBuffer reader;
		}

		public override ObjectHeaderAttributes ReadHeaderAttributes(ByteArrayBuffer reader
			)
		{
			return new ObjectHeaderAttributes(reader);
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
			return fieldList.IsNull(fieldIndex);
		}

		public override void WriteObjectClassID(ByteArrayBuffer reader, int id)
		{
			reader.WriteInt(-id);
		}

		public override void SkipMarshallerInfo(ByteArrayBuffer reader)
		{
			reader.IncrementOffset(1);
		}
	}
}
