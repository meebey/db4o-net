/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class VersionFieldMetadata : VirtualFieldMetadata
	{
		internal VersionFieldMetadata(ObjectContainerBase stream) : base()
		{
			SetName(VirtualField.VERSION);
			i_handler = new LongHandler(stream);
		}

		public override void AddFieldIndex(MarshallerFamily mf, ClassMetadata yapClass, StatefulBuffer
			 writer, Slot oldSlot)
		{
			writer.WriteLong(writer.GetStream().GenerateTimeStampId());
		}

		public override void Delete(MarshallerFamily mf, StatefulBuffer a_bytes, bool isUpdate
			)
		{
			a_bytes.IncrementOffset(LinkLength());
		}

		internal override void Instantiate1(Transaction a_trans, ObjectReference a_yapObject
			, Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			a_yapObject.VirtualAttributes().i_version = a_bytes.ReadLong();
		}

		internal override void Marshall1(ObjectReference a_yapObject, StatefulBuffer a_bytes
			, bool a_migrating, bool a_new)
		{
			ObjectContainerBase stream = a_bytes.GetStream().i_parent;
			VirtualAttributes va = a_yapObject.VirtualAttributes();
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
			return Const4.LONG_LENGTH;
		}

		internal override void MarshallIgnore(Db4objects.Db4o.Internal.Buffer writer)
		{
			writer.WriteLong(0);
		}
	}
}
