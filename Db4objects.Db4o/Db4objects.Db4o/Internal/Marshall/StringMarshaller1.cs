using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class StringMarshaller1 : StringMarshaller
	{
		private const int DEFRAGMENT_INCREMENT_OFFSET = Const4.INT_LENGTH * 2;

		public override bool InlinedStrings()
		{
			return true;
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
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

		public override object WriteNew(object obj, bool topLevel, StatefulBuffer writer, 
			bool redirect)
		{
			ObjectContainerBase stream = writer.GetStream();
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
			StatefulBuffer bytes = new StatefulBuffer(writer.GetTransaction(), length);
			WriteShort(stream, str, bytes);
			writer.WritePayload(bytes, topLevel);
			return bytes;
		}

		public override Db4objects.Db4o.Internal.Buffer ReadIndexEntry(StatefulBuffer parentSlot
			)
		{
			int payLoadOffSet = parentSlot.ReadInt();
			int length = parentSlot.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			return parentSlot.ReadPayloadWriter(payLoadOffSet, length);
		}

		public override Db4objects.Db4o.Internal.Buffer ReadSlotFromParentSlot(ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader)
		{
			int payLoadOffSet = reader.ReadInt();
			int length = reader.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			return reader.ReadPayloadReader(payLoadOffSet, length);
		}

		public override void Defrag(ISlotReader reader)
		{
			reader.IncrementOffset(DEFRAGMENT_INCREMENT_OFFSET);
		}
	}
}
