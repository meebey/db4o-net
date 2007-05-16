/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class FieldMarshaller0 : IFieldMarshaller
	{
		public virtual int MarshalledLength(ObjectContainerBase stream, FieldMetadata field
			)
		{
			int len = stream.StringIO().ShortLength(field.GetName());
			if (field.NeedsArrayAndPrimitiveInfo())
			{
				len += 1;
			}
			if (field.NeedsHandlerId())
			{
				len += Const4.ID_LENGTH;
			}
			return len;
		}

		public virtual RawFieldSpec ReadSpec(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			string name = null;
			try
			{
				name = StringMarshaller.ReadShort(stream, reader);
			}
			catch (CorruptionException)
			{
				return null;
			}
			if (name.IndexOf(Const4.VIRTUAL_FIELD_PREFIX) == 0)
			{
				if (stream.i_handlers.VirtualFieldByName(name) != null)
				{
					return new RawFieldSpec(name);
				}
			}
			int handlerID = reader.ReadInt();
			byte attribs = reader.ReadByte();
			return new RawFieldSpec(name, handlerID, attribs);
		}

		public FieldMetadata Read(ObjectContainerBase stream, FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			RawFieldSpec spec = ReadSpec(stream, reader);
			return FromSpec(spec, stream, field);
		}

		protected virtual FieldMetadata FromSpec(RawFieldSpec spec, ObjectContainerBase stream
			, FieldMetadata field)
		{
			if (spec == null)
			{
				return field;
			}
			string name = spec.Name();
			if (spec.IsVirtual())
			{
				return stream.i_handlers.VirtualFieldByName(name);
			}
			field.Init(field.GetParentYapClass(), name);
			field.Init(spec.HandlerID(), spec.IsPrimitive(), spec.IsArray(), spec.IsNArray());
			field.LoadHandler(stream);
			field.Alive();
			return field;
		}

		public virtual void Write(Transaction trans, ClassMetadata clazz, FieldMetadata field
			, Db4objects.Db4o.Internal.Buffer writer)
		{
			field.Alive();
			writer.WriteShortString(trans, field.GetName());
			if (field.IsVirtual())
			{
				return;
			}
			ITypeHandler4 handler = field.GetHandler();
			if (handler is ClassMetadata)
			{
				if (handler.GetID() == 0)
				{
					trans.Stream().NeedsUpdate(clazz);
				}
			}
			int handlerID = 0;
			try
			{
				handlerID = handler.GetID();
			}
			catch (Exception e)
			{
			}
			if (handlerID == 0)
			{
				handlerID = field.GetHandlerID();
			}
			writer.WriteInt(handlerID);
			BitMap4 bitmap = new BitMap4(3);
			bitmap.Set(0, field.IsPrimitive());
			bitmap.Set(1, handler is ArrayHandler);
			bitmap.Set(2, handler is MultidimensionalArrayHandler);
			writer.Append(bitmap.GetByte(0));
		}

		public virtual void Defrag(ClassMetadata yapClass, FieldMetadata yapField, LatinStringIO
			 sio, ReaderPair readers)
		{
			readers.ReadShortString(sio);
			if (yapField.IsVirtual())
			{
				return;
			}
			readers.CopyID();
			readers.IncrementOffset(1);
		}
	}
}
