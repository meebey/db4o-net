using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class BackupExceptionTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public static void Main(string[] args)
		{
			new BackupExceptionTestCase().RunAll();
		}

		private static readonly string BACKUP_FILE = "backup.db4o";

		protected override void Db4oSetupBeforeStore()
		{
			File4.Delete(BACKUP_FILE);
		}

		protected override void Db4oCustomTearDown()
		{
			File4.Delete(BACKUP_FILE);
		}
	}
}
