/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.IO;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public sealed class SwitchingFilesFromClientUtil
	{
		internal static readonly string FILENAME_A = "switchto.a.db4o";

		internal static readonly string FILENAME_B = "switchto.b.db4o";

		public static readonly string MAINFILE_NAME = "mainfile";

		private SwitchingFilesFromClientUtil()
		{
		}

		internal static void DeleteFiles()
		{
			File4.Delete(MAINFILE_NAME);
			File4.Delete(FILENAME_A);
			File4.Delete(FILENAME_B);
		}
	}
}
