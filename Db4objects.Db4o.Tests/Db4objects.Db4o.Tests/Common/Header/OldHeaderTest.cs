/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Header
{
	public class OldHeaderTest : ITestCase
	{
		private static readonly string OriginalFile = WorkspaceServices.WorkspaceTestFilePath
			("db4oVersions/db4o_5.5.2");

		private static readonly string DbFile = WorkspaceServices.WorkspaceTestFilePath("db4oVersions/db4o_5.5.2.yap"
			);

		/// <exception cref="IOException"></exception>
		public virtual void Test()
		{
			if (!System.IO.File.Exists(OriginalFile))
			{
				TestPlatform.EmitWarning(OriginalFile + " does not exist. Can not run " + GetType
					().FullName);
				return;
			}
			File4.Copy(OriginalFile, DbFile);
			Db4oFactory.Configure().AllowVersionUpdates(true);
			IObjectContainer oc = Db4oFactory.OpenFile(DbFile);
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
