namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class FieldMarshaller0 : Db4objects.Db4o.Internal.Marshall.IFieldMarshaller
	{
		public virtual int MarshalledLength(Db4objects.Db4o.Internal.ObjectContainerBase 
			stream, Db4objects.Db4o.Internal.FieldMetadata field)
		{
			int len = stream.StringIO().ShortLength(field.GetName());
			if (field.NeedsArrayAndPrimitiveInfo())
			{
				len += 1;
			}
			if (field.NeedsHandlerId())
			{
				len += Db4objects.Db4o.Internal.Const4.ID_LENGTH;
			}
			return len;
		}

		public virtual Db4objects.Db4o.Internal.Marshall.RawFieldSpec ReadSpec(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader)
		{
			string name = null;
			try
			{
				name = Db4objects.Db4o.Internal.Marshall.StringMarshaller.ReadShort(stream, reader
					);
			}
			catch (Db4objects.Db4o.CorruptionException)
			{
				return null;
			}
			if (name.IndexOf(Db4objects.Db4o.Internal.Const4.VIRTUAL_FIELD_PREFIX) == 0)
			{
				if (stream.i_handlers.VirtualFieldByName(name) != null)
				{
					return new Db4objects.Db4o.Internal.Marshall.RawFieldSpec(name);
				}
			}
			int handlerID = reader.ReadInt();
			byte attribs = reader.ReadByte();
			return new Db4objects.Db4o.Internal.Marshall.RawFieldSpec(name, handlerID, attribs
				);
		}

		public Db4objects.Db4o.Internal.FieldMetadata Read(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			Db4objects.Db4o.Internal.Marshall.RawFieldSpec spec = ReadSpec(stream, reader);
			return FromSpec(spec, stream, field);
		}

		protected virtual Db4objects.Db4o.Internal.FieldMetadata FromSpec(Db4objects.Db4o.Internal.Marshall.RawFieldSpec
			 spec, Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.FieldMetadata
			 field)
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

		public virtual void Write(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, Db4objects.Db4o.Internal.FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 writer)
		{
			field.Alive();
			writer.WriteShortString(trans, field.GetName());
			if (field.IsVirtual())
			{
				return;
			}
			Db4objects.Db4o.Internal.ITypeHandler4 handler = field.GetHandler();
			if (handler is Db4objects.Db4o.Internal.ClassMetadata)
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
			catch (System.Exception e)
			{
			}
			if (handlerID == 0)
			{
				handlerID = field.GetHandlerID();
			}
			writer.WriteInt(handlerID);
			Db4objects.Db4o.Foundation.BitMap4 bitmap = new Db4objects.Db4o.Foundation.BitMap4
				(3);
			bitmap.Set(0, field.IsPrimitive());
			bitmap.Set(1, handler is Db4objects.Db4o.Internal.Handlers.ArrayHandler);
			bitmap.Set(2, handler is Db4objects.Db4o.Internal.Handlers.MultidimensionalArrayHandler
				);
			writer.Append(bitmap.GetByte(0));
		}

		public virtual void Defrag(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.FieldMetadata
			 yapField, Db4objects.Db4o.Internal.LatinStringIO sio, Db4objects.Db4o.Internal.ReaderPair
			 readers)
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
