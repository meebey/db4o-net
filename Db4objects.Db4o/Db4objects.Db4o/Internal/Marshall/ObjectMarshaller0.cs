using System.IO;
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
			ObjectMarshaller.TraverseFieldCommand command = new _AnonymousInnerClass17(this, 
				yc, writer, oldSlot);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass17 : ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass17(ObjectMarshaller0 _enclosing, ClassMetadata yc, StatefulBuffer
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

		public override TreeInt CollectFieldIDs(TreeInt tree, ClassMetadata yc, ObjectHeaderAttributes
			 attributes, StatefulBuffer writer, string name)
		{
			TreeInt[] ret = new TreeInt[] { tree };
			ObjectMarshaller.TraverseFieldCommand command = new _AnonymousInnerClass27(this, 
				name, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass27 : ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass27(ObjectMarshaller0 _enclosing, string name, TreeInt[]
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

			private readonly TreeInt[] ret;

			private readonly StatefulBuffer writer;
		}

		public override void DeleteMembers(ClassMetadata yc, ObjectHeaderAttributes attributes
			, StatefulBuffer writer, int type, bool isUpdate)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _AnonymousInnerClass41(this, 
				writer, isUpdate);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass41 : ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass41(ObjectMarshaller0 _enclosing, StatefulBuffer writer
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

		public override bool FindOffset(ClassMetadata yc, ObjectHeaderAttributes attributes
			, Db4objects.Db4o.Internal.Buffer writer, FieldMetadata field)
		{
			bool[] ret = new bool[] { false };
			ObjectMarshaller.TraverseFieldCommand command = new _AnonymousInnerClass51(this, 
				field, ret, writer);
			TraverseFields(yc, writer, attributes, command);
			return ret[0];
		}

		private sealed class _AnonymousInnerClass51 : ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass51(ObjectMarshaller0 _enclosing, FieldMetadata field, 
				bool[] ret, Db4objects.Db4o.Internal.Buffer writer)
			{
				this._enclosing = _enclosing;
				this.field = field;
				this.ret = ret;
				this.writer = writer;
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
				writer.IncrementOffset(curField.LinkLength());
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly FieldMetadata field;

			private readonly bool[] ret;

			private readonly Db4objects.Db4o.Internal.Buffer writer;
		}

		protected int HeaderLength()
		{
			return Const4.OBJECT_LENGTH + Const4.ID_LENGTH;
		}

		public override void InstantiateFields(ClassMetadata yc, ObjectHeaderAttributes attributes
			, ObjectReference yapObject, object onObject, StatefulBuffer writer)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _AnonymousInnerClass70(this, 
				yapObject, onObject, writer);
			TraverseFields(yc, writer, attributes, command);
		}

		private sealed class _AnonymousInnerClass70 : ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass70(ObjectMarshaller0 _enclosing, ObjectReference yapObject
				, object onObject, StatefulBuffer writer)
			{
				this._enclosing = _enclosing;
				this.yapObject = yapObject;
				this.onObject = onObject;
				this.writer = writer;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				try
				{
					field.Instantiate(this._enclosing._family, yapObject, onObject, writer);
				}
				catch (CorruptionException)
				{
					this.Cancel();
				}
				catch (IOException)
				{
					this.Cancel();
				}
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly ObjectReference yapObject;

			private readonly object onObject;

			private readonly StatefulBuffer writer;
		}

		private int LinkLength(ClassMetadata yc, ObjectReference yo)
		{
			int length = Const4.INT_LENGTH;
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

		protected virtual int LinkLength(FieldMetadata yf, ObjectReference yo)
		{
			return yf.LinkLength();
		}

		private void Marshall(ClassMetadata yapClass, ObjectReference a_yapObject, object
			 a_object, StatefulBuffer writer, bool a_new)
		{
			MarshallDeclaredFields(yapClass, a_yapObject, a_object, writer, a_new);
		}

		private void MarshallDeclaredFields(ClassMetadata yapClass, ObjectReference yapObject
			, object @object, StatefulBuffer writer, bool isNew)
		{
			Config4Class config = yapClass.ConfigOrAncestorConfig();
			Transaction trans = writer.GetTransaction();
			ObjectMarshaller.TraverseFieldCommand command = new _AnonymousInnerClass114(this, 
				writer, trans, @object, yapObject, config, isNew);
			TraverseFields(yapClass, writer, ReadHeaderAttributes(writer), command);
		}

		private sealed class _AnonymousInnerClass114 : ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass114(ObjectMarshaller0 _enclosing, StatefulBuffer writer
				, Transaction trans, object @object, ObjectReference yapObject, Config4Class config
				, bool isNew)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
				this.trans = trans;
				this.@object = @object;
				this.yapObject = yapObject;
				this.config = config;
				this.isNew = isNew;
			}

			public override int FieldCount(ClassMetadata yc, Db4objects.Db4o.Internal.Buffer 
				reader)
			{
				writer.WriteInt(yc.i_fields.Length);
				return yc.i_fields.Length;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				object obj = field.GetOrCreate(trans, @object);
				if (obj is IDb4oTypeImpl)
				{
					obj = ((IDb4oTypeImpl)obj).StoredTo(trans);
				}
				field.Marshall(yapObject, obj, this._enclosing._family, writer, config, isNew);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly StatefulBuffer writer;

			private readonly Transaction trans;

			private readonly object @object;

			private readonly ObjectReference yapObject;

			private readonly Config4Class config;

			private readonly bool isNew;
		}

		protected virtual int MarshalledLength(FieldMetadata yf, ObjectReference yo)
		{
			return 0;
		}

		public override StatefulBuffer MarshallNew(Transaction a_trans, ObjectReference yo
			, int a_updateDepth)
		{
			StatefulBuffer writer = CreateWriterForNew(a_trans, yo, a_updateDepth, ObjectLength
				(yo));
			ClassMetadata yc = yo.GetYapClass();
			object obj = yo.GetObject();
			if (yc.IsPrimitive())
			{
				((PrimitiveFieldHandler)yc).i_handler.WriteNew(MarshallerFamily.Current(), obj, false
					, writer, true, false);
			}
			else
			{
				WriteObjectClassID(writer, yc.GetID());
				yc.CheckUpdateDepth(writer);
				Marshall(yc, yo, obj, writer, true);
			}
			return writer;
		}

		public override void MarshallUpdate(Transaction trans, int updateDepth, ObjectReference
			 yapObject, object obj)
		{
			StatefulBuffer writer = CreateWriterForUpdate(trans, updateDepth, yapObject.GetID
				(), 0, ObjectLength(yapObject));
			ClassMetadata yapClass = yapObject.GetYapClass();
			yapClass.CheckUpdateDepth(writer);
			writer.WriteInt(yapClass.GetID());
			Marshall(yapClass, yapObject, obj, writer, false);
			MarshallUpdateWrite(trans, yapObject, obj, writer);
		}

		private int ObjectLength(ObjectReference yo)
		{
			return HeaderLength() + LinkLength(yo.GetYapClass(), yo);
		}

		public override ObjectHeaderAttributes ReadHeaderAttributes(Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			return null;
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
			catch (IOException exc)
			{
				throw new FieldIndexException(exc, field);
			}
		}

		public override void ReadVirtualAttributes(Transaction trans, ClassMetadata yc, ObjectReference
			 yo, ObjectHeaderAttributes attributes, Db4objects.Db4o.Internal.Buffer reader)
		{
			ObjectMarshaller.TraverseFieldCommand command = new _AnonymousInnerClass202(this, 
				trans, reader, yo);
			TraverseFields(yc, reader, attributes, command);
		}

		private sealed class _AnonymousInnerClass202 : ObjectMarshaller.TraverseFieldCommand
		{
			public _AnonymousInnerClass202(ObjectMarshaller0 _enclosing, Transaction trans, Db4objects.Db4o.Internal.Buffer
				 reader, ObjectReference yo)
			{
				this._enclosing = _enclosing;
				this.trans = trans;
				this.reader = reader;
				this.yo = yo;
			}

			public override void ProcessField(FieldMetadata field, bool isNull, ClassMetadata
				 containingClass)
			{
				field.ReadVirtualAttribute(trans, reader, yo);
			}

			private readonly ObjectMarshaller0 _enclosing;

			private readonly Transaction trans;

			private readonly Db4objects.Db4o.Internal.Buffer reader;

			private readonly ObjectReference yo;
		}

		protected override bool IsNull(ObjectHeaderAttributes attributes, int fieldIndex)
		{
			return false;
		}

		public override void DefragFields(ClassMetadata yapClass, ObjectHeader header, ReaderPair
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
