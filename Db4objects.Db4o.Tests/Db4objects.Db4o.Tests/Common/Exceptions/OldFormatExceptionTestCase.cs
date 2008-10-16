/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Exceptions;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	/// <exclude></exclude>
	public class OldFormatExceptionTestCase : ITestCase, IOptOutNoFileSystemData
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(OldFormatExceptionTestCase)).Run();
		}

		// It is also regression test for COR-634.
		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			if (WorkspaceServices.WorkspaceRoot == null)
			{
				Sharpen.Runtime.Err.WriteLine("Build environment not available. Skipping test case..."
					);
				return;
			}
			if (!System.IO.File.Exists(SourceFile()))
			{
				Sharpen.Runtime.Err.WriteLine("Test source file " + SourceFile() + " not available. Skipping test case..."
					);
				return;
			}
			Db4oFactory.Configure().ReflectWith(Platform4.ReflectorForType(typeof(OldFormatExceptionTestCase
				)));
			Db4oFactory.Configure().AllowVersionUpdates(false);
			string oldDatabaseFilePath = OldDatabaseFilePath();
			Assert.Expect(typeof(OldFormatException), new _ICodeBlock_43(oldDatabaseFilePath)
				);
			Db4oFactory.Configure().AllowVersionUpdates(true);
			IObjectContainer container = null;
			try
			{
				container = Db4oFactory.OpenFile(oldDatabaseFilePath);
			}
			finally
			{
				if (container != null)
				{
					container.Close();
				}
			}
		}

		private sealed class _ICodeBlock_43 : ICodeBlock
		{
			public _ICodeBlock_43(string oldDatabaseFilePath)
			{
				this.oldDatabaseFilePath = oldDatabaseFilePath;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenFile(oldDatabaseFilePath);
			}

			private readonly string oldDatabaseFilePath;
		}

		/// <exception cref="IOException"></exception>
		protected virtual string OldDatabaseFilePath()
		{
			string oldFile = IOServices.BuildTempPath("old_db.yap");
			File4.Copy(SourceFile(), oldFile);
			return oldFile;
		}

		private string SourceFile()
		{
			return WorkspaceServices.WorkspaceTestFilePath("db4oVersions/db4o_3.0.3");
		}
	}
}
