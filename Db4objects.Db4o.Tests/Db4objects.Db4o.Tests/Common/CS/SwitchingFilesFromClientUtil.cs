/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Foundation.IO;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public sealed class SwitchingFilesFromClientUtil
	{
		internal static readonly string FilenameA = Path.GetTempFileName();

		internal static readonly string FilenameB = Path.GetTempFileName();

		public static readonly string MainfileName = Path.GetTempFileName();

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
