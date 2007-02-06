namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	internal class ObjectMarshaller0 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller
	{
		public override void AddFieldIndices(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.StatefulBuffer writer, Db4objects.Db4o.Internal.Slots.Slot
			 oldSlot)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass15(this, yc, writer, oldSlot);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass15 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass15(ObjectMarshaller0 _enclosing, Db4objects.Db4o.Internal.ClassMetadata
				 yc, Db4objects.Db4o.Internal.StatefulBuffer writer, Db4objects.Db4o.Internal.Slots.Slot
				 oldSlot)
			{
				this._enclosing = _enclosing;
				this.yc = yc;
				this.writer = writer;
				this.oldSlot = oldSlot;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				field.AddFieldIndex(this._enclosing._family, yc, writer, oldSlot);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.Internal.ClassMetadata yc;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;

			private readonly Db4objects.Db4o.Internal.Slots.Slot oldSlot;
		}

		public override Db4objects.Db4o.Internal.TreeInt CollectFieldIDs(Db4objects.Db4o.Internal.TreeInt
			 tree, Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.StatefulBuffer writer, string name)
		{
			Db4objects.Db4o.Internal.TreeInt[] ret = { tree };
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass25(this, name, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass25 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass25(ObjectMarshaller0 _enclosing, string name, Db4objects.Db4o.Internal.TreeInt[]
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
				if (name.Equals(field.GetName()))
				{
					ret[0] = field.CollectIDs(this._enclosing._family, ret[0], writer);
				}
				else
				{
					field.IncrementOffset(writer);
				}
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly string name;

			private readonly Db4objects.Db4o.Internal.TreeInt[] ret;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;
		}

		public override void DeleteMembers(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.StatefulBuffer writer, int type, bool isUpdate
			)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass39(this, writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass39 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass39(ObjectMarshaller0 _enclosing, Db4objects.Db4o.Internal.StatefulBuffer
				 writer, bool isUpdate)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.isUpdate = isUpdate;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				field.Delete(this._enclosing._family, writer, isUpdate);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;

			private readonly bool isUpdate;
		}

		public override bool FindOffset(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.Internal.Buffer writer, Db4objects.Db4o.Internal.FieldMetadata
			 field)
		{
			bool[] ret = { false };
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass49(this, field, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass49 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass49(ObjectMarshaller0 _enclosing, Db4objects.Db4o.Internal.FieldMetadata
				 field, bool[] ret, Db4objects.Db4o.Internal.Buffer writer)
			{
				this._enclosing = _enclosing;
				this.field = field;
				this.ret = ret;
				this.writer = writer;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata curField
				, bool isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				if (curField == field)
				{
					ret[0] = true;
					this.Cancel();
					return;
				}
				writer.IncrementOffset(curField.LinkLength());
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.Internal.FieldMetadata field;

			private readonly bool[] ret;

			private readonly Db4objects.Db4o.Internal.Buffer writer;
		}

		protected int HeaderLength()
		{
			return Db4objects.Db4o.Internal.Const4.OBJECT_LENGTH + Db4objects.Db4o.Internal.Const4
				.ID_LENGTH;
		}

		public override void InstantiateFields(Db4objects.Db4o.Internal.ClassMetadata yc, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.ObjectReference
			 yapObject, object onObject, Db4objects.Db4o.Internal.StatefulBuffer writer)
		{
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass68(this, yapObject, onObject, writer);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass68 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass68(ObjectMarshaller0 _enclosing, Db4objects.Db4o.Internal.ObjectReference
				 yapObject, object onObject, Db4objects.Db4o.Internal.StatefulBuffer writer)
			{
				this._enclosing = _enclosing;
				this.yapObject = yapObject;
				this.onObject = onObject;
				this.writer = writer;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				try
				{
					field.Instantiate(this._enclosing._family, yapObject, onObject, writer);
				}
				catch (Db4objects.Db4o.CorruptionException)
				{
					this.Cancel();
				}
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.Internal.ObjectReference yapObject;

			private readonly object onObject;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;
		}

		private int LinkLength(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.ObjectReference
			 yo)
		{
			int length = Db4objects.Db4o.Internal.Const4.INT_LENGTH;
			if (yc.i_fields != null)
			{
				for (int i = 0; i < yc.i_fields.Length; i++)
				{
					length += LinkLength(yc.i_fields[i], yo);
				}
			}
			if (yc.i_ancestor != null)
			{
				length += LinkLength(yc.i_ancestor, yo);
			}
			return length;
		}

		protected virtual int LinkLength(Db4objects.Db4o.Internal.FieldMetadata yf, Db4objects.Db4o.Internal.ObjectReference
			 yo)
		{
			return yf.LinkLength();
		}

		private void Marshall(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.ObjectReference
			 a_yapObject, object a_object, Db4objects.Db4o.Internal.StatefulBuffer writer, bool
			 a_new)
		{
			MarshallDeclaredFields(yapClass, a_yapObject, a_object, writer, a_new);
		}

		private void MarshallDeclaredFields(Db4objects.Db4o.Internal.ClassMetadata yapClass
			, Db4objects.Db4o.Internal.ObjectReference yapObject, object @object, Db4objects.Db4o.Internal.StatefulBuffer
			 writer, bool isNew)
		{
			Db4objects.Db4o.Internal.Config4Class config = yapClass.ConfigOrAncestorConfig();
			Db4objects.Db4o.Internal.Transaction trans = writer.GetTransaction();
			Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand command = 
				new _AnonymousInnerClass108(this, writer, trans, @object, yapObject, config, isNew
				);
			TraverseFields(yapClass, writer, ReadHeaderAttributes(writer), command);
		}

		private sealed class _AnonymousInnerClass108 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass108(ObjectMarshaller0 _enclosing, Db4objects.Db4o.Internal.StatefulBuffer
				 writer, Db4objects.Db4o.Internal.Transaction trans, object @object, Db4objects.Db4o.Internal.ObjectReference
				 yapObject, Db4objects.Db4o.Internal.Config4Class config, bool isNew)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.trans = trans;
				this.@object = @object;
				this.yapObject = yapObject;
				this.config = config;
				this.isNew = isNew;
			}

			public override int FieldCount(Db4objects.Db4o.Internal.ClassMetadata yc, Db4objects.Db4o.Internal.Buffer
				 reader)
			{
				writer.WriteInt(yc.i_fields.Length);
				return yc.i_fields.Length;
			}

			public override void ProcessField(Db4objects.Db4o.Internal.FieldMetadata field, bool
				 isNull, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				object obj = field.GetOrCreate(trans, @object);
				if (obj is Db4objects.Db4o.Internal.IDb4oTypeImpl)
				{
					obj = ((Db4objects.Db4o.Internal.IDb4oTypeImpl)obj).StoredTo(trans);
				}
				field.Marshall(yapObject, obj, this._enclosing._family, writer, config, isNew);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.Internal.StatefulBuffer writer;

			private readonly Db4objects.Db4o.Internal.Transaction trans;

			private readonly object @object;

			private readonly Db4objects.Db4o.Internal.ObjectReference yapObject;

			private readonly Db4objects.Db4o.Internal.Config4Class config;

			private readonly bool isNew;
		}

		protected virtual int MarshalledLength(Db4objects.Db4o.Internal.FieldMetadata yf, 
			Db4objects.Db4o.Internal.ObjectReference yo)
		{
			return 0;
		}

		public override Db4objects.Db4o.Internal.StatefulBuffer MarshallNew(Db4objects.Db4o.Internal.Transaction
			 a_trans, Db4objects.Db4o.Internal.ObjectReference yo, int a_updateDepth)
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = CreateWriterForNew(a_trans, yo, 
				a_updateDepth, ObjectLength(yo));
			Db4objects.Db4o.Internal.ClassMetadata yc = yo.GetYapClass();
			object obj = yo.GetObject();
			if (yc.IsPrimitive())
			{
				((Db4objects.Db4o.Internal.PrimitiveFieldHandler)yc).i_handler.WriteNew(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
					.Current(), obj, false, writer, true, false);
			}
			else
			{
				WriteObjectClassID(writer, yc.GetID());
				yc.CheckUpdateDepth(writer);
				Marshall(yc, yo, obj, writer, true);
			}
			return writer;
		}

		public override void MarshallUpdate(Db4objects.Db4o.Internal.Transaction trans, int
			 updateDepth, Db4objects.Db4o.Internal.ObjectReference yapObject, object obj)
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = CreateWriterForUpdate(trans, updateDepth
				, yapObject.GetID(), 0, ObjectLength(yapObject));
			Db4objects.Db4o.Internal.ClassMetadata yapClass = yapObject.GetYapClass();
			yapClass.CheckUpdateDepth(writer);
			writer.WriteInt(yapClass.GetID());
			Marshall(yapClass, yapObject, obj, writer, false);
			MarshallUpdateWrite(trans, yapObject, obj, writer);
		}

		private int ObjectLength(Db4objects.Db4o.Internal.ObjectReference yo)
		{
			return HeaderLength() + LinkLength(yo.GetYapClass(), yo);
		}

		public override Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes ReadHeaderAttributes
			(Db4objects.Db4o.Internal.Buffer reader)
		{
			return null;
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
				new _AnonymousInnerClass190(this, trans, reader, yo);
			TraverseFields(yc, reader, attributes, command);
		}

		private sealed class _AnonymousInnerClass190 : Db4objects.Db4o.Internal.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass190(ObjectMarshaller0 _enclosing, Db4objects.Db4o.Internal.Transaction
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
				field.ReadVirtualAttribute(trans, reader, yo);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.Internal.Transaction trans;

			private readonly Db4objects.Db4o.Internal.Buffer reader;

			private readonly Db4objects.Db4o.Internal.ObjectReference yo;
		}

		protected override bool IsNull(Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes
			 attributes, int fieldIndex)
		{
			return false;
		}

		public override void DefragFields(Db4objects.Db4o.Internal.ClassMetadata yapClass
			, Db4objects.Db4o.Internal.Marshall.ObjectHeader header, Db4objects.Db4o.Internal.ReaderPair
			 readers)
		{
		}

		public override void WriteObjectClassID(Db4objects.Db4o.Internal.Buffer reader, int
			 id)
		{
			reader.WriteInt(id);
		}

		public override void SkipMarshallerInfo(Db4objects.Db4o.Internal.Buffer reader)
		{
		}
	}
}
