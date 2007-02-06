namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ObjectMarshaller1 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller
	{
		public override void AddFieldIndices(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.StatefulBuffer writer, Db4objects.Db4o.Internal.Slots.Slot
			 oldSlot)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass15(this, writer, yc, oldSlot);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass15 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass15(ObjectMarshaller1 _enclosing, Db4objects.Db4o.Internal.StatefulBuffer
				 writer, Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Slots.Slot
				 oldSlot)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.yc = yc;
				this.oldSlot = oldSlot;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
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

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;

			private readonly Db4objects.Db4o.Internal.ClassMetadata yc;

			private readonly Db4objects.Db4o.Internal.Slots.Slot oldSlot;
		}

		public override Db4objects.Db4o.Internal.TreeInt CollectFieldIDs(Db4objects.Db4o.Internal.TreeInt
			 tree, Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.StatefulBuffer writer, string name)
		{
			Db4objects.Db4o.Internal.TreeInt[] ret = { tree };
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass30(this, name, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass30 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass30(ObjectMarshaller1 _enclosing, string name, Db4objects.Db4o.Internal.TreeInt[]
				 ret, Db4objects.Db4o.Internal.StatefulBuffer writer)
			{
				this._enclosing = _enclosing;
				this.name = name;
				this.ret = ret;
				this.writer = writer;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
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

			private readonly Db4objects.Db4o.Internal.TreeInt[] ret;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;
		}

		public override void DeleteMembers(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.StatefulBuffer writer, int type, bool isUpdate
			)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass48(this, writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass48 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass48(ObjectMarshaller1 _enclosing, Db4objects.Db4o.Internal.StatefulBuffer
				 writer, bool isUpdate)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.isUpdate = isUpdate;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
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

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;

			private readonly bool isUpdate;
		}

		public override bool FindOffset(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.FieldMetadata
			 field)
		{
			bool[] ret = { false };
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass62(this, field, ret, reader);
			TraverseFields(yc, reader, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass62 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass62(ObjectMarshaller1 _enclosing, Db4objects.Db4o.Internal.FieldMetadata
				 field, bool[] ret, Db4objects.Db4o.Internal.Buffer reader)
			{
				this._enclosing = _enclosing;
				this.field = field;
				this.ret = ret;
				this.reader = reader;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata curField
				, bool isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
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

			private readonly Db4objects.Db4o.Internal.FieldMetadata field;

			private readonly bool[] ret;

			private readonly Db4objects.Db4o.Internal.Buffer reader;
		}

		public override void InstantiateFields(Db4objects.Db4o.Internal.ClassMetadata yc, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.ObjectReference
			 yapObject, object onObject, Db4objects.Db4o.Internal.StatefulBuffer writer)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass79(this, onObject, yapObject, writer);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass79 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass79(ObjectMarshaller1 _enclosing, object onObject, Db4objects.Db4o.Internal.ObjectReference
				 yapObject, Db4objects.Db4o.Internal.StatefulBuffer writer)
			{
				this._enclosing = _enclosing;
				this.onObject = onObject;
				this.yapObject = yapObject;
				this.writer = writer;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				if (isNull)
				{
					field.Set(onObject, null);
					return;
				}
				try
				{
					field.Instantiate(this._enclosing._family, yapObject, onObject, writer);
				}
				catch (Db4objects.Db4o.CorruptionException)
				{
					this.Cancel();
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly object onObject;

			private readonly Db4objects.Db4o.Internal.ObjectReference yapObject;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;
		}

		private void Marshall(Db4objects.Db4o.Internal.ObjectReference yo, object obj, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes1
			 attributes, Db4objects.Db4o.Internal.StatefulBuffer writer, bool isNew)
		{
			Db4objects.Db4o.Internal.ClassMetadata yc = yo.GetYapClass();
			WriteObjectClassID(writer, yc.GetID());
			attributes.Write(writer);
			yc.CheckUpdateDepth(writer);
			Db4objects.Db4o.Internal.Transaction trans = writer.GetTransaction();
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass102(this, trans, writer, obj, yo, isNew);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass102 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass102(ObjectMarshaller1 _enclosing, Db4objects.Db4o.Internal.Transaction
				 trans, Db4objects.Db4o.Internal.StatefulBuffer writer, object obj, Db4objects.Db4o.Internal.ObjectReference
				 yo, bool isNew)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.writer = writer;
				this.obj = obj;
				this.yo = yo;
				this.isNew = isNew;
			}

			public override int FieldCount(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.Buffer
				 reader)
			{
				reader.WriteInt(yapClass.i_fields.Length);
				return yapClass.i_fields.Length;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				if (isNull)
				{
					field.AddIndexEntry(trans, writer.GetID(), null);
					return;
				}
				object child = field.GetOrCreate(trans, obj);
				if (child is Db4objects.Db4o.Internal.IDb4oTypeImpl)
				{
					child = ((Db4objects.Db4o.Internal.IDb4oTypeImpl)child).StoredTo(trans);
				}
				field.Marshall(yo, child, this._enclosing._family, writer, containingClass.ConfigOrAncestorConfig
					(), isNew);
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly Db4objects.Db4o.Internal.Transaction trans;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;

			private readonly object obj;

			private readonly Db4objects.Db4o.Internal.ObjectReference yo;

			private readonly bool isNew;
		}

		public override Db4objects.Db4o.Internal.StatefulBuffer MarshallNew(Db4objects.Db4o.Internal.Transaction
			 a_trans, Db4objects.Db4o.Internal.ObjectReference yo, int a_updateDepth)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes1 attributes = new Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes1
				(yo);
			Db4objects.Db4o.Internal.StatefulBuffer writer = CreateWriterForNew(a_trans, yo, 
				a_updateDepth, attributes.ObjectLength());
			Marshall(yo, yo.GetObject(), attributes, writer, true);
			return writer;
		}

		public override void MarshallUpdate(Db4objects.Db4o.Internal.Transaction trans, int
			 updateDepth, Db4objects.Db4o.Internal.ObjectReference yo, object obj)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes1 attributes = new Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes1
				(yo);
			Db4objects.Db4o.Internal.StatefulBuffer writer = CreateWriterForUpdate(trans, updateDepth
				, yo.GetID(), 0, attributes.ObjectLength());
			if (trans.i_file != null)
			{
				trans.i_file.GetSlotForUpdate(writer);
			}
			Marshall(yo, obj, attributes, writer, false);
			MarshallUpdateWrite(trans, yo, obj, writer);
		}

		public override Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes ReadHeaderAttributes
			(Db4objects.Db4o.Internal.Buffer reader)
		{
			return new Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes1(reader);
		}

		public override object ReadIndexEntry(Db4objects.Db4o.Internal.ClassMetadata yc, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.FieldMetadata
			 yf, Db4objects.Db4o.Internal.StatefulBuffer reader)
		{
			if (yc == null)
			{
				return null;
			}
			if (!FindOffset(yc, attributes, reader, yf))
			{
				return null;
			}
			return yf.ReadIndexEntry(_family, reader);
		}

		public override void ReadVirtualAttributes(Db4objects.Db4o.Internal.Transaction trans
			, Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.ObjectReference
			 yo, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass186(this, trans, reader, yo);
			TraverseFields(yc, reader, attributes, command);
		}

		private sealed class _AnonymousInnerClass186 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass186(ObjectMarshaller1 _enclosing, Db4objects.Db4o.Internal.Transaction
				 trans, Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.ObjectReference
				 yo)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.reader = reader;
				this.yo = yo;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				if (!isNull)
				{
					field.ReadVirtualAttribute(trans, reader, yo);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly Db4objects.Db4o.Internal.Transaction trans;

			private readonly Db4objects.Db4o.Internal.Buffer reader;

			private readonly Db4objects.Db4o.Internal.ObjectReference yo;
		}

		protected override bool IsNull(Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, int fieldIndex)
		{
			return ((Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes1)attributes).IsNull
				(fieldIndex);
		}

		public override void DefragFields(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeader
			 header, Db4objects.Db4o.Internal.ReaderPair readers)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass201(this, readers);
			TraverseFields(yc, null, header._headerAttributes, command);
		}

		private sealed class _AnonymousInnerClass201 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass201(ObjectMarshaller1 _enclosing, Db4objects.Db4o.Internal.ReaderPair
				 readers)
			{
				this._enclosing = _enclosing;
				this.readers = readers;
			}

			public override int FieldCount(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.Buffer
				 reader)
			{
				return readers.ReadInt();
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				if (!isNull)
				{
					field.DefragField(this._enclosing._family, readers);
				}
			}

			private readonly ObjectMarshaller1 _enclosing;

			private readonly Db4objects.Db4o.Internal.ReaderPair readers;
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
