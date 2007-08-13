/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
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
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_15(this
				, writer, yc, oldSlot);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_15 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_15(ObjectMarshaller1 _enclosing, StatefulBuffer writer
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
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_30(this
				, name, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _TraverseFieldCommand_30 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_30(ObjectMarshaller1 _enclosing, string name, TreeInt[]
				 ret, StatefulBuffer writer)
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
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_48(this
				, writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_48 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_48(ObjectMarshaller1 _enclosing, StatefulBuffer writer
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

		public override bool FindOffset(ClassMetadata yc, ObjectHeaderAttributes attributes
			, Db4objects.Db4o.Internal.Buffer reader, FieldMetadata field)
		{
			bool[] ret = new bool[] { false };
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_62(this
				, field, ret, reader);
			TraverseFields(yc, reader, attributes, command);
			return ret[0];
		}

		private sealed class _TraverseFieldCommand_62 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_62(ObjectMarshaller1 _enclosing, FieldMetadata field
				, bool[] ret, Db4objects.Db4o.Internal.Buffer reader)
			{
				this._enclosing = _enclosing;
				this.field = field;
				this.ret = ret;
				this.reader = reader;
			}

			public override void ProcessField(FieldMetadata curField, bool isNull, ClassMetadata
				 containingClass)
			{
				if (curField == field)
				{
					ret[0] = !isNull;
					this.Cancel();
					return;
				}
				if (!isNull)
				{
					curField.IncrementOffset(reader);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly FieldMetadata field;

			private readonly bool[] ret;

			private readonly Db4objects.Db4o.Internal.Buffer reader;
		}

		public override void InstantiateFields(ClassMetadata yc, ObjectHeaderAttributes attributes
			, ObjectReference yapObject, object onObject, StatefulBuffer writer)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_79(this
				, onObject, yapObject, writer);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_79 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_79(ObjectMarshaller1 _enclosing, object onObject, ObjectReference
				 yapObject, StatefulBuffer writer)
			{
				this._enclosing = _enclosing;
				this.onObject = onObject;
				this.yapObject = yapObject;
				this.writer = writer;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (isNull)
				{
					field.Set(onObject, null);
					return;
				}
				bool ok = false;
				try
				{
					field.Instantiate(this._enclosing._family, yapObject, onObject, writer);
					ok = true;
				}
				catch (CorruptionException)
				{
				}
				finally
				{
					if (!ok)
					{
						this.Cancel();
					}
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly object onObject;

			private readonly ObjectReference yapObject;

			private readonly StatefulBuffer writer;
		}

		private void Marshall(ObjectReference yo, object obj, ObjectHeaderAttributes1 attributes
			, StatefulBuffer writer, bool isNew)
		{
			ClassMetadata yc = yo.GetYapClass();
			WriteObjectClassID(writer, yc.GetID());
			attributes.Write(writer);
			yc.CheckUpdateDepth(writer);
			Transaction trans = writer.GetTransaction();
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_108(this
				, trans, writer, obj, yo, isNew);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _TraverseFieldCommand_108 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_108(ObjectMarshaller1 _enclosing, Transaction trans, 
				StatefulBuffer writer, object obj, ObjectReference yo, bool isNew)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.writer = writer;
				this.obj = obj;
				this.yo = yo;
				this.isNew = isNew;
			}

			public override int FieldCount(ClassMetadata yapClass, Db4objects.Db4o.Internal.Buffer
				 reader)
			{
				reader.WriteInt(yapClass.i_fields.Length);
				return yapClass.i_fields.Length;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (isNull)
				{
					field.AddIndexEntry(trans, writer.GetID(), null);
					return;
				}
				object child = field.GetOrCreate(trans, obj);
				if (child is IDb4oTypeImpl)
				{
					child = ((IDb4oTypeImpl)child).StoredTo(trans);
				}
				field.Marshall(yo, child, this._enclosing._family, writer, containingClass.ConfigOrAncestorConfig
					(), isNew);
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly Transaction trans;

			private readonly StatefulBuffer writer;

			private readonly object obj;

			private readonly ObjectReference yo;

			private readonly bool isNew;
		}

		public override StatefulBuffer MarshallNew(Transaction a_trans, ObjectReference yo
			, int a_updateDepth)
		{
			ObjectHeaderAttributes1 attributes = new ObjectHeaderAttributes1(yo);
			StatefulBuffer writer = CreateWriterForNew(a_trans, yo, a_updateDepth, attributes
				.ObjectLength());
			Marshall(yo, yo.GetObject(), attributes, writer, true);
			return writer;
		}

		public override void MarshallUpdate(Transaction trans, int updateDepth, ObjectReference
			 yo, object obj)
		{
			ObjectHeaderAttributes1 attributes = new ObjectHeaderAttributes1(yo);
			StatefulBuffer writer = CreateWriterForUpdate(trans, updateDepth, yo.GetID(), 0, 
				attributes.ObjectLength());
			if (trans is LocalTransaction)
			{
				((LocalTransaction)trans).File().GetSlotForUpdate(writer);
			}
			Marshall(yo, obj, attributes, writer, false);
			MarshallUpdateWrite(trans, yo, obj, writer);
		}

		public override ObjectHeaderAttributes ReadHeaderAttributes(Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			return new ObjectHeaderAttributes1(reader);
		}

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
			 yo, ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.Buffer reader)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_196(this
				, trans, reader, yo);
			TraverseFields(yc, reader, attributes, command);
		}

		private sealed class _TraverseFieldCommand_196 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_196(ObjectMarshaller1 _enclosing, Transaction trans, 
				Db4objects.Db4o.Internal.Buffer reader, ObjectReference yo)
			{
				this._enclosing = _enclosing;
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

			private readonly ObjectMarshaller1 _enclosing;

			private readonly Transaction trans;

			private readonly Db4objects.Db4o.Internal.Buffer reader;

			private readonly ObjectReference yo;
		}

		protected override bool IsNull(ObjectHeaderAttributes attributes, int fieldIndex)
		{
			return ((ObjectHeaderAttributes1)attributes).IsNull(fieldIndex);
		}

		public override void DefragFields(ClassMetadata yc, ObjectHeader header, BufferPair
			 readers)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _TraverseFieldCommand_211(this
				, readers);
			TraverseFields(yc, null, header._headerAttributes, command);
		}

		private sealed class _TraverseFieldCommand_211 : ObjectMarshaller.TraverseFieldCommand
		{
			public _TraverseFieldCommand_211(ObjectMarshaller1 _enclosing, BufferPair readers
				)
			{
				this._enclosing = _enclosing;
				this.readers = readers;
			}

			public override int FieldCount(ClassMetadata yapClass, Db4objects.Db4o.Internal.Buffer
				 reader)
			{
				return readers.ReadInt();
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				if (!isNull)
				{
					field.DefragField(this._enclosing._family, readers);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly BufferPair readers;
		}

		public override void WriteObjectClassID(Db4objects.Db4o.Internal.Buffer reader, int
			 id)
		{
			reader.WriteInt(-id);
		}

		public override void SkipMarshallerInfo(Db4objects.Db4o.Internal.Buffer reader)
		{
			reader.IncrementOffset(1);
		}
	}
}
