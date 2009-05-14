using System;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public partial class FormatMigrationTestCaseBase
	{
		private static string GetTempPath()
		{
#if !CF && !SILVERLIGHT
			return Environment.GetEnvironmentVariable("TEMP");
#else
			return System.IO.Path.GetTempPath();
#endif
		}
	}
}
