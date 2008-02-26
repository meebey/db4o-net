/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Marshall;
using Sharpen;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class DateTimeHandler : StructHandler
	{
		public override Object DefaultValue()
		{
			return DateTime.MinValue;
		}

		public override Object Read(byte[] bytes, int offset)
		{
			long ticks = 0;
			for (int i = 0; i < 8; i++)
			{
				ticks = (ticks << 8) + (long)(bytes[offset++] & 255);
			}
			return new DateTime(ticks);
		}

		public override int TypeID()
		{
			return 25;
		}

		public override void Write(object obj, byte[] bytes, int offset)
		{
			long ticks = ((DateTime)obj).Ticks;
			for (int i = 0; i < 8; i++)
			{
				bytes[offset++] = (byte)(int)(ticks >> (7 - i) * 8);
			}
		}

		public override object Read(IReadContext context)
		{	
			long ticks = context.ReadLong();
			return new DateTime(ticks);
		}

		public override void Write(IWriteContext context, object obj)
		{
			long ticks = ((DateTime)obj).Ticks;
			context.WriteLong(ticks);
		}

        public override IPreparedComparison InternalPrepareComparison(object obj)
        {
            return new PreparedComparasionFor<DateTime>(((DateTime)obj));
        }
	}
}
