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
					field.AddIndexEntry(writer.GetTransaction(), writer.GetID(), null);
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

		public override TreeInt CollectFieldIDs(TreeInt tree, ClassMetadata yc, ObjectHeaderAttributes
			 attributes, StatefulBuffer writer, string name)
		{
			TreeInt[] ret = new TreeInt[] { tree };
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_31(this
				, name, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _TraverseFieldCommand_31 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_31(ObjectMarshaller1 _enclosing, string name, TreeInt
				[] ret, StatefulBuffer writer)
			{
				this._enclosing = _enclosing;
				this.name = name;
				this.ret = ret;
				this.writer = writer;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (isNull)
				{
					return;
				}
				if (name.Equals(field.GetName()))
				{
					ret[0] = field.CollectIDs(this._enclosing._family, ret[0], writer);
				}
				else
				{
					field.IncrementOffset(writer);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly string name;

			private readonly TreeInt[] ret;

			private readonly StatefulBuffer writer;
		}

		public override void DeleteMembers(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, int type, bool isUpdate)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_49(this
				, writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_49 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_49(ObjectMarshaller1 _enclosing, StatefulBuffer writer
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
					field.RemoveIndexEntry(writer.GetTransaction(), writer.GetID(), null);
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
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_63(field
				, found, reader);
			TraverseFields(yc, reader, fieldListInfo, command);
			return found.value;
		}

		private sealed class _TraverseFieldCommand_63 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_63(FieldMetadata field, BooleanByRef found, ByteArrayBuffer
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

		public override void ReadVirtualAttributes(Transaction trans, ClassMetadata yc, ObjectReference
			 yo, ObjectHeaderAttributes attributes, ByteArrayBuffer reader)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_100(trans
				, reader, yo);
			TraverseFields(yc, reader, attributes, command);
		}

		private sealed class _TraverseFieldCommand_100 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_100(Transaction trans, ByteArrayBuffer reader, ObjectReference
				 yo)
			{
				this.trans = trans;
				this.reader = reader;
				this.yo = yo;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (!isNull)
				{
					field.ReadVirtualAttribute(trans, reader, yo);
				}
			}

			private readonly Transaction trans;

			private readonly ByteArrayBuffer reader;

			private readonly ObjectReference yo;
		}

		protected override bool IsNull(IFieldListInfo fieldList, int fieldIndex)
		{
			return fieldList.IsNull(fieldIndex);
		}

		public override void DefragFields(ClassMetadata clazz, ObjectHeader header, DefragmentContextImpl
			 context)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_115(this
				, context);
			TraverseFields(clazz, null, header._headerAttributes, command);
		}

		private sealed class _TraverseFieldCommand_115 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_115(ObjectMarshaller1 _enclosing, DefragmentContextImpl
				 context)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public override int FieldCount(ClassMetadata yapClass, ByteArrayBuffer reader)
			{
				return context.ReadInt();
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (!isNull)
				{
					field.DefragField(this._enclosing._family, context);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly DefragmentContextImpl context;
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
