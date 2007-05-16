/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Header
{
	public class OldHeaderTest : ITestCase
	{
		private static readonly string ORIGINAL_FILE = WorkspaceServices.WorkspaceTestFilePath
			("db4oVersions/db4o_5.5.2");

		private static readonly string DB_FILE = WorkspaceServices.WorkspaceTestFilePath(
			"db4oVersions/db4o_5.5.2.yap");

		public virtual void Test()
		{
			if (!System.IO.File.Exists(ORIGINAL_FILE))
			{
				TestPlatform.EmitWarning(ORIGINAL_FILE + " does not exist. Can not run " + GetType
					().FullName);
				return;
			}
			File4.Copy(ORIGINAL_FILE, DB_FILE);
			Db4oFactory.Configure().AllowVersionUpdates(true);
			IObjectContainer oc = Db4oFactory.OpenFile(DB_FILE);
			try
			{
				Assert.IsNotNull(oc);
			}
			finally
			{
				oc.Close();
				Db4oFactory.Configure().AllowVersionUpdates(false);
			}
		}
	}
}
