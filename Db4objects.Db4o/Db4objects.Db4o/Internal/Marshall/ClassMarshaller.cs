/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Sharpen;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class ClassMarshaller
	{
		public MarshallerFamily _family;

		public virtual RawClassSpec ReadSpec(Transaction trans, BufferImpl reader)
		{
			byte[] nameBytes = ReadName(trans, reader);
			string className = trans.Container().StringIO().Read(nameBytes);
			ReadMetaClassID(reader);
			// skip
			int ancestorID = reader.ReadInt();
			reader.IncrementOffset(Const4.IntLength);
			// index ID
			int numFields = reader.ReadInt();
			return new RawClassSpec(className, ancestorID, numFields);
		}

		public virtual void Write(Transaction trans, ClassMetadata clazz, BufferImpl writer
			)
		{
			writer.WriteShortString(trans, clazz.NameToWrite());
			int intFormerlyKnownAsMetaClassID = 0;
			writer.WriteInt(intFormerlyKnownAsMetaClassID);
			writer.WriteIDOf(trans, clazz.i_ancestor);
			WriteIndex(trans, clazz, writer);
			FieldMetadata[] fields = clazz.i_fields;
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

		protected virtual void WriteIndex(Transaction trans, ClassMetadata clazz, BufferImpl
			 writer)
		{
			int indexID = clazz.Index().Write(trans);
			writer.WriteInt(IndexIDForWriting(indexID));
		}

		protected abstract int IndexIDForWriting(int indexID);

		public virtual byte[] ReadName(Transaction trans, BufferImpl reader)
		{
			byte[] name = ReadName(trans.Container().StringIO(), reader);
			return name;
		}

		public virtual int ReadMetaClassID(BufferImpl reader)
		{
			return reader.ReadInt();
		}

		private byte[] ReadName(LatinStringIO sio, BufferImpl reader)
		{
			int len = reader.ReadInt();
			len = len * sio.BytesPerChar();
			byte[] nameBytes = new byte[len];
			System.Array.Copy(reader._buffer, reader._offset, nameBytes, 0, len);
			nameBytes = Platform4.UpdateClassName(nameBytes);
			reader.IncrementOffset(len);
			return nameBytes;
		}

		public void Read(ObjectContainerBase stream, ClassMetadata clazz, BufferImpl reader
			)
		{
			clazz.SetAncestor(stream.ClassMetadataForId(reader.ReadInt()));
			if (clazz.CallConstructor())
			{
				// The logic further down checks the ancestor YapClass, whether
				// or not it is allowed, not to call constructors. The ancestor
				// YapClass may possibly have not been loaded yet.
				clazz.CreateConstructor(stream, clazz.ClassReflector(), clazz.GetName(), true);
			}
			clazz.CheckType();
			ReadIndex(stream, clazz, reader);
			clazz.i_fields = CreateFields(clazz, reader.ReadInt());
			ReadFields(stream, reader, clazz.i_fields);
		}

		protected abstract void ReadIndex(ObjectContainerBase stream, ClassMetadata clazz
			, BufferImpl reader);

		private FieldMetadata[] CreateFields(ClassMetadata clazz, int fieldCount)
		{
			FieldMetadata[] fields = new FieldMetadata[fieldCount];
			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = new FieldMetadata(clazz);
				fields[i].SetArrayPosition(i);
			}
			return fields;
		}

		private void ReadFields(ObjectContainerBase stream, BufferImpl reader, FieldMetadata
			[] fields)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = _family._field.Read(stream, fields[i], reader);
			}
		}

		public virtual int MarshalledLength(ObjectContainerBase stream, ClassMetadata clazz
			)
		{
			int len = stream.StringIO().ShortLength(clazz.NameToWrite()) + Const4.ObjectLength
				 + (Const4.IntLength * 2) + (Const4.IdLength);
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

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public virtual void Defrag(ClassMetadata classMetadata, LatinStringIO sio, DefragmentContextImpl
			 context, int classIndexID)
		{
			ReadName(sio, context.SourceBuffer());
			ReadName(sio, context.TargetBuffer());
			int metaClassID = 0;
			context.WriteInt(metaClassID);
			// ancestor ID
			context.CopyID();
			context.WriteInt(IndexIDForWriting(classIndexID));
			// field length
			int numFields = context.ReadInt();
			FieldMetadata[] fields = classMetadata.i_fields;
			if (numFields > fields.Length)
			{
				throw new InvalidOperationException();
			}
			for (int fieldIdx = 0; fieldIdx < numFields; fieldIdx++)
			{
				_family._field.Defrag(classMetadata, fields[fieldIdx], sio, context);
			}
		}
	}
}
