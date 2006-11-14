namespace Db4objects.Db4o.Inside.Marshall
{
	public class StringMarshaller1 : Db4objects.Db4o.Inside.Marshall.StringMarshaller
	{
		private const int DEFRAGMENT_INCREMENT_OFFSET = Db4objects.Db4o.YapConst.INT_LENGTH
			 * 2;

		public override bool InlinedStrings()
		{
			return true;
		}

		public override void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, bool topLevel, object obj, bool withIndirection)
		{
			if (topLevel)
			{
				header.AddBaseLength(LinkLength());
				header.PrepareIndexedPayLoadEntry(trans);
			}
			else
			{
				if (withIndirection)
				{
					header.AddPayLoadLength(LinkLength());
				}
			}
			if (obj == null)
			{
				return;
			}
			header.AddPayLoadLength(trans.Stream().StringIO().Length((string)obj));
		}

		public override object WriteNew(object obj, bool topLevel, Db4objects.Db4o.YapWriter
			 writer, bool redirect)
		{
			Db4objects.Db4o.YapStream stream = writer.GetStream();
			string str = (string)obj;
			if (!redirect)
			{
				if (str != null)
				{
					WriteShort(stream, str, writer);
				}
				return str;
			}
			if (str == null)
			{
				writer.WriteEmbeddedNull();
				return null;
			}
			int length = stream.StringIO().Length(str);
			Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(writer.GetTransaction
				(), length);
			WriteShort(stream, str, bytes);
			writer.WritePayload(bytes, topLevel);
			return bytes;
		}

		public override Db4objects.Db4o.YapReader ReadIndexEntry(Db4objects.Db4o.YapWriter
			 parentSlot)
		{
			int payLoadOffSet = parentSlot.ReadInt();
			int length = parentSlot.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			return parentSlot.ReadPayloadWriter(payLoadOffSet, length);
		}

		public override Db4objects.Db4o.YapReader ReadSlotFromParentSlot(Db4objects.Db4o.YapStream
			 stream, Db4objects.Db4o.YapReader reader)
		{
			int payLoadOffSet = reader.ReadInt();
			int length = reader.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			return reader.ReadPayloadReader(payLoadOffSet, length);
		}

		public override void Defrag(Db4objects.Db4o.ISlotReader reader)
		{
			reader.IncrementOffset(DEFRAGMENT_INCREMENT_OFFSET);
		}
	}
}
