/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class SwitchingFilesFromClientTestCase : ClientServerTestCaseBase
	{
		public virtual void TestSwitch()
		{
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FILENAME_A);
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FILENAME_B);
			Client().SwitchToMainFile();
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FILENAME_A);
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FILENAME_A);
		}

		protected override void Db4oSetupBeforeStore()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}

		protected override void Db4oTearDownAfterClean()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}
	}
}
