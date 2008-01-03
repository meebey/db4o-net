/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.IO;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public sealed class SwitchingFilesFromClientUtil
	{
		internal static readonly string FilenameA = "switchto.a.db4o";

		internal static readonly string FilenameB = "switchto.b.db4o";

		public static readonly string MainfileName = "mainfile";

		private SwitchingFilesFromClientUtil()
		{
		}

		internal static void DeleteFiles()
		{
			File4.Delete(MainfileName);
			File4.Delete(FilenameA);
			File4.Delete(FilenameB);
		}
	}
}
