/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class StringMarshaller0 : StringMarshaller
	{
		public override bool InlinedStrings()
		{
			return false;
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
		{
		}

		public override object WriteNew(object a_object, bool topLevel, StatefulBuffer a_bytes
			, bool redirect)
		{
			if (a_object == null)
			{
				a_bytes.WriteEmbeddedNull();
				return null;
			}
			ObjectContainerBase stream = a_bytes.GetStream();
			string str = (string)a_object;
			int length = stream.StringIO().Length(str);
			StatefulBuffer bytes = new StatefulBuffer(a_bytes.GetTransaction(), length);
			WriteShort(stream, str, bytes);
			bytes.SetID(a_bytes._offset);
			a_bytes.GetStream().WriteEmbedded(a_bytes, bytes);
			a_bytes.IncrementOffset(Const4.ID_LENGTH);
			a_bytes.WriteInt(length);
			return bytes;
		}

		public override Db4objects.Db4o.Internal.Buffer ReadIndexEntry(StatefulBuffer parentSlot
			)
		{
			return parentSlot.GetStream().ReadWriterByAddress(parentSlot.GetTransaction(), parentSlot
				.ReadInt(), parentSlot.ReadInt());
		}

		public override Db4objects.Db4o.Internal.Buffer ReadSlotFromParentSlot(ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader)
		{
			return reader.ReadEmbeddedObject(stream.GetTransaction());
		}

		public override void Defrag(ISlotReader reader)
		{
		}
	}
}
