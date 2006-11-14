namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public sealed class ObjectHeader
	{
		private readonly Db4objects.Db4o.YapClass _yapClass;

		public readonly Db4objects.Db4o.Inside.Marshall.MarshallerFamily _marshallerFamily;

		public readonly Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes _headerAttributes;

		public ObjectHeader(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapReader reader
			) : this(stream, null, reader)
		{
		}

		public ObjectHeader(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapReader 
			reader) : this(null, yapClass, reader)
		{
		}

		public ObjectHeader(Db4objects.Db4o.YapWriter writer) : this(writer.GetStream(), 
			writer)
		{
		}

		public ObjectHeader(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapClass yc
			, Db4objects.Db4o.YapReader reader)
		{
			int classID = reader.ReadInt();
			_marshallerFamily = ReadMarshallerFamily(reader, classID);
			classID = NormalizeID(classID);
			_yapClass = (yc != null ? yc : stream.GetYapClass(classID));
			_headerAttributes = ReadAttributes(_marshallerFamily, reader);
		}

		public static Db4objects.Db4o.Inside.Marshall.ObjectHeader Defrag(Db4objects.Db4o.ReaderPair
			 readers)
		{
			Db4objects.Db4o.YapReader source = readers.Source();
			Db4objects.Db4o.YapReader target = readers.Target();
			Db4objects.Db4o.Inside.Marshall.ObjectHeader header = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
				(readers.Context().SystemTrans().Stream(), null, source);
			int newID = readers.Mapping().MappedID(header.YapClass().GetID());
			header._marshallerFamily._object.WriteObjectClassID(target, newID);
			header._marshallerFamily._object.SkipMarshallerInfo(target);
			ReadAttributes(header._marshallerFamily, target);
			return header;
		}

		public Db4objects.Db4o.Inside.Marshall.ObjectMarshaller ObjectMarshaller()
		{
			return _marshallerFamily._object;
		}

		private Db4objects.Db4o.Inside.Marshall.MarshallerFamily ReadMarshallerFamily(Db4objects.Db4o.YapReader
			 reader, int classID)
		{
			bool marshallerAware = MarshallerAware(classID);
			byte marshallerVersion = 0;
			if (marshallerAware)
			{
				marshallerVersion = reader.ReadByte();
			}
			Db4objects.Db4o.Inside.Marshall.MarshallerFamily marshallerFamily = Db4objects.Db4o.Inside.Marshall.MarshallerFamily
				.Version(marshallerVersion);
			return marshallerFamily;
		}

		private static Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes ReadAttributes
			(Db4objects.Db4o.Inside.Marshall.MarshallerFamily marshallerFamily, Db4objects.Db4o.YapReader
			 reader)
		{
			return marshallerFamily._object.ReadHeaderAttributes(reader);
		}

		private bool MarshallerAware(int id)
		{
			return id < 0;
		}

		private int NormalizeID(int id)
		{
			return (id < 0 ? -id : id);
		}

		public Db4objects.Db4o.YapClass YapClass()
		{
			return _yapClass;
		}
	}
}
