namespace Db4objects.Db4o.Inside.Marshall
{
	public class StringMarshaller0 : Db4objects.Db4o.Inside.Marshall.StringMarshaller
	{
		public override bool InlinedStrings()
		{
			return false;
		}

		public override void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, bool topLevel, object obj, bool withIndirection)
		{
		}

		public override object WriteNew(object a_object, bool topLevel, Db4objects.Db4o.YapWriter
			 a_bytes, bool redirect)
		{
			if (a_object == null)
			{
				a_bytes.WriteEmbeddedNull();
				return null;
			}
			Db4objects.Db4o.YapStream stream = a_bytes.GetStream();
			string str = (string)a_object;
			int length = stream.StringIO().Length(str);
			Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(a_bytes.GetTransaction
				(), length);
			WriteShort(stream, str, bytes);
			bytes.SetID(a_bytes._offset);
			a_bytes.GetStream().WriteEmbedded(a_bytes, bytes);
			a_bytes.IncrementOffset(Db4objects.Db4o.YapConst.ID_LENGTH);
			a_bytes.WriteInt(length);
			return bytes;
		}

		public override Db4objects.Db4o.YapReader ReadIndexEntry(Db4objects.Db4o.YapWriter
			 parentSlot)
		{
			return parentSlot.GetStream().ReadWriterByAddress(parentSlot.GetTransaction(), parentSlot
				.ReadInt(), parentSlot.ReadInt());
		}

		public override Db4objects.Db4o.YapReader ReadSlotFromParentSlot(Db4objects.Db4o.YapStream
			 stream, Db4objects.Db4o.YapReader reader)
		{
			return reader.ReadEmbeddedObject(stream.GetTransaction());
		}
	}
}
