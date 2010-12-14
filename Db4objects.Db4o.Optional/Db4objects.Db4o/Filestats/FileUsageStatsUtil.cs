/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Filestats
{
	/// <exclude></exclude>
	public sealed class FileUsageStatsUtil
	{
		private static readonly string Padding = "                    ";

		private FileUsageStatsUtil()
		{
		}

		public static string FormatLine(string label, long amount)
		{
			return PadLeft(label, 20) + ": " + PadLeft(amount.ToString(), 12) + "\n";
		}

		private static string PadLeft(string val, int len)
		{
			return Sharpen.Runtime.Substring(Padding, 0, len - val.Length) + val;
		}
	}
}
