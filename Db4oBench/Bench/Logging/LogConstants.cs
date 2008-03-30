/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Sharpen.Util;

namespace Db4objects.Db4o.Bench.Logging
{
	public class LogConstants
	{
		public static readonly string ReadEntry = "READ ";

		public static readonly string WriteEntry = "WRITE ";

		public static readonly string SyncEntry = "SYNC ";

		public static readonly string SeekEntry = "SEEK ";

		public static readonly string[] AllConstants = new string[] { ReadEntry, WriteEntry
			, SyncEntry, SeekEntry };

		public static readonly string Separator = ",";

		public static ISet AllEntries()
		{
			HashSet entries = new HashSet();
			entries.AddAll(Sharpen.Util.Arrays.AsList(AllConstants));
			return entries;
		}
	}
}
