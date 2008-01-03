/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class SwitchingFilesFromClientTestCase : ClientServerTestCaseBase
	{
		public virtual void TestSwitch()
		{
			if (IsMTOC())
			{
				return;
			}
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameA);
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameB);
			Client().SwitchToMainFile();
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameA);
			Client().SwitchToFile(SwitchingFilesFromClientUtil.FilenameA);
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupBeforeStore()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oTearDownAfterClean()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}
	}
}
