namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class ClassMarshaller
	{
		public Db4objects.Db4o.Internal.Marshall.MarshallerFamily _family;

		public virtual Db4objects.Db4o.Internal.Marshall.RawClassSpec ReadSpec(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Buffer reader)
		{
			byte[] nameBytes = ReadName(trans, reader);
			string className = trans.Stream().StringIO().Read(nameBytes);
			ReadMetaClassID(reader);
			int ancestorID = reader.ReadInt();
			reader.IncrementOffset(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			int numFields = reader.ReadInt();
			return new Db4objects.Db4o.Internal.Marshall.RawClassSpec(className, ancestorID, 
				numFields);
		}

		public virtual void Write(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, Db4objects.Db4o.Internal.Buffer writer)
		{
			writer.WriteShortString(trans, clazz.NameToWrite());
			int intFormerlyKnownAsMetaClassID = 0;
			writer.WriteInt(intFormerlyKnownAsMetaClassID);
			writer.WriteIDOf(trans, clazz.i_ancestor);
			WriteIndex(trans, clazz, writer);
			Db4objects.Db4o.Internal.FieldMetadata[] fields = clazz.i_fields;
			if (fields == null)
			{
				writer.WriteInt(0);
				return;
			}
			writer.WriteInt(fields.Length);
			for (int i = 0; i < fields.Length; i++)
			{
				_family._field.Write(trans, clazz, fields[i], writer);
			}
		}

		protected virtual void WriteIndex(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, Db4objects.Db4o.Internal.Buffer writer)
		{
			int indexID = clazz.Index().Write(trans);
			writer.WriteInt(IndexIDForWriting(indexID));
		}

		protected abstract int IndexIDForWriting(int indexID);

		public virtual byte[] ReadName(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			byte[] name = ReadName(trans.Stream().StringIO(), reader);
			return name;
		}

		public virtual int ReadMetaClassID(Db4objects.Db4o.Internal.Buffer reader)
		{
			return reader.ReadInt();
		}

		private byte[] ReadName(Db4objects.Db4o.Internal.LatinStringIO sio, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			int len = reader.ReadInt();
			len = len * sio.BytesPerChar();
			byte[] nameBytes = new byte[len];
			System.Array.Copy(reader._buffer, reader._offset, nameBytes, 0, len);
			nameBytes = Db4objects.Db4o.Internal.Platform4.UpdateClassName(nameBytes);
			reader.IncrementOffset(len);
			return nameBytes;
		}

		public virtual void Read(Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, Db4objects.Db4o.Internal.Buffer reader)
		{
			clazz.i_ancestor = stream.GetYapClass(reader.ReadInt());
			if (clazz.i_dontCallConstructors)
			{
				clazz.CreateConstructor(stream, clazz.ClassReflector(), clazz.GetName(), true);
			}
			clazz.CheckType();
			ReadIndex(stream, clazz, reader);
			clazz.i_fields = CreateFields(clazz, reader.ReadInt());
			ReadFields(stream, reader, clazz.i_fields);
		}

		protected abstract void ReadIndex(Db4objects.Db4o.Internal.ObjectContainerBase stream
			, Db4objects.Db4o.Internal.ClassMetadata clazz, Db4objects.Db4o.Internal.Buffer 
			reader);

		private Db4objects.Db4o.Internal.FieldMetadata[] CreateFields(Db4objects.Db4o.Internal.ClassMetadata
			 clazz, int fieldCount)
		{
			Db4objects.Db4o.Internal.FieldMetadata[] fields = new Db4objects.Db4o.Internal.FieldMetadata
				[fieldCount];
			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = new Db4objects.Db4o.Internal.FieldMetadata(clazz);
				fields[i].SetArrayPosition(i);
			}
			return fields;
		}

		private void ReadFields(Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader, Db4objects.Db4o.Internal.FieldMetadata[] fields)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = _family._field.Read(stream, fields[i], reader);
			}
		}

		public virtual int MarshalledLength(Db4objects.Db4o.Internal.ObjectContainerBase 
			stream, Db4objects.Db4o.Internal.ClassMetadata clazz)
		{
			int len = stream.StringIO().ShortLength(clazz.NameToWrite()) + Db4objects.Db4o.Internal.Const4
				.OBJECT_LENGTH + (Db4objects.Db4o.Internal.Const4.INT_LENGTH * 2) + (Db4objects.Db4o.Internal.Const4
				.ID_LENGTH);
			len += clazz.Index().OwnLength();
			if (clazz.i_fields != null)
			{
				for (int i = 0; i < clazz.i_fields.Length; i++)
				{
					len += _family._field.MarshalledLength(stream, clazz.i_fields[i]);
				}
			}
			return len;
		}

		public virtual void Defrag(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.LatinStringIO
			 sio, Db4objects.Db4o.Internal.ReaderPair readers, int classIndexID)
		{
			ReadName(sio, readers.Source());
			ReadName(sio, readers.Target());
			int metaClassID = 0;
			readers.WriteInt(metaClassID);
			readers.CopyID();
			readers.WriteInt(IndexIDForWriting(classIndexID));
			readers.IncrementIntSize();
			Db4objects.Db4o.Internal.FieldMetadata[] fields = yapClass.i_fields;
			for (int fieldIdx = 0; fieldIdx < fields.Length; fieldIdx++)
			{
				_family._field.Defrag(yapClass, fields[fieldIdx], sio, readers);
			}
		}
	}
}
