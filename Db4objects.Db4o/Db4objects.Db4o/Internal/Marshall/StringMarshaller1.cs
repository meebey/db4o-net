/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class StringMarshaller1 : StringMarshaller
	{
		public override bool InlinedStrings()
		{
			return true;
		}

		/// <exception cref="CorruptionException"></exception>
		public override BufferImpl ReadIndexEntry(StatefulBuffer parentSlot)
		{
			int payLoadOffSet = parentSlot.ReadInt();
			int length = parentSlot.ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			return parentSlot.ReadPayloadWriter(payLoadOffSet, length);
		}
	}
}
