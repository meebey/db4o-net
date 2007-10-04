/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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

		public override void Defrag(ISlotBuffer reader)
		{
			reader.IncrementOffset(DEFRAGMENT_INCREMENT_OFFSET);
		}
	}
}
