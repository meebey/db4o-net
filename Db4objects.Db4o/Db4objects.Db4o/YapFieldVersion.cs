namespace Db4objects.Db4o
{
	internal class YapFieldVersion : Db4objects.Db4o.YapFieldVirtual
	{
		internal YapFieldVersion(Db4objects.Db4o.YapStream stream) : base()
		{
			i_name = Db4objects.Db4o.Ext.VirtualField.VERSION;
			i_handler = new Db4objects.Db4o.YLong(stream);
		}

		public override void AddFieldIndex(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapWriter writer, Db4objects.Db4o.Inside.Slots.Slot
			 oldSlot)
		{
			writer.WriteLong(writer.GetStream().GenerateTimeStampId());
		}

		public override void Delete(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, 
			Db4objects.Db4o.YapWriter a_bytes, bool isUpdate)
		{
			a_bytes.IncrementOffset(LinkLength());
		}

		internal override void Instantiate1(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapObject
			 a_yapObject, Db4objects.Db4o.YapReader a_bytes)
		{
			a_yapObject.i_virtualAttributes.i_version = a_bytes.ReadLong();
		}

		internal override void Marshall1(Db4objects.Db4o.YapObject a_yapObject, Db4objects.Db4o.YapWriter
			 a_bytes, bool a_migrating, bool a_new)
		{
			Db4objects.Db4o.YapStream stream = a_bytes.GetStream().i_parent;
			if (!a_migrating)
			{
				a_yapObject.i_virtualAttributes.i_version = stream.GenerateTimeStampId();
			}
			if (a_yapObject.i_virtualAttributes == null)
			{
				a_bytes.WriteLong(0);
			}
			else
			{
				a_bytes.WriteLong(a_yapObject.i_virtualAttributes.i_version);
			}
		}

		public override int LinkLength()
		{
			return Db4objects.Db4o.YapConst.LONG_LENGTH;
		}

		internal override void MarshallIgnore(Db4objects.Db4o.YapReader writer)
		{
			writer.WriteLong(0);
		}
	}
}
