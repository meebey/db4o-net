/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Bench.Delaying
{
	public class Delays
	{
		public const int Read = 0;

		public const int Write = 1;

		public const int Seek = 2;

		public const int Sync = 3;

		public const int Count = 4;

		public static readonly string units = "nanoseconds";

		public long[] values;

		public Delays(long read, long write, long seek, long sync)
		{
			values = new long[] { read, write, seek, sync };
		}

		public override string ToString()
		{
			return "[delays in " + units + "] read: " + values[Read] + " | write: " + values[
				Write] + " | seek: " + values[Seek] + " | sync: " + values[Sync];
		}
	}
}
