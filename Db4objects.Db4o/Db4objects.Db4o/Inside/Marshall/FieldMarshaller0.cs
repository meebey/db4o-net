namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public class FieldMarshaller0 : Db4objects.Db4o.Inside.Marshall.IFieldMarshaller
	{
		public virtual int MarshalledLength(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapField
			 field)
		{
			int len = stream.StringIO().ShortLength(field.GetName());
			if (field.NeedsArrayAndPrimitiveInfo())
			{
				len += 1;
			}
			if (field.NeedsHandlerId())
			{
				len += Db4objects.Db4o.YapConst.ID_LENGTH;
			}
			return len;
		}

		public virtual Db4objects.Db4o.Inside.Marshall.RawFieldSpec ReadSpec(Db4objects.Db4o.YapStream
			 stream, Db4objects.Db4o.YapReader reader)
		{
			string name = null;
			try
			{
				name = Db4objects.Db4o.Inside.Marshall.StringMarshaller.ReadShort(stream, reader);
			}
			catch (Db4objects.Db4o.CorruptionException ce)
			{
				return null;
			}
			if (name.IndexOf(Db4objects.Db4o.YapConst.VIRTUAL_FIELD_PREFIX) == 0)
			{
				Db4objects.Db4o.YapFieldVirtual[] virtuals = stream.i_handlers.i_virtualFields;
				for (int i = 0; i < virtuals.Length; i++)
				{
					if (name.Equals(virtuals[i].GetName()))
					{
						return new Db4objects.Db4o.Inside.Marshall.RawFieldSpec(name);
					}
				}
			}
			int handlerID = reader.ReadInt();
			byte attribs = reader.ReadByte();
			return new Db4objects.Db4o.Inside.Marshall.RawFieldSpec(name, handlerID, attribs);
		}

		public Db4objects.Db4o.YapField Read(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapField
			 field, Db4objects.Db4o.YapReader reader)
		{
			Db4objects.Db4o.Inside.Marshall.RawFieldSpec spec = ReadSpec(stream, reader);
			return FromSpec(spec, stream, field);
		}

		protected virtual Db4objects.Db4o.YapField FromSpec(Db4objects.Db4o.Inside.Marshall.RawFieldSpec
			 spec, Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapField field)
		{
			if (spec == null)
			{
				return field;
			}
			string name = spec.Name();
			if (spec.IsVirtual())
			{
				Db4objects.Db4o.YapFieldVirtual[] virtuals = stream.i_handlers.i_virtualFields;
				for (int i = 0; i < virtuals.Length; i++)
				{
					if (name.Equals(virtuals[i].GetName()))
					{
						return virtuals[i];
					}
				}
			}
			field.Init(field.GetParentYapClass(), name);
			field.Init(spec.HandlerID(), spec.IsPrimitive(), spec.IsArray(), spec.IsNArray());
			field.LoadHandler(stream);
			return field;
		}

		public virtual void Write(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass
			 clazz, Db4objects.Db4o.YapField field, Db4objects.Db4o.YapReader writer)
		{
			field.Alive();
			writer.WriteShortString(trans, field.GetName());
			if (field.IsVirtual())
			{
				return;
			}
			Db4objects.Db4o.ITypeHandler4 handler = field.GetHandler();
			if (handler is Db4objects.Db4o.YapClass)
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
			Db4objects.Db4o.YapBit yb = new Db4objects.Db4o.YapBit(0);
			yb.Set(handler is Db4objects.Db4o.YapArrayN);
			yb.Set(handler is Db4objects.Db4o.YapArray);
			yb.Set(field.IsPrimitive());
			writer.Append(yb.GetByte());
		}

		public virtual void Defrag(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapField
			 yapField, Db4objects.Db4o.YapStringIO sio, Db4objects.Db4o.ReaderPair readers)
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
