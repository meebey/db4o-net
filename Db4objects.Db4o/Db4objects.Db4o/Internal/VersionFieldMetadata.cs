namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class VersionFieldMetadata : Db4objects.Db4o.Internal.VirtualFieldMetadata
	{
		internal VersionFieldMetadata(Db4objects.Db4o.Internal.ObjectContainerBase stream
			) : base()
		{
			i_name = Db4objects.Db4o.Ext.VirtualField.VERSION;
			i_handler = new Db4objects.Db4o.Internal.Handlers.LongHandler(stream);
		}

		public override void AddFieldIndex(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.StatefulBuffer
			 writer, Db4objects.Db4o.Internal.Slots.Slot oldSlot)
		{
			writer.WriteLong(writer.GetStream().GenerateTimeStampId());
		}

		public override void Delete(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool isUpdate)
		{
			a_bytes.IncrementOffset(LinkLength());
		}

		internal override void Instantiate1(Db4objects.Db4o.Internal.Transaction a_trans, 
			Db4objects.Db4o.Internal.ObjectReference a_yapObject, Db4objects.Db4o.Internal.Buffer
			 a_bytes)
		{
			a_yapObject.VirtualAttributes().i_version = a_bytes.ReadLong();
		}

		internal override void Marshall1(Db4objects.Db4o.Internal.ObjectReference a_yapObject
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool a_migrating, bool a_new)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase stream = a_bytes.GetStream().i_parent;
			Db4objects.Db4o.Internal.VirtualAttributes va = a_yapObject.VirtualAttributes();
			if (!a_migrating)
			{
				va.i_version = stream.GenerateTimeStampId();
			}
			if (va == null)
			{
				a_bytes.WriteLong(0);
			}
			else
			{
				a_bytes.WriteLong(va.i_version);
			}
		}

		public override int LinkLength()
		{
			return Db4objects.Db4o.Internal.Const4.LONG_LENGTH;
		}

		internal override void MarshallIgnore(Db4objects.Db4o.Internal.Buffer writer)
		{
			writer.WriteLong(0);
		}
	}
}
