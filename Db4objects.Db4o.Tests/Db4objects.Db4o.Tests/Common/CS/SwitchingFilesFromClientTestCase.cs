/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class SwitchingFilesFromClientTestCase : ClientServerTestCaseBase
	{
		[System.ObsoleteAttribute(@"using deprecated api")]
		public virtual void TestSwitch()
		{
			if (IsMTOC())
			{
				// Cast to ExtClient won't work and switching files is 
				// not supported.
				return;
			}
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameA);
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameB);
			Client().SwitchToMainFile();
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameA);
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameA);
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Db4oSetupBeforeStore()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Db4oTearDownAfterClean()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}
	}
}
