/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public sealed class ObjectHeader
	{
		private readonly Db4objects.Db4o.Internal.ClassMetadata _classMetadata;

		public readonly MarshallerFamily _marshallerFamily;

		public readonly ObjectHeaderAttributes _headerAttributes;

		private int _handlerVersion;

		public ObjectHeader(ObjectContainerBase container, IReadWriteBuffer reader) : this
			(container, null, reader)
		{
		}

		public ObjectHeader(Db4objects.Db4o.Internal.ClassMetadata yapClass, IReadWriteBuffer
			 reader) : this(null, yapClass, reader)
		{
		}

		public ObjectHeader(StatefulBuffer writer) : this(writer.Container(), writer)
		{
		}

		public ObjectHeader(ObjectContainerBase stream, Db4objects.Db4o.Internal.ClassMetadata
			 yc, IReadWriteBuffer reader)
		{
			int classID = reader.ReadInt();
			_marshallerFamily = ReadMarshallerFamily(reader, classID);
			classID = NormalizeID(classID);
			_classMetadata = (yc != null ? yc : stream.ClassMetadataForId(classID));
			// This check has been added to cope with defragment in debug mode: SlotDefragment#setIdentity()
			// will trigger calling this constructor with a source db yap class and a target db stream,
			// thus _yapClass==null. There may be a better solution, since this call is just meant to
			// skip the object header.
			_headerAttributes = SlotFormat().ReadHeaderAttributes((ByteArrayBuffer)reader);
		}

		public static Db4objects.Db4o.Internal.Marshall.ObjectHeader Defrag(DefragmentContextImpl
			 context)
		{
			ByteArrayBuffer source = context.SourceBuffer();
			ByteArrayBuffer target = context.TargetBuffer();
			Db4objects.Db4o.Internal.Marshall.ObjectHeader header = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
				(context.Services().SystemTrans().Container(), null, source);
			int newID = context.Mapping().MappedID(header.ClassMetadata().GetID());
			Db4objects.Db4o.Internal.Marshall.SlotFormat slotFormat = header.SlotFormat();
			slotFormat.WriteObjectClassID(target, newID);
			slotFormat.SkipMarshallerInfo(target);
			slotFormat.ReadHeaderAttributes(target);
			return header;
		}

		private Db4objects.Db4o.Internal.Marshall.SlotFormat SlotFormat()
		{
			return Db4objects.Db4o.Internal.Marshall.SlotFormat.ForHandlerVersion(HandlerVersion
				());
		}

		private MarshallerFamily ReadMarshallerFamily(IReadWriteBuffer reader, int classID
			)
		{
			bool marshallerAware = MarshallerAware(classID);
			_handlerVersion = 0;
			if (marshallerAware)
			{
				_handlerVersion = reader.ReadByte();
			}
			MarshallerFamily marshallerFamily = MarshallerFamily.Version(_handlerVersion);
			return marshallerFamily;
		}

		private bool MarshallerAware(int id)
		{
			return id < 0;
		}

		private int NormalizeID(int id)
		{
			return (id < 0 ? -id : id);
		}

		public Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _classMetadata;
		}

		public int HandlerVersion()
		{
			return _handlerVersion;
		}

		public static Db4objects.Db4o.Internal.Marshall.ObjectHeader ScrollBufferToContent
			(LocalObjectContainer container, ByteArrayBuffer buffer)
		{
			return new Db4objects.Db4o.Internal.Marshall.ObjectHeader(container, buffer);
		}
	}
}
