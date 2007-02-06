namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public sealed class ObjectHeader
	{
		private readonly Db4objects.Db4o.Internal.ClassMetadata _yapClass;

		public readonly Db4objects.Db4o.Internal.Marshall.MarshallerFamily _marshallerFamily;

		public readonly Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes _headerAttributes;

		public ObjectHeader(Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader) : this(stream, null, reader)
		{
		}

		public ObjectHeader(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.Buffer
			 reader) : this(null, yapClass, reader)
		{
		}

		public ObjectHeader(Db4objects.Db4o.Internal.StatefulBuffer writer) : this(writer
			.GetStream(), writer)
		{
		}

		public ObjectHeader(Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.ClassMetadata
			 yc, Db4objects.Db4o.Internal.Buffer reader)
		{
			int classID = reader.ReadInt();
			_marshallerFamily = ReadMarshallerFamily(reader, classID);
			classID = NormalizeID(classID);
			_yapClass = (yc != null ? yc : stream.GetYapClass(classID));
			_headerAttributes = ReadAttributes(_marshallerFamily, reader);
		}

		public static Db4objects.Db4o.Internal.Marshall.ObjectHeader Defrag(Db4objects.Db4o.Internal.ReaderPair
			 readers)
		{
			Db4objects.Db4o.Internal.Buffer source = readers.Source();
			Db4objects.Db4o.Internal.Buffer target = readers.Target();
			Db4objects.Db4o.Internal.Marshall.ObjectHeader header = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
				(readers.Context().SystemTrans().Stream(), null, source);
			int newID = readers.Mapping().MappedID(header.YapClass().GetID());
			header._marshallerFamily._object.WriteObjectClassID(target, newID);
			header._marshallerFamily._object.SkipMarshallerInfo(target);
			ReadAttributes(header._marshallerFamily, target);
			return header;
		}

		public Db4objects.Db4o.Internal.Marshall.ObjectMarshaller ObjectMarshaller()
		{
			return _marshallerFamily._object;
		}

		private Db4objects.Db4o.Internal.Marshall.MarshallerFamily ReadMarshallerFamily(Db4objects.Db4o.Internal.Buffer
			 reader, int classID)
		{
			bool marshallerAware = MarshallerAware(classID);
			byte marshallerVersion = 0;
			if (marshallerAware)
			{
				marshallerVersion = reader.ReadByte();
			}
			Db4objects.Db4o.Internal.Marshall.MarshallerFamily marshallerFamily = Db4objects.Db4o.Internal.Marshall.MarshallerFamily
				.Version(marshallerVersion);
			return marshallerFamily;
		}

		private static Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes ReadAttributes
			(Db4objects.Db4o.Internal.Marshall.MarshallerFamily marshallerFamily, Db4objects.Db4o.Internal.Buffer
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

		public Db4objects.Db4o.Internal.ClassMetadata YapClass()
		{
			return _yapClass;
		}
	}
}
