namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	internal class ObjectMarshaller0 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller
	{
		public override void AddFieldIndices(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter writer, Db4objects.Db4o.Inside.Slots.Slot
			 oldSlot)
		{
			Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand command = new 
				_AnonymousInnerClass15(this, yc, writer, oldSlot);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass15 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass15(ObjectMarshaller0 _enclosing, Db4objects.Db4o.YapClass
				 yc, Db4objects.Db4o.YapWriter writer, Db4objects.Db4o.Inside.Slots.Slot oldSlot
				)
			{
				this._enclosing = _enclosing;
				this.yc = yc;
				this.writer = writer;
				this.oldSlot = oldSlot;
			}

			public override void ProcessField(Db4objects.Db4o.YapField field, bool isNull, Db4objects.Db4o.YapClass
				 containingClass)
			{
				field.AddFieldIndex(this._enclosing._family, yc, writer, oldSlot);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.YapClass yc;

			private readonly Db4objects.Db4o.YapWriter writer;

			private readonly Db4objects.Db4o.Inside.Slots.Slot oldSlot;
		}

		public override Db4objects.Db4o.TreeInt CollectFieldIDs(Db4objects.Db4o.TreeInt tree
			, Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter writer, string name)
		{
			Db4objects.Db4o.TreeInt[] ret = { tree };
			Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand command = new 
				_AnonymousInnerClass25(this, name, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass25 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass25(ObjectMarshaller0 _enclosing, string name, Db4objects.Db4o.TreeInt[]
				 ret, Db4objects.Db4o.YapWriter writer)
			{
				this._enclosing = _enclosing;
				this.name = name;
				this.ret = ret;
				this.writer = writer;
			}

			public override void ProcessField(Db4objects.Db4o.YapField field, bool isNull, Db4objects.Db4o.YapClass
				 containingClass)
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

			private readonly Db4objects.Db4o.TreeInt[] ret;

			private readonly Db4objects.Db4o.YapWriter writer;
		}

		public override void DeleteMembers(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter writer, int type, bool isUpdate)
		{
			Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand command = new 
				_AnonymousInnerClass39(this, writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass39 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass39(ObjectMarshaller0 _enclosing, Db4objects.Db4o.YapWriter
				 writer, bool isUpdate)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.isUpdate = isUpdate;
			}

			public override void ProcessField(Db4objects.Db4o.YapField field, bool isNull, Db4objects.Db4o.YapClass
				 containingClass)
			{
				field.Delete(this._enclosing._family, writer, isUpdate);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.YapWriter writer;

			private readonly bool isUpdate;
		}

		public override bool FindOffset(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapReader writer, Db4objects.Db4o.YapField field)
		{
			bool[] ret = { false };
			Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand command = new 
				_AnonymousInnerClass49(this, field, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass49 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass49(ObjectMarshaller0 _enclosing, Db4objects.Db4o.YapField
				 field, bool[] ret, Db4objects.Db4o.YapReader writer)
			{
				this._enclosing = _enclosing;
				this.field = field;
				this.ret = ret;
				this.writer = writer;
			}

			public override void ProcessField(Db4objects.Db4o.YapField curField, bool isNull, 
				Db4objects.Db4o.YapClass containingClass)
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

			private readonly Db4objects.Db4o.YapField field;

			private readonly bool[] ret;

			private readonly Db4objects.Db4o.YapReader writer;
		}

		protected int HeaderLength()
		{
			return Db4objects.Db4o.YapConst.OBJECT_LENGTH + Db4objects.Db4o.YapConst.ID_LENGTH;
		}

		public override void InstantiateFields(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapObject yapObject, object onObject, Db4objects.Db4o.YapWriter
			 writer)
		{
			Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand command = new 
				_AnonymousInnerClass68(this, yapObject, onObject, writer);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass68 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass68(ObjectMarshaller0 _enclosing, Db4objects.Db4o.YapObject
				 yapObject, object onObject, Db4objects.Db4o.YapWriter writer)
			{
				this._enclosing = _enclosing;
				this.yapObject = yapObject;
				this.onObject = onObject;
				this.writer = writer;
			}

			public override void ProcessField(Db4objects.Db4o.YapField field, bool isNull, Db4objects.Db4o.YapClass
				 containingClass)
			{
				try
				{
					field.Instantiate(this._enclosing._family, yapObject, onObject, writer);
				}
				catch (Db4objects.Db4o.CorruptionException e)
				{
					this.Cancel();
				}
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.YapObject yapObject;

			private readonly object onObject;

			private readonly Db4objects.Db4o.YapWriter writer;
		}

		private int LinkLength(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.YapObject yo)
		{
			int length = Db4objects.Db4o.YapConst.INT_LENGTH;
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

		protected virtual int LinkLength(Db4objects.Db4o.YapField yf, Db4objects.Db4o.YapObject
			 yo)
		{
			return yf.LinkLength();
		}

		private void Marshall(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapObject
			 a_yapObject, object a_object, Db4objects.Db4o.YapWriter writer, bool a_new)
		{
			MarshallDeclaredFields(yapClass, a_yapObject, a_object, writer, a_new);
		}

		private void MarshallDeclaredFields(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapObject
			 yapObject, object @object, Db4objects.Db4o.YapWriter writer, bool isNew)
		{
			Db4objects.Db4o.Config4Class config = yapClass.ConfigOrAncestorConfig();
			Db4objects.Db4o.Transaction trans = writer.GetTransaction();
			Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand command = new 
				_AnonymousInnerClass108(this, writer, trans, @object, yapObject, config, isNew);
			TraverseFields(yapClass, writer, ReadHeaderAttributes(writer), command);
		}

		private sealed class _AnonymousInnerClass108 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass108(ObjectMarshaller0 _enclosing, Db4objects.Db4o.YapWriter
				 writer, Db4objects.Db4o.Transaction trans, object @object, Db4objects.Db4o.YapObject
				 yapObject, Db4objects.Db4o.Config4Class config, bool isNew)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.trans = trans;
				this.@object = @object;
				this.yapObject = yapObject;
				this.config = config;
				this.isNew = isNew;
			}

			public override int FieldCount(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.YapReader
				 reader)
			{
				writer.WriteInt(yc.i_fields.Length);
				return yc.i_fields.Length;
			}

			public override void ProcessField(Db4objects.Db4o.YapField field, bool isNull, Db4objects.Db4o.YapClass
				 containingClass)
			{
				object obj = field.GetOrCreate(trans, @object);
				if (obj is Db4objects.Db4o.IDb4oTypeImpl)
				{
					obj = ((Db4objects.Db4o.IDb4oTypeImpl)obj).StoredTo(trans);
				}
				field.Marshall(yapObject, obj, this._enclosing._family, writer, config, isNew);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.YapWriter writer;

			private readonly Db4objects.Db4o.Transaction trans;

			private readonly object @object;

			private readonly Db4objects.Db4o.YapObject yapObject;

			private readonly Db4objects.Db4o.Config4Class config;

			private readonly bool isNew;
		}

		protected virtual int MarshalledLength(Db4objects.Db4o.YapField yf, Db4objects.Db4o.YapObject
			 yo)
		{
			return 0;
		}

		public override Db4objects.Db4o.YapWriter MarshallNew(Db4objects.Db4o.Transaction
			 a_trans, Db4objects.Db4o.YapObject yo, int a_updateDepth)
		{
			Db4objects.Db4o.YapWriter writer = CreateWriterForNew(a_trans, yo, a_updateDepth, 
				ObjectLength(yo));
			Db4objects.Db4o.YapClass yc = yo.GetYapClass();
			object obj = yo.GetObject();
			if (yc.IsPrimitive())
			{
				((Db4objects.Db4o.YapClassPrimitive)yc).i_handler.WriteNew(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
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

		public override void MarshallUpdate(Db4objects.Db4o.Transaction trans, int updateDepth
			, Db4objects.Db4o.YapObject yapObject, object obj)
		{
			Db4objects.Db4o.YapWriter writer = CreateWriterForUpdate(trans, updateDepth, yapObject
				.GetID(), 0, ObjectLength(yapObject));
			Db4objects.Db4o.YapClass yapClass = yapObject.GetYapClass();
			yapClass.CheckUpdateDepth(writer);
			writer.WriteInt(yapClass.GetID());
			Marshall(yapClass, yapObject, obj, writer, false);
			MarshallUpdateWrite(trans, yapObject, obj, writer);
		}

		private int ObjectLength(Db4objects.Db4o.YapObject yo)
		{
			return HeaderLength() + LinkLength(yo.GetYapClass(), yo);
		}

		public override Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes ReadHeaderAttributes
			(Db4objects.Db4o.YapReader reader)
		{
			return null;
		}

		public override object ReadIndexEntry(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapField yf, Db4objects.Db4o.YapWriter reader)
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

		public override void ReadVirtualAttributes(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass
			 yc, Db4objects.Db4o.YapObject yo, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapReader reader)
		{
			Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand command = new 
				_AnonymousInnerClass190(this, trans, reader, yo);
			TraverseFields(yc, reader, attributes, command);
		}

		private sealed class _AnonymousInnerClass190 : Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass190(ObjectMarshaller0 _enclosing, Db4objects.Db4o.Transaction
				 trans, Db4objects.Db4o.YapReader reader, Db4objects.Db4o.YapObject yo)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.reader = reader;
				this.yo = yo;
			}

			public override void ProcessField(Db4objects.Db4o.YapField field, bool isNull, Db4objects.Db4o.YapClass
				 containingClass)
			{
				field.ReadVirtualAttribute(trans, reader, yo);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Db4objects.Db4o.Transaction trans;

			private readonly Db4objects.Db4o.YapReader reader;

			private readonly Db4objects.Db4o.YapObject yo;
		}

		protected override bool IsNull(Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, int fieldIndex)
		{
			return false;
		}

		public override void DefragFields(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.Inside.Marshall.ObjectHeader
			 header, Db4objects.Db4o.ReaderPair readers)
		{
		}

		public override void WriteObjectClassID(Db4objects.Db4o.YapReader reader, int id)
		{
			reader.WriteInt(id);
		}

		public override void SkipMarshallerInfo(Db4objects.Db4o.YapReader reader)
		{
		}

		public override void MapStringIDs(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapReader reader, Db4objects.Db4o.IIDMapping mapping
			, int sourceBaseID, int targetBaseID)
		{
			throw new System.NotImplementedException();
		}
	}
}
